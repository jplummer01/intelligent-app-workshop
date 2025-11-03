using Microsoft.AspNetCore.Antiforgery;
using Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// See: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Required to generate enumeration values in Swagger doc
builder.Services.AddControllersWithViews().AddJsonOptions(options => 
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOutputCache();
builder.Services.AddAntiforgery(options => { 
    options.HeaderName = "X-CSRF-TOKEN-HEADER"; 
    options.FormFieldName = "X-CSRF-TOKEN-FORM"; });
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:80", "http://localhost", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add MAF services
builder.Services.AddMafServices();

// Load user secrets
builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();
app.UseRouting();
app.UseCors();
app.UseAntiforgery();
app.MapControllers();

app.Use(next => context =>
{
    var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens?.RequestToken ?? string.Empty, new CookieOptions() { HttpOnly = false });
    return next(context);
});

app.Map("/", () => Results.Redirect("/swagger"));

app.MapControllerRoute(
    "default",
    "{controller=ChatController}");

app.Run();