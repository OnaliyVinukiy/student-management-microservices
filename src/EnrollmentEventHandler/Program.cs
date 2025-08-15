IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.UseRabbitMQMessageHandler(hostContext.Configuration);

        services.AddTransient<EnrollmentDBContext>((svc) =>
        {
            var sqlConnectionString = hostContext.Configuration.GetConnectionString("EnrollmentManagementCN");
            var dbContextOptions = new DbContextOptionsBuilder<EnrollmentDBContext>()
                .UseSqlServer(sqlConnectionString)
                .Options;
            var dbContext = new EnrollmentDBContext(dbContextOptions);

            DBInitializer.Initialize(dbContext);

            return dbContext;
        });

        services.AddHostedService<EventHandlerWorker>();
    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();