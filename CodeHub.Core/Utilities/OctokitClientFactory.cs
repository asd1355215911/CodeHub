﻿using System;
using Octokit.Internal;
using Splat;
using CodeHub.Core.Services;
using Octokit;
using System.Net.Http;

namespace CodeHub.Core.Utilities
{
    public static class OctokitClientFactory
    {
        public static readonly string[] Scopes = { "user", "repo", "gist", "notifications" };

        public static GitHubClient Create(Uri domain, Credentials credentials)
        {
            // Decorate the HttpClient
            //IHttpClient httpClient = new HttpClientAdapter();
            IHttpClient httpClient = new OctokitModernHttpClient();
            //httpClient = new OctokitCacheClient(httpClient);
            httpClient = new OctokitNetworkClient(httpClient, Locator.Current.GetService<INetworkActivityService>());

            var connection = new Connection(
                new ProductHeaderValue("CodeHub"),
                domain,
                new InMemoryCredentialStore(credentials),
                httpClient,
                new SimpleJsonSerializer());
            return new GitHubClient(connection);
        }
    }
}

