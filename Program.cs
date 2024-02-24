using CamplusBetaBackend.Data;
using CamplusBetaBackend.Services.Implentations;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;

namespace CamplusBetaBackend
{
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            Env.Load();

            // Add services to the container.

            builder.Services.AddControllers();
            // builder.Services.AddCors();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options => {
                PasswordOptions? passwordSettings = builder.Configuration.GetSection("IdentityOptions:Password").Get<PasswordOptions>();
                options.Password = passwordSettings;
            });

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

            app.UseCors(policy =>
                    policy.WithOrigins("http://localhost:3000") // replace with your React app's origin
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}