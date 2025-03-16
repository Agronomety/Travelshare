using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TravelshareBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;



var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped(sp => 
{
    
    var apiUrl = builder.Environment.IsDevelopment()
        ? (builder.Configuration["ApiUrl"] ?? "https://localhost:7091")
        : "https://travelshare-h8f5.onrender.com"; 
    return new HttpClient { BaseAddress = new Uri(apiUrl) };
});

builder.Services.AddScoped<BlogPostService>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

if (app.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();



app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();