using Microsoft.AspNetCore.Diagnostics;
using Milimoe.Oshima.Core.Configs;

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

app.Urls.Add("http://0.0.0.0:27799");

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

app.Run();

static void OSMCoreInit(){
    GeneralSettings.LoadSetting();
    GeneralSettings.SaveConfig();
    Daily.InitDaily();
    SayNo.InitSayNo();
    Ignore.InitIgnore();
}