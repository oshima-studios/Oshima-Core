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
                    // 清空运势
                    Daily.ClearDaily();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("已重置所有人的今日运势。");
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
            // OSM指令
            if (read.Length >= 4 && read[..4].Equals(".osm", StringComparison.CurrentCultureIgnoreCase))
            {
                //MasterCommand.Execute(read, GeneralSettings.Master, false, GeneralSettings.Master, false);
                Console.WriteLine("暂不支持OSM Core指令。");
                continue;
            }
            switch (read.ToLower().Trim() ?? "")
            {
                case "debug on":
                    GeneralSettings.IsDebug = true;
                    Console.WriteLine("开启Debug模式");
                    break;
                case "debug off":
                    GeneralSettings.IsDebug = false;
                    Console.WriteLine("关闭Debug模式");
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