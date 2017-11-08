using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace QueryBug.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentEntities",
                columns: table => new
                {
                    ParentEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentEntities", x => x.ParentEntityId);
                });

            migrationBuilder.CreateTable(
                name: "RootEntities",
                columns: table => new
                {
                    RootEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootEntities", x => x.RootEntityId);
                    table.ForeignKey(
                        name: "FK_RootEntities_ParentEntities_ParentEntityId",
                        column: x => x.ParentEntityId,
                        principalTable: "ParentEntities",
                        principalColumn: "ParentEntityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChildEntities",
                columns: table => new
                {
                    ChildEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RootEntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildEntities", x => x.ChildEntityId);
                    table.ForeignKey(
                        name: "FK_ChildEntities_RootEntities_RootEntityId",
                        column: x => x.RootEntityId,
                        principalTable: "RootEntities",
                        principalColumn: "RootEntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildEntities_RootEntityId",
                table: "ChildEntities",
                column: "RootEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RootEntities_ParentEntityId",
                table: "RootEntities",
                column: "ParentEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildEntities");

            migrationBuilder.DropTable(
                name: "RootEntities");

            migrationBuilder.DropTable(
                name: "ParentEntities");
        }
    }
}
