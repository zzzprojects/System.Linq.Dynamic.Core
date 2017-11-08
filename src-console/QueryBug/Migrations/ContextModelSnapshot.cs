﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace QueryBug.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ChildEntity", b =>
                {
                    b.Property<int>("ChildEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RootEntityId");

                    b.HasKey("ChildEntityId");

                    b.HasIndex("RootEntityId");

                    b.ToTable("ChildEntities");
                });

            modelBuilder.Entity("ParentEntity", b =>
                {
                    b.Property<int>("ParentEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ParentEntityId");

                    b.ToTable("ParentEntities");
                });

            modelBuilder.Entity("RootEntity", b =>
                {
                    b.Property<int>("RootEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("ParentEntityId");

                    b.HasKey("RootEntityId");

                    b.HasIndex("ParentEntityId");

                    b.ToTable("RootEntities");
                });

            modelBuilder.Entity("ChildEntity", b =>
                {
                    b.HasOne("RootEntity", "RootEntity")
                        .WithMany("Children")
                        .HasForeignKey("RootEntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RootEntity", b =>
                {
                    b.HasOne("ParentEntity", "ParentEntity")
                        .WithMany()
                        .HasForeignKey("ParentEntityId");
                });
#pragma warning restore 612, 618
        }
    }
}
