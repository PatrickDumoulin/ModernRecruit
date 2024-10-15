using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Service;
using System.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Authentification
var connectionString = builder.Configuration.GetConnectionString("DefaultAuthConnection") ?? throw new InvalidOperationException("Connection string 'CorrectifAuthMVCContextConnection' not found.");

builder.Services.AddDbContext<UtilisateurDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UtilisateurDbContext>();

//JOURNALISATION
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventLog();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IOffresEmploisService, OffresEmploisServiceProxy>(client 
    => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiOffreEmploi")));

builder.Services.AddHttpClient<IFavorisService, FavorisServiceProxy>(client
    => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiFavoris")));

builder.Services.AddHttpClient<IDocumentsService, DocumentsServiceProxy>(client 
    => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiDocument")));

builder.Services.AddHttpClient<IPostulationsService, PostulationsServiceProxy>(client
    => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiPostulation")));
builder.Services.AddHttpClient<INoteService, NotesServiceProxy>(client
    => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiPostulation")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/CodeStatus?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
