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
    MinorShockDetected,
    HarshAcceleration,
    HarshBraking,
    HarshCornering,
    GpsSignalLost,
    GpsSignalRestored,
    PanicButtonPressed,
    TamperingDetected,
    AppConnected,
    AppDisconnected,
    DriverIdentified,
}
