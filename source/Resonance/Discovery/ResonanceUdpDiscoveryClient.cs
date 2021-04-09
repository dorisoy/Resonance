﻿using Resonance.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Resonance.Discovery
{
    /// <summary>
    /// Represents a UDP discovery client for scanning for <see cref="ResonanceUdpDiscoveryService{TDiscoveryInfo, TEncoder}"/> on the local area network.
    /// </summary>
    /// <typeparam name="TDiscoveryInfo">The type of the discovery information.</typeparam>
    /// <typeparam name="TDecoder">The type of the decoder.</typeparam>
    /// <seealso cref="Resonance.ResonanceObject" />
    public class ResonanceUdpDiscoveryClient<TDiscoveryInfo, TDecoder> : ResonanceObject, IResonanceDiscoveryClient<TDiscoveryInfo, TDecoder, ResonanceUdpDiscoveredService<TDiscoveryInfo>> where TDiscoveryInfo : class, new() where TDecoder : IResonanceDecoder, new()
    {
        private int _componentCounter;
        private UdpClient _udpClient;
        private Thread _receiveThread;
        private List<ResonanceUdpDiscoveredService<TDiscoveryInfo>> _discoveredServices;
        private readonly Func<ResonanceUdpDiscoveredService<TDiscoveryInfo>, ResonanceUdpDiscoveredService<TDiscoveryInfo>, bool> _discoveredServiceCompareFunc;

        /// <summary>
        /// Occurs when a matching service has been discovered.
        /// </summary>
        public event EventHandler<ResonanceDiscoveredServiceEventArgs<ResonanceUdpDiscoveredService<TDiscoveryInfo>, TDiscoveryInfo>> ServiceDiscovered;

        /// <summary>
        /// Occurs when a discovered service is no longer responding.
        /// </summary>
        public event EventHandler<ResonanceDiscoveredServiceEventArgs<ResonanceUdpDiscoveredService<TDiscoveryInfo>, TDiscoveryInfo>> ServiceLost;

        /// <summary>
        /// Gets the decoder used to decode the service information message.
        /// </summary>
        public TDecoder Decoder { get; }

        /// <summary>
        /// Gets a value indicating whether this client has started.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets the remote discovery service port.
        /// Default is 2021.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets a value indicating whether validate the discovered service existence using TCP connection.
        /// If set to false, the client will not be able to notify about discovered services disappearing.
        /// </summary>
        public bool EnableTcpValidation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResonanceUdpDiscoveryClient{TDiscoveryInfo, TDecoder}"/> class.
        /// </summary>
        public ResonanceUdpDiscoveryClient()
        {
            _componentCounter = ResonanceComponentCounterManager.Default.GetIncrement(this);
            EnableTcpValidation = true;
            Port = 2021;
            Decoder = Activator.CreateInstance<TDecoder>();
            _discoveredServiceCompareFunc = (s1, s2) => s1.Address == s2.Address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResonanceUdpDiscoveryClient{TDiscoveryInfo, TDecoder}"/> class.
        /// </summary>
        /// <param name="port">The remote discovery service port.</param>
        public ResonanceUdpDiscoveryClient(int port) : this()
        {
            Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResonanceUdpDiscoveryClient{TDiscoveryInfo, TDecoder}"/> class.
        /// </summary>
        /// <param name="port">The remote discovery service port.</param>
        /// <param name="discoveredServiceCompareFunc">The discovered service compare function.</param>
        public ResonanceUdpDiscoveryClient(int port, Func<ResonanceUdpDiscoveredService<TDiscoveryInfo>, ResonanceUdpDiscoveredService<TDiscoveryInfo>, bool> discoveredServiceCompareFunc) : this(port)
        {
            _discoveredServiceCompareFunc = discoveredServiceCompareFunc;
        }

        /// <summary>
        /// Start discovering.
        /// </summary>
        public Task Start()
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsStarted)
                {
                    Log.Info($"{this}: Starting...");

                    _udpClient = new UdpClient();
                    _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                    _udpClient.EnableBroadcast = true;

                    _discoveredServices = new List<ResonanceUdpDiscoveredService<TDiscoveryInfo>>();
                    _receiveThread = new Thread(ReceiveThreadMethod);
                    _receiveThread.IsBackground = true;


                    Log.Info($"{this}: Started.");
                    IsStarted = true;
                    _receiveThread.Start();
                }
            });
        }

        /// <summary>
        /// Stop discovering.
        /// </summary>
        public Task Stop()
        {
            return Task.Factory.StartNew(() =>
            {
                if (IsStarted)
                {
                    Log.Info($"{this}: Stopping...");

                    try
                    {
                        _udpClient?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error disposing udp client.");
                    }
                    finally
                    {
                        Log.Info($"{this}: Stopped...");
                        IsStarted = false;
                    }
                }
            });
        }

        /// <summary>
        /// Asynchronous method for collecting discovered services within the given duration.
        /// Will start the discovery client if not started already, but will not stop it on completion.
        /// </summary>
        /// <param name="maxDuration">The maximum duration to perform the scan.</param>
        /// <param name="maxServices">Drop the scanning after the maximum services discovered.</param>
        /// <returns></returns>
        public Task<List<ResonanceUdpDiscoveredService<TDiscoveryInfo>>> Discover(TimeSpan maxDuration, int? maxServices = null)
        {
            Start();

            return Task.Factory.StartNew<List<ResonanceUdpDiscoveredService<TDiscoveryInfo>>>(() => 
            {
                DateTime startTime = DateTime.Now;

                while (DateTime.Now < startTime + maxDuration)
                {
                    Thread.Sleep(10);

                    if (maxServices != null && _discoveredServices.Count >= maxServices.Value)
                    {
                        break;
                    }
                }

                if (maxServices != null)
                {
                    return _discoveredServices.Take(maxServices.Value).ToList();
                }
                else
                {
                    return _discoveredServices.ToList();
                }
            });
        }

        private void ReceiveThreadMethod()
        {
            while (IsStarted)
            {
                try
                {
                    var clientEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    var data = _udpClient.Receive(ref clientEndPoint);

                    Log.Debug($"{this}: Data received ({data.ToFriendlyByteSize()}), decoding discovery information...");

                    TDiscoveryInfo discoveryInfo = Decoder.Decode<TDiscoveryInfo>(data);

                    Log.Debug($"{this}: Discovery information decoded:\n{discoveryInfo.ToJsonString()}");

                    string address = clientEndPoint.Address.ToString();

                    Log.Debug($"{this}: Service host address: {address}.");

                    ResonanceUdpDiscoveredService<TDiscoveryInfo> discoveredService = new ResonanceUdpDiscoveredService<TDiscoveryInfo>(discoveryInfo, address);

                    //validate service existence using TCP connection.
                    if (EnableTcpValidation)
                    {
                        Log.Debug($"{this}: Validating service existence using TCP...");

                        try
                        {
                            TcpClient client = new TcpClient();
                            client.Connect(address, Port);
                            client.Dispose();
                            Log.Debug($"{this}: Service validated.");
                        }
                        catch
                        {
                            var missingService = _discoveredServices.ToList().FirstOrDefault(existingService => _discoveredServiceCompareFunc(existingService, discoveredService));

                            if (missingService != null)
                            {
                                Log.Debug($"{this}: Service TCP validation failed. Reporting service lost...");
                                _discoveredServices.Remove(missingService);
                                Log.Debug($"{this}: Total discovered services: {_discoveredServices.Count}.");
                                ServiceLost?.Invoke(this, new ResonanceDiscoveredServiceEventArgs<ResonanceUdpDiscoveredService<TDiscoveryInfo>, TDiscoveryInfo>(missingService));
                            }
                            else
                            {
                                Log.Debug($"{this}: Service TCP validation failed.");
                            }

                            continue;
                        }
                    }

                    if (!_discoveredServices.ToList().Exists(existingService => _discoveredServiceCompareFunc(existingService, discoveredService)))
                    {
                        Log.Info($"{this}: New service discovered on address {discoveredService.Address}. Reporting service discovered...");
                        _discoveredServices.Add(discoveredService);
                        Log.Debug($"{this}: Total discovered services: {_discoveredServices.Count}.");
                        ServiceDiscovered?.Invoke(this, new ResonanceDiscoveredServiceEventArgs<ResonanceUdpDiscoveredService<TDiscoveryInfo>, TDiscoveryInfo>(discoveredService));
                    }
                    else
                    {
                        Log.Debug($"{this}: Service was already discovered.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex,$"{this}: Error occurred on discovery method.");
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Disposes component resources asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task DisposeAsync()
        {
            return Stop();
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return $"UDP Discovery Client {_componentCounter}";
        }
    }
}
