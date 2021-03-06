using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resonance.SignalR.Clients
{
    /// <summary>
    /// Represents a SignalR client.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ISignalRClient : IDisposable
    {
        /// <summary>
        /// Gets the hub URL in SignalR core, or service url/hub in SignalR legacy.
        /// </summary>
        String Url { get; }

        /// <summary>
        /// Starts the connection.
        /// </summary>
        /// <returns></returns>
        Task Start();

        /// <summary>
        /// Stops the connection.
        /// </summary>
        /// <returns></returns>
        Task Stop();

        /// <summary>
        /// Gets a value indicating whether this client has started.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Invokes the specified hub method without expecting a return value.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        Task Invoke(String methodName, params object[] args);

        /// <summary>
        /// Invokes the specified hub method and return a value of type T.
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        Task<T> Invoke<T>(String methodName, params object[] args);

        /// <summary>
        /// Register a callback method for a hub event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="action">The callback.</param>
        /// <returns></returns>
        IDisposable On(String eventName, Action action);

        /// <summary>
        ///Register a callback method for a hub event.
        /// </summary>
        /// <typeparam name="T">Type of expected callback parameter.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="action">The callback.</param>
        /// <returns></returns>
        IDisposable On<T>(String eventName, Action<T> action);

        /// <summary>
        ///Register a callback method for a hub event.
        /// </summary>
        /// <typeparam name="T1">Type of first expected callback parameter.</typeparam>
        /// <typeparam name="T2">Type of second expected callback parameter.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="action">The callback.</param>
        /// <returns></returns>
        IDisposable On<T1, T2>(String eventName, Action<T1, T2> action);

        /// <summary>
        ///Register a callback method for a hub event.
        /// </summary>
        /// <typeparam name="T1">Type of first expected callback parameter.</typeparam>
        /// <typeparam name="T2">Type of second expected callback parameter.</typeparam>
        /// <typeparam name="T3">Type of third expected callback parameter.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="action">The callback.</param>
        /// <returns></returns>
        IDisposable On<T1, T2, T3>(String eventName, Action<T1, T2, T3> action);

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <returns></returns>
        Task DisposeAsync();
    }
}
