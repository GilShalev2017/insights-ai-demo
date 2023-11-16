using FRServer.FrHub;
using FRServer.Utility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          );
});
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
builder.Services.AddSingleton<ProcessManager>();
var app = builder.Build();
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());// .WithOrigins("http://localhost:4200;"));
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/chatHub");

// Check for the StaticFiles directory
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")))
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles"));
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles\Audios"));
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles\Faces"));
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles\Videos"));
}


app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
    RequestPath = new PathString("/StaticFiles")
});
app.UseAuthorization();
app.MapControllers();

app.Run();
