using System;
using Npgsql; // Installiere das NuGet-Paket "Npgsql" für PostgreSQL-Zugriffe

namespace TourPlanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Verbindung zur PostgreSQL-Datenbank
            string connectionString =
                "Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Verbindung zur Datenbank erfolgreich!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Herstellen der Verbindung: {ex.Message}");
            }
        }
    }
}