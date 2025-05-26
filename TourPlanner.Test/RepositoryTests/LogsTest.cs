using TourPlanner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

namespace TourPlanner.Test.RepositoryTests
{
    public class LogsTest
    {
        private readonly string _connectionString = "Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;";

        [Fact]
        public async Task LogCreatedAndStoredTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseNpgsql(_connectionString)
                .Options;

            using var context = new TourPlannerContext(options);
            var logsRepo = new LogsRepository(context);
            var tourRepo = new TourRepository(context);

            // Create a test tour first (because a log needs a valid tourId)
            var testTour = new Tour
            {
                Name = "TestTour",
                Description = "TestDescription",
                FromLocation = "Korneuburg",
                ToLocation = "Vienna", 
                TransportType = "Car",
                Distance = 16.0f,
                EstimatedTime = 200,
                RouteInformation = "testPic"
            };
            
            var createdTour = await tourRepo.CreateAsync(testTour);
            
            // Create a new log
            var newLog = new TourLog
            {
                Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Comment = "Test Log Comment",
                Difficulty = 3,
                Rating = 4, 
                Duration = TimeSpan.FromMinutes(60),
                TourId = createdTour.Id
            };

            // Act
            var createdLog = await logsRepo.CreateLogAsync(newLog);

            // Verify using direct database connection
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // Check if log exists in database
                using var command = new NpgsqlCommand(
                    @"SELECT * FROM ""TourLogs"" WHERE ""Id"" = @id", connection);
                command.Parameters.AddWithValue("@id", createdLog.Id);
                
                using var reader = await command.ExecuteReaderAsync();
                
                // Assert
                Assert.True(await reader.ReadAsync(), "Log should exist in database");
                Assert.Equal(newLog.Comment, reader["Comment"]);
                Assert.Equal(newLog.Difficulty, (int)reader["Difficulty"]);
                Assert.Equal(newLog.Rating, (int)reader["Rating"]);
                Assert.Equal(createdTour.Id, reader["TourId"]);
                
                // DateTime and TimeSpan need special handling
                var dbDate = (DateTime)reader["Date"];
                Assert.Equal(newLog.Date.Date, dbDate.Date);
                
                // For TimeSpan, compare total minutes
                var dbDuration = (TimeSpan)reader["Duration"];
                Assert.Equal(newLog.Duration.TotalMinutes, dbDuration.TotalMinutes, 1); // Allow 1 minute precision
            }

            // Cleanup - delete the test log and tour
            await logsRepo.DeleteLogAsync(createdLog.Id);
            await tourRepo.DeleteAsync(createdTour.Id);
        }
        
        [Fact]
        public async Task GetLogsByTourIdTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseNpgsql(_connectionString)
                .Options;

            using var context = new TourPlannerContext(options);
            var logsRepo = new LogsRepository(context);
            var tourRepo = new TourRepository(context);

            // Create a test tour
            var testTour = new Tour
            {
                Name = "TestTourForLogs",
                Description = "TestDescription",
                FromLocation = "Korneuburg",
                ToLocation = "Vienna", 
                TransportType = "Car",
                Distance = 16.0f,
                EstimatedTime = 200,
                RouteInformation = "testPic"
            };
            
            var createdTour = await tourRepo.CreateAsync(testTour);
            
            // Create multiple logs for the tour
            var log1 = new TourLog
            {
                Date = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Utc),
                Comment = "Log 1",
                Difficulty = 2,
                Rating = 3,
                Duration = TimeSpan.FromMinutes(45),
                TourId = createdTour.Id
            };
            
            var log2 = new TourLog
            {
                Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Comment = "Log 2",
                Difficulty = 4,
                Rating = 5,
                Duration = TimeSpan.FromMinutes(90),
                TourId = createdTour.Id
            };
            
            await logsRepo.CreateLogAsync(log1);
            await logsRepo.CreateLogAsync(log2);
            
            // Act
            var logs = await logsRepo.GetLogsByTourIdAsync(createdTour.Id);
            
            // Assert
            var logsList = logs.ToList();
            Assert.Equal(2, logsList.Count);
            Assert.Contains(logsList, l => l.Comment == "Log 1");
            Assert.Contains(logsList, l => l.Comment == "Log 2");
            
            // Cleanup
            foreach (var log in logsList)
            {
                await logsRepo.DeleteLogAsync(log.Id);
            }
            await tourRepo.DeleteAsync(createdTour.Id);
        }
        
        [Fact]
        public async Task UpdateLogTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseNpgsql(_connectionString)
                .Options;

            using var context = new TourPlannerContext(options);
            var logsRepo = new LogsRepository(context);
            var tourRepo = new TourRepository(context);

            // Create a test tour
            var testTour = new Tour
            {
                Name = "TestTourForUpdate",
                Description = "TestDescription",
                FromLocation = "Korneuburg",
                ToLocation = "Vienna", 
                TransportType = "Car",
                Distance = 16.0f,
                EstimatedTime = 200,
                RouteInformation = "testPic"
            };
            
            var createdTour = await tourRepo.CreateAsync(testTour);
            
            // Create a log to update
            var newLog = new TourLog
            {
                Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Comment = "Original Comment",
                Difficulty = 2,
                Rating = 3,
                Duration = TimeSpan.FromMinutes(45),
                TourId = createdTour.Id
            };
            
            var createdLog = await logsRepo.CreateLogAsync(newLog);
            
            // Update the log
            createdLog.Comment = "Updated Comment";
            createdLog.Difficulty = 4;
            createdLog.Rating = 5;
            createdLog.Duration = TimeSpan.FromMinutes(60);
            
            // Act
            var updatedLog = await logsRepo.UpdateLogAsync(createdLog);
            
            // Verify using direct database connection
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new NpgsqlCommand(
                    @"SELECT * FROM ""TourLogs"" WHERE ""Id"" = @id", connection);
                command.Parameters.AddWithValue("@id", createdLog.Id);
                
                using var reader = await command.ExecuteReaderAsync();
                
                // Assert
                Assert.True(await reader.ReadAsync(), "Log should exist in database");
                Assert.Equal("Updated Comment", reader["Comment"]);
                Assert.Equal(4, (int)reader["Difficulty"]);
                Assert.Equal(5, (int)reader["Rating"]);
                
                // For TimeSpan, compare total minutes
                var dbDuration = (TimeSpan)reader["Duration"];
                Assert.Equal(60, dbDuration.TotalMinutes, 1); // Allow 1 minute precision
            }
            
            // Cleanup
            await logsRepo.DeleteLogAsync(createdLog.Id);
            await tourRepo.DeleteAsync(createdTour.Id);
        }
        
        [Fact]
        public async Task DeleteLogTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseNpgsql(_connectionString)
                .Options;

            using var context = new TourPlannerContext(options);
            var logsRepo = new LogsRepository(context);
            var tourRepo = new TourRepository(context);

            // Create a test tour
            var testTour = new Tour
            {
                Name = "TestTourForDelete",
                Description = "TestDescription",
                FromLocation = "Korneuburg",
                ToLocation = "Vienna", 
                TransportType = "Car",
                Distance = 16.0f,
                EstimatedTime = 200,
                RouteInformation = "testPic"
            };
            
            var createdTour = await tourRepo.CreateAsync(testTour);
            
            // Create a log to delete
            var newLog = new TourLog
            {
                Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Comment = "Log to Delete",
                Difficulty = 2,
                Rating = 3,
                Duration = TimeSpan.FromMinutes(45),
                TourId = createdTour.Id
            };
            
            var createdLog = await logsRepo.CreateLogAsync(newLog);
            
            // Act
            await logsRepo.DeleteLogAsync(createdLog.Id);
            
            // Verify using direct database connection
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TourLogs"" WHERE ""Id"" = @id", connection);
                command.Parameters.AddWithValue("@id", createdLog.Id);
                
                var count = (long)await command.ExecuteScalarAsync();
                
                // Assert
                Assert.Equal(0, count);
            }
            
            // Cleanup the test tour
            await tourRepo.DeleteAsync(createdTour.Id);
        }
    }
} 