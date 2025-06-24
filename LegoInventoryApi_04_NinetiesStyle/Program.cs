using LegoInventoryApi.Services;
using Microsoft.OpenApi.Models; 
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger BEFORE building the app
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🧱 LEGO Inventory API",
        Version = "v1.0",
        Description = @"
# 🏗️ Welcome to the LEGO Inventory API! 

## 🎯 What This API Does
This powerful API helps you manage your LEGO collection like a pro! Whether you're a casual builder or a serious AFOL (Adult Fan of LEGO), this API has got you covered.

## 🧱 Features
- **📦 Part Management**: Add, update, and remove LEGO parts from your inventory
- **🔍 Smart Search**: Find parts by name, color, category, or any criteria you need  
- **📊 Analytics**: Get insights into your collection
- **🎨 Rich Metadata**: Detailed information including colors, categories, and descriptions

## 🏛️ Architecture Overview
![LEGO Inventory Architecture](/images/LegoInventoryC4Diagram.png)

## 🚀 Getting Started
1. **Browse the endpoints** below to see what's available
2. **Try it out** using the interactive interface
3. **Build something awesome** with your LEGO data!

## 💡 Pro Tips
- Use the `GET /api/parts` endpoint to retrieve all parts
- Filter and search using query parameters
- All responses are in JSON format for easy integration

*Happy building! 🎉*",
        Contact = new OpenApiContact
        {
            Name = "LEGO API Team",
            Email = "api@lego-inventory.com",
            Url = new Uri("https://github.com/your-repo/lego-inventory-api")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Enable annotations but configure them properly
    c.EnableAnnotations();
});

// Register our custom services
builder.Services.AddSingleton<PartService>();

// Build the app AFTER configuring services
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lego Inventory API v1");
        c.IndexStream = () => File.OpenRead(Path.Combine(app.Environment.WebRootPath, "swagger-custom-clean.html"));
        c.DocumentTitle = "🧱 LEGO Inventory API";
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles(); // This enables serving static files from wwwroot
app.MapControllers();

app.Run();