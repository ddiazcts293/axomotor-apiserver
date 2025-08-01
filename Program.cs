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

// 💡 Agrega CORS con política abierta para desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Configuración de base de datos
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
ConfigureMySQL(builder.Services, builder.Configuration.GetSection("MySQL"));
// Agrega el servicio de AxoMotor
ConfigureAxoMotorService(builder.Services, builder.Configuration.GetSection("MQTT"));

builder.Services.AddSingleton<TripService>();
builder.Services.AddSingleton<IncidentService>();
builder.Services.AddSingleton<PositionService>();
builder.Services.AddSingleton<DeviceEventService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// ✅ Usa la política CORS antes de Authorization
app.UseCors("AllowAll");

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
