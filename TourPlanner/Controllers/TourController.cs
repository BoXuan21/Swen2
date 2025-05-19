using Microsoft.AspNetCore.Mvc;
using TourPlanner.Data.Repositories;
using TourPlanner.Models;

namespace TourPlanner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourController : ControllerBase
    {
        private readonly ITourRepository _tourRepository;
        private readonly ILogger<TourController> _logger;

        public TourController(ITourRepository tourRepository, ILogger<TourController> logger)
        {
            _tourRepository = tourRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all tours");
                var tours = await _tourRepository.GetAllAsync();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tours");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Getting tour by ID: {Id}", id);
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null)
                {
                    return NotFound();
                }
                return Ok(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour by ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Tour>> Create(Tour tour)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new tour: {Name}", tour.Name);
                var createdTour = await _tourRepository.CreateAsync(tour);
                return CreatedAtAction(nameof(GetById), new { id = createdTour.Id }, createdTour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tour");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Tour tour)
        {
            try
            {
                if (id != tour.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating tour: {Id}", id);
                await _tourRepository.UpdateAsync(tour);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting tour: {Id}", id);
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null)
                {
                    return NotFound();
                }

                await _tourRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("list/{listId}")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetByListId(string listId)
        {
            try
            {
                _logger.LogInformation("Getting tours by list ID: {ListId}", listId);
                var tours = await _tourRepository.GetByListIdAsync(listId);
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tours by list ID: {ListId}", listId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 