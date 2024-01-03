using Cobalt.Modbus.Device;
using Cobalt.Modbus.ProtocolDataUnit;

namespace Cobalt.Modbus.Reader.DataStorage;

// Explicit inputs for primary constructor
public class StorageModel(
    string deviceName,
    byte unitId,
    string registerName,
    ushort startingAddress,
    ushort registerSpan,
    bool? boolValue,
    byte? byteValue,
    ushort? ushortValue,
    short? shortValue,
    uint? uintValue,
    int? intValue,
    float? floatValue,
    double? doubleValue,
    long? longValue,
    ulong? ulongValue,
    string? stringValue,
    DateTime recorded,
    byte[] rawContent)
{
    // Overload to take in root objects
    public StorageModel(ModbusDevice device, ModbusAccessor accessor, ModbusPduResponseData value) : this(
        device.Name, device.UnitId, accessor.ReadableName,
        accessor.StartingRegisterAddress, accessor.CountOrValue, value.BoolValue, value.ByteValue,
        value.UshortValue, value.ShortValue, value.UintValue, value.IntValue, value.FloatValue, value.DoubleValue,
        value.LongValue, value.UlongValue, value.StringValue, DateTime.UtcNow, value.Content)
    {
    }

    public string DeviceName { get; set; } = deviceName;
    public byte DeviceUnitId { get; set; } = unitId;
    public string RegisterName { get; set; } = registerName;
    public ushort StartingAddress { get; set; } = startingAddress;
    public ushort RegisterSpan { get; set; } = registerSpan;
    public bool? BoolValue { get; set; } = boolValue;
    public byte? ByteValue { get; set; } = byteValue;
    public ushort? UshortValue { get; set; } = ushortValue;
    public short? ShortValue { get; set; } = shortValue;
    public uint? UintValue { get; set; } = uintValue;
    public int? IntValue { get; set; } = intValue;
    public float? FloatValue { get; set; } = floatValue;
    public double? DoubleValue { get; set; } = doubleValue;
    public long? LongValue { get; set; } = longValue;
    public ulong? UlongValue { get; set; } = ulongValue;
    public string? StringValue { get; set; } = stringValue;
    public DateTime Recorded { get; set; } = recorded;
    public string RawContent { get; set; } = BitConverter.ToString(rawContent).Replace("-", "");
}