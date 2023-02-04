﻿// <auto-generated />
using System;
using GusMelfordBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GusMelfordBot.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.7.22376.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ContentUser", b =>
                {
                    b.Property<Guid>("ContentsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("ContentsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ContentUser");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.AttemptMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Attempt")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("AttemptMessages");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.AuthorizationUserDatum", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("AuthorizationUserData");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Content", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MetaContentId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OriginalLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("MetaContentId");

                    b.HasIndex("OriginalLink")
                        .IsUnique();

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Feature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Features");

                    b.HasData(
                        new
                        {
                            Id = new Guid("2d4cde7f-d469-4acd-ae22-5afe940c64fe"),
                            CreatedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7625),
                            ModifiedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7625),
                            Name = "Abyss"
                        });
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.FunnyPhrase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FunnyPhrases");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.MetaContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Duration")
                        .HasColumnType("integer");

                    b.Property<int?>("Height")
                        .HasColumnType("integer");

                    b.Property<bool>("IsSaved")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("TelegramMessageId")
                        .HasColumnType("integer");

                    b.Property<int?>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("MetaContents");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("114a09a9-801e-4863-b5d7-388c49291e8c"),
                            CreatedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7544),
                            ModifiedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7546),
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("f5ed7961-ad8a-4f4d-9144-d6678df1c557"),
                            CreatedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7590),
                            ModifiedOn = new DateTime(2023, 2, 4, 15, 14, 30, 742, DateTimeKind.Utc).AddTicks(7590),
                            Name = "User"
                        });
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.TelegramUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.UserContentComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ContentId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserContentComments");
                });

            modelBuilder.Entity("ContentUser", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Content", null)
                        .WithMany()
                        .HasForeignKey("ContentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.AttemptMessage", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Feature", "Feature")
                        .WithMany()
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GusMelfordBot.Infrastructure.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.AuthorizationUserDatum", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", "User")
                        .WithOne("AuthorizationUserDatum")
                        .HasForeignKey("GusMelfordBot.Infrastructure.Models.AuthorizationUserDatum", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Content", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GusMelfordBot.Infrastructure.Models.MetaContent", "MetaContent")
                        .WithMany()
                        .HasForeignKey("MetaContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("MetaContent");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.FunnyPhrase", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Group", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Feature", "Feature")
                        .WithMany("Groups")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.TelegramUser", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.User", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.UserContentComment", b =>
                {
                    b.HasOne("GusMelfordBot.Infrastructure.Models.Content", "Content")
                        .WithMany("UserContentComments")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GusMelfordBot.Infrastructure.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Content");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Content", b =>
                {
                    b.Navigation("UserContentComments");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Feature", b =>
                {
                    b.Navigation("Groups");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GusMelfordBot.Infrastructure.Models.User", b =>
                {
                    b.Navigation("AuthorizationUserDatum")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}