using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppCrudCore.Migrations
{
    /// <inheritdoc />
    public partial class CreateTransactionTablesClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionDetalle_TransaccionBiblioteca_TransaccionBibliotecaId",
                table: "TransaccionDetalle");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion",
                table: "TransaccionBiblioteca",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPagado",
                table: "TransaccionBiblioteca",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NumeroTransaccion",
                table: "TransaccionBiblioteca",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "TransaccionBiblioteca",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenciaPago",
                table: "TransaccionBiblioteca",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPago",
                table: "TransaccionBiblioteca",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionBiblioteca_FechaCreacion",
                table: "TransaccionBiblioteca",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionBiblioteca_NumeroTransaccion",
                table: "TransaccionBiblioteca",
                column: "NumeroTransaccion",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionDetalle_TransaccionBiblioteca_TransaccionBibliotecaId",
                table: "TransaccionDetalle",
                column: "TransaccionBibliotecaId",
                principalTable: "TransaccionBiblioteca",
                principalColumn: "IdTransaccionBiblioteca",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionDetalle_TransaccionBiblioteca_TransaccionBibliotecaId",
                table: "TransaccionDetalle");

            migrationBuilder.DropIndex(
                name: "IX_TransaccionBiblioteca_FechaCreacion",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropIndex(
                name: "IX_TransaccionBiblioteca_NumeroTransaccion",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "MontoPagado",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "NumeroTransaccion",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "ReferenciaPago",
                table: "TransaccionBiblioteca");

            migrationBuilder.DropColumn(
                name: "TipoPago",
                table: "TransaccionBiblioteca");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionDetalle_TransaccionBiblioteca_TransaccionBibliotecaId",
                table: "TransaccionDetalle",
                column: "TransaccionBibliotecaId",
                principalTable: "TransaccionBiblioteca",
                principalColumn: "IdTransaccionBiblioteca",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
