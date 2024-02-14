
using Amazon.S3;
using Amazon;
using CamplusBetaBackend.Data;
using CamplusBetaBackend.Services.Implentations;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CamplusBetaBackend
{
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IClubService, ClubService>();
            builder.Services.AddScoped<IHostService, HostService>();

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    builder => {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            app.UseCors("AllowAll"); // Apply the CORS policy

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}