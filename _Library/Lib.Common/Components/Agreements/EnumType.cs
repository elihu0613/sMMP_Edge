namespace Lib.Common.Components.Agreements
{
    public enum WorkTasks
    {
        Undefined = 0,
        Collection = 1,
        Command = 2
    }

    public enum HostTransaction
    {
        Undefined = 0,
        WebService = 1,
        Kafka = 2,
        Eai = 3,
        sMMP = 4
    }

    public enum HttpMethodType
    {
        HEAD = 1,
        GET = 2,
        POST = 3,
        PUT = 4,
        PATCH = 5,
        DELETE = 6
    }

    public enum Condition
    {
        Undefined = 0,
        Success = 1,
        Error = 2
    }

    public enum Communication
    {
        Undefined = 0,
        EdgeService = 1,
        ModbusTcp = 2,
        WebApi = 3,
        OpcUa = 4,
        CsvFile = 5
    }

    public enum HostChannel
    {
        Undefined = 0,
        Status = 1,
        Parameter = 2,
        Production = 3,
        RemoteCommand = 4,
        ProductionCommand = 5,
        ParameterCommand = 6
    }
}
