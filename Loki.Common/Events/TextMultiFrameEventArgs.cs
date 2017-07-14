namespace Loki.Common.Events
{
    public class TextMultiFrameEventArgs : TextFrameEventArgs
    {
        public readonly bool IsLastFrame;

        public TextMultiFrameEventArgs(string message, bool isLastFrame) : base (message)
        {
            IsLastFrame = isLastFrame;
        }
    }
}