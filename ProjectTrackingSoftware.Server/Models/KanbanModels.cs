namespace ProjectTrackingSoftware.Server.Models
{
    public class KanbanModel
    {
        public int Id { get; set; }

        // FK to Project (One Project has one Kanban Board)
        public int ProjectId { get; set; }
        public ProjectModel Project { get; set; }

        // Navigation to Columns
        public ICollection<KanbanColumn> Columns { get; set; }
    }

    public class KanbanColumn
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Use double/float for flexible re-ordering without mass updates
        public double SortOrder { get; set; }

        public string BackgroundColor { get; set; } = "#2c3e50"; // Dark Navy
        public string TextColor { get; set; } = "#ffffff";     // White

        // FK to Board
        public int BoardId { get; set; }
        public KanbanModel Board { get; set; }

        // Navigation to Tickets
        public ICollection<KanbanTicket> Tickets { get; set; }
    }

    public class KanbanTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double SortOrder { get; set; } // Position within the column

        public string BackgroundColor { get; set; } = "#ecf0f1"; // Light Gray
        public string TextColor { get; set; } = "#34495e";     // Dark Blue/Gray

        // FK to Column
        public int ColumnId { get; set; }
        public KanbanColumn Column { get; set; }

        // Owner (the creator)
        public string OwnerUserId { get; set; }
        public ApplicationUser Owner { get; set; }

        // Assignee (can be null)
        public string? AssigneeId { get; set; }
        public ApplicationUser? Assignee { get; set; }
    }
}
