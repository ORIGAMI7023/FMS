using FMS.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace FMS.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 注册服务
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=fms.db"));

            // 显式绑定监听端口
            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    //options.ListenAnyIP(7050); // HTTP
            //    options.ListenAnyIP(7051, listenOptions =>
            //    {
            //        listenOptions.UseHttps();
            //    });
            //});
            //使用IIS托管，移除Kestrel监听配置

            var app = builder.Build();

            // 开发环境中启用 Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 中间件配置
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // 启动服务
            app.Run();
        }
    }
}

