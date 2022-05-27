using System;
using System.Collections.Generic;
using System.Data;
using FTMContext.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AzureContext.Models
{
    public partial class AzureDBContext : DbContext
    {


        public AzureDBContext(string connectionString)
        {
            ConString = connectionString;
        }

        public AzureDBContext(DbContextOptions<AzureDBContext> options)
            : base(options)
        {


        }

        public virtual DbSet<ApiClaims> ApiClaims { get; set; }
        public virtual DbSet<ApiProperties> ApiProperties { get; set; }
        public virtual DbSet<ApiResources> ApiResources { get; set; }
        public virtual DbSet<ApiScopeClaims> ApiScopeClaims { get; set; }
        public virtual DbSet<ApiScopes> ApiScopes { get; set; }
        public virtual DbSet<ApiSecrets> ApiSecrets { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<ClientClaims> ClientClaims { get; set; }
        public virtual DbSet<ClientCorsOrigins> ClientCorsOrigins { get; set; }
        public virtual DbSet<ClientGrantTypes> ClientGrantTypes { get; set; }
        public virtual DbSet<ClientIdPrestrictions> ClientIdPrestrictions { get; set; }
        public virtual DbSet<ClientPostLogoutRedirectUris> ClientPostLogoutRedirectUris { get; set; }
        public virtual DbSet<ClientProperties> ClientProperties { get; set; }
        public virtual DbSet<ClientRedirectUris> ClientRedirectUris { get; set; }
        public virtual DbSet<ClientScopes> ClientScopes { get; set; }
        public virtual DbSet<ClientSecrets> ClientSecrets { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<DeviceCodes> DeviceCodes { get; set; }
        public virtual DbSet<Functions> Functions { get; set; }
        public virtual DbSet<IdentityClaims> IdentityClaims { get; set; }
        public virtual DbSet<IdentityProperties> IdentityProperties { get; set; }
        public virtual DbSet<IdentityResources> IdentityResources { get; set; }
        public virtual DbSet<LincsWills> LincsWills { get; set; }
        public virtual DbSet<Marriages> Marriages { get; set; }
        public virtual DbSet<MissingRecords> MissingRecords { get; set; }
        public virtual DbSet<MsgapplicationMapGroup> MsgapplicationMapGroup { get; set; }
        public virtual DbSet<Msgapplications> Msgapplications { get; set; }
        public virtual DbSet<MsgfunctionMapGroup> MsgfunctionMapGroup { get; set; }
        public virtual DbSet<Msgfunctions> Msgfunctions { get; set; }
        public virtual DbSet<MsggroupMapUser> MsggroupMapUser { get; set; }
        public virtual DbSet<Msggroups> Msggroups { get; set; }
        public virtual DbSet<NorfolkWillsRaw> NorfolkWillsRaw { get; set; }
        public virtual DbSet<ParishRecordSource> ParishRecordSource { get; set; }
        public virtual DbSet<ParishRecords> ParishRecords { get; set; }
        public virtual DbSet<ParishTranscriptionDetails> ParishTranscriptionDetails { get; set; }

        public virtual DbSet<ParishTranscriptionDetails2> ParishTranscriptionDetails2 { get; set; }
        public virtual DbSet<Parishs> Parishs { get; set; }
        public virtual DbSet<PersistedGrants> PersistedGrants { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<PersonsOfInterest> PersonsOfInterest { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<RelationTypes> RelationTypes { get; set; }
        public virtual DbSet<Relations> Relations { get; set; }
        public virtual DbSet<SourceMappingParishs> SourceMappingParishs { get; set; }
        public virtual DbSet<SourceMappingTypes> SourceMappingTypes { get; set; }
        public virtual DbSet<SourceMappings> SourceMappings { get; set; }
        public virtual DbSet<SourceTypes> SourceTypes { get; set; }
        public virtual DbSet<Sources> Sources { get; set; }
        public virtual DbSet<Tokens> Tokens { get; set; }

        public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
        public virtual DbSet<DupeEntry> DupeEntries { get; set; }

        public virtual DbSet<TreeRecord> TreeRecord { get; set; }

        public virtual DbSet<Relationships> Relationships { get; set; }

        private string ConString = "";


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConString);
            }
        }

        public void ExecuteCommand(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {

                var command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ident stuff

            modelBuilder.Entity<MsgapplicationMapGroup>(entity =>
            {
                entity.ToTable("MSGApplicationMapGroup");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.MsgapplicationMapGroup)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MSGApplicationMapGroup_MSGApplications");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MsgapplicationMapGroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MSGApplicationMapGroup_MSGGroups");
            });

            modelBuilder.Entity<Msgapplications>(entity =>
            {
                entity.ToTable("MSGApplications");

                entity.Property(e => e.ApplicationName).IsRequired();
            });

            modelBuilder.Entity<MsgfunctionMapGroup>(entity =>
            {
                entity.ToTable("MSGFunctionMapGroup");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.MsgfunctionMapGroup)
                    .HasForeignKey(d => d.FunctionId)
                    .HasConstraintName("FK_MSGFunctionMapGroup_MSGFunctions");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MsgfunctionMapGroup)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_MSGFunctionMapGroup_MSGGroups");
            });

            modelBuilder.Entity<Msgfunctions>(entity =>
            {
                entity.ToTable("MSGFunctions");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<MsggroupMapUser>(entity =>
            {
                entity.ToTable("MSGGroupMapUser");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MsggroupMapUser)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_MSGGroupMapUser_MSGGroups");
            });

            modelBuilder.Entity<Msggroups>(entity =>
            {
                entity.ToTable("MSGGroups");
            });


            modelBuilder.Entity<Tokens>(entity =>
            {
                entity.Property(e => e.ImageUrl).HasMaxLength(200);

                entity.Property(e => e.Locale).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Refresh).HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(200);
            });


            modelBuilder.Entity<ApiClaims>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiClaims)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiProperties>(entity =>
            {
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiProperties)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiResources>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ApiScopeClaims>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiScope)
                    .WithMany(p => p.ApiScopeClaims)
                    .HasForeignKey(d => d.ApiScopeId);
            });

            modelBuilder.Entity<ApiScopes>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiScopes)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiSecrets>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiSecrets)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NameIdentifier).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<ClientClaims>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientClaims)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientCorsOrigins>(entity =>
            {
                entity.Property(e => e.Origin)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientCorsOrigins)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientGrantTypes>(entity =>
            {
                entity.Property(e => e.GrantType)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientGrantTypes)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientIdPrestrictions>(entity =>
            {
                entity.ToTable("ClientIdPRestrictions");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientIdPrestrictions)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUris>(entity =>
            {
                entity.Property(e => e.PostLogoutRedirectUri)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientPostLogoutRedirectUris)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientProperties>(entity =>
            {
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientProperties)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientRedirectUris>(entity =>
            {
                entity.Property(e => e.RedirectUri)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientRedirectUris)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientScopes>(entity =>
            {
                entity.Property(e => e.Scope)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientScopes)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientSecrets>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientSecrets)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<Clients>(entity =>
            {
                entity.Property(e => e.BackChannelLogoutUri).HasMaxLength(2000);

                entity.Property(e => e.ClientClaimsPrefix).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ClientName).HasMaxLength(200);

                entity.Property(e => e.ClientUri).HasMaxLength(2000);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.FrontChannelLogoutUri).HasMaxLength(2000);

                entity.Property(e => e.LogoUri).HasMaxLength(2000);

                entity.Property(e => e.PairWiseSubjectSalt).HasMaxLength(200);

                entity.Property(e => e.ProtocolType)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserCodeType).HasMaxLength(100);
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.Property(e => e.IdentityId).HasMaxLength(450);

                entity.HasOne(d => d.Identity)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.IdentityId);
            });

            modelBuilder.Entity<DeviceCodes>(entity =>
            {
                entity.HasKey(e => e.UserCode);

                entity.Property(e => e.UserCode).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.DeviceCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SubjectId).HasMaxLength(200);
            });

            modelBuilder.Entity<Functions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<IdentityClaims>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdentityResource)
                    .WithMany(p => p.IdentityClaims)
                    .HasForeignKey(d => d.IdentityResourceId);
            });

            modelBuilder.Entity<IdentityProperties>(entity =>
            {
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.IdentityResource)
                    .WithMany(p => p.IdentityProperties)
                    .HasForeignKey(d => d.IdentityResourceId);
            });

            modelBuilder.Entity<IdentityResources>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            #endregion


            modelBuilder.Entity<NorfolkWillsRaw>(entity =>
            {
                entity.ToTable("NorfolkWillsRaw", "Wills");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<LincsWills>(entity =>
            {
                entity.ToTable("LincsWills", "Wills");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Aliases).IsUnicode(false);

                entity.Property(e => e.Collection).IsUnicode(false);

                entity.Property(e => e.DateString).HasMaxLength(500);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.Occupation).IsUnicode(false);

                entity.Property(e => e.Place).IsUnicode(false);

                entity.Property(e => e.Reference).IsUnicode(false);

                entity.Property(e => e.Surname).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);
            });




            modelBuilder.Entity<DupeEntry>(entity =>
            {
                entity.ToTable("DupeEntries", "DNA");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Ident).HasMaxLength(500);

                entity.Property(e => e.Origin).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(500);

                entity.Property(e => e.Surname).HasMaxLength(500);

            });

            modelBuilder.Entity<FTMPersonView>(entity =>
            {
                entity.ToTable("FTMPersonView", "DNA");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FirstName).HasMaxLength(500);

                entity.Property(e => e.Surname).HasMaxLength(500);

                entity.Property(e => e.Origin).HasMaxLength(250);

                entity.Property(e => e.BirthLat).HasColumnType("decimal(14, 10)");

                entity.Property(e => e.BirthLong).HasColumnType("decimal(14, 10)");

                entity.Property(e => e.AltLat).HasColumnType("decimal(14, 10)");

                entity.Property(e => e.AltLong).HasColumnType("decimal(14, 10)");

            });

            modelBuilder.Entity<Relationships>(entity =>
            {
                entity.ToTable("Relationships", "DNA");

                entity.Property(e => e.Id).ValueGeneratedNever();
                 

            });

            modelBuilder.Entity<TreeRecord>(entity =>
            {
                entity.ToTable("TreeRecord", "DNA");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name);

                entity.Property(e => e.Origin).HasMaxLength(250);



            });

            modelBuilder.Entity<PersonsOfInterest>(entity =>
            {
                entity.ToTable("PersonsOfInterest", "DNA");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TreeUrl).HasColumnName("TreeURL");
            });


            modelBuilder.Entity<Places>(entity =>
            {
                entity.ToTable("Places", "UKP");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Bua11cd)
                    .HasColumnName("bua11cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ctry15nm)
                    .HasColumnName("ctry15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cty15cd)
                    .HasColumnName("cty15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cty15nm)
                    .HasColumnName("cty15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ctyhistnm)
                    .HasColumnName("ctyhistnm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ctyltnm)
                    .HasColumnName("ctyltnm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descnm)
                    .HasColumnName("descnm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eer15cd)
                    .HasColumnName("eer15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eer15nm)
                    .HasColumnName("eer15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fid)
                    .HasColumnName("FID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Grid1km)
                    .HasColumnName("grid1km")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gridgb1e)
                    .HasColumnName("gridgb1e")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gridgb1m)
                    .HasColumnName("gridgb1m")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gridgb1n)
                    .HasColumnName("gridgb1n")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Hlth12cd)
                    .HasColumnName("hlth12cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Hlth12nm)
                    .HasColumnName("hlth12nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lad15cd)
                    .HasColumnName("lad15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lad15nm)
                    .HasColumnName("lad15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Laddescnm)
                    .HasColumnName("laddescnm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lat)
                    .HasColumnName("lat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Long)
                    .HasColumnName("long")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Npark15cd)
                    .HasColumnName("npark15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Npark15nm)
                    .HasColumnName("npark15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Par15cd)
                    .HasColumnName("par15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pcon15cd)
                    .HasColumnName("pcon15cd")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Pcon15nm)
                    .HasColumnName("pcon15nm")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Pfa15cd)
                    .HasColumnName("pfa15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pfa15nm)
                    .HasColumnName("pfa15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Place15cd)
                    .HasColumnName("place15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Place15nm)
                    .HasColumnName("place15nm")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Placeid).HasColumnName("placeid");

                entity.Property(e => e.Placesort)
                    .HasColumnName("placesort")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Popcnt)
                    .HasColumnName("popcnt")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Regd15cd)
                    .HasColumnName("regd15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Regd15nm)
                    .HasColumnName("regd15nm")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Rgn15cd)
                    .HasColumnName("rgn15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rgn15nm)
                    .HasColumnName("rgn15nm")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Splitind)
                    .HasColumnName("splitind")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Wd15cd)
                    .HasColumnName("wd15cd")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });


            #region thackraydb

            modelBuilder.Entity<Marriages>(entity =>
            {
                entity.HasKey(e => e.MarriageId);

                entity.ToTable("Marriages", "TDB");

                entity.Property(e => e.MarriageId)
                    .HasColumnName("Marriage_Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Date)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.DateLastEdit).HasColumnType("date");

                entity.Property(e => e.FemaleCname)
                    .HasColumnName("FemaleCName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FemaleInfo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FemaleLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FemaleOccupation).HasMaxLength(500);

                entity.Property(e => e.FemaleSname)
                    .HasColumnName("FemaleSName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaleCname)
                    .HasColumnName("MaleCName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaleInfo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaleLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaleOccupation).HasMaxLength(500);

                entity.Property(e => e.MaleSname)
                    .HasColumnName("MaleSName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarriageCounty).HasMaxLength(500);

                entity.Property(e => e.MarriageLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrigFemaleSurname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrigMaleSurname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Source).HasMaxLength(50);

                entity.Property(e => e.UniqueRef).HasMaxLength(50);

                entity.Property(e => e.Witness1).HasMaxLength(500);

                entity.Property(e => e.Witness2).HasMaxLength(500);

                entity.Property(e => e.Witness3).HasMaxLength(500);

                entity.Property(e => e.Witness4).HasMaxLength(500);
            });

            modelBuilder.Entity<MissingRecords>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MissingRecords", "TDB");

                entity.Property(e => e.MissingRecordId).ValueGeneratedOnAdd();

                entity.Property(e => e.RecordType)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ParishRecordSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ParishRecordSource", "TDB");

                entity.Property(e => e.RecordTypeDescription).HasMaxLength(500);

                entity.Property(e => e.RecordTypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<ParishRecords>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ParishRecords", "TDB");

                entity.Property(e => e.RecordType)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ParishTranscriptionDetails>(entity =>
            {
                entity.HasKey(e => e.ParishTranscriptionId);

                //            entity.ToTable("ParishTranscriptionDetails", "TDB");

                entity.Property(e => e.ParishDataString).HasMaxLength(500);
            });

            modelBuilder.Entity<ParishTranscriptionDetails2>(entity =>
            {
                entity.HasKey(e => e.ParishTranscriptionId);

                //            entity.ToTable("ParishTranscriptionDetails", "TDB");

                entity.Property(e => e.ParishDataString).HasMaxLength(500);
            });

            modelBuilder.Entity<Parishs>(entity =>
            {
                entity.HasKey(e => e.ParishId);

                entity.ToTable("Parishs", "TDB");

                entity.Property(e => e.ParishId).ValueGeneratedNever();

                entity.Property(e => e.ParentParish).HasMaxLength(500);

                entity.Property(e => e.ParishCounty).HasMaxLength(50);

                entity.Property(e => e.ParishName).HasMaxLength(500);

                entity.Property(e => e.ParishNotes).HasMaxLength(1000);

                entity.Property(e => e.ParishRegistersDeposited).HasMaxLength(500);

                entity.Property(e => e.ParishX).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.ParishY).HasColumnType("decimal(12, 6)");
            });

            modelBuilder.Entity<PersistedGrants>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.Property(e => e.Key).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.SubjectId).HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Persons>(entity =>
            {
                entity.HasKey(e => e.PersonId);

                entity.ToTable("Persons", "TDB");

                entity.Property(e => e.PersonId)
                    .HasColumnName("Person_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BaptismDateStr).HasMaxLength(50);

                entity.Property(e => e.BirthCounty).HasMaxLength(500);

                entity.Property(e => e.BirthDateStr).HasMaxLength(50);

                entity.Property(e => e.BirthLocation).HasMaxLength(500);

                entity.Property(e => e.ChristianName).HasMaxLength(150);

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.DateLastEdit).HasColumnType("date");

                entity.Property(e => e.DeathCounty).HasMaxLength(500);

                entity.Property(e => e.DeathDateStr).HasMaxLength(50);

                entity.Property(e => e.DeathLocation).HasMaxLength(500);

                entity.Property(e => e.FatherChristianName).HasMaxLength(150);

                entity.Property(e => e.FatherOccupation).HasMaxLength(150);

                entity.Property(e => e.FatherSurname).HasMaxLength(500);

                entity.Property(e => e.MotherChristianName).HasMaxLength(500);

                entity.Property(e => e.MotherSurname).HasMaxLength(500);

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Occupation).HasMaxLength(150);

                entity.Property(e => e.OrigFatherSurname).HasMaxLength(150);

                entity.Property(e => e.OrigMotherSurname).HasMaxLength(150);

                entity.Property(e => e.OrigSurname).HasMaxLength(150);

                entity.Property(e => e.ReferenceDateStr).HasMaxLength(50);

                entity.Property(e => e.ReferenceLocation).HasMaxLength(150);

                entity.Property(e => e.Source).HasMaxLength(50);

                entity.Property(e => e.SpouseName).HasMaxLength(150);

                entity.Property(e => e.SpouseSurname).HasMaxLength(150);

                entity.Property(e => e.Surname).HasMaxLength(100);

                entity.Property(e => e.UniqueRef).HasMaxLength(50);
            });

            modelBuilder.Entity<RelationTypes>(entity =>
            {
                entity.HasKey(e => e.RelationTypeId);

                entity.ToTable("RelationTypes", "TDB");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.RelationName).HasMaxLength(50);
            });

            modelBuilder.Entity<Relations>(entity =>
            {
                entity.HasKey(e => e.RelationId);

                entity.ToTable("Relations", "TDB");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");
            });

            modelBuilder.Entity<SourceMappingParishs>(entity =>
            {
                entity.HasKey(e => e.SourceMappingParishsRowId);

                entity.ToTable("SourceMappingParishs", "TDB");

                entity.Property(e => e.SourceMappingParishsRowId).ValueGeneratedNever();

                entity.Property(e => e.SourceMappingDateAdded).HasColumnType("datetime");
            });

            modelBuilder.Entity<SourceMappingTypes>(entity =>
            {
                entity.HasKey(e => e.SourceMapTypeId);

                entity.ToTable("SourceMappingTypes", "TDB");

                entity.Property(e => e.SourceMapTypeId).ValueGeneratedNever();

                entity.Property(e => e.SourceMapTypeDescription)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SourceMappings>(entity =>
            {
                entity.HasKey(e => e.MappingId)
                    .HasName("PK_RecordMapSource");

                entity.ToTable("SourceMappings", "TDB");

                entity.Property(e => e.MappingId).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("date");
            });

            modelBuilder.Entity<SourceTypes>(entity =>
            {
                entity.HasKey(e => e.SourceTypeId);

                entity.ToTable("SourceTypes", "TDB");

                entity.Property(e => e.SourceDateAdded)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.SourceTypeDesc).HasMaxLength(50);
            });

            modelBuilder.Entity<Sources>(entity =>
            {
                entity.HasKey(e => e.SourceId);

                entity.ToTable("Sources", "TDB");

                entity.Property(e => e.SourceId).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.OriginalLocation).HasMaxLength(500);

                entity.Property(e => e.SourceDateStr)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.SourceDateStrTo)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.SourceDescription).HasColumnType("text");

                entity.Property(e => e.SourceNotes).HasColumnType("text");

                entity.Property(e => e.SourceRef).HasMaxLength(500);
            });

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        

        public static void BulkInsert(string connectionString, List<FTMPersonView> rows)
        {
            using (var executor = new AzureDBContext(connectionString))
            {
                 
                executor.FTMPersonView.Add(new FTMPersonView()
                {
                    FirstName = "x",
                    AltLat = 0,
                    AltLocation = "x",
                    AltLocationDesc = "x",
                    AltLong = 0,
                    BirthFrom = 0,
                    BirthLat = 0,
                    BirthLocation = "x",
                    BirthLong = 0,
                    BirthTo = 0,
                    FatherId = 0,
                    Id = 0,
                    MotherId = 0,
                    Origin = "x",
                    PersonId = 0,
                    Surname = "x"
                });

                executor.SaveChanges();
            }

            var dt = CreateDataTable(connectionString);

            using (var executor = new AzureDBContext(connectionString))
            {
                executor.ExecuteCommand("TRUNCATE TABLE DNA.FTMPersonView"); 
            }


            int idx = 0;
            foreach (var row in rows)
            {
                dt.Rows.Add(idx,
                    row.FirstName,
                    row.Surname,
                    row.BirthFrom,
                    row.BirthTo, 
                    row.BirthLocation, 
                    row.BirthLat, 
                    row.BirthLong, 
                    row.AltLocationDesc, 
                    row.AltLocation, row.AltLat, row.AltLong, row.Origin, row.PersonId, row.FatherId, row.MotherId);
 
                idx++;
            }


            using (var copy = new SqlBulkCopy(connectionString))
            {


                copy.DestinationTableName = "dna.FTMPersonView";
                copy.BulkCopyTimeout = 600;
                copy.ColumnMappings.Add("ID", "ID");
                copy.ColumnMappings.Add("FirstName", "FirstName");
                copy.ColumnMappings.Add("Surname", "Surname");
                copy.ColumnMappings.Add("BirthFrom", "BirthFrom");
                copy.ColumnMappings.Add("BirthTo", "BirthTo");
                copy.ColumnMappings.Add("BirthLocation", "BirthLocation");
                copy.ColumnMappings.Add("BirthLat", "BirthLat");
                copy.ColumnMappings.Add("BirthLong", "BirthLong");
                copy.ColumnMappings.Add("AltLocationDesc", "AltLocationDesc");
                copy.ColumnMappings.Add("AltLocation", "AltLocation");
                copy.ColumnMappings.Add("AltLat", "AltLat");
                copy.ColumnMappings.Add("AltLong", "AltLong");
                copy.ColumnMappings.Add("Origin", "Origin");
                copy.ColumnMappings.Add("PersonID", "PersonID");
                copy.ColumnMappings.Add("FatherId", "FatherId");
                copy.ColumnMappings.Add("MotherId", "MotherId");

                copy.WriteToServer(dt);
            }
        }


        private static DataTable CreateDataTable(string connectionString)
        {
           

            
             
            DataColumnCollection Columns;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("select top 1 * from dna.FTMPersonView", con))
                {
                    using (var r = command.ExecuteReader())
                    {
                        using (var dt = new DataTable())
                        {
                            dt.Load(r);
                            Columns = dt.Columns;
                        }
                    }

                }
                con.Close();
            }



            DataTable dataTable = new DataTable();

            while (Columns.Count > 0)
            {
                DataColumn c = Columns[0];
                c.Table.Columns.Remove(c);

                dataTable.Columns.Add(c);
            }
            Columns = dataTable.Columns;
            
            return dataTable;

        }
    }
}
