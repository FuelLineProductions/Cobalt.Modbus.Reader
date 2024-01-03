using Cobalt.Modbus.Device;
using Cobalt.Modbus.ProtocolDataUnit;

namespace Cobalt.Modbus.Reader.DataStorage;

public interface IStoreReadingValue
{
    /// <summary>
    ///     Stores the register value for the register and device
    /// </summary>
    /// <param name="modbusDevice"></param>
    /// <param name="access"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task StoreValue(ModbusDevice modbusDevice, ModbusAccessor access,
        ModbusPduResponseData value);
}