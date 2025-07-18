namespace AxoMotor.ApiServer.Config;

public class MongoDBSettings
{
    public string ConnectionUri { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public MongoDBCollections Collections { get; set; } = null!;
}
