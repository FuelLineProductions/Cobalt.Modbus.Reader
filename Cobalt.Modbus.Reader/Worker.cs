using Cobalt.Modbus.Device;
using Cobalt.Modbus.FunctionCodes;
using Cobalt.Modbus.Reader.DeviceReaders;
using Serilog;

namespace Cobalt.Modbus.Reader;

public class Worker(IConfiguration configuration, IModbusDeviceReader deviceReader) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IModbusDeviceReader _modbusDeviceReader = deviceReader;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Worker running at: {time}", DateTimeOffset.Now);

            // Get modbus devices from appsettings configuration
            var modbusDevices = _configuration.GetSection("ModbusDevices").GetChildren();
            var devices = new List<ModbusDevice>();
            foreach (var deviceConfig in modbusDevices)
            {
                // Get device base setup
                var ip = deviceConfig.GetSection("IPAddress").Value;
                var port = deviceConfig.GetValue<int>("Port");
                var unitId = deviceConfig.GetValue<byte>("UnitId");
                var protocolId = deviceConfig.GetValue<ushort>("ProtocolId");
                var name = deviceConfig.GetSection("Name").Value;

                // Get accessors
                var accessorConfigs = deviceConfig.GetSection("Accessors").GetChildren();
                var accessors = (from config in accessorConfigs
                    let startingAddress = config.GetValue<ushort>("StartingAddress")
                    let readCountOrWriteValue = config.GetValue<ushort>("ReadCountOrValueWrite")
                    let readableName = config.GetSection("ReadableName").Value
                    let functionCode = config.GetValue<byte>("FunctionCode")
                    let writeValue = config.GetValue<ushort?>("WriteValue")
                    let reverseByteOrder = config.GetValue<bool>("ReverseReadByteOrder")
                    select new ModbusAccessor((ModbusFunctionCodes.FunctionCode)functionCode, startingAddress,
                        readableName, readCountOrWriteValue, reverseByteOrder, writeValue)).ToList();

                // Add new device
                devices.Add(new ModbusDevice(ip ?? throw new InvalidOperationException(), port, unitId,
                    accessors, name ?? throw new InvalidOperationException(), protocolId));
            }

            // Setup task runners
            List<Task> tasks = [];
            tasks.AddRange(devices.Select(device => _modbusDeviceReader.Read(device, stoppingToken)));

            var task = Task.WhenAll(tasks);

            await task.WaitAsync(stoppingToken);

            if (task.IsCompletedSuccessfully)
                Log.Information("All device read tasks completed");
            else
                Log.Warning("Some device read tasks failed");

            var pollTime = _configuration.GetValue<int>("PollIntervalSeconds");
            var delaySpan = DateTime.UtcNow.AddSeconds(pollTime) - DateTime.UtcNow;
            Log.Information("Delay Span: {s}", delaySpan);
            await Task.Delay(delaySpan, stoppingToken);
        }
    }
}