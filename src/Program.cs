using Microsoft.AspNetCore.Diagnostics;
using Milimoe.Oshima.Core.Configs;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    OSMCoreInit();

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature != null)
            {
                await context.Response.WriteAsync(new
                {
                    context.Response.StatusCode,
                    Message = "Internal Server Error.",
                    Detailed = contextFeature.Error.Message
                }.ToString() ?? "");
            }
        });
    });

    _ = Task.Factory.StartNew(async () =>
    {
        while (true)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (now.Hour == 8 && now.Minute == 30 && !Daily.DailyNews)
                {
                    Daily.DailyNews = true;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if (now.Hour == 8 && now.Minute == 31)
                {
                    Daily.DailyNews = false;
                }
                if (now.Hour == 0 && now.Minute == 0 && Daily.ClearDailys)
                {
                    Daily.ClearDailys = false;
                    // �������
                    Daily.ClearDaily();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("�����������˵Ľ������ơ�");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if (now.Hour == 0 && now.Minute == 1)
                {
                    Daily.ClearDailys = true;
                }
                await Task.Delay(1000);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    });

    _ = Task.Factory.StartNew(() =>
    {
        while (true)
        {
            string read = Console.ReadLine() ?? "";
            if (read == "quit" || read == "exit") break;
            // OSMָ��
            if (read.Length >= 4 && read[..4].Equals(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Console.WriteLine("�ݲ�֧��OSM Coreָ�");
                continue;
            }
            switch (read.ToLower().Trim() ?? "")
            {
                case "debug on":
                    GeneralSettings.IsDebug = true;
                    Console.WriteLine("����Debugģʽ");
                    break;
                case "debug off":
                    GeneralSettings.IsDebug = false;
                    Console.WriteLine("�ر�Debugģʽ");
                    break;
            }
        }
    });

    app.Run();
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e);
    Console.ForegroundColor = ConsoleColor.Gray;
}

Console.ReadKey();

static void OSMCoreInit()
{
    GeneralSettings.LoadSetting();
    GeneralSettings.SaveConfig();
    Daily.InitDaily();
    SayNo.InitSayNo();
    Ignore.InitIgnore();
}