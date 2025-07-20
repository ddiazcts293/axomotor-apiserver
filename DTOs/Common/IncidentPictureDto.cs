namespace AxoMotor.ApiServer.DTOs.Common;

public class IncidentPictureDto
{
    public required string FileId { get; set; }
    
    public required TripPositionDto Position { get; set; }
}
