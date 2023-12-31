using Serilog.Sinks.Graylog;
using Serilog;
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
            if ((env["AMQP_HOSTNAME"] != null) && (env["AMQP_PORT"] != null) && (env["AMQP_USERNAME"] != null) && (env["AMQP_PASSWORD"] != null))
            {
                builder.Services.RegisterEasyNetQ((config) =>
                {
                    EasyNetQ.ConnectionConfiguration cfg = new();
                    EasyNetQ.HostConfiguration hostcfg = new()
                    {
                        Host = env["AMQP_HOSTNAME"].ToString().ToLower(),
                        Port = ushort.Parse(env["AMQP_PORT"].ToString())
                    };
                    cfg.Hosts.Add(hostcfg);
                    cfg.UserName = env["AMQP_USERNAME"].ToString();
                    cfg.Password = env["AMQP_PASSWORD"].ToString();
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