using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeRomanHsieh.API.Migrations
{
    public partial class OrdersMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TranscationMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "FB6D4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                column: "ConcurrencyStamp",
                value: "07fa7658-b7ea-4d14-8d13-0faca5c43898");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "XXXD4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d6b85bc2-090f-46d5-8648-88ec32b4755b", "AQAAAAEAACcQAAAAEMpPiDLjOTgmTfex3WRuat7af45xxHzI9uJfDFNU5rzapjR3ceeWZOLuAKxU/V/f5A==", "f5b6285d-7936-420b-9512-c816a61fba60" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "FB6D4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                column: "ConcurrencyStamp",
                value: "1046765e-4383-44e1-b9eb-0984757db7b8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "XXXD4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c210fdf3-4bb4-49f3-90d5-ae6e34966990", "AQAAAAEAACcQAAAAEDtAMPwjDotBuV0+CtPOSTfEf/hPg3YJCOp9YupHZzzsnHGAwjFnVlHmF2ORdSQz2w==", "24fdeb5c-3f81-4634-8037-21832cc1b89a" });
        }
    }
}
