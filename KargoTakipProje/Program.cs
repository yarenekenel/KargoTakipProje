using KargoTakip.Data.Context;
using KargoTakip.Data.Repositories;
using KargoTakip.Data.Repository;
using KargoTakip.Service.Interface;
using KargoTakip.Service.Service;
using KargoTakip.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- MailSettings appsettings.json'dan baðlanýyor ---
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// --- Dapper Context ---
builder.Services.AddSingleton<DapperContext>();

// --- Repository Kayýtlarý ---
builder.Services.AddScoped<IKargoRepository, KargoRepository>();
builder.Services.AddScoped<IKargoDurumGecmisiRepository, KargoDurumGecmisiRepository>();
builder.Services.AddScoped<IKargoDosyaRepository, KargoDosyaRepository>();

// --- Service Kayýtlarý ---
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IKargoService, KargoService>();

// --- DosyaService: uploadRootPath parametresi olduðu için özel kayýt ---
builder.Services.AddScoped<IDosyaService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var kargoDosyaRepo = sp.GetRequiredService<IKargoDosyaRepository>();
    return new DosyaService(kargoDosyaRepo, env.WebRootPath);
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
