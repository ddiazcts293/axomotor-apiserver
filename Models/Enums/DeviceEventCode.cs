namespace AxoMotor.ApiServer.Models.Enums;

public enum DeviceEventCode
{
    DeviceReset,
    StorageFull,
    StorageFailure,
    VideoRecordingStarted,
    VideoRecordingStopped,
    CameraFailure,
    ImpactDetected,
    CollisionDetected,
    MinorShockDetected,
    HarshAcceleration,
    HarshBraking,
    HarshCornering,
    RouteDeviationDetected,
    GpsSignalLost,
    GpsSignalRestored,
    PanicButtonPressed,
    TamperingDetected,
    DriverIdentified,
    AppConnected,
}
