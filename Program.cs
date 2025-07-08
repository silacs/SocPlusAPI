using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;
using SocPlus.Utilities;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o => {
    o.Listen(IPAddress.Any, 80);
    o.Listen(IPAddress.Any, 443, o => {
        o.UseHttps();
    });
    //o.Listen(IPAddress.Any, 443, listenOptions => {
    //    listenOptions.UseHttps(h => {
    //        //h.UseLettuceEncrypt(appServices);
    //    });
    //});
    //o.ConfigureHttpsDefaults(o => {
    //    o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
    //    o.UseLettuceEncrypt(appServices);
    //});
});


builder.Configuration.CheckVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureApp(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
