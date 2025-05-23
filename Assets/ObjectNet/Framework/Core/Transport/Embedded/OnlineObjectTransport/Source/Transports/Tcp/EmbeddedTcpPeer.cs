﻿using System;
using System.Net.Sockets;

namespace com.onlineobject.objectnet.embedded.Transports.Tcp {
    /// <summary>Provides base send &#38; receive functionality for <see cref="EmbeddedTcpServer"/> and <see cref="EmbeddedTcpClient"/>.</summary>
    public abstract class EmbeddedTcpPeer
    {
        /// <inheritdoc cref="IEmbeddedPeer.Disconnected"/>
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        /// <summary>An array that incoming data is received into.</summary>
        internal readonly byte[] ReceiveBuffer;
        /// <summary>An array that outgoing data is sent out of.</summary>
        internal readonly byte[] SendBuffer;

        /// <summary>The default size used for the socket's send and receive buffers.</summary>
        protected const int DefaultSocketBufferSize = 1024 * 1024; // 1MB
        /// <summary>The size to use for the socket's send and receive buffers.</summary>
        protected readonly int socketBufferSize;
        /// <summary>The main socket, either used for listening for connections or for sending and receiving data.</summary>
        protected Socket socket;
        /// <summary>The minimum size that may be used for the socket's send and receive buffers.</summary>
        private const int MinSocketBufferSize = 256 * 1024; // 256KB

        /// <summary>Initializes the transport.</summary>
        /// <param name="socketBufferSize">How big the socket's send and receive buffers should be.</param>
        protected EmbeddedTcpPeer(int socketBufferSize = DefaultSocketBufferSize)
        {
            if (socketBufferSize < MinSocketBufferSize)
                throw new ArgumentOutOfRangeException(nameof(socketBufferSize), $"The minimum socket buffer size is {MinSocketBufferSize}!");

            this.socketBufferSize = socketBufferSize;
            ReceiveBuffer = new byte[EmbeddedMessage.MaxSize];
            SendBuffer = new byte[EmbeddedMessage.MaxSize + sizeof(int)]; // Need room for the entire message plus the message length (since this is TCP)
        }

        /// <summary>Handles received data.</summary>
        /// <param name="amount">The number of bytes that were received.</param>
        /// <param name="fromConnection">The connection from which the data was received.</param>
        protected internal abstract void OnDataReceived(int amount, EmbeddedTcpConnection fromConnection);

        /// <summary>Invokes the <see cref="Disconnected"/> event.</summary>
        /// <param name="connection">The closed connection.</param>
        /// <param name="reason">The reason for the disconnection.</param>
        protected internal virtual void OnDisconnected(EmbeddedConnection connection, DisconnectReason reason)
        {
            Disconnected?.Invoke(this, new DisconnectedEventArgs(connection, reason));
        }
    }
}
