using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public UserAccountType Type { get; set; }

    public UserAccountStatus Status { get; set; }

    public bool IsLoggedIn { get; set; }

    [BsonIgnoreIfNull]
    public IList<UserSession>? Sessions { get; init; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset RegistrationDate { get; set; }

    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastLogInDate { get; set; }
}
