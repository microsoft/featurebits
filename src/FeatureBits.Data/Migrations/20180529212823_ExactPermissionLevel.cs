using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FeatureBitsData.Migrations
{
    public partial class ExactPermissionLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExactAllowedPermissionLevel",
                table: "FeatureBitDefinitions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExactAllowedPermissionLevel",
                table: "FeatureBitDefinitions");
        }
    }
}
