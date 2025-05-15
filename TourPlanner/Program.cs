using System;
using System.IO;
using Npgsql;
using TourPlanner.Data;

namespace TourPlanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;";

            string sqlFolderPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SQL");
            sqlFolderPath = Path.GetFullPath(sqlFolderPath);

            Console.WriteLine(sqlFolderPath);

            // Sicherstellen, dass der Ordner für SQL-Skripte existiert
            if (!Directory.Exists(sqlFolderPath))
            {
                Directory.CreateDirectory(sqlFolderPath);
                CreateSqlFiles(sqlFolderPath);
                Console.WriteLine($"SQL-Verzeichnis und Skriptdateien wurden unter folgendem Pfad erstellt: {sqlFolderPath}");
            }

            try
            {
                var dbInitializer = new DatabaseInitializer(connectionString, sqlFolderPath);

                dbInitializer.InitializeDatabase(); // Datenbank initialisieren
                
                Console.WriteLine("\nDie Datenbank wurde erfolgreich initialisiert.");
                Console.WriteLine("Drücken Sie die Eingabetaste, um das Programm zu beenden und die Tabellen zu löschen...");
                Console.ReadLine();

                dbInitializer.DropTables(); // Tabellen löschen
                Console.WriteLine("Das Programm wurde beendet und die Datenbanktabellen wurden gelöscht.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
                Console.WriteLine("Das Programm wird beendet. Drücken Sie die Eingabetaste, um fortzufahren...");
                Console.ReadLine();
            }
        }

        private static void CreateSqlFiles(string sqlFolderPath)
        {
            string dropSqlPath = Path.Combine(sqlFolderPath, "drop.sql");
            string initSqlPath = Path.Combine(sqlFolderPath, "init.sql");

            if (!File.Exists(dropSqlPath))
            {
                File.WriteAllText(dropSqlPath, "-- SQL zum Löschen aller Tabellen\n");
                Console.WriteLine("drop.sql automatisch erstellt.");
            }

            if (!File.Exists(initSqlPath))
            {
                File.WriteAllText(initSqlPath, "-- SQL zur Initialisierung der Datenbank\n");
                Console.WriteLine("init.sql automatisch erstellt.");
            }
        }
    }
}