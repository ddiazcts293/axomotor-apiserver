namespace AxoMotor.ApiServer.Models;

public class IncidentPicture
{
    public required string FileId { get; set; }
    
    public required TripPosition Position { get; set; }
}
