using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_AG.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FondosMonetarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreFondo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoFondoMonetario = table.Column<int>(type: "int", nullable: false),
                    DescripcionFondo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondosMonetarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposGasto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaDeposito = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NombreComercio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosGasto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosGasto_FondosMonetarios_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresupuestosTipoGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    MontoPresupuestado = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresupuestosTipoGasto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresupuestosTipoGasto_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresupuestosTipoGasto_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistroGastoId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesGasto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesGasto_RegistrosGasto_RegistroGastoId",
                        column: x => x.RegistroGastoId,
                        principalTable: "RegistrosGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesGasto_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Contrasena", "NombreUsuario" },
                values: new object[] { 1, "9K7Cwg3Qw/8FR/S9VvrNdgl8znxhPagMZ4QrajV/3AQ=", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_FondoMonetarioId",
                table: "Depositos",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGasto_RegistroGastoId",
                table: "DetallesGasto",
                column: "RegistroGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGasto_TipoGastoId",
                table: "DetallesGasto",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_PresupuestosTipoGasto_TipoGastoId",
                table: "PresupuestosTipoGasto",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_PresupuestosTipoGasto_UsuarioId",
                table: "PresupuestosTipoGasto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosGasto_FondoMonetarioId",
                table: "RegistrosGasto",
                column: "FondoMonetarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "DetallesGasto");

            migrationBuilder.DropTable(
                name: "PresupuestosTipoGasto");

            migrationBuilder.DropTable(
                name: "RegistrosGasto");

            migrationBuilder.DropTable(
                name: "TiposGasto");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "FondosMonetarios");
        }
    }
}
