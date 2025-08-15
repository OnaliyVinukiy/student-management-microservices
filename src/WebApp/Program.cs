var builder = WebApplication.CreateBuilder(args);

//builder.UseKestrel();
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

// setup logging
builder.Host.UseSerilog((context, logContext) => 
    logContext
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithMachineName()
);

// Add framework services
builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false)
    .AddNewtonsoftJson();

// Add health checks
builder.Services.AddHealthChecks();

// add custom services
builder.Services.AddHttpClient<IStudentManagementAPI, StudentManagementAPI>(c => 
    c.BaseAddress = new Uri(Configuration["Services:StudentManagementAPI"]));
builder.Services.AddHttpClient<ICourseManagementAPI, CourseManagementAPI>(c => 
    c.BaseAddress = new Uri(Configuration["Services:CourseManagementAPI"]));
builder.Services.AddHttpClient<IEnrollmentAPI, EnrollmentAPI>(c => 
    c.BaseAddress = new Uri(Configuration["Services:EnrollmentManagementAPI"]));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseBrowserLink();
}
else
{
    app.UseHsts();
    app.UseExceptionHandler("/Home/Error");
}

app.UseMvc();
//app.UseDefaultFiles();
app.UseStaticFiles();


app.UseHealthChecks("/hc");

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();