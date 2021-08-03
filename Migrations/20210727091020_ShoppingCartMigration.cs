using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeRomanHsieh.API.Migrations
{
    public partial class ShoppingCartMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "FB6D4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                column: "ConcurrencyStamp",
                value: "d6784643-105b-41ee-9780-474e5073a371");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "XXXD4F10-79ED-4AFF-A915-4CE29DC9C7E9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2c307bd8-1d1f-4a89-aa02-edd665a3eacd", "AQAAAAEAACcQAAAAELNjDtCDr8MxbiE6heZ9s8iXRaCVgFrr/b3EizRYX76GOkSN/xBH4Mq5OpJH/GSBSQ==", "1167803b-3463-46b3-a8e4-7e1848b64912" });
        }
    }
}
