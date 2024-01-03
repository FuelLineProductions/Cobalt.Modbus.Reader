using Cobalt.Modbus.Device;

namespace Cobalt.Modbus.Reader.DeviceReaders;

public interface IModbusDeviceReader
{
    /// <summary>
    ///     Reads the holding registers from the device
    /// </summary>
    /// <param name="modbusDevice"></param>
    /// <param name="reverseByteOrder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Read(ModbusDevice modbusDevice, CancellationToken cancellationToken);
}