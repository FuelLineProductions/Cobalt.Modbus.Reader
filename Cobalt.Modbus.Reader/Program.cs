using Cobalt.Modbus.ProtocolDataUnit;
using Cobalt.Modbus.Reader;
using Cobalt.Modbus.Reader.DataStorage;
using Cobalt.Modbus.Reader.DeviceReaders;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<IModbusPdu, ModbusPdu>();
builder.Services.AddTransient<IStoreReadingValue, StoreReadingValue>();
builder.Services.AddTransient<IModbusDeviceReader, ModbusDeviceReader>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
var host = builder.Build();
host.Run();