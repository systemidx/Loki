namespace Loki.Common.Events
{
    public class BinaryMultiFrameEventArgs : BinaryFrameEventArgs
    {
        public readonly bool IsLastFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMultiFrameEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="isLastFrame">if set to <c>true</c> [is last frame].</param>
        public BinaryMultiFrameEventArgs(byte[] payload, bool isLastFrame) : base (payload)
        {
            IsLastFrame = isLastFrame;
        }
    }
}