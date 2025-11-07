using Microsoft.EntityFrameworkCore;
using OhMyNails.Services;
using OhMyNails.Data;

var builder = WebApplication.CreateBuilder(args);

// Conexion con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CitaService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<RecordatorioService>();
builder.Services.AddSession();
var app = builder.Build();
app.UseSession();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
