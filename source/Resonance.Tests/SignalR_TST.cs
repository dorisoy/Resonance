﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resonance.Adapters.SignalR;
using Resonance.Messages;
using Resonance.SignalR;
using Resonance.SignalR.Services;
using Resonance.Tests.Common;
using Resonance.Tests.SignalR;
using Resonance.Transporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Resonance.Tests
{
    [TestClass]
    [TestCategory("SignalR")]
    public class SignalR_TST : ResonanceTest
    {
        [TestMethod]
        public async Task SignalR_Legacy_Reading_Writing()
        {
            return;
            Init();

            if (IsRunningOnAzurePipelines) return;

            String hostUrl = "http://localhost:8080";
            String hubUrl = $"{hostUrl}/TestHub";

            SignalRServer server = new SignalRServer(hostUrl);
            server.Start();

            await SignalR_Reading_Writing(hubUrl, SignalRMode.Legacy);
        }

        [TestMethod]
        public async Task SignalR_Core_Reading_Writing()
        {
            return;
            Init();

            if (IsRunningOnAzurePipelines) return;

            String webApiProjectPath = Path.Combine(TestHelper.GetSolutionFolder(), "Resonance.Tests.SignalRCore.WebAPI");

            Process cmd = new Process();
            cmd.StartInfo.WorkingDirectory = webApiProjectPath;
            cmd.StartInfo.FileName = "dotnet";
            cmd.StartInfo.Arguments = "run";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            Thread.Sleep(4000);

            String expected = "Resonance SignalR Core Unit Test Feedback Service";
            String result = String.Empty;

            while (result != expected)
            {
                try
                {
                    using (HttpClient http = new HttpClient())
                    {
                        result = http.GetStringAsync("http://localhost:27210/home").GetAwaiter().GetResult();
                    }
                }
                catch { }

                Thread.Sleep(1000);
            }

            await SignalR_Reading_Writing("http://localhost:27210/hubs/TestHub", SignalRMode.Core);

            cmd.Kill();
        }

        private async Task SignalR_Reading_Writing(String url, SignalRMode mode)
        {
            TestCredentials credentials = new TestCredentials() { Name = "Test" };
            TestServiceInformation serviceInfo = new TestServiceInformation() { ServiceId = "My Test Service" };

            var registeredService = ResonanceServiceFactory.Default.RegisterService<TestCredentials, TestServiceInformation, TestAdapterInformation>(credentials, serviceInfo, url, mode).GetAwaiter().GetResult();

            Assert.AreSame(registeredService.Credentials, credentials);
            Assert.AreSame(registeredService.ServiceInformation, serviceInfo);
            Assert.AreEqual(registeredService.Mode, mode);

            bool connected = false;

            ResonanceJsonTransporter serviceTransporter = new ResonanceJsonTransporter();

            registeredService.ConnectionRequest += async (_, e) =>
            {
                Assert.IsTrue(e.RemoteAdapterInformation.Information == "No information on the remote adapter");
                serviceTransporter.Adapter = e.Accept();
                await serviceTransporter.Connect();
                connected = true;
            };

            var remoteServices = await ResonanceServiceFactory.Default.GetAvailableServices<TestCredentials, TestServiceInformation>(credentials, url, mode);

            Assert.IsTrue(remoteServices.Count == 1);

            var remoteService = remoteServices.First();

            Assert.AreEqual(remoteService.ServiceId, registeredService.ServiceInformation.ServiceId);

            ResonanceJsonTransporter clientTransporter = new ResonanceJsonTransporter(new SignalRAdapter<TestCredentials>(credentials, url, remoteService.ServiceId, mode));

            await clientTransporter.Connect();

            while (!connected)
            {
                Thread.Sleep(10);
            }

            await TestUtils.Read_Write_Test(this, serviceTransporter, clientTransporter, false, false, 1000, 20);
        }
    }
}
