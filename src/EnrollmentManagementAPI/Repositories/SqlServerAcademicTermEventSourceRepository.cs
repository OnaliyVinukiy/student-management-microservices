using StudentSystem.EnrollmentManagementAPI.Repositories.Model; 
using System.Globalization;
namespace StudentSystem.EnrollmentManagementAPI.Repositories;

public class SqlServerAcademicTermEventSourceRepository : IEventSourceRepository<AcademicTerm>
{
    private static readonly JsonSerializerSettings _serializerSettings;
    private readonly string _connectionString;

    static SqlServerAcademicTermEventSourceRepository()
    {
        _serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
    }

    public SqlServerAcademicTermEventSourceRepository(string connectionString)
    {
        _connectionString = connectionString;
        // Init DB
        Policy.Handle<Exception>()
            .WaitAndRetryAsync(10, r => TimeSpan.FromSeconds(10), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 10 sec."); })
            .ExecuteAsync(InitializeDatabaseAsync).Wait();
    }

    public async Task<AcademicTerm> GetByIdAsync(string id)
    {
        using var conn = new SqlConnection(_connectionString);
        var aggregate = await conn.QuerySingleOrDefaultAsync<Model.Aggregate>("SELECT * FROM AcademicTerm WHERE Id = @Id", new { Id = id });

        if (aggregate == null) return null;

        var aggregateEvents = await conn.QueryAsync<Model.AggregateEvent>("SELECT * FROM AcademicTermEvent WHERE Id = @Id ORDER BY [Version]", new { Id = id });
        var events = aggregateEvents.Select(e => DeserializeEventData(e.MessageType, e.EventData));
        
        var year = int.Parse(id.Split('-')[0]);
        var month = id.Split('-')[1] == "FALL" ? 9 : 2; // Assume Fall starts in Sept, Spring in Feb
        var reconstructedDate = new DateOnly(year, month, 1);
        
        return new AcademicTerm(reconstructedDate, events);
    }

    public async Task SaveAsync(string id, int originalVersion, int newVersion, IEnumerable<Event> newEvents)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        using var transaction = conn.BeginTransaction();

        var aggregate = await conn.QuerySingleOrDefaultAsync<Model.Aggregate>("SELECT * FROM AcademicTerm WHERE Id = @Id", new { Id = id }, transaction);
        int affectedRows;
        if (aggregate != null)
        {
            affectedRows = await conn.ExecuteAsync(
                "UPDATE AcademicTerm SET [CurrentVersion] = @NewVersion WHERE [Id] = @Id AND [CurrentVersion] = @OriginalVersion",
                new { Id = id, NewVersion = newVersion, OriginalVersion = originalVersion }, transaction);
        }
        else
        {
            affectedRows = await conn.ExecuteAsync(
                "INSERT AcademicTerm ([Id], [CurrentVersion]) VALUES (@Id, @CurrentVersion)",
                new { Id = id, CurrentVersion = newVersion }, transaction);
        }

        if (affectedRows == 0)
        {
            transaction.Rollback();
            throw new ConcurrencyException();
        }

        int eventVersion = originalVersion;
        foreach (var e in newEvents)
        {
            eventVersion++;
            await conn.ExecuteAsync(
                "INSERT AcademicTermEvent ([Id], [Version], [Timestamp], [MessageType], [EventData]) VALUES (@Id, @Version, @Timestamp, @MessageType, @EventData)",
                new { Id = id, Version = eventVersion, Timestamp = DateTime.UtcNow, MessageType = e.MessageType, EventData = SerializeEventData(e) },
                transaction);
        }

        transaction.Commit();
    }

    private async Task InitializeDatabaseAsync()
    {
        using (var conn = new SqlConnection(_connectionString.Replace("EnrollmentEventStore", "master")))
        {
            await conn.OpenAsync();
            await conn.ExecuteAsync("IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='EnrollmentEventStore') CREATE DATABASE EnrollmentEventStore;");
        }

        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            await conn.ExecuteAsync(@"
                IF OBJECT_ID('AcademicTerm') IS NULL CREATE TABLE AcademicTerm ([Id] VARCHAR(50) NOT NULL PRIMARY KEY, [CurrentVersion] INT NOT NULL);
                IF OBJECT_ID('AcademicTermEvent') IS NULL CREATE TABLE AcademicTermEvent ([Id] VARCHAR(50) NOT NULL REFERENCES AcademicTerm([Id]), [Version] INT NOT NULL, [Timestamp] DATETIME2(7) NOT NULL, [MessageType] VARCHAR(100) NOT NULL, [EventData] NVARCHAR(MAX), PRIMARY KEY([Id], [Version]));
            ");
        }
    }

    private string SerializeEventData(Event eventData) => JsonConvert.SerializeObject(eventData, _serializerSettings);
    
    private Event DeserializeEventData(string messageType, string eventData)
    {
        Type eventType = Type.GetType($"StudentSystem.EnrollmentManagementAPI.Events.{messageType}, StudentSystem.EnrollmentManagementAPI");
        return JsonConvert.DeserializeObject(eventData, eventType, _serializerSettings) as Event;
    }
}