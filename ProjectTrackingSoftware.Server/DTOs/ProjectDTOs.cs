// File: ProjectTrackingSoftware.Server/DTOs/ProjectDtos.cs

using ProjectTrackingSoftware.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectTrackingSoftware.Server.DTOs
{
    // --- Request DTOs ---

    /// <summary>
    /// Request object for POST /api/projects (Create Project).
    /// </summary>
    public class CreateProjectRequestDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Use 'Color' for client simplicity, maps to ProjectModel.BackgroundColor
        public string Color { get; set; } = "#3498db";
    }

    // --- Response DTOs ---

    /// <summary>
    /// Response object for GET /api/projects and GET /api/projects/{id}.
    /// </summary>
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty; // User's display name
        public string UserRole { get; set; } = string.Empty; // Current user's role in this project
        public DateTime CreatedAt { get; set; }
    }
}