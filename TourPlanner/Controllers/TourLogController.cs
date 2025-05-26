using Microsoft.AspNetCore.Mvc;
using TourPlanner.Data.Repositories;
using TourPlanner.Models;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Data;

namespace TourPlanner.Controllers
{
    [ApiController]
    [Route("api/TourLog")]
    public class TourLogController : ControllerBase
    {
        private readonly ILogsRepository _logsRepository;
        private readonly ILogger<TourLogController> _logger;
        private readonly TourPlannerContext _context;

        public TourLogController(ILogsRepository logsRepository, ILogger<TourLogController> logger, TourPlannerContext context)
        {
            _logsRepository = logsRepository;
            _logger = logger;
            _context = context;
        }

        [HttpGet("tour/{tourId}")]
        public async Task<ActionResult<IEnumerable<TourLog>>> GetLogsByTourId(string tourId)
        {
            try
            {
                _logger.LogInformation("Getting logs for tour: {TourId}", tourId);
                var logs = await _logsRepository.GetLogsByTourIdAsync(tourId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs for tour: {TourId}", tourId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TourLog>> GetLogById(string id)
        {
            try
            {
                _logger.LogInformation("Getting log by ID: {Id}", id);
                var log = await _logsRepository.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound();
                }
                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log by ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TourLog>> CreateLog([FromBody] TourLog log)
        {
            try
            {
                _logger.LogInformation("Received log creation request: {@Log}", log);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                // Validate the tour exists
                var tour = await _context.Tours.FindAsync(log.TourId);
                if (tour == null)
                {
                    _logger.LogWarning("Tour not found: {TourId}", log.TourId);
                    return BadRequest($"Tour with ID {log.TourId} not found");
                }

                _logger.LogInformation("Creating new log for tour: {TourId}", log.TourId);
                _logger.LogInformation("test here");
                var createdLog = await _logsRepository.CreateLogAsync(log);
                return CreatedAtAction(nameof(GetLogById), new { id = createdLog.Id }, createdLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating log: {@Log}", log);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(string id, TourLog log)
        {
            try
            {
                if (id != log.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating log: {Id}", id);
                var updatedLog = await _logsRepository.UpdateLogAsync(log);
                if (updatedLog == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating log: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(string id)
        {
            try
            {
                _logger.LogInformation("Deleting log: {Id}", id);
                var log = await _logsRepository.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound();
                }

                await _logsRepository.DeleteLogAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting log: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourLog>>> GetAllLogs()
        {
            try
            {
                _logger.LogInformation("Getting all logs");
                // Get all tours to access their logs
                var allLogs = await (
                    from log in _context.Logs
                    join tour in _context.Tours on log.TourId equals tour.Id
                    select new
                    {
                        LogId = log.Id,
                        log.Comment,
                        log.Date,
                        log.Difficulty,
                        log.Rating,
                        log.Duration,
                        TourId = tour.Id,
                        TourName = tour.Name
                    }
                ).ToListAsync();
                
                return Ok(allLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all logs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 