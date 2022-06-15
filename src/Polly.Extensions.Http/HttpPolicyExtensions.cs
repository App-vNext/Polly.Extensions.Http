using System;
using System.Net;
using System.Net.Http;

namespace Polly.Extensions.Http
{
    /// <summary>
    /// Contains opinionated convenience methods for configuring policies to handle conditions typically representing transient faults when making <see cref="HttpClient"/> requests.
    /// </summary>
    public static class HttpPolicyExtensions
    {
        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = (response) =>
        {
            return (int)response.StatusCode > 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
        };

        /// <summary>
        /// Builds a <see cref="PolicyBuilder{HttpResponseMessage}"/> to configure a <see cref="Policy{HttpResponseMessage}"/> which will handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. 
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<HttpResponseMessage> HandleTransientHttpError()
        {
            return Policy<HttpResponseMessage>.Handle<HttpRequestException>().OrTransientHttpStatusCode();
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. 
        /// <para>The <see cref="HttpStatusCode"/>s configured to be handled are:
        /// <list type="bullet">
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. </returns>
        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. 
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. 
        /// <para>The <see cref="HttpStatusCode"/>s configured to be handled are:
        /// <list type="bullet">
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. </returns>
        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder<HttpResponseMessage> policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. 
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder<HttpResponseMessage> policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }
    }
}
