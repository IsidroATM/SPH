using Microsoft.EntityFrameworkCore;
using SPH.Persistence;
using SPH.Repositories.Implementations;
using SPH.Repositories.Interfaces;

//Cookies
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using SPH.Persistence.Extensions.SPH.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Añadimos la conexion SQL
var connectionString = builder.Configuration.GetConnectionString("ConexionSqlServer");
builder.Services.AddDbContext<SPHDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

//Añadimos los repositorios(UnitWork)


builder.Services.AddScoped<IUnitWork, UnitWork>();
// Añade el MigrationManager como servicio
builder.Services.AddScoped<MigrationManager>();


//Añadir autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => 
 { options.LoginPath = "/Users/Login";
   options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
 });

builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add(

		new ResponseCacheAttribute
		{
			NoStore = true,
			Location = ResponseCacheLocation.None,
        }
    );
});


var app = builder.Build();

// Aplicar migraciones al inicio
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var migrationManager = services.GetRequiredService<MigrationManager>();
    migrationManager.ApplyMigrations();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
void ConfigureServices(IServiceCollection services)
{
    // Configuración del tamaño máximo de carga de archivos
    services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 60000000; // Ejemplo: límite de 60 MB
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//continuacion coockies
app.UseAuthentication();

app.UseAuthorization();



app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Users}/{action=Login}/{id?}");



app.Run();
