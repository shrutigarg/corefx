// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Tests;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Tests
{
    public class HttpClientHandlerTest
    {
        private readonly ITestOutputHelper _output;
        private const string dataKey = "data";
        private const string mediaTypeJson = "application/json";

        private static bool JsonMessageContainsKeyValue(string message, string key, string value)
        {
            // Poor Man's json parsing.
            // Should we consider depending on a real json parser?
            var pattern = string.Format(@"""{0}""\s*:\s*""{1}""", key, value);
            var regex = new Regex(pattern);
            return regex.IsMatch(message);
        }

        public HttpClientHandlerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SendAsync_SimpleGet_Success()
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);

            // TODO: This is a placeholder until GitHub Issue #2383 gets resolved.
            var response = client.GetAsync(HttpTestServers.RemoteGetServer).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            _output.WriteLine(responseContent);
        }

        [Fact]
        public async Task GetAsync_Cancel_CancellationTokenPropagates()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            try
            {
                var client = new HttpClient();
                Task <HttpResponseMessage> task = client.GetAsync(HttpTestServers.RemoteGetServer, cts.Token);
                await task;

                Assert.True(false, "Expected TaskCanceledException to be thrown.");
            }
            catch (TaskCanceledException ex)
            {
                Assert.True(cts.Token.IsCancellationRequested,
                    "Expected cancellation requested on original token.");

                Assert.True(ex.CancellationToken.IsCancellationRequested,
                    "Expected cancellation requested on token attached to exception.");
            }
        }

        [Fact]
        public async Task PostAsync_CallMethod_StringContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    var data = "Test String";
                    var stringContent = new StringContent(data, UnicodeEncoding.UTF8, mediaTypeJson);
                    HttpResponseMessage response =
                        await client.PostAsync(HttpTestServers.RemotePostServer, stringContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Assert.True(JsonMessageContainsKeyValue(responseContent, dataKey, data));
                    _output.WriteLine(responseContent);
                }
            }
        }


        [Fact]
        public async Task PostAsync_CallMethod_FormUrlEncodedContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    var values = new Dictionary<string, string> {{"thing1", "hello"}, {"thing2", "world"}};
                    var content = new FormUrlEncodedContent(values);
                    HttpResponseMessage response = await client.PostAsync(HttpTestServers.RemotePostServer, content);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Assert.True(JsonMessageContainsKeyValue(responseContent, "thing1", "hello"));
                    Assert.True(JsonMessageContainsKeyValue(responseContent, "thing2", "world"));
                    _output.WriteLine(responseContent);
                }
            }
        }

        [Fact]
        public async Task PostAsync_CallMethod_UploadFile()
        {
            string fileName = Path.GetTempFileName();
            string fileTitle = "fileToUpload";
            string fileContent = "This file to test POST Scenario";

            try
            {
                //Delete File if exists
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                //Create file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    byte[] author = new UTF8Encoding(true).GetBytes(fileContent);
                    fs.Write(author, 0, author.Length);
                }

                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler))
                    {
                        var form = new MultipartFormDataContent();
                        var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        var content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = fileTitle,
                            FileName = fileName
                        };
                        form.Add(content);
                        HttpResponseMessage response = await client.PostAsync(HttpTestServers.RemotePostServer, form);
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        Assert.True(JsonMessageContainsKeyValue(responseContent, fileTitle, fileContent));
                        _output.WriteLine(responseContent);
                    }
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Fact]
        public async Task PostAsync_CallMethod_NullContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    HttpContent obj = new StringContent(String.Empty);
                    HttpResponseMessage response = await client.PostAsync(HttpTestServers.RemotePostServer, null);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Assert.True(JsonMessageContainsKeyValue(responseContent, dataKey, String.Empty));
                    _output.WriteLine(responseContent);
                }
            }
        }

        [Fact]
        public async Task PostAsync_CallMethodTwice_StringContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    var data = "Test String";
                    var stringContent = new StringContent(data, UnicodeEncoding.UTF8, mediaTypeJson);
                    HttpResponseMessage response =
                        await client.PostAsync(HttpTestServers.RemotePostServer, stringContent);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    _output.WriteLine(responseContent);

                    //Repeat call
                    stringContent = new StringContent(data, UnicodeEncoding.UTF8, mediaTypeJson);
                    response = await client.PostAsync(HttpTestServers.RemotePostServer, stringContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    responseContent = response.Content.ReadAsStringAsync().Result;
                    Assert.True(JsonMessageContainsKeyValue(responseContent, dataKey, data));
                    _output.WriteLine(responseContent);
                }
            }
        }

        [Fact]
        public async Task PostAsync_CallMethod_UnicodeStringContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {

                    var stringContent = new StringContent("\ub4f1\uffc7\u4e82\u67ab4\uc6d4\ud1a0\uc694\uc77c\uffda3\u3155\uc218\uffdb", UnicodeEncoding.UTF8,
                        mediaTypeJson);
                    HttpResponseMessage response =
                        await client.PostAsync(HttpTestServers.RemotePostServer, stringContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    _output.WriteLine(responseContent);
                }
            }
        }

        [Fact]
        public async Task PutAsync_CallMethod_StringContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    var stringContent = new StringContent("{ \"firstName\": \"John\" }", UnicodeEncoding.UTF32,
                        mediaTypeJson);
                    HttpResponseMessage response = await client.PutAsync(HttpTestServers.RemotePutServer, stringContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    _output.WriteLine(responseContent);
                }
            }
        }


        [Fact]
        public async Task PutAsync_CallMethod_NullContent()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.PutAsync(HttpTestServers.RemotePutServer, null);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Assert.True(JsonMessageContainsKeyValue(responseContent, dataKey, String.Empty));
                    _output.WriteLine(responseContent);
                }
            }
        }

        [Fact]
        public async Task PostAsync_IncorrectUri_MethodNotAllowed()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.PutAsync(HttpTestServers.RemoteGetServer, null);
                    Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task PutAsync_IncorrectUri_MethodNotAllowed()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.PutAsync(HttpTestServers.RemoteGetServer, null);
                    Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
                }
            }
        }

    }
}
