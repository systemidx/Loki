namespace Loki.Common.Enum.Frame
{
    /// <summary>
    /// The WebSocket OpCode
    /// </summary>
    public enum WebSocketOpCode
    {
        ContinuationFrame = 0x0,
        TextFrame = 0x1,
        BinaryFrame = 0x2,
        ConnectionClose = 0x8,
        Ping = 0x9,
        Pong = 0x10
    }
}
