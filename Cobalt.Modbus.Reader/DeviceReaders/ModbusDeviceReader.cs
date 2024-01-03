using Cobalt.Modbus.Connections;
using Cobalt.Modbus.Device;
using Cobalt.Modbus.ProtocolDataUnit;
using Cobalt.Modbus.Reader.DataStorage;
using Serilog;

namespace Cobalt.Modbus.Reader.DeviceReaders;

public class ModbusDeviceReader(IModbusPdu modbusPdu, IStoreReadingValue store)
    : IModbusDeviceReader
{
    private readonly IModbusPdu _modbusPdu = modbusPdu;
    private readonly IStoreReadingValue _storeReadingValue = store;

    /// <inheritdoc />
    public async Task Read(ModbusDevice modbusDevice, CancellationToken cancellationToken)
    {
        ushort transactionId = 0;
        foreach (var accessor in modbusDevice.Accessors)
        {
            try
            {
                var registerValue = await Reader(modbusDevice, accessor, transactionId);
                await _storeReadingValue.StoreValue(modbusDevice, accessor, registerValue);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(
                    "Failed to read on transaction ID {txn} device {deviceName} accessor {accessorName}  with error: {ex}",
                    transactionId, modbusDevice.Name, accessor.ReadableName, ex);
            }

            transactionId++;
            if (transactionId > 10000) transactionId = 0;
        }
    }

    /// <summary>
    ///     Generic response reader
    /// </summary>
    /// <returns></returns>
    private async Task<ModbusPduResponseData> Reader(ModbusDevice modbusDevice, ModbusAccessor accessor,
        ushort transactionId)
    {
        // Attempt to reconnect if disconnected
        if (!modbusDevice.TcpConnection.TcpClient.Connected)
            modbusDevice.TcpConnection =
                new TcpConnection(modbusDevice.TcpConnection.IpAddress, modbusDevice.TcpConnection.Port);

        var request = _modbusPdu.BuildPduRequest(accessor);
        var packet = new ModbusTcpPacket(transactionId, modbusDevice.UnitId, request.RequestData,
            modbusDevice.ProtocolId);
        var response = await modbusDevice.TcpConnection.ReadPacket(packet.PacketAsByteArray());
        var registerValue = _modbusPdu.ReadResponse(response);
        registerValue.ConvertContentToType(accessor.ReverseResponseByteOrder);
        return registerValue;
    }
}