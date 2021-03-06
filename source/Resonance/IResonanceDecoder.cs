using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Resonance
{
    /// <summary>
    /// Represents a Resonance decoder capable of decoding data received by an <see cref="IResonanceAdapter"/>.
    /// </summary>
    /// <seealso cref="Resonance.IResonanceComponent" />
    public interface IResonanceDecoder : IResonanceComponent
    {
        /// <summary>
        /// Gets or sets the message compression configuration.
        /// </summary>
        ResonanceCompressionConfiguration CompressionConfiguration { get; }

        /// <summary>
        /// Gets the encryption configuration.
        /// </summary>
        ResonanceEncryptionConfiguration EncryptionConfiguration { get; }

        /// <summary>
        /// Decodes the specified data and populates the specified decoding information.
        /// </summary>
        /// <param name="data">The encoded data.</param>
        /// <param name="info">The decoding information object to populate.</param>
        void Decode(byte[] data, ResonanceDecodingInformation info);

        /// <summary>
        /// Decodes the specified data and returns the <see cref="ResonanceTranscodingInformation.Message"/> as type T.
        /// </summary>
        /// <typeparam name="T">Type of expected message.</typeparam>
        /// <param name="data">The encoded data.</param>
        /// <returns></returns>
        T Decode<T>(byte[] data);

        /// <summary>
        /// Decodes a message from the specified memory stream.
        /// </summary>
        /// <param name="stream">The memory stream.</param>
        Object Decode(MemoryStream stream);
    }
}
