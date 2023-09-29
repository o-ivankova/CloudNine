using Azure.Identity;
using Azure.Storage.Queues;
using CloudTestingApp.Services;
using Microsoft.Extensions.Azure;

namespace CloudTestingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<WallpaperInfoService, WallpaperInfoService>();
            builder.Services.AddSingleton<ToDoItemService, ToDoItemService>();

            builder.Services.AddHostedService<ToDoItemQueueService>();

            builder.Services.AddAzureClients(builder =>
            {
                builder.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
                {
                    var credential = new DefaultAzureCredential();
                    var queueUri = new Uri("https://ivankova1.queue.core.windows.net/myq");
                    return new QueueClient(queueUri, credential);
                });

                builder.AddBlobServiceClient(new Uri("https://ivankova1.blob.core.windows.net"));
            });

            //builder.Configuration.AddAzureKeyVault(builder.Configuration["VaultUri"]);
            var a = builder.Configuration["VaultUri"];
            builder.Configuration.AddAzureKeyVault(
                    vaultUri: new Uri(builder.Configuration["VaultUri"]),
                    credential: new DefaultAzureCredential(),
                    options: new Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions
                    {
                        ReloadInterval = TimeSpan.FromMinutes(5)
                    });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();


        }
    }
}