using MediatR;

namespace Consumer.Models;

public class DebeziumMessage : IRequest
{
    public DebeziumSchema Schema { get; set; }
    public DebeziumPayload Payload { get; set; }
}

public class DebeziumSchema
{
    public string Type { get; set; } // Should always be "struct"
    public bool Optional { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
    public List<DebeziumField> Fields { get; set; }
}

public class DebeziumField
{
    public string Type { get; set; } // Can be "int32", "string", "struct", etc.
    public bool Optional { get; set; }
    public string Field { get; set; } // Name of the property within the struct
    public string NameSpace { get; set; } // Optional property for nested structs
}

public class DebeziumPayload
{
    public DebeziumBefore Before { get; set; }
    public DebeziumAfter After { get; set; }
    public DebeziumSource Source { get; set; }
    public string Op { get; set; } // Operation type like "c" (create)
    public long TsMs { get; set; } // Timestamp in milliseconds
    public DebeziumTransaction Transaction { get; set; }
}

public class DebeziumBefore
{
    public int? Id { get; set; } // Nullable int for optional field
    public string Name { get; set; }
}

public class DebeziumAfter
{
    public int Id { get; set; } // Not nullable for required field
    public string Name { get; set; }
}

public class DebeziumSource
{
    public string Version { get; set; }
    public string Connector { get; set; }
    public string Name { get; set; }
    public long TsMs { get; set; }
    public string Snapshot { get; set; }
    public string Db { get; set; }
    public string Sequence { get; set; }
    public string Schema { get; set; }
    public string Table { get; set; }
    public string ChangeLsn { get; set; }
    public string CommitLsn { get; set; }
    public long? EventSerialNo { get; set; } // Nullable long for optional field
}

public class DebeziumTransaction
{
    public string Id { get; set; }
    public long TotalOrder { get; set; }
    public long DataCollectionOrder { get; set; }
}