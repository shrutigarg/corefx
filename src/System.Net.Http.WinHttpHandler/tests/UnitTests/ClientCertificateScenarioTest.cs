// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography.X509Certificates;

using Xunit;

namespace System.Net.Http.WinHttpHandlerUnitTests
{
    public class ClientCertificateScenarioTest
    {
        public static object[][] ValidClientCertificates
        {
            get
            {
                var helper = new ClientCertificateHelper();
                return helper.ValidClientCertificates;
            }
        }

        public static object[][] InvalidClientCertificates
        {
            get
            {
                var helper = new ClientCertificateHelper();
                return helper.InvalidClientCertificates;
            }
        }

        public ClientCertificateScenarioTest()
        {
            TestControl.ResetAll();
        }

        [Fact]
        public void NonSecureRequest_AddNoCertificates_CertificateContextNotSet()
        {
            using (var handler = new WinHttpHandler())
            {
                using (HttpResponseMessage response = SendRequestHelper.Send(
                    handler,
                    () => { },
                    TestServer.FakeServerEndpoint))
                {
                    Assert.Equal(0, APICallHistory.WinHttpOptionClientCertContext.Count);
                }
            }
        }

        [Theory, MemberData("ValidClientCertificates")]
        public void NonSecureRequest_AddValidCertificate_CertificateContextNotSet(X509Certificate2 certificate)
        {
            using (var handler = new WinHttpHandler())
            {
                handler.ClientCertificates.Add(certificate);
                using (HttpResponseMessage response = SendRequestHelper.Send(
                    handler,
                    () => { },
                    TestServer.FakeServerEndpoint))
                {
                    Assert.Equal(0, APICallHistory.WinHttpOptionClientCertContext.Count);
                }
            }
        }

        [Fact]
        public void SecureRequest_AddNoCertificates_NullCertificateContextSet()
        {
            using (var handler = new WinHttpHandler())
            {
                using (HttpResponseMessage response = SendRequestHelper.Send(
                    handler,
                    () => { },
                    TestServer.FakeSecureServerEndpoint))
                {
                    Assert.Equal(1, APICallHistory.WinHttpOptionClientCertContext.Count);
                    Assert.Equal(IntPtr.Zero, APICallHistory.WinHttpOptionClientCertContext[0]);
                }
            }
        }

        [Theory, MemberData("ValidClientCertificates")]
        public void SecureRequest_AddValidCertificate_ValidCertificateContextSet(X509Certificate2 certificate)
        {
            using (var handler = new WinHttpHandler())
            {
                handler.ClientCertificates.Add(certificate);
                using (HttpResponseMessage response = SendRequestHelper.Send(
                    handler,
                    () => { },
                    TestServer.FakeSecureServerEndpoint))
                {
                    Assert.Equal(1, APICallHistory.WinHttpOptionClientCertContext.Count);
                    Assert.NotEqual(IntPtr.Zero, APICallHistory.WinHttpOptionClientCertContext[0]);
                }
            }
        }

        [Theory, MemberData("InvalidClientCertificates")]
        public void SecureRequest_AddInvalidCertificate_NullCertificateContextSet(X509Certificate2 certificate)
        {
            using (var handler = new WinHttpHandler())
            {
                handler.ClientCertificates.Add(certificate);
                using (HttpResponseMessage response = SendRequestHelper.Send(
                    handler,
                    () => { },
                    TestServer.FakeSecureServerEndpoint))
                {
                    Assert.Equal(1, APICallHistory.WinHttpOptionClientCertContext.Count);
                    Assert.Equal(IntPtr.Zero, APICallHistory.WinHttpOptionClientCertContext[0]);
                }
            }
        }        
    }
}
