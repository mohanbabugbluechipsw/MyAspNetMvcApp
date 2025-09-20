using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Model_New.Models;

public partial class MrAppDbNewContext : DbContext
{
    public MrAppDbNewContext()
    {
    }

    public MrAppDbNewContext(DbContextOptions<MrAppDbNewContext> options)
        : base(options)
    {
    }






    public virtual DbSet<OutLetMasterDetail> OutLetMasterDetails { get; set; }




    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblDistributor> TblDistributors { get; set; }


    public virtual DbSet<TblOfficeLocation> TblOfficeLocations { get; set; }



    public virtual DbSet<TblSystemUser> TblSystemUsers { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserType> TblUserTypes { get; set; }

    public virtual DbSet<TblUslworkLevel> TblUslworkLevels { get; set; }


    

    public DbSet<Tbl_capturedata_log> tblcapturedatalog { get; set; }

    public DbSet<Tbl_SRMaster_data>tbl_SRMaster_Datas { get; set; }

    public DbSet<tbl_OSA_question>Tbl_OSA_Questions { get; set; }


    public DbSet<Tbl_OSA_ReviewAnswer_MR>tbl_OSA_ReviewAnswer_MR { get; set; }


    public DbSet<Tbl_ModelDisplay> tbl_ModelDisplays { get; set; }

    public DbSet<Tbl_SachetHanger> tbl_SachetHangers { get; set; }

    public DbSet<Tbl_SavourySection> tbl_SavourySections { get; set; }

    public DbSet<Tbl_LaundrySection> tbl_LaundrySections { get; set; }

    public DbSet<Tbl_HfdSection> tbl_HfdSections { get; set; }

    public DbSet<Tbl_Distributor_Region>tbl_Distributor_Regions { get; set; }

    public DbSet<Tbl_FSWSdetail>  tbl_FSWSdetails { get; set; }

    public DbSet<Tbl_ChannelHierarchy_Master> tbl_ChannelHierarchy_Masters { get; set; }

    public DbSet<Tbl_Placement_descrption> tbl_Placement_Descrptions { get; set; }

    public DbSet<Tbl_Placement_EmergencyLM> tbl_Placement_EmergencyLMs { get; set; }

    public DbSet<Tbl_MRSrMapping> tbl_MRSrMappings { get; set; }


    public DbSet<Tbl_Text_question_detail> Tbl_Text_question_details { get; set; }


    public DbSet<Tbl_LogindetailsCapture> Tbl_LogindetailsCapture { get; set; }


    public DbSet<Tbl_selfservicequestion_details> tbl_Selfservicequestion_Details { get; set; }

    public DbSet<tbl_UserGeoCodeDetails> tbl_UserGeoCodeDetails { get; set; }

    public DbSet<tbl_Savereview_details> tbl_Savereview_details { get; set; }

    public DbSet<Tbl_QuestionLink> Tbl_QuestionLink { get; set; }

    public DbSet<tbl_ReviewProgress> tbl_ReviewProgress { get; set; }









    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    
    => optionsBuilder.UseSqlServer("Server=tcp:sqlmrtestindian.database.windows.net,1433;Initial Catalog=Sqldb-MRtest-India-South-002;Persist Security Info=False;User ID=MRATest;Password=Inter!face1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    );


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       

      

      

      
       

        modelBuilder.Entity<OutLetMasterDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OutLetMa__3214EC0746EEAAAD");

            entity.ToTable("OutLetMaster_Details");

            entity.Property(e => e.Address1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address4)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(18, 10)")
                .HasColumnName("LATITUDE");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(18, 10)")
                .HasColumnName("LONGITUDE");
            entity.Property(e => e.OlCreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("OL_CREATED_DATE");
            entity.Property(e => e.ParStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAR_STATUS");
            entity.Property(e => e.PartyHllcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PartyHLLCode");
            entity.Property(e => e.PartyMasterCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PartyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryChannel)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PrimarychannelCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Primarychannel_Code");
            entity.Property(e => e.RsName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("RS_NAME");
            entity.Property(e => e.Rscode).HasColumnName("RSCODE");
            entity.Property(e => e.SecondaryChannel)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SecondarychannelCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Secondarychannel_Code");
            entity.Property(e => e.UpdateStamp).HasColumnName("UPDATE_STAMP");
        });

      

       

       

      

      
       
        modelBuilder.Entity<TblDepartment>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Tbl_Depa__B2079BEDB4296274");

            entity.ToTable("Tbl_Department");

            entity.Property(e => e.DepartmentName).HasMaxLength(255);
        });

        modelBuilder.Entity<TblDistributor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Dist__3214EC07E4640F3C");

            entity.ToTable("Tbl_Distributor");

            entity.Property(e => e.DistributorName)
                .HasMaxLength(255)
                .HasColumnName("Distributor_Name");
        });

      

        modelBuilder.Entity<TblOfficeLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Tbl_Offi__E7FEA497DC008CF0");

            entity.ToTable("Tbl_OfficeLocation");

            entity.Property(e => e.LocationName).HasMaxLength(255);
        });



        modelBuilder.Entity<TblSystemUser>(entity =>
        {
            entity.HasKey(e => e.SystemUserId).HasName("PK__Tbl_Syst__8788C295B9C08C11");

            entity.ToTable("Tbl_SystemUser");

            entity.HasIndex(e => e.EmpNo, "UQ_EmpNo").IsUnique();

            entity.Property(e => e.EmpNo).HasMaxLength(6);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.UserType)
                  .WithMany() // No navigation collection in TblUserType
                  .HasForeignKey(d => d.UserTypeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_UserType");
        });




        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Tbl_User__1788CC4C9F12E7F0");

            entity.ToTable("Tbl_User");

            entity.Property(e => e.EmpNo).HasMaxLength(10);
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.EmpName).HasMaxLength(255);
            entity.Property(e => e.EmpEmail).HasMaxLength(255);
            entity.Property(e => e.HireDate).HasColumnType("datetime");
            entity.Property(e => e.LineManager).HasMaxLength(255);
            entity.Property(e => e.LineManagerEmail).IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            // 🔗 Location relation
            entity.HasOne(d => d.Location)
                .WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Location");

            // 🔗 UserType relation
            entity.HasOne(d => d.UserType)
                .WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserType");
        });


        modelBuilder.Entity<TblUserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeId).HasName("PK__Tbl_User__40D2D816ED87254C");

            entity.ToTable("Tbl_UserType");

            entity.Property(e => e.UserTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<TblUslworkLevel>(entity =>
        {
            entity.HasKey(e => e.Wlid).HasName("PK__Tbl_USLW__F973E5E277F59662");

            entity.ToTable("Tbl_USLWorkLevel");

            entity.Property(e => e.Wlid)
                .ValueGeneratedNever()
                .HasColumnName("WLId");

            entity.Property(e => e.Wl)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WL");

            entity.Property(e => e.Wlstatus)
                .HasColumnName("WLStatus");
        });


    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





   





}
