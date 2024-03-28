using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CodeFirst.DataAccess.Migrations
{
    public partial class PostgreSqlInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actors",
                columns: table => new
                {
                    actor_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("actor_pkey", x => x.actor_id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("client_pkey", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    salary = table.Column<float>(type: "real", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_id", x => x.employee_id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: false),
                    age_restriction = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("movies_pkey", x => x.movie_id);
                });

            migrationBuilder.CreateTable(
                name: "copies",
                columns: table => new
                {
                    copy_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    available = table.Column<bool>(type: "boolean", nullable: false),
                    movie_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("copies_pkey", x => x.copy_id);
                    table.ForeignKey(
                        name: "FK_copies_movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "starring",
                columns: table => new
                {
                    actor_id = table.Column<int>(type: "integer", nullable: false),
                    movie_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_starring", x => new { x.actor_id, x.movie_id });
                    table.ForeignKey(
                        name: "FK_starring_actors_actor_id",
                        column: x => x.actor_id,
                        principalTable: "actors",
                        principalColumn: "actor_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_starring_movies_movie_id",
                        column: x => x.movie_id,
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    copy_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    date_of_rental = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_of_return = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rentals_pkey", x => new { x.client_id, x.copy_id });
                    table.ForeignKey(
                        name: "FK_rentals_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rentals_copies_copy_id",
                        column: x => x.copy_id,
                        principalTable: "copies",
                        principalColumn: "copy_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "actors",
                columns: new[] { "actor_id", "birthday", "first_name", "last_name" },
                values: new object[,]
                {
                    { 1, new DateTime(1947, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arnold", "Schwarzenegger" },
                    { 28, new DateTime(1962, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tom", "Cruise" },
                    { 27, new DateTime(1981, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Natalie", "Portman" },
                    { 26, new DateTime(1958, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gary", "Oldman" },
                    { 24, new DateTime(1948, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Billy", "Crystal" },
                    { 23, new DateTime(1948, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jean", "Reno" },
                    { 22, new DateTime(1966, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emmanuelle", "Seigner" },
                    { 21, new DateTime(1942, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harrison", "Ford" },
                    { 20, new DateTime(1965, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charlie", "Sheen" },
                    { 19, new DateTime(1955, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Willem", "Dafoe" },
                    { 18, new DateTime(1949, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tom", "Berenger" },
                    { 17, new DateTime(1950, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cybill", "Shepherd" },
                    { 16, new DateTime(1939, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harvey", "Keitel" },
                    { 15, new DateTime(1962, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jodie", "Foster" },
                    { 25, new DateTime(1963, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lisa", "Kudrow" },
                    { 13, new DateTime(1949, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sigourney", "Weaver" },
                    { 12, new DateTime(1952, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dan", "Aykroyd" },
                    { 11, new DateTime(1950, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bill", "Murray" },
                    { 10, new DateTime(1956, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Linda", "Hamilton" },
                    { 9, new DateTime(1956, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael", "Biehn" },
                    { 8, new DateTime(1935, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Peter", "Mayhew" },
                    { 7, new DateTime(1944, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "Prowse" },
                    { 6, new DateTime(1913, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Peter", "Cushing" },
                    { 5, new DateTime(1914, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alec", "Guiness" },
                    { 4, new DateTime(1956, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carrie", "Fisher" },
                    { 3, new DateTime(1942, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harrison", "Ford" },
                    { 2, new DateTime(1946, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anthony", "Daniels" },
                    { 14, new DateTime(1943, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "De Niro" }
                });

            migrationBuilder.InsertData(
                table: "clients",
                columns: new[] { "client_id", "birthday", "first_name", "last_name" },
                values: new object[,]
                {
                    { 6, new DateTime(1965, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rick", "Sanchez" },
                    { 5, new DateTime(2012, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lisa", "Simpson" },
                    { 4, new DateTime(1977, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bob", "Belcher" },
                    { 2, new DateTime(2011, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brian", "Griffin" },
                    { 1, new DateTime(1954, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hank", "Hill" },
                    { 3, new DateTime(1989, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gary", "Goodspeed" }
                });

            migrationBuilder.InsertData(
                table: "employees",
                columns: new[] { "employee_id", "City", "first_name", "last_name", "salary" },
                values: new object[,]
                {
                    { 1, "New York", "John", "Smith", 150f },
                    { 2, "New York", "Ben", "Johnson", 250f },
                    { 3, "New Orleans", "Louis", "Armstrong", 75f },
                    { 4, "London", "John", "Lennon", 300f },
                    { 5, "London", "Peter", "Gabriel", 150f }
                });

            migrationBuilder.InsertData(
                table: "movies",
                columns: new[] { "movie_id", "age_restriction", "price", "title", "year" },
                values: new object[,]
                {
                    { 8, 16, 10.5f, "Analyze This", 1999 },
                    { 7, 13, 9.5f, "Ronin", 1998 },
                    { 6, 15, 8.5f, "Frantic", 1988 },
                    { 5, 18, 5f, "Platoon", 1986 },
                    { 1, 12, 10f, "Star Wars Episode IV: A New Hope", 1979 },
                    { 3, 15, 8.5f, "Terminator", 1984 },
                    { 2, 12, 5.5f, "Ghostbusters", 1984 },
                    { 9, 16, 8.5f, "Leon: the Professional", 1994 },
                    { 4, 17, 5f, "Taxi Driver", 1976 },
                    { 10, 13, 8.5f, "Mission Impossible", 1996 }
                });

            migrationBuilder.InsertData(
                table: "copies",
                columns: new[] { "copy_id", "available", "movie_id" },
                values: new object[,]
                {
                    { 1, true, 1 },
                    { 11, true, 6 },
                    { 8, false, 5 },
                    { 12, true, 7 },
                    { 13, true, 7 },
                    { 7, true, 4 },
                    { 14, false, 8 },
                    { 6, true, 3 },
                    { 10, false, 6 },
                    { 4, true, 3 },
                    { 5, false, 3 },
                    { 17, false, 10 },
                    { 2, false, 1 },
                    { 20, true, 10 },
                    { 19, true, 10 },
                    { 18, true, 10 },
                    { 15, true, 9 },
                    { 9, true, 6 },
                    { 3, true, 2 },
                    { 16, true, 10 }
                });

            migrationBuilder.InsertData(
                table: "starring",
                columns: new[] { "actor_id", "movie_id" },
                values: new object[,]
                {
                    { 23, 7 },
                    { 14, 8 },
                    { 14, 7 },
                    { 27, 9 },
                    { 23, 9 },
                    { 22, 6 },
                    { 21, 6 },
                    { 24, 8 },
                    { 25, 8 },
                    { 18, 5 },
                    { 19, 5 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 11, 2 },
                    { 20, 5 },
                    { 12, 2 },
                    { 1, 3 },
                    { 9, 3 },
                    { 10, 3 },
                    { 14, 4 },
                    { 15, 4 },
                    { 16, 4 },
                    { 17, 4 },
                    { 23, 10 },
                    { 13, 2 },
                    { 28, 10 }
                });

            migrationBuilder.InsertData(
                table: "rentals",
                columns: new[] { "client_id", "copy_id", "date_of_rental", "date_of_return" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2005, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 1, new DateTime(2005, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 3, new DateTime(2005, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, new DateTime(2005, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, new DateTime(2005, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 6, new DateTime(2005, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 7, new DateTime(2005, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 7, new DateTime(2005, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 7, new DateTime(2005, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 11, new DateTime(2005, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 12, new DateTime(2005, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 13, new DateTime(2005, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 19, new DateTime(2005, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 20, new DateTime(2005, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_copies_movie_id",
                table: "copies",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_rentals_copy_id",
                table: "rentals",
                column: "copy_id");

            migrationBuilder.CreateIndex(
                name: "IX_starring_movie_id",
                table: "starring",
                column: "movie_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "rentals");

            migrationBuilder.DropTable(
                name: "starring");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "copies");

            migrationBuilder.DropTable(
                name: "actors");

            migrationBuilder.DropTable(
                name: "movies");
        }
    }
}
