using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Mqtt;
using AxoMotor.ApiServer.Exceptions;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace AxoMotor.ApiServer.Services;

public partial class AxoMotorService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AxoMotorService> _logger;
    private readonly MqttSettings _mqttSettings;
    private readonly AxoMotorSettings _axomotorSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public AxoMotorService(
        IServiceProvider services,
        ILogger<AxoMotorService> logger,
        IOptions<MqttSettings> mqttOptions,
        IOptions<AxoMotorSettings> axomotorOptions
    )
    {
        _services = services;
        _logger = logger;
        _mqttSettings = mqttOptions.Value;
        _axomotorSettings = axomotorOptions.Value;
        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new JsonStringEnumConverter<TripPositionSource>(
                    JsonNamingPolicy.CamelCase
                ),
                new JsonStringEnumConverter<DeviceEventCode>(
                    JsonNamingPolicy.CamelCase
                ),
                new JsonStringEnumConverter<KPIStatus>(
                    JsonNamingPolicy.CamelCase
                )
            }
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // crea y configura un cliente MQTT
        var mqttFactory = new MqttClientFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_mqttSettings?.Host ?? "localhost", _mqttSettings?.Port ?? 1883);

        if (!string.IsNullOrWhiteSpace(_mqttSettings?.ClientId))
        {
            mqttClientOptionsBuilder.WithClientId(_mqttSettings?.ClientId);
        }

        if (!string.IsNullOrWhiteSpace(_mqttSettings?.User))
        {
            mqttClientOptionsBuilder.WithCredentials(
                _mqttSettings.User,
                _mqttSettings?.Password ?? string.Empty
            );
        }

        // establece las suscripciones de MQTT donde los dispositivos publican
        // mensajes
        var mqttClientOptions = mqttClientOptionsBuilder.Build();
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter("device/+/event")
            .WithTopicFilter("device/+/ping/pong")
            .WithTopicFilter("trip/+/position")
            .WithTopicFilter("trip/+/stats")
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

        await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);

        if (_axomotorSettings.KpiUpdateInterval == 0)
        {
            _axomotorSettings.KpiUpdateInterval = 10;
        }

        TimeSpan interval = TimeSpan.FromSeconds(_axomotorSettings.KpiUpdateInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishKpiValues(mqttClient, stoppingToken);
            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task PublishKpiValues(IMqttClient client, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateAsyncScope();
        var kpiService = scope.ServiceProvider.GetRequiredService<KpiService>();
        var kpiValueSet = await kpiService.GetKpiValues(cancellationToken);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(kpiValueSet, _jsonOptions);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic("dashboard")
            .WithPayload(payload)
            .WithRetainFlag(true)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
            .Build();

        await client.PublishAsync(message, cancellationToken);
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string clientId = e.ClientId;
        string topic = e.ApplicationMessage.Topic;
        long length = e.ApplicationMessage.Payload.Length;

        _logger.LogInformation(
            "Message published on {topic} by {clientId} ({length} bytes)",
            topic,
            clientId,
            length
        );

        try
        {
            string payload = e.ApplicationMessage.ConvertPayloadToString();

            if (topic.StartsWith("device/"))
            {
                int vehicleId = GetDeviceIdFromTopic(e.ApplicationMessage.Topic);

                if (topic.EndsWith("/event"))
                {
                    // deserializa el JSON
                    var message = JsonSerializer.Deserialize<RegisterDeviceEventMessage>(
                        payload, _jsonOptions);

                    AxomotorMqttReceiverException.ThrowIfNull(message);
                    message!.VehicleId = vehicleId;
                    await RegisterEvent(message!);
                }
            }
            else if (topic.StartsWith("trip/"))
            {
                string tripId = GetTripIdFromTopic(e.ApplicationMessage.Topic);

                if (topic.EndsWith("/position"))
                {
                    // deserializa el JSON
                    var message = JsonSerializer.Deserialize<RegisterTripPositionMessage>(
                        payload, _jsonOptions);

                    AxomotorMqttReceiverException.ThrowIfNull(message);
                    message!.TripId = tripId;
                    await RegisterPosition(message);
                }
                else if (topic.EndsWith("/stats"))
                {
                    // registrar estadísticas
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("MQTT receiver exception: {message}", ex.Message);
        }
    }

    private async Task RegisterPosition(RegisterTripPositionMessage message)
    {
        // obtiene el modelo
        var model = message.ToValidatedModel();

        // crea una instancia del servicio de registro de posiciones
        using var scope = _services.CreateAsyncScope();
        var positionService = scope.ServiceProvider.GetRequiredService<PositionService>();
        var tripService = scope.ServiceProvider.GetRequiredService<TripService>();
        var tripStatus = await tripService.GetStatus(message.TripId);

        // verifica si el viaje existe
        if (!tripStatus.HasValue)
            throw new AxomotorMqttReceiverException("Trip not found");
        // verifica si el viaje está activo (en ruta o señal de GPS perdida)
        else if (tripStatus.Value is not (TripStatus.GpsSignalLost or TripStatus.OnRoute))
            throw new AxomotorMqttReceiverException("Trip is not active");

        await positionService.PushOneAsync(model);
    }

    private async Task RegisterEvent(RegisterDeviceEventMessage message)
    {
        // valida y obtiene el modelo y lo registra
        var model = message.ToValidatedModel();

        // crea una instancia del servicio de registro de eventos
        using var scope = _services.CreateAsyncScope();
        var eventService = scope.ServiceProvider.GetRequiredService<DeviceEventService>();
        // instancia de la base de datos
        var dbContext = scope.ServiceProvider.GetRequiredService<AxoMotorContext>();

        // obtiene el vehículo asociado
        var vehicle = await dbContext.Vehicles.FindAsync(message.VehicleId);
        var eventInfo = await dbContext.DeviceEventCatalog.FindAsync(model.Code)
            ?? throw new AxomotorMqttReceiverException("Invalid event code");

        if (vehicle is null)
            throw new AxomotorMqttReceiverException("Vehicle not found");
        else if (!vehicle.InUse)
            throw new AxomotorMqttReceiverException("Vehicle is not in use");
        
        model.Type = eventInfo.Type;
        model.Severity = eventInfo.Severity;
        await eventService.PushOneAsync(model);
    }

    private static string GetTripIdFromTopic(string topic)
    {
        Match match = TopicTripIdRegex().Match(topic);
        if (match.Success)
            return match.Groups[1].Value;

        throw new AxomotorMqttReceiverException("Invalid trip identifier");
    }

    private static int GetDeviceIdFromTopic(string topic)
    {
        Match match = TopicDeviceIdRegex().Match(topic);
        if (match.Success)
            return int.Parse(match.Groups[1].Value);

        throw new AxomotorMqttReceiverException("Invalid device identifier");
    }

    [GeneratedRegex(@"(?:^trip/)([\da-fA-F]+)(?:/\w+)")]
    private static partial Regex TopicTripIdRegex();

    [GeneratedRegex(@"(?:^device/)(\d+)(?:/\w+)")]
    private static partial Regex TopicDeviceIdRegex();
}
