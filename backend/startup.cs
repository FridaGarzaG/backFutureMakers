using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using backend.Data;  // Asegúrate que el namespace y la clase AppDbContext coincidan con tu proyecto
using System;

namespace backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string baseConnection = Configuration.GetConnectionString("DefaultConnection"); // termina en Password=
            string password = Configuration["DBPassword"]; // leído desde secrets.json

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("La contraseña para la base de datos no está configurada.");
            }

            string fullConnectionString = $"{baseConnection}{password}";

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(fullConnectionString, new MySqlServerVersion(new Version(8, 0, 26))));

            // 👉 Agregar CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFlutter", builder =>
                {
                    builder.WithOrigins("http://localhost:59848") // Cambia si Flutter usa otro puerto
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Verificación de conexión a MySQL al iniciar la app
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    if (db.Database.CanConnect())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✅ Conexión exitosa a la base de datos MySQL.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ No se pudo conectar a la base de datos.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Error de conexión a la base de datos: {ex.Message}");
                }
                Console.ResetColor();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // 👉 Activar CORS
            app.UseCors("AllowFlutter");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
