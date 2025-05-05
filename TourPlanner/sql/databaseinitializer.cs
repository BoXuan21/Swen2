using System;
using System.IO;
using Npgsql;

namespace TourPlanner
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _sqlFolderPath;

        public DatabaseInitializer(string connectionString, string sqlFolderPath)
        {
            _connectionString = connectionString;
            _sqlFolderPath = sqlFolderPath;
        }

        public void InitializeDatabase()
        {
            string dropSqlPath = Path.Combine(_sqlFolderPath, "drop.sql");
            string initSqlPath = Path.Combine(_sqlFolderPath, "init.sql");

            Console.WriteLine($"Looking for drop.sql at: {dropSqlPath}");
            Console.WriteLine($"Looking for init.sql at: {initSqlPath}");

            if (!File.Exists(dropSqlPath))
            {
                Console.WriteLine($"Error: drop.sql not found at {dropSqlPath}");
                return;
            }

            if (!File.Exists(initSqlPath))
            {
                Console.WriteLine($"Error: init.sql not found at {initSqlPath}");
                return;
            }

            try
            {
                string dropSql = File.ReadAllText(dropSqlPath);
                string initSql = File.ReadAllText(initSqlPath);

                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var command = new NpgsqlCommand(dropSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Executed drop.sql script.");
                }

                using (var command = new NpgsqlCommand(initSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Executed init.sql script.");
                }

                Console.WriteLine("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");

                if (ex is NpgsqlException npgEx && npgEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {npgEx.InnerException.Message}");
                }
            }
        }

        public void DropTables()
        {
            string dropSqlPath = Path.Combine(_sqlFolderPath, "drop.sql");

            if (!File.Exists(dropSqlPath))
            {
                Console.WriteLine($"Error: drop.sql not found at {dropSqlPath}");
                return;
            }

            try
            {
                string dropSql = File.ReadAllText(dropSqlPath);

                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var command = new NpgsqlCommand(dropSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Tables dropped successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dropping tables: {ex.Message}");
            }
        }
    }
}