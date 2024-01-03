using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cobalt.Modbus.Device;
using Cobalt.Modbus.ProtocolDataUnit;

namespace Cobalt.Modbus.Reader.DataStorage;

public class StoreReadingValue : IStoreReadingValue
{
    /// <inheritdoc />
    public Task StoreValue(ModbusDevice modbusDevice, ModbusAccessor access,
        ModbusPduResponseData value)
    {
        // For now make a file, in the future make this a NoSql or SQL DB Call
        var storageModel = new StorageModel(modbusDevice, access, value);

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        var serialisedFile = JsonSerializer.Serialize(storageModel, jsonSerializerOptions);

        var path = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}storage";
        Directory.CreateDirectory(path);
        var fileName = new StringBuilder();
        fileName.Append(path);
        fileName.Append(Path.DirectorySeparatorChar);
        fileName.Append($"{modbusDevice.Name}-{access.ReadableName}-{DateTime.UtcNow:yyyyMMMMdd-hhmmss}.json");

        return File.WriteAllTextAsync(fileName.ToString(), serialisedFile);
    }
}