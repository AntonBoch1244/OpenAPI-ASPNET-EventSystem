using Serilog.Sinks.Graylog;
using Serilog;
using LinqToDB.Data;
using Microsoft.EntityFrameworkCore;

namespace OpenAPIASPNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = Environment.GetEnvironmentVariables();

            // Logging
            LoggerConfiguration loggerConfiguration = new();

            if (env["GRAYLOG_ADDRESS"] != null)
            {
                string NormalizedAddress = env["GRAYLOG_ADDRESS"].ToString().ToLower();

                GraylogSinkOptions graylog = new()
                {
                    HostnameOrAddress = NormalizedAddress
                };

                loggerConfiguration.WriteTo.Graylog(graylog);
            }

            loggerConfiguration.WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();

            builder.Host.UseSerilog(Log.Logger);
            // Logging

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Message Queue
            if ((env["AMPQ_HOSTNAME"] != null) && (env["AMPQ_PORT"] != null) && (env["AMPQ_USERNAME"] != null) && (env["AMPQ_PASSWORD"] != null))
            {
                builder.Services.RegisterEasyNetQ((config) =>
                {
                    EasyNetQ.ConnectionConfiguration cfg = new();
                    EasyNetQ.HostConfiguration hostcfg = new();
                    hostcfg.Host = env["AMPQ_HOSTNAME"].ToString().ToLower();
                    hostcfg.Port = ushort.Parse(env["AMPQ_PORT"].ToString());
                    cfg.Hosts.Add(hostcfg);
                    cfg.UserName = env["AMPQ_USERNAME"].ToString();
                    cfg.Password = env["AMPQ_PASSWORD"].ToString();
                    return cfg;
                });
            }
            // Message Queue

            // PostgreSQL
            if (env["POSTGRESQL_CONNECTION"] != null)
            {
                builder.Services.AddDbContext<Contexts.OpenAPIASPNETContext>(action => action.UseNpgsql(env["POSTGRESQL_CONNECTION"].ToString()));
            }
            // PostgreSQL

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Database Migration
            if (env["POSTGRESQL_CONNECTION"] != null)
            {
                using (IServiceScope services = app.Services.CreateScope())
                    using (Contexts.OpenAPIASPNETContext db = services.ServiceProvider.GetRequiredService<Contexts.OpenAPIASPNETContext>())
                    {
                db.Database.Migrate();
                    }
            }
            // Database Migration

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseHttpsRedirection();

            //app.UseAuthorization();
            //app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}