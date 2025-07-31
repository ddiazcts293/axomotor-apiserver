using System.Text.Json;
using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Scalar.AspNetCore;

ConventionRegistry.Register("EnumRepresentationConvention",
    new ConventionPack() { new EnumRepresentationConvention(BsonType.String) },
    _ => true);

ConventionRegistry.Register("CamelCaseConvention",
    new ConventionPack() { new CamelCaseElementNameConvention() }, _ => true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Establece la configuración de MongoDB
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
ConfigureMySQL(builder.Services, builder.Configuration.GetSection("MySQL"));
// Agrega el servicio de AxoMotor
ConfigureAxoMotorService(builder.Services, builder.Configuration.GetSection("MQTT"));

// Agrega los servicios para manejar colecciones en MongoDB
builder.Services.AddSingleton<TripService>();
builder.Services.AddSingleton<IncidentService>();
builder.Services.AddSingleton<PositionService>();
builder.Services.AddSingleton<DeviceEventService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void ConfigureMySQL(IServiceCollection services, IConfigurationSection config)
{
    string? connectionString = config["ConnectionString"];
    var version = new MySqlServerVersion(config["ServerVersion"]);

    services.AddDbContextPool<AxoMotorContext>(options =>
        options.UseMySql(connectionString, version)
    );
}

static void ConfigureAxoMotorService(IServiceCollection services, IConfigurationSection config)
{
    if (config.GetValue<bool>("Enable"))
    {
        services.Configure<MqttSettings>(config);
        services.AddHostedService<AxoMotorService>();
    }
}
