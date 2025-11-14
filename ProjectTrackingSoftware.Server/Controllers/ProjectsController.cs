// File: ProjectTrackingSoftware.Server/Controllers/ProjectsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTrackingSoftware.Server.Data;
using ProjectTrackingSoftware.Server.DTOs;
using ProjectTrackingSoftware.Server.Models;
using System.Security.Claims;

namespace ProjectTrackingSoftware.Server.Controllers
{
    [Authorize] // All endpoints in this controller require authentication
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProjectsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // --- A. ProjectsController: Core Access and Membership ---

        /// <summary>
        /// POST /api/projects
        /// Creates a new Project and adds the creator as an 'Owner'.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDTO request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("Authenticated UserId {UserId} not found in UserManager during project creation.", userId);
                return Unauthorized();
            }

            // Removed transaction for simplicity and focusing only on the save process
            try
            {
                // 1. Create the Project Entity
                var newProject = new ProjectModel
                {
                    Title = request.Title,
                    Description = request.Description,
                    BackgroundColor = request.Color,
                    OwnerId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Projects.Add(newProject);

                // 2. Add Creator as Project Member with "Owner" role
                var projectMember = new ProjectMember
                {
                    Project = newProject, // Link via object reference
                    UserId = userId,
                    Role = "Owner"
                };
                _context.ProjectMembers.Add(projectMember);

                // REMOVED: Kanban and Note module creation logic

                // 3. Final Commit: Execute all staged database operations atomically.
                await _context.SaveChangesAsync();

                // 4. Return the created project DTO
                var projectDto = new ProjectDTO
                {
                    Id = newProject.Id,
                    Title = newProject.Title,
                    Description = newProject.Description,
                    Color = newProject.BackgroundColor,
                    OwnerId = newProject.OwnerId,
                    OwnerName = user.UserName ?? user.Email,
                    UserRole = projectMember.Role,
                    CreatedAt = newProject.CreatedAt
                };

                return CreatedAtAction(nameof(GetProjectDetails), new { id = newProject.Id }, projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Project creation failed for user {UserId}.", userId);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An internal server error occurred during project creation. The operation was rolled back." });
            }
        }

        /// <summary>
        /// GET /api/projects/{id}
        /// Retrieves the details for a single project.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectDetails(int id)
        {
            // ... (Logic remains the same as it only depends on Project and ProjectMember)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var projectDto = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == id && pm.UserId == userId)
                .Join(_context.Projects,
                    pm => pm.ProjectId,
                    p => p.Id,
                    (pm, p) => new { pm, p })
                .Join(_userManager.Users,
                    joined => joined.p.OwnerId,
                    u => u.Id,
                    (joined, u) => new ProjectDTO
                    {
                        Id = joined.p.Id,
                        Title = joined.p.Title,
                        Description = joined.p.Description,
                        Color = joined.p.BackgroundColor,
                        OwnerId = joined.p.OwnerId,
                        OwnerName = u.UserName ?? u.Email,
                        UserRole = joined.pm.Role,
                        CreatedAt = joined.p.CreatedAt
                    })
                .FirstOrDefaultAsync();

            if (projectDto == null)
            {
                return NotFound(new { message = $"Project with ID {id} not found, or user is not a member." });
            }

            return Ok(projectDto);
        }

        /// <summary>
        /// GET /api/projects
        /// Lists all projects the current user is a member of.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListUserProjects()
        {
            // ... (Logic remains the same as it only depends on Project and ProjectMember)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var userProjects = await _context.ProjectMembers
                    .Where(pm => pm.UserId == userId)
                    .Join(_context.Projects,
                        pm => pm.ProjectId,
                        p => p.Id,
                        (pm, p) => new { pm, p })
                    .Join(_userManager.Users,
                        joined => joined.p.OwnerId,
                        u => u.Id,
                        (joined, u) => new ProjectDTO
                        {
                            Id = joined.p.Id,
                            Title = joined.p.Title,
                            Description = joined.p.Description,
                            Color = joined.p.BackgroundColor,
                            OwnerId = joined.p.OwnerId,
                            OwnerName = u.UserName ?? u.Email,
                            UserRole = joined.pm.Role,
                            CreatedAt = joined.p.CreatedAt
                        })
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(userProjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list projects for user {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new { error = "An internal server error occurred while fetching projects." });
            }
        }
    }
}