using Microsoft.EntityFrameworkCore;

namespace RootsLib.Model
{
    public partial class Roots : DbContext
    {
        public Roots()
        {
        }

        public Roots(DbContextOptions<Roots> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressLinkTable> AddressLinkTable { get; set; }
        public virtual DbSet<AddressTable> AddressTable { get; set; }
        public virtual DbSet<ChildTable> ChildTable { get; set; }
        public virtual DbSet<CitationTable> CitationTable { get; set; }
        public virtual DbSet<ConfigTable> ConfigTable { get; set; }
        public virtual DbSet<EventTable> EventTable { get; set; }
        public virtual DbSet<ExclusionTable> ExclusionTable { get; set; }
        public virtual DbSet<FactTypeTable> FactTypeTable { get; set; }
        public virtual DbSet<FamilyTable> FamilyTable { get; set; }
        public virtual DbSet<GroupTable> GroupTable { get; set; }
        public virtual DbSet<LabelTable> LabelTable { get; set; }
        public virtual DbSet<LinkAncestryTable> LinkAncestryTable { get; set; }
        public virtual DbSet<LinkTable> LinkTable { get; set; }
        public virtual DbSet<MediaLinkTable> MediaLinkTable { get; set; }
        public virtual DbSet<NameTable> NameTable { get; set; }
        public virtual DbSet<PersonTable> PersonTable { get; set; }
        public virtual DbSet<PlaceTable> PlaceTable { get; set; }
        public virtual DbSet<ResearchItemTable> ResearchItemTable { get; set; }
        public virtual DbSet<ResearchTable> ResearchTable { get; set; }
        public virtual DbSet<RoleTable> RoleTable { get; set; }
        public virtual DbSet<SourceTable> SourceTable { get; set; }
        public virtual DbSet<SourceTemplateTable> SourceTemplateTable { get; set; }
        public virtual DbSet<Urltable> Urltable { get; set; }
        public virtual DbSet<WitnessTable> WitnessTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-KGS70RI\\SQL2016EX;Database=Roots;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "3.0.0-preview.18572.1");

            modelBuilder.Entity<AddressLinkTable>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__AddressL__2D1221550CEE3576");

                entity.Property(e => e.LinkId).ValueGeneratedNever();

                entity.Property(e => e.Details).IsUnicode(false);
            });

            modelBuilder.Entity<AddressTable>(entity =>
            {
                entity.HasKey(e => e.AddressId)
                    .HasName("PK__AddressT__091C2A1BEB7C30BC");

                entity.Property(e => e.AddressId).ValueGeneratedNever();

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Fax).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Phone1).IsUnicode(false);

                entity.Property(e => e.Phone2).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.Street1).IsUnicode(false);

                entity.Property(e => e.Street2).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);

                entity.Property(e => e.Zip).IsUnicode(false);
            });

            modelBuilder.Entity<ChildTable>(entity =>
            {
                entity.HasKey(e => e.RecId)
                    .HasName("PK__ChildTab__360414FF9F89B398");

                entity.Property(e => e.RecId).ValueGeneratedNever();

                entity.Property(e => e.Note).IsUnicode(false);
            });

            modelBuilder.Entity<CitationTable>(entity =>
            {
                entity.HasKey(e => e.CitationId)
                    .HasName("PK__Citation__EAD2AD1BA2E109B8");

                entity.Property(e => e.CitationId).ValueGeneratedNever();

                entity.Property(e => e.ActualText).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Fields).IsUnicode(false);

                entity.Property(e => e.Quality).IsUnicode(false);

                entity.Property(e => e.RefNumber).IsUnicode(false);
            });

            modelBuilder.Entity<ConfigTable>(entity =>
            {
                entity.HasKey(e => e.RecId)
                    .HasName("PK__ConfigTa__360414FFA5C3273A");

                entity.Property(e => e.RecId).ValueGeneratedNever();

                entity.Property(e => e.DataRec).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<EventTable>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK__EventTab__7944C87063D65952");

                entity.Property(e => e.EventId).ValueGeneratedNever();

                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.Details).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Sentence).IsUnicode(false);
            });

            modelBuilder.Entity<ExclusionTable>(entity =>
            {
                entity.HasKey(e => e.RecId)
                    .HasName("PK__Exclusio__360414FFD2DE2891");

                entity.Property(e => e.RecId).ValueGeneratedNever();
            });

            modelBuilder.Entity<FactTypeTable>(entity =>
            {
                entity.HasKey(e => e.FactTypeId)
                    .HasName("PK__FactType__A97D03944C78D824");

                entity.Property(e => e.FactTypeId).ValueGeneratedNever();

                entity.Property(e => e.Abbrev).IsUnicode(false);

                entity.Property(e => e.GedcomTag).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Sentence).IsUnicode(false);
            });

            modelBuilder.Entity<FamilyTable>(entity =>
            {
                entity.HasKey(e => e.FamilyId)
                    .HasName("PK__FamilyTa__41D82F4BC3F16152");

                entity.Property(e => e.FamilyId).ValueGeneratedNever();

                entity.Property(e => e.Note).IsUnicode(false);
            });

            modelBuilder.Entity<GroupTable>(entity =>
            {
                entity.HasKey(e => e.RecId)
                    .HasName("PK__GroupTab__360414FFC240D58E");

                entity.Property(e => e.RecId).ValueGeneratedNever();
            });

            modelBuilder.Entity<LabelTable>(entity =>
            {
                entity.HasKey(e => e.LabelId)
                    .HasName("PK__LabelTab__397E2BA3D5DA2857");

                entity.Property(e => e.LabelId).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.LabelName).IsUnicode(false);
            });

            modelBuilder.Entity<LinkAncestryTable>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__LinkAnce__2D1221554118A33B");

                entity.Property(e => e.LinkId).ValueGeneratedNever();

                entity.Property(e => e.ExtId).IsUnicode(false);

                entity.Property(e => e.ExtVersion).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);
            });

            modelBuilder.Entity<LinkTable>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__LinkTabl__2D122155ECC9A003");

                entity.Property(e => e.LinkId).ValueGeneratedNever();

                entity.Property(e => e.ExtId).IsUnicode(false);

                entity.Property(e => e.ExtVersion).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);
            });

            modelBuilder.Entity<MediaLinkTable>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__MediaLin__2D122155AEADE68F");

                entity.Property(e => e.LinkId).ValueGeneratedNever();

                entity.Property(e => e.Caption).IsUnicode(false);

                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.RefNumber).IsUnicode(false);
            });

            modelBuilder.Entity<NameTable>(entity =>
            {
                entity.HasKey(e => e.NameId)
                    .HasName("PK__NameTabl__EE1C17C1B398800C");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("idxNameOwnerID");

                entity.Property(e => e.NameId).ValueGeneratedNever();

                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.Given).IsUnicode(false);

                entity.Property(e => e.Nickname).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Prefix).IsUnicode(false);

                entity.Property(e => e.Sentence).IsUnicode(false);

                entity.Property(e => e.Suffix).IsUnicode(false);

                entity.Property(e => e.Surname).IsUnicode(false);
            });

            modelBuilder.Entity<PersonTable>(entity =>
            {
                entity.HasKey(e => e.PersonId)
                    .HasName("PK__PersonTa__AA2FFB859AD8A82D");

                entity.Property(e => e.PersonId).ValueGeneratedNever();

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.UniqueId).IsUnicode(false);
            });

            modelBuilder.Entity<PlaceTable>(entity =>
            {
                entity.HasKey(e => e.PlaceId)
                    .HasName("PK__PlaceTab__D5222B4E8D1B3DCF");

                entity.Property(e => e.PlaceId).ValueGeneratedNever();

                entity.Property(e => e.Abbrev).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Normalized).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);
            });

            modelBuilder.Entity<ResearchItemTable>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__Research__727E83EB88BA7884");

                entity.Property(e => e.ItemId).ValueGeneratedNever();

                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.Goal).IsUnicode(false);

                entity.Property(e => e.RefNumber).IsUnicode(false);

                entity.Property(e => e.Repository).IsUnicode(false);

                entity.Property(e => e.Result).IsUnicode(false);

                entity.Property(e => e.Source).IsUnicode(false);
            });

            modelBuilder.Entity<ResearchTable>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("PK__Research__7C6949D1C943866B");

                entity.Property(e => e.TaskId).ValueGeneratedNever();

                entity.Property(e => e.Date1).IsUnicode(false);

                entity.Property(e => e.Date2).IsUnicode(false);

                entity.Property(e => e.Date3).IsUnicode(false);

                entity.Property(e => e.Details).IsUnicode(false);

                entity.Property(e => e.Filename).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.RefNumber).IsUnicode(false);
            });

            modelBuilder.Entity<RoleTable>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__RoleTabl__8AFACE3A63485DA2");

                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.RoleName).IsUnicode(false);

                entity.Property(e => e.Sentence).IsUnicode(false);
            });

            modelBuilder.Entity<SourceTable>(entity =>
            {
                entity.HasKey(e => e.SourceId)
                    .HasName("PK__SourceTa__16E019F998A8B850");

                entity.Property(e => e.SourceId).ValueGeneratedNever();

                entity.Property(e => e.ActualText).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Fields).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.RefNumber).IsUnicode(false);
            });

            modelBuilder.Entity<SourceTemplateTable>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK__SourceTe__F87ADD07AD87360D");

                entity.Property(e => e.TemplateId).ValueGeneratedNever();

                entity.Property(e => e.Bibliography).IsUnicode(false);

                entity.Property(e => e.Category).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.FieldDefs).IsUnicode(false);

                entity.Property(e => e.Footnote).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.ShortFootnote).IsUnicode(false);
            });

            modelBuilder.Entity<Urltable>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__URLTable__2D1221555875C285");

                entity.Property(e => e.LinkId).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);
            });

            modelBuilder.Entity<WitnessTable>(entity =>
            {
                entity.HasKey(e => e.WitnessId)
                    .HasName("PK__WitnessT__5FB1431DC9F5F250");

                entity.Property(e => e.WitnessId).ValueGeneratedNever();

                entity.Property(e => e.Given).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Prefix).IsUnicode(false);

                entity.Property(e => e.Sentence).IsUnicode(false);

                entity.Property(e => e.Suffix).IsUnicode(false);

                entity.Property(e => e.Surname).IsUnicode(false);
            });
        }
    }
}
