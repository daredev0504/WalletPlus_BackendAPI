using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletPlusIncAPI.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OauthIssuer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OauthSubject",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OauthIssuer",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OauthSubject",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
