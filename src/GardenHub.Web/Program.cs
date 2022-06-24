using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using GardenHub.Web.Services;
using GardenHub.Web.Services.Data;
using Microsoft.Net.Http.Headers;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();

builder.Services.AddHttpClient("GardenHub", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrl"));
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();