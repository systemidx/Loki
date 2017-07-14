using System;

namespace Loki.Common.Events
{
    public class BinaryFrameEventArgs : EventArgs
    {
        public readonly byte[] Payload;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFrameEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public BinaryFrameEventArgs(byte[] payload)
        {
            Payload = payload;
        }
    }
}