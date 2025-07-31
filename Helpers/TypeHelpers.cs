using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Helpers;

public static class TypeHelpers
{
    public static bool IsFinished(this TripStatus status) => status is
        TripStatus.Cancelled or
        TripStatus.Finished or
        TripStatus.Aborted;
}
