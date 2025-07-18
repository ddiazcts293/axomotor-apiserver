using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class IncidentService
{
    private readonly IMongoCollection<Incident> _collection;

    public IncidentService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);

        _collection = database.GetCollection<Incident>(
            settings.Value.Collections.Incidents
        );
    }

    public async Task CreateAsync(Incident incident)
    {
        incident.CreationDate = DateTimeOffset.UtcNow;
        await _collection.InsertOneAsync(incident);
    }

    public async Task<Incident?> GetAsync(string incidentId) =>
        await _collection.Find(x => x.Id == incidentId)
            .SingleOrDefaultAsync();

    public async Task<IList<Incident>> GetAsync(
        string? code = null,
        IncidentType? type = null,
        IncidentStatus? status = null,
        IncidentPriority? priority = null,
        string? revisedBy = null,
        string? closedBy = null,
        DateTimeOffset? periodStart = null,
        DateTimeOffset? periodEnd = null
    )
    { 
        var builder = Builders<Incident>.Filter;
        List<FilterDefinition<Incident>> filters = [];

        if (!string.IsNullOrWhiteSpace(code))
        {
            filters.Add(builder.Eq(x => x.Code, code));
        }
        if (type is not null)
        {
            filters.Add(builder.Eq(x => x.Type, type));
        }
        if (status is not null)
        {
            filters.Add(builder.Eq(x => x.Status, status));
        }
        if (priority is not null)
        {
            filters.Add(builder.Eq(x => x.Priority, priority));
        }
        if (!string.IsNullOrWhiteSpace(revisedBy))
        {
            filters.Add(builder.Eq(x => x.RevisedBy, revisedBy));
        }
        if (!string.IsNullOrWhiteSpace(closedBy))
        {
            filters.Add(builder.Eq(x => x.ClosedBy, closedBy));
        }
        
        if (filters.Count == 0)
        {
            filters.Add(builder.Empty);
        }

        return await _collection.Find(builder.And(filters)).ToListAsync();
    }

    public async Task<bool> UpdateAsync(
        string incidentId,
        IncidentStatus? status = null,
        IncidentPriority? priority = null,
        string? comments = null,
        string? relatedIncident = null
    )
    {
        var filter = Builders<Incident>.Filter.Eq(x => x.Id, incidentId);
        var update = Builders<Incident>.Update.CurrentDate(x => x.LastUpdateDate);

        if (status is not null)
        {
            update = update.Set(x => x.Status, status);
        }
        if (priority is not null)
        {
            update = update.Set(x => x.Priority, priority);
        }
        if (comments is not null)
        {
            update = update.Set(x => x.Comments, comments);
        }
        if (relatedIncident is not null)
        {
            update = update.Set(x => x.RelatedIncident, relatedIncident);
        }

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string incidentId)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == incidentId);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
