using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PrepaidAccountFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTransactions_Activities_ActivityId1",
                table: "AccountTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AccountTransactions_ActivityId1",
                table: "AccountTransactions");

            migrationBuilder.DropColumn(
                name: "ActivityId1",
                table: "AccountTransactions");

            migrationBuilder.DropColumn(
                name: "AttendanceId",
                table: "AccountTransactions");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AccountTransactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_ActivityId",
                table: "AccountTransactions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_IdempotencyKey",
                table: "AccountTransactions",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] <> ''");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTransactions_Activities_ActivityId",
                table: "AccountTransactions",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTransactions_Activities_ActivityId",
                table: "AccountTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AccountTransactions_ActivityId",
                table: "AccountTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AccountTransactions_IdempotencyKey",
                table: "AccountTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AccountTransactions");

            migrationBuilder.AddColumn<string>(
                name: "ActivityId1",
                table: "AccountTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AttendanceId",
                table: "AccountTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_ActivityId1",
                table: "AccountTransactions",
                column: "ActivityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTransactions_Activities_ActivityId1",
                table: "AccountTransactions",
                column: "ActivityId1",
                principalTable: "Activities",
                principalColumn: "Id");
        }
    }
}
