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
var allowSpecificOrigins = "_allowSpecificOrigins";
builder.Services.AddCors(options => 
{
    options.AddPolicy(name: allowSpecificOrigins,
                        policy => {
                            policy.WithOrigins("http://localhost:5173");
                            policy.WithMethods("GET", "POST", "OPTIONS", "PUT", "PATCH", "DELETE");
                            policy.AllowAnyHeader();
                        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddHealthChecks();

builder.Services.AddJWT(builder.Configuration);

//IoC
{
    //Services
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<UserService>();    

    //Repositories
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
}


builder.Services.AddLogging(loggingBuilder => {
	loggingBuilder.AddFile("log_requests_{0:yyyy}-{0:MM}-{0:dd}_0.log", fileLoggerOpt => {
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
            return msg.LogLevel == LogLevel.Information && msg.LogName == "log_requests";;
        };
    });
});

var app = builder.Build();

// // migrate any database changes on startup (includes initial db creation)
// using (var scope = app.Services.CreateScope())
// {
//     if (env.IsProduction()){
//         var domainContext = scope.ServiceProvider.GetRequiredService<DomainContext>();    
//         domainContext.Database.Migrate();
//     }
//     else{
//         var domainContext = scope.ServiceProvider.GetRequiredService<DevContext>();    
//         domainContext.Database.Migrate();
//     }
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<HttpLoggingMiddleware>();
app.InvokeExceptionHandlerLocal();

app.UseHttpsRedirection();

// global cors policy
app.UseCors(allowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("Application Starting");
app.Run();
    