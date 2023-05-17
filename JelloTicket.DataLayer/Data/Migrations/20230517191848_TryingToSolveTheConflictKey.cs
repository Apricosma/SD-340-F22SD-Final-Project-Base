using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JelloTicket.DataLayer.Data.Migrations
{
    public partial class TryingToSolveTheConflictKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Projects_ProjectId",
                table: "Tickets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
