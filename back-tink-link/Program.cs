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

builder.Services.AddLogging(loggingBuilder => {
	loggingBuilder.AddFile("all_requests_{0:yyyy}-{0:MM}-{0:dd}_0.log", fileLoggerOpt => {
        fileLoggerOpt.FormatLogFileName = fname => {
            return String.Format(fname, DateTime.UtcNow);
        };

        fileLoggerOpt.Append = true;
        fileLoggerOpt.MinLevel = LogLevel.Information;
        fileLoggerOpt.FileSizeLimitBytes = 1000000;
        fileLoggerOpt.MaxRollingFiles = 3;

        fileLoggerOpt.HandleFileError = (err) => {
		    err.UseNewLogFileName(Path.GetFileNameWithoutExtension(err.LogFileName)+ "_alt" + Path.GetExtension(err.LogFileName) );
	    };

        fileLoggerOpt.FilterLogEntry = (msg) => {
            return msg.LogLevel == LogLevel.Information && msg.LogName == "all_requests";;
        };
    });
});

builder.Services.AddLogging(loggingBuilder => {
	loggingBuilder.AddFile("logged_user_requests_{0:yyyy}-{0:MM}-{0:dd}_0.log", fileLoggerOpt => {
        fileLoggerOpt.FormatLogFileName = fname => {
            return String.Format(fname, DateTime.UtcNow);
        };

        fileLoggerOpt.Append = true;
        fileLoggerOpt.MinLevel = LogLevel.Information;
        fileLoggerOpt.FileSizeLimitBytes = 1000000;
        fileLoggerOpt.MaxRollingFiles = 5;

        fileLoggerOpt.HandleFileError = (err) => {
		    err.UseNewLogFileName(Path.GetFileNameWithoutExtension(err.LogFileName)+ "_alt" + Path.GetExtension(err.LogFileName) );
	    };

        fileLoggerOpt.FilterLogEntry = (msg) => {
            return msg.LogLevel == LogLevel.Information && msg.LogName == "logged_user_requests";
        };
    });
});

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

app.UseMiddleware<HttpLoggingMiddleware>();
app.InvokeExceptionHandlerLocal();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("Application Starting");
app.Run();
    