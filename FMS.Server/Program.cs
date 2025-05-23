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
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7050); // 明确绑定到 0.0.0.0:7050
            });

            var app = builder.Build();

            // 配置监听地址（冗余安全写法，可保留）
            app.Urls.Add("http://0.0.0.0:7050");

            // 开发环境中启用 Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 中间件配置
            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // 启动服务
            app.Run();
        }
    }
}
