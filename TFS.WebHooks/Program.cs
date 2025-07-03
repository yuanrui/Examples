using Microsoft.AspNetCore.Http.Json;

namespace TFS.Webhooks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.Title = "TFS Webhooks";
            }

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<Dictionary<string, string>>(builder.Configuration.GetSection("Account"));

            builder.Services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });
            builder.Services.AddMvc();
            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNameCaseInsensitive = false;
            });

            var app = builder.Build();

            app.UseCors(options =>
            {
                options.SetIsOriginAllowed(host => true);
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowCredentials();
            });

            app.MapControllers();

            app.Run();
        }
    }
}