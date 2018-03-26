using System.Reflection;

[assembly: AssemblyProduct("Polly.Extensions.Http")]
[assembly: AssemblyCompany("App vNext")]
[assembly: AssemblyDescription("Polly.Extensions.Http is an extensions package containing opinionated convenience methods for configuring Polly policies to handle transient faults typical of calls through HttpClient.")]
[assembly: AssemblyCopyright("Copyright (c) 2018, App vNext")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
