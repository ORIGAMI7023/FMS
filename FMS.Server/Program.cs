using FMS.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace FMS.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ע�����
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=fms.db"));

            // ��ʽ�󶨼����˿�
            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    //options.ListenAnyIP(7050); // HTTP
            //    options.ListenAnyIP(7051, listenOptions =>
            //    {
            //        listenOptions.UseHttps();
            //    });
            //});
            //ʹ��IIS�йܣ��Ƴ�Kestrel��������

            var app = builder.Build();

            // �������������� Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // �м������
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // ��������
            app.Run();
        }
    }
}

