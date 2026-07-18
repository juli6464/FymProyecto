using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fym.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 18, 14, 4, 13, 266, DateTimeKind.Utc).AddTicks(4032), "$2a$11$2SgJrIhloqs7KEx32gdj5OBcCy7LS/XTBbvaqTGkBktEi1xcj0ca2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 18, 13, 36, 19, 111, DateTimeKind.Utc).AddTicks(9013), "$2a$11$qR3Bw0M2Wb4mHhWn7vFEU.uXfGv8m/A9SgUoR1Uv4TfU6.rE6Ym2q" });
        }
    }
}
