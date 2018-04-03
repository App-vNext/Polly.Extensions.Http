///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// EXTERNAL NUGET TOOLS
//////////////////////////////////////////////////////////////////////

#Tool "xunit.runner.console"
#Tool "GitVersion.CommandLine"
#Tool "Brutal.Dev.StrongNameSigner"

//////////////////////////////////////////////////////////////////////
// EXTERNAL NUGET LIBRARIES
//////////////////////////////////////////////////////////////////////

#addin "Cake.FileHelpers"
#addin "System.Text.Json"
using System.Text.Json;

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projectName = "Polly.Extensions.Http";
var keyName = "Polly.snk";

var solutions = GetFiles("./**/*.sln");
var solutionPaths = solutions.Select(solution => solution.GetDirectory());

var srcDir = Directory("./src");
var buildDir = Directory("./build");
var artifactsDir = Directory("./artifacts");
var testResultsDir = artifactsDir + Directory("test-results");

// NuGet
var nuspecExtension = ".nuspec";
var signed = "-Signed";
var nuspecFolder = "nuget-package";
var nuspecSrcFile = srcDir + File(projectName + nuspecExtension);
var nuspecDestFile = buildDir + File(projectName + nuspecExtension);
var nuspecSignedDestFile = buildDir + File(projectName + signed + nuspecExtension);
var nupkgUnsignedDestDir = artifactsDir + Directory(nuspecFolder);
var nupkgSignedDestDir = artifactsDir + Directory(nuspecFolder + signed);
var snkFile = srcDir + File(keyName);

var projectToNugetFolderMap = new Dictionary<string, string[]>() {
    { "NetStandard11", new [] {"netstandard1.1"} },
    { "NetStandard11-Signed", new [] {"netstandard1.1"} },
};

// Gitversion
var gitVersionPath = ToolsExePath("GitVersion.exe");
Dictionary<string, object> gitVersionOutput;

// StrongNameSigner
var strongNameSignerPath = ToolsExePath("StrongNameSigner.Console.exe");


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(_ =>
{
    Information("");
    Information(@" ____   __   __    __    _  _   ____  _  _  ____  ____  __ _  ____  __  __   __ _  ____     _  _  ____  ____  ____ ");
    Information(@"(  _ \ /  \ (  )  (  )  ( \/ ) (  __)( \/ )(_  _)(  __)(  ( \/ ___)(  )/  \ (  ( \/ ___)   / )( \(_  _)(_  _)(  _ \");
    Information(@" ) __/(  O )/ (_/\/ (_/\ )  /_  ) _)  )  (   )(   ) _) /    /\___ \ )((  O )/    /\___ \ _ ) __ (  )(    )(   ) __/");
    Information(@"(__)   \__/ \____/\____/(__/(_)(____)(_/\_) (__) (____)\_)__)(____/(__)\__/ \_)__)(____/(_)\_)(_/ (__)  (__) (__)  ");

    Information("");
});

Teardown(_ =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
// PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Clean")
    .Does(() =>
{
    DirectoryPath[] cleanDirectories = new DirectoryPath[] {
        buildDir,
        testResultsDir,
        nupkgUnsignedDestDir,
        nupkgSignedDestDir,
        artifactsDir
  	};

    CleanDirectories(cleanDirectories);

    foreach(var path in cleanDirectories) { EnsureDirectoryExists(path); }

    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
});

Task("__RestoreNugetPackages")
    .Does(() =>
{
    foreach(var solution in solutions)
    {
        Information("Restoring NuGet Packages for {0}", solution);
        NuGetRestore(solution);
    }
});

Task("__UpdateAssemblyVersionInformation")
    .Does(() =>
{
    var gitVersionSettings = new ProcessSettings()
        .SetRedirectStandardOutput(true);

    IEnumerable<string> outputLines;
    StartProcess(gitVersionPath, gitVersionSettings, out outputLines);

    var output = string.Join("\n", outputLines);
    gitVersionOutput = new JsonParser().Parse<Dictionary<string, object>>(output);

    Information("Updated GlobalAssemblyInfo");
    Information("AssemblyVersion -> {0}", gitVersionOutput["AssemblySemVer"]);
    Information("AssemblyFileVersion -> {0}", gitVersionOutput["MajorMinorPatch"]);
    Information("AssemblyInformationalVersion -> {0}", gitVersionOutput["InformationalVersion"]);
});

Task("__UpdateDotNetStandardAssemblyVersionNumber")
    .Does(() =>
{
    // NOTE: TEMPORARY fix only, while GitVersionTask does not support .Net Standard assemblies.  See https://github.com/App-vNext/Polly/issues/176.  
    // This build Task can be removed when GitVersionTask supports .Net Standard assemblies.
    var assemblySemVer = gitVersionOutput["AssemblySemVer"].ToString();
    Information("Updating NetStandard AssemblyVersions to {0}", assemblySemVer);
    var assemblyInfosToUpdate = GetFiles("./src/**/Properties/AssemblyInfo.cs")
        .Select(f => f.FullPath)
        .Where(f => !f.Contains("Specs"));

    foreach(var assemblyInfo in assemblyInfosToUpdate) {
        var replacedFiles = ReplaceRegexInFiles(assemblyInfo, "AssemblyVersion[(]\".*\"[)]", "AssemblyVersion(\"" + assemblySemVer +"\")");
        if (!replacedFiles.Any())
        {
             throw new Exception($"AssemblyVersion could not be updated in {assemblyInfo}.");
        }
    }
});

Task("__UpdateAppVeyorBuildNumber")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    var fullSemVer = gitVersionOutput["FullSemVer"].ToString();
    AppVeyor.UpdateBuildVersion(fullSemVer);
});

Task("__BuildSolutions")
    .Does(() =>
{
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);

        MSBuild(solution, settings =>
            settings
                .SetConfiguration(configuration)
                .WithProperty("TreatWarningsAsErrors", "true")
                .UseToolVersion(MSBuildToolVersion.VS2017)
                .SetVerbosity(Verbosity.Minimal)
                .SetNodeReuse(false));
    }
});

Task("__RunTests")
    .Does(() =>
{
    foreach(var specsProj in GetFiles("./src/**/*.Specs.csproj")) {
        DotNetCoreTest(specsProj.FullPath, new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true
        });
    }
});

Task("__CopyNonSignedOutputToNugetFolder")
    .Does(() =>
{
    foreach(var project in projectToNugetFolderMap.Keys
        .Where(p => !p.Contains(signed))
    ) {
        var sourceDir = srcDir + Directory(projectName + "." + project) + Directory("bin") + Directory(configuration);

        foreach(var targetFolder in projectToNugetFolderMap[project]) {
            var destDir = buildDir + Directory("lib");

            Information("Copying {0} -> {1}.", sourceDir, destDir);
            CopyDirectory(sourceDir, destDir);
       }
    }

    CopyFile(nuspecSrcFile, nuspecDestFile);
});

Task("__CopySignedOutputToNugetFolder")
    .Does(() =>
{
    foreach(var project in projectToNugetFolderMap.Keys
        .Where(p => p.Contains(signed))
    ) {
        var sourceDir = srcDir + Directory(projectName + "." + project) + Directory("bin") + Directory(configuration);

        foreach(var targetFolder in projectToNugetFolderMap[project]) {
            var destDir = buildDir + Directory("lib");

            Information("Copying {0} -> {1}.", sourceDir, destDir);
            CopyDirectory(sourceDir, destDir);
       }
    }

    CopyFile(nuspecSrcFile, nuspecSignedDestFile);
    
    var replacedFiles = ReplaceTextInFiles(nuspecSignedDestFile, "dependency id=\"Polly\"", "dependency id=\"Polly-Signed\"");
    if (!replacedFiles.Any())
    {
        throw new Exception("Could not set Polly dependency to Polly-Signed, for -Signed nuget package.");
    }
});

Task("__CreateNonSignedNugetPackage")
    .Does(() =>
{
    var nugetVersion = gitVersionOutput["NuGetVersion"].ToString();
    var packageName = projectName;

    Information("Building {0}.{1}.nupkg", packageName, nugetVersion);

    var nuGetPackSettings = new NuGetPackSettings {
        Id = packageName,
        Title = packageName,
        Version = nugetVersion,
        OutputDirectory = nupkgUnsignedDestDir
    };

    NuGetPack(nuspecDestFile, nuGetPackSettings);
});

Task("__CreateSignedNugetPackage")
    .Does(() =>
{
    var nugetVersion = gitVersionOutput["NuGetVersion"].ToString();
    var packageName = projectName + "-Signed";

    Information("Building {0}.{1}.nupkg", packageName, nugetVersion);

    var nuGetPackSettings = new NuGetPackSettings {
        Id = packageName,
        Title = packageName,
        Version = nugetVersion,
        OutputDirectory = nupkgSignedDestDir
    };

    NuGetPack(nuspecSignedDestFile, nuGetPackSettings);
});

Task("__StronglySignAssemblies")
    .Does(() =>
{
    //see: https://github.com/brutaldev/StrongNameSigner
    var strongNameSignerSettings = new ProcessSettings()
        .WithArguments(args => args
            .Append("-in")
            .AppendQuoted(buildDir)
            .Append("-k")
            .AppendQuoted(snkFile)
            .Append("-l")
            .AppendQuoted("Changes"));

    StartProcess(strongNameSignerPath, strongNameSignerSettings);
});

//////////////////////////////////////////////////////////////////////
// BUILD TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__RestoreNugetPackages")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
    .IsDependentOn("__UpdateDotNetStandardAssemblyVersionNumber")
    .IsDependentOn("__UpdateAppVeyorBuildNumber")
    .IsDependentOn("__BuildSolutions")
    .IsDependentOn("__RunTests")
    .IsDependentOn("__CopyNonSignedOutputToNugetFolder")
    .IsDependentOn("__CreateNonSignedNugetPackage")
    .IsDependentOn("__CopySignedOutputToNugetFolder")
    .IsDependentOn("__StronglySignAssemblies")
    .IsDependentOn("__CreateSignedNugetPackage");

///////////////////////////////////////////////////////////////////////////////
// PRIMARY TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

//////////////////////////////////////////////////////////////////////
// HELPER FUNCTIONS
//////////////////////////////////////////////////////////////////////

string ToolsExePath(string exeFileName) {
    var exePath = System.IO.Directory.GetFiles(@".\Tools", exeFileName, SearchOption.AllDirectories).FirstOrDefault();
    return exePath;
}
