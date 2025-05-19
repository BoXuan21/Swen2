using Microsoft.EntityFrameworkCore;
using System.Text;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using TourPlanner.Models;
using Npgsql;
using System.IO;

// Clear console and show startup banner
Console.Clear();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("====================================================");
Console.WriteLine("       TOUR PLANNER API SERVER STARTING UP          ");
Console.WriteLine("====================================================");
Console.ResetColor();
Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
Console.WriteLine($"Time: {DateTime.Now}");
Console.WriteLine();

// Get connection string from configuration or use default
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
    "Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;";

Console.WriteLine("Database connection string: " + connectionString);

// Find the SQL folder
string sqlFolderPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SQL");
sqlFolderPath = Path.GetFullPath(sqlFolderPath);
Console.WriteLine($"SQL folder path: {sqlFolderPath}");

// Clear and Initialize database BEFORE starting the server
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("PHASE 1: DATABASE INITIALIZATION");
Console.WriteLine("---------------------------------------");
Console.ResetColor();

try
{
    // Ensure SQL folder exists
    if (!Directory.Exists(sqlFolderPath))
    {
        Directory.CreateDirectory(sqlFolderPath);
        Console.WriteLine($"Created SQL folder at: {sqlFolderPath}");
    }

    // Path to SQL files
    string dropSqlPath = Path.Combine(sqlFolderPath, "drop.sql");
    string initSqlPath = Path.Combine(sqlFolderPath, "init.sql");

    // Check if files exist
    if (!File.Exists(dropSqlPath))
    {
        Console.WriteLine($"Warning: drop.sql not found at {dropSqlPath}");
        // Create a default drop.sql file
        string defaultDropSql = @"
-- Drop tables in correct order to respect foreign keys
DROP TABLE IF EXISTS ""TourLogs"";
DROP TABLE IF EXISTS ""Tours"";
DROP TABLE IF EXISTS ""Lists"";";
        File.WriteAllText(dropSqlPath, defaultDropSql);
        Console.WriteLine("Created default drop.sql file");
    }

    if (!File.Exists(initSqlPath))
    {
        Console.WriteLine($"Warning: init.sql not found at {initSqlPath}");
        Console.WriteLine("Please create an init.sql file in the SQL folder with your database schema.");
        throw new FileNotFoundException($"Required file init.sql not found at {initSqlPath}");
    }

    // Read SQL scripts
    string dropSql = File.ReadAllText(dropSqlPath);
    string initSql = File.ReadAllText(initSqlPath);

    // Execute the SQL scripts
    using (var connection = new NpgsqlConnection(connectionString))
    {
        connection.Open();
        
        // First drop tables
        Console.WriteLine("Dropping existing tables...");
        using (var cmd = new NpgsqlCommand(dropSql, connection))
        {
            cmd.ExecuteNonQuery();
        }
        Console.WriteLine("Tables dropped successfully.");
        
        // Then create tables
        Console.WriteLine("Creating database tables...");
        using (var cmd = new NpgsqlCommand(initSql, connection))
        {
            cmd.ExecuteNonQuery();
        }
        Console.WriteLine("Tables created successfully.");
    }
    
    Console.WriteLine("Database initialization completed successfully.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR INITIALIZING DATABASE: " + ex.Message);
    Console.ResetColor();
    Console.WriteLine("Server will start anyway, but functionality may be limited.");
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("PHASE 2: STARTING WEB SERVER");
Console.WriteLine("---------------------------------------");
Console.ResetColor();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Console.WriteLine("Registering services...");
builder.Services.AddControllers();

// Configure PostgreSQL database connection
builder.Services.AddDbContext<TourPlannerContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories
builder.Services.AddScoped<ITourRepository, TourRepository>();

// Add API explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow any origin (required for desktop apps)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure HTTP request pipeline for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    Console.WriteLine("Development mode: Swagger UI enabled at /swagger");
}

// Seed the database with initial data
Console.WriteLine("Seeding database with initial data...");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TourPlannerContext>();
        SeedData(dbContext);
        Console.WriteLine("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DATABASE SEEDING ERROR: " + ex.Message);
        Console.ResetColor();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Use the "AllowAll" CORS policy
app.UseAuthorization();
app.MapControllers();

// Get server URLs and display them
var serverAddresses = app.Urls.ToList();
Console.WriteLine("\nServer URLs:");
foreach (var address in serverAddresses)
{
    Console.WriteLine($"  - {address}");
    if (address.StartsWith("https"))
        Console.WriteLine($"    Swagger UI: {address}/swagger");
}

Console.WriteLine("\nServer is ready. Press Ctrl+C to shut down.");
Console.WriteLine("====================================================");

// Start the server
try 
{
    app.Run();
    Console.WriteLine("Server has stopped.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("SERVER ERROR: " + ex.Message);
    Console.ResetColor();
}

// Seed method for initial data
void SeedData(TourPlannerContext context)
{
    // No seeding needed - database will start empty
    Console.WriteLine("Database is empty and ready for use.");
}