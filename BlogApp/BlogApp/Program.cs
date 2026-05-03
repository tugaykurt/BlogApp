using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BlogContext>(options =>
{
    //options.UseSqlite(builder.Configuration.GetConnectionString("sql_connection"));
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer_connection"));
});
builder.Services.AddScoped<IPostRepository, EFPostRepository>();
builder.Services.AddScoped<ITagRepository, EFTagRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();
builder.Services.AddScoped<IUserRepository, EFUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login";
});

var app = builder.Build();

SeedData.TestVerileriniDoldur(app);  


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

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

// localhost://post/react-dersleri
// localhost://post/php
// Aþaðýda default olan route pattern'ýnýn haricinde bir pattern daha tanýmladýk, yani bir endpoint oluþturduk. Peki bu bizi nereye ve nasýl götürüyor?: post-details adýndaki route pattern'ý, pattern olarak post/{url} alýr. Buradaki, {} içinde olmayan kýsým sabit bir route string'i. Yani route'a istek geldiðinde, eðer sabit url'nin arkasýnda post text'i gelirse bu pattern'ý arar. Akabinde de {url} alýyor. Bu url, ayný id gibi bir pattern parçasýdýr. Yani action'dan parametre olarak gelir.
// Daha sonra controller'daki action metodunda parametre olarak, ayný id alýyormuþ gibi, buradaki gerekli deðeri alýyoruz. Tabi view sayfasýnda bir link tanýmladýysak onuda bu pattern'a göre düzenliyoruz.
// Route'larý ismiyle de çaðýrabiliyoruz.
// Costume route...
app.MapControllerRoute(
    name: "posts_details",
    pattern: "post/{url}",
    defaults: new {controller = "Posts", action = "Details"}
    );

app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{tag}",
    defaults: new { controller = "Posts", action = "Index" }
    );
app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile"}
    );
// Default route...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}"
    );


app.Run();
