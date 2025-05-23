
using FMS.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace FMS.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=fms.db")); // 或者从配置读取连接串
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=fms.db"));

        }
    }
}
