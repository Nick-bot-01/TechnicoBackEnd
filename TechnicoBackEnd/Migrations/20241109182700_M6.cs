using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechnicoBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class M6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "IsActive", "Name", "Password", "Phone", "Surname", "Type", "VATNum" },
                values: new object[,]
                {
                    { -10, "p0k3b4ll 43", "m.tyson@aol.com", true, "Mike", "Str0ngP4ssw0rd", "3423423423", "Tyson", 0, "M1K3_TYS0N" },
                    { -9, "zei sti thalassa se anana 85", "ariel_onlyfans@gmail.com", true, "Mikri", "NotFound", "34545343423", "Gorgona", 0, "Sp0ng3B0b" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -9);
        }
    }
}
