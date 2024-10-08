using GradutionProject.Components;
using GradutionProject.Data;
using GradutionProject.Interfaces;
using GradutionProject.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var desktopInfo = System.Net.Dns.GetHostName();
var connectionString = $"Server={desktopInfo}\\MSSQLSERVERTPS;Database=Gra;User Id=sa; Password=sas;Trusted_Connection=false; MultipleActiveResultSets=true;Encrypt=no";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Transient, ServiceLifetime.Transient
);



//Add Services
builder.Services.AddMudServices();
builder.Services.AddScoped<IAuthentication, AuthenticationRepository>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
