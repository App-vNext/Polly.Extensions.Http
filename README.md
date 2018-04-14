# Polly.Extensions.Http

Polly.Extensions.Http is an extensions package containing opinionated convenience methods for configuring [Polly](https://github.com/App-vNext/Polly) policies to handle transient faults typical of calls through HttpClient.

Polly.Extensions.Http targets .NET Standard 1.1.

[Polly](https://github.com/App-vNext/Polly) is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.  

[<img align="right" src="https://github.com/dotnet/swag/raw/master/logo/dotnetfoundation_v4_small.png" width="100" />](https://www.dotnetfoundation.org/)
Polly is a member of the [.NET Foundation](https://www.dotnetfoundation.org/about)!

#### TODO: link Nuget package and AppVeyor build here when ready

![](https://raw.github.com/App-vNext/Polly/master/Polly-Logo.png)

# Installing via NuGet

    Install-Package Polly.Extensions.Http

You can install the Strongly Named version via: 

    Install-Package Polly.Extensions.Http-Signed

The strongly-named version works with the strongly-named Polly-Signed nuget package.

# Convenience methods for transient faults of HttpClient calls

This repo offers opinionated convenience methods for defining Polly policies to handle transient faults which may be raised by calls through `HttpClient`.  

```csharp
using Polly.Extensions.Http;

// ..

// Handles HttpRequestException, Http status codes >= 500 (server errors) and status code 408 (request timeout)
var policy = HttpPolicyExtensions
  .HandleTransientHttpError()
  .RetryAsync(3); // A very simple retry-3-times. See https://github.com/App-vNext/Polly for the many richer Policy configuration options available.
```

The pre-defined set of conditions to handle can be freely extended using Polly's usual Handle and Or clauses:

```csharp
// Pre-canned http fault handling plus extra http status codes
var policy = HttpPolicyExtensions
  .HandleTransientHttpError()
  .OrResult(response => (int)response.StatusCode == someStatusCode) 
  .RetryAsync(3);

// Pre-canned http fault handling plus extra exceptions
var policy = HttpPolicyExtensions
  .HandleTransientHttpError()
  .Or<TimeoutRejectedException>() // TimeoutRejectedException from Polly's TimeoutPolicy
  .RetryAsync(3);
```

Related overloads in Polly's 'Or' syntax are also provided:

```csharp
// Handles SomeException, HttpRequestException, Http status codes >= 500 (server errors) and status code 408 (request timeout)
// ('or' syntax after your own custom 'handle' clause)
var policy = Policy
  .Handle<SomeException>()
  .OrTransientHttpError()
  .RetryAsync(3);

// Handles SomeException, Http status codes >= 500 (server errors) and status code 408 (request timeout)
// ('or' syntax after your own custom 'handle' clause)
var policy = Policy
  .Handle<SomeException>()
  .OrTransientHttpStatusCode()
  .RetryAsync(3);
```

# Using Polly.Extensions.Http with HttpClientFactory

Polly.Extensions.Http is ideal for creating custom Polly policies for use with HttpClientFactory ([preview1 blog post](https://blogs.msdn.microsoft.com/webdev/2018/02/28/asp-net-core-2-1-preview1-introducing-httpclient-factory/); [preview2 blog post](https://blogs.msdn.microsoft.com/webdev/2018/04/12/asp-net-core-2-1-0-preview2-now-available/)), available from ASP.NET Core 2.1.

```csharp
var retryPolicy = HttpPolicyExtensions
  .HandleTransientHttpError()
  .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner execution times out
  .RetryAsync(3);

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);  

serviceCollection.AddHttpClient("example.com", c => c.BaseAddress = new Uri("http://example.com"))
  .AddPolicyHandler(retryPolicy)
  .AddPolicyHandler(timeoutPolicy);
```

#### TODO: link to public HttpClientFactory documentation and/or expand example, when that documentation is published.

# For more information on Polly

For more information on Polly and configuring Polly policies see the [main Polly repository](https://github.com/App-vNext/Polly) and [wiki](https://github.com/App-vNext/Polly/wiki).

# Release notes

For details of changes by release see the [change log](CHANGELOG.md).  

# 3rd Party Libraries and Contributions

* [Fluent Assertions](https://github.com/fluentassertions/fluentassertions) - A set of .NET extension methods that allow you to more naturally specify the expected outcome of a TDD or BDD-style test | [Apache License 2.0 (Apache)](https://github.com/dennisdoomen/fluentassertions/blob/develop/LICENSE)
* [xUnit.net](https://github.com/xunit/xunit) - Free, open source, community-focused unit testing tool for the .NET Framework | [Apache License 2.0 (Apache)](https://github.com/xunit/xunit/blob/master/license.txt)
* Build powered by [Cake](http://cakebuild.net/) and [GitVersionTask](https://github.com/GitTools/GitVersion).

# Acknowledgements

* Developed in collaboration between the Polly team ([@reisenberger](https://github.com/reisenberger) and [@joelhulen](https://github.com/joelhulen)) and Microsoft [ASP.NET Core](https://www.asp.net/core/) team members [Ryan Nowak](https://github.com/rynowak) and [Glenn Condron](https://github.com/glennc).

# Instructions for Contributing

Since Polly is part of the .NET Foundation, we ask our contributors to abide by their [Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).  To contribute (beyond trivial typo corrections), review and sign the [.Net Foundation Contributor License Agreement](https://cla.dotnetfoundation.org/). This ensures the community is free to use your contributions.  The registration process can be completed entirely online.

Also, we've stood up a [Slack](http://www.pollytalk.org) channel for easier real-time discussion of ideas and the general direction of Polly as a whole. 

# License

Licensed under the terms of the [New BSD License](http://opensource.org/licenses/BSD-3-Clause)
