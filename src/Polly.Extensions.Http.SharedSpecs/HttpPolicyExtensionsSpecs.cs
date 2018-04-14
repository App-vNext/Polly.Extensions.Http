using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Polly;
using Polly.Extensions.Http;
using Polly.Fallback;
using Xunit;


namespace Polly.Extensions.Http.Specs
{
    public class HttpPolicyExtensionsSpecs
    {
        [Fact]
        public void Should_be_able_to_reference_HandleTransientHttpError()
        {
            HttpPolicyExtensions.HandleTransientHttpError()
                .Should().BeOfType<PolicyBuilder<HttpResponseMessage>>();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpRequestException()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = HttpPolicyExtensions.HandleTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpStatusCode_RequestTimeout()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = HttpPolicyExtensions.HandleTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpStatusCode_InternalServerError()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = HttpPolicyExtensions.HandleTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_not_handle_HttpStatusCode_BadRequest()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = HttpPolicyExtensions.HandleTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        private class CustomException : Exception { }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError()
        {
            Policy.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<HttpResponseMessage>>();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpRequestException()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpStatusCode_RequestTimeout()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpStatusCode_InternalServerError()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_not_handle_HttpStatusCode_BadRequest()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError_onGenericPolicyBuilder()
        {
            Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<HttpResponseMessage>>();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpRequestException()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_RequestTimeout()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_InternalServerError()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_not_handle_HttpStatusCode_BadRequest()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode()
        {
            Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .Should().BeOfType<PolicyBuilder<HttpResponseMessage>>();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode_onGenericPolicyBuilder()
        {
            Policy<HttpResponseMessage>.Handle<CustomException>().OrTransientHttpStatusCode()
                .Should().BeOfType<PolicyBuilder<HttpResponseMessage>>();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_not_handle_HttpRequestException()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_handle_HttpStatusCode_RequestTimeout()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_handle_HttpStatusCode_InternalServerError()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_not_handle_HttpStatusCode_BadRequest()
        {
            bool policyHandled = false;
            FallbackPolicy<HttpResponseMessage> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token =>
                {
                    policyHandled = true;
                    return Task.FromResult<HttpResponseMessage>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }
    }
}