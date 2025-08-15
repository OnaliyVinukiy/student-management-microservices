var builder = WebApplication.CreateBuilder(args);

// setup logging
builder.Host.UseSerilog((context, logContext) => 
    logContext.ReadFrom.Configuration(builder.Configuration).Enrich.WithMachineName());

// Add repositories
var eventStoreConnectionString = builder.Configuration.GetConnectionString("EventStoreCN");
builder.Services.AddTransient<IEventSourceRepository<AcademicTerm>>(sp => 
    new SqlServerAcademicTermEventSourceRepository(eventStoreConnectionString));

var readModelConnectionString = builder.Configuration.GetConnectionString("ReadModelCN");
builder.Services.AddTransient<ICourseRepository>(sp => new SqlServerRefDataRepository(readModelConnectionString));
builder.Services.AddTransient<IStudentRepository>(sp => new SqlServerRefDataRepository(readModelConnectionString));

// Add message publisher
builder.Services.UseRabbitMQMessagePublisher(builder.Configuration);

// Add command handlers
builder.Services.AddCommandHandlers();

// Add framework services
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Enrollment Management API", Version = "v1" });
});
builder.Services.AddHealthChecks()
    .AddSqlServer(eventStoreConnectionString, name: "EventStoreHC")
    .AddSqlServer(readModelConnectionString, name: "ReadModelHC");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enrollment Management API - v1");
});
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks("/hc");

app.Run();