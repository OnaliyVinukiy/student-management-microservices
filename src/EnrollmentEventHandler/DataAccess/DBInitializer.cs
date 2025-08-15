namespace StudentSystem.EnrollmentEventHandler.DataAccess;

public static class DBInitializer
{
    public static void Initialize(EnrollmentDBContext context)
    {
        Log.Information("Ensure EnrollmentManagement Database");

        Policy
        .Handle<Exception>()
        .WaitAndRetry(5, r => TimeSpan.FromSeconds(5), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 5 sec."); })
        .Execute(() => context.Database.Migrate());

        Log.Information("EnrollmentManagement Database available");
    }
}