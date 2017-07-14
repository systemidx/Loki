namespace Loki.Common.Enum.Frame
{
    public enum WebSocketCloseCode
    {
        Normal = 1000,
        GoingAway = 1001,
        ProtocolError = 1002,
        DataTypeNotSupported = 1003,
        Reserved = 1004,
        ReservedNoStatusCode = 1005,
        ReservedAbnormalClosure = 1006,
        DataTypeMismatch = 1007,
        PolicyViolation = 1008,
        MessageTooLarge = 1009,
        EndpointExpectsExtension = 1010,
        ServerInternalError = 1011,
        ReservedTlsFailure = 1015
    }
}
