using Microsoft.AspNetCore.Identity;

namespace ProjectTrackingSoftware.Server.Models
{
    // Assuming ApplicationUser is your Identity User Class
    public class ApplicationUser : IdentityUser
    {
        // ... any other user properties
    }

    public class ProjectModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Theme color
        public string BackgroundColor { get; set; } = "#3498db"; // Bright Blue

        // Relationships
        public string OwnerId { get; set; } // FK to ApplicationUser.Id
        public ApplicationUser Owner { get; set; }

        public ICollection<ProjectMember> Members { get; set; }
        public KanbanModel KanbanBoard { get; set; } // One-to-one to Kanban
        public NoteModel NoteModule { get; set; }   // One-to-one to Notes
    }

    public class ProjectMember
    {
        // Composite Key (configured in DbContext)
        public int ProjectId { get; set; }
        public string UserId { get; set; } // FK to ApplicationUser.Id

        public string Role { get; set; } = "Member"; // e.g., "Owner", "Admin", "Member"

        // Navigation Properties
        public ProjectModel Project { get; set; }
        public ApplicationUser User { get; set; }
    }
}
