using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var env = builder.Environment;

//use sql server db in production and sqlite db in development
if (env.IsProduction())
    builder.Services.AddDbContext<DomainContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
else
{
    builder.Services.AddDbContext<DevContext>(options => 
    {
        DbConnection _connection = new SqliteConnection("DataSource=localdb.db");
        _connection.Open();

        options.UseSqlite(_connection);
    });
}

builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddHealthChecks();

builder.Services.AddJWT(builder.Configuration);

//Auth
builder.Services.AddScoped<AuthService>();

//Services
builder.Services.AddScoped<UserService>();

//Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    if (env.IsProduction()){
        var domainContext = scope.ServiceProvider.GetRequiredService<DomainContext>();    
        domainContext.Database.Migrate();
    }
    else{
        var domainContext = scope.ServiceProvider.GetRequiredService<DevContext>();    
        domainContext.Database.Migrate();
    }
}

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<HttpLoggingMiddleware>();

//Initialize Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try
{
    Log.Information("Application Starting");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}