using FinShark.Data;
using FinShark.Services.CommentService;
using FinShark.Services.StockService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace FinShark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
