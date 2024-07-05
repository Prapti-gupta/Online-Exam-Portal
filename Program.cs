using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using onlineexamproject.Models;
using onlineexamproject.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<onlineexamprojectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("onlineexamprojectContext") ?? throw new InvalidOperationException("Connection string 'onlineexamprojectContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session state services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Enable session state
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// custom routes added to handle exam routing
app.MapControllerRoute(
    name: "exam",
    pattern: "Exam/{examId}",
    defaults: new { controller = "Exam", action = "Exam" });

app.MapControllerRoute(
    name: "result",
    pattern: "Result/{examId}",
    defaults: new { controller = "Result", action = "Result" });

app.Run();
