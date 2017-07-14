using System;

namespace Loki.Common.Events
{
    public class TextFrameEventArgs : EventArgs
    {
        public readonly string Message;

        public TextFrameEventArgs(string message)
        {
            Message = message;
        }
    }
}