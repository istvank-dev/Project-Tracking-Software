using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProjectTrackingSoftware.Server.Models; // Assuming this is where your new models are located

namespace ProjectTrackingSoftware.Server.Data
{
    // NOTE: Replace 'ApplicationUser' with your actual Identity User class if it's named differently.
    // We assume ApplicationUser inherits from IdentityUser, which is common.
    // If ApplicationUser is not defined in a separate file, define it first:
    /*
    public class ApplicationUser : IdentityUser
    {
        // ... custom properties ...
    }
    */

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================================================================
        // New Project Module DbSets
        // =========================================================================

        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<KanbanModel> KanbanBoards { get; set; }
        public DbSet<KanbanColumn> KanbanColumns { get; set; }
        public DbSet<KanbanTicket> KanbanTickets { get; set; }

        public DbSet<NoteModel> NoteModules { get; set; }
        public DbSet<NoteEntry> NoteEntries { get; set; }


        // =========================================================================
        // Fluent API Configuration
        // =========================================================================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. ProjectMember Composite Key (ProjectId and UserId make up the key)
            builder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            // 2. Project -> Owner (User) Relationship
            builder.Entity<ProjectModel>()
                .HasOne(p => p.Owner)
                .WithMany() // ApplicationUser does not need a list of owned projects
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.NoAction); // Prevents deleting the Project if the user is deleted

            // 3. Project <-> KanbanBoard (One-to-One)
            builder.Entity<ProjectModel>()
                .HasOne(p => p.KanbanBoard)
                .WithOne(b => b.Project)
                .HasForeignKey<KanbanModel>(b => b.ProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Project <-> NoteModule (One-to-One)
            builder.Entity<ProjectModel>()
                .HasOne(p => p.NoteModule)
                .WithOne(m => m.Project)
                .HasForeignKey<NoteModel>(m => m.ProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // 5. KanbanTicket -> Owner (Creator)
            builder.Entity<KanbanTicket>()
                .HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 6. KanbanTicket -> Assignee (Optional)
            builder.Entity<KanbanTicket>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .IsRequired(false) // Assignee can be NULL
                .OnDelete(DeleteBehavior.NoAction);

            // 7. NoteEntry -> Owner (Creator)
            builder.Entity<NoteEntry>()
                .HasOne(n => n.Owner)
                .WithMany()
                .HasForeignKey(n => n.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}