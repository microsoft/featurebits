using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FeatureBitsData.Migrations
{
    public partial class ExactPermissionLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "IFeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExactAllowedPermissionLevel",
                table: "IFeatureBitDefinitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PartitionKey",
                table: "IFeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowKey",
                table: "IFeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "IFeatureBitDefinitions",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "IFeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "ExactAllowedPermissionLevel",
                table: "IFeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "PartitionKey",
                table: "IFeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "RowKey",
                table: "IFeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "IFeatureBitDefinitions");
        }
    }
}
