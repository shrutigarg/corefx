// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Test.Common;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Net.Http.Functional.Tests
{
    public class HttpClientPerfTest
    {
        public readonly static object[][] EchoServers = HttpTestServers.EchoServers;
        const int innerIterations = 1000;
        [Benchmark, MemberData(nameof(EchoServers))]
        public void PutMethodPerf_SingleInstance(Uri uri)
        {
            string data = "Test String";

            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < innerIterations; i++)
                        {
                            var content = new StringContent(data, Encoding.UTF8);
                            content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);


                            client.PutAsync(uri, content).Wait();



                        }
                    }
                }
                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

        [Benchmark, MemberData(nameof(EchoServers))]
        public void PostMethodPerf_SingleInstance(Uri uri)
        {
            string data = "Test String";

            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < innerIterations; i++)
                        {
                            var content = new StringContent(data, Encoding.UTF8);
                            content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                            client.PostAsync(uri, content).Wait();
                        }
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

       [Benchmark, MemberData(nameof(EchoServers))]
        public void GetMethodPerf_SingleInstance(Uri uri)
       {
           Console.WriteLine("running test for uri : " + uri.ToString());
            string data = "Test String";
            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < innerIterations; i++)
                        {
                            var content = new StringContent(data, Encoding.UTF8);
                            content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                            client.GetAsync(uri).Wait();
                        }
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

       [Benchmark, MemberData(nameof(EchoServers))]
        public void PutMethodPerf_MultiInstance(Uri uri)
        {
            string data = "Test String";
            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        Parallel.For(
                            0,
                            1000,
                            (i) =>
                            {
                                var content = new StringContent(data, Encoding.UTF8);
                                content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                                client.PutAsync(uri, content).Wait();

                            });
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

       [Benchmark, MemberData(nameof(EchoServers))]
        public void PostMethodPerf_MultiInstance(Uri uri)
        {
            string data = "Test String";
            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        Parallel.For(
                            0,
                            1000,
                            (i) =>
                            {
                                var content = new StringContent(data, Encoding.UTF8);
                                content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                                client.PutAsync(uri, content).Wait();

                            });
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

       [Benchmark, MemberData(nameof(EchoServers))]
        public void GetMethodPerf_MultiInstance(Uri uri)
        {
            string data = "Test String";
            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        Parallel.For(
                            0,
                            1000,
                            (i) =>
                            {
                                var content = new StringContent(data, Encoding.UTF8);
                                content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                                client.GetAsync(uri).Wait();

                            });
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }


       [Benchmark, MemberData(nameof(EchoServers))]
        public void PutMethodPerf_SingleInstance_LargeData(Uri uri)
        {
            byte[] buf = new byte[10000];
            for (int i = 0; i < 9999; i++)
            {
                buf[i] = (byte)i;
            }
            string data = Encoding.UTF8.GetString(buf);

            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < innerIterations; i++)
                        {
                            var content = new StringContent(data, Encoding.UTF8);
                            content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);


                            client.PutAsync(uri, content).Wait();



                        }
                    }
                }
                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }

       [Benchmark, MemberData(nameof(EchoServers))]
        public void PostMethodPerf_SingleInstance_LargeData(Uri uri)
        {
            byte[] buf = new byte[10000];
            for (int i = 0; i < 9999; i++)
            {
                buf[i] = (byte)i;
            }
            string data = Encoding.UTF8.GetString(buf);

            using (var client = new HttpClient())
            {
                Stopwatch timeProgramStart = new Stopwatch();
                timeProgramStart.Start();
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < innerIterations; i++)
                        {
                            var content = new StringContent(data, Encoding.UTF8);
                            content.Headers.ContentMD5 = TestHelper.ComputeMD5Hash(data);
                            client.PostAsync(uri, content).Wait();
                        }
                    }
                }

                timeProgramStart.Stop();
                Console.WriteLine("timetaken : " + timeProgramStart.ElapsedMilliseconds);
            }
        }
    }
}
