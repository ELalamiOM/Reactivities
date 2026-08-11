using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrepaidAccountTransc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrepaidAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrepaidAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "TEXT", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActivityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ActivityId1 = table.Column<string>(type: "TEXT", nullable: true),
                    AttendanceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_Activities_ActivityId1",
                        column: x => x.ActivityId1,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountTransactions_PrepaidAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "PrepaidAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_AccountId",
                table: "AccountTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_ActivityId1",
                table: "AccountTransactions",
                column: "ActivityId1");

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidAccounts_UserId",
                table: "PrepaidAccounts",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTransactions");

            migrationBuilder.DropTable(
                name: "PrepaidAccounts");
        }
    }
}
