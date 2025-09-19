using ECFautoecole.Data;

namespace ECFautoecole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ====== CORS (autorise le front en Live Server) ======
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins(
                            "http://127.0.0.1:5500",
                            "http://localhost:5500"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // ====== MVC / Swagger ======
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ====== SQL Connection Factory ======
            builder.Services.AddScoped<SqlConnectionFactory>();

            var app = builder.Build();

            // ====== Pipeline ======
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // CORS DOIT être avant MapControllers
            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
