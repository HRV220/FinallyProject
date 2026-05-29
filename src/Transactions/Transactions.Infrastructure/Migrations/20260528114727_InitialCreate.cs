using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "transactions");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nominal = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    NumericCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    UnitRate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "transactions",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToWalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_ToWalletId",
                        column: x => x.ToWalletId,
                        principalSchema: "transactions",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "transactions",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                schema: "transactions",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Date",
                schema: "transactions",
                table: "Transactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ToWalletId",
                schema: "transactions",
                table: "Transactions",
                column: "ToWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                schema: "transactions",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CurrencyId",
                schema: "transactions",
                table: "Wallets",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ProfileId",
                schema: "transactions",
                table: "Wallets",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "transactions");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "transactions");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "transactions");
        }
    }
}
