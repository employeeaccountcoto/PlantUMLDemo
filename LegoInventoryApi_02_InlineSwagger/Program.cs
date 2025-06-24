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
        Title = "Lego Inventory API",
        Version = "v1",
        Description = @"
## API Architecture Diagram
![Lego Inventory Architecture](/images/LegoInventoryC4Diagram.png)

## API Documentation
This API provides access to Lego part information."
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Register our custom services
builder.Services.AddSingleton<PartService>();

// Build the app AFTER configuring services
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles(); // This enables serving static files from wwwroot
app.MapControllers();

app.Run();