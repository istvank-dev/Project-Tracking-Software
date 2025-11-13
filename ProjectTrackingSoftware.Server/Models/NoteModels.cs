namespace ProjectTrackingSoftware.Server.Models
{
    public class NoteModel
    {
        public int Id { get; set; }

        // FK to Project (One Project has one Note Module)
        public int ProjectId { get; set; }
        public ProjectModel Project { get; set; }

        // Navigation to Note Entries
        public ICollection<NoteEntry> Notes { get; set; }
    }

    public class NoteEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // The content of the note

        public string BackgroundColor { get; set; } = "#f1c40f"; // Yellow/Gold
        public string TextColor { get; set; } = "#34495e";     // Dark Blue/Gray

        // FK to NoteModule
        public int NoteModuleId { get; set; }
        public NoteModel NoteModule { get; set; }

        // Owner (the creator)
        public string OwnerUserId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
