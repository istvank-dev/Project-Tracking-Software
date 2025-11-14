using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ProjectTrackingSoftware.Server.Models
{

    public class ProjectModel
    {
        public int Id { get; set; }
        // Keep these required, as they are mandatory data fields
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Theme color
        public string BackgroundColor { get; set; } = "#3498db"; // Bright Blue

        // Relationships (FKs)
        public required string OwnerId { get; set; } // FK to ApplicationUser.Id

        // Navigation Properties: Set to null! to satisfy the compiler/NRT checks
        // as they are guaranteed to be set when loaded via EF Core.
        public ApplicationUser Owner { get; set; } = null!; // <-- FIX 1: Set to null!

        // Collection Property: Initialized to an empty list to satisfy NRT checks.
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>(); // <-- FIX 2: Removed 'required', initialized list
    }

    public class ProjectMember
    {
        // Composite Key (configured in DbContext)
        public int ProjectId { get; set; }
        public required string UserId { get; set; } // FK to ApplicationUser.Id

        public string Role { get; set; } = "Member"; // e.g., "Owner", "Admin", "Member"

        // Navigation Properties: Set to null! to satisfy the compiler/NRT checks
        public ProjectModel Project { get; set; } = null!; // <-- FIX 3: Set to null!
        public ApplicationUser User { get; set; } = null!; // <-- FIX 4: Set to null!
    }
}