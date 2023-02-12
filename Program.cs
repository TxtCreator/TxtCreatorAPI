using Microsoft.AspNetCore.Cors.Infrastructure;
using TxtCreatorAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ITxtService, TxtService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("UI", 
        new CorsPolicyBuilder()
            .WithOrigins("https://txtcreator.pl")    
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials().Build());
});
var app = builder.Build();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors("UI");
app.MapControllers();
app.Run();