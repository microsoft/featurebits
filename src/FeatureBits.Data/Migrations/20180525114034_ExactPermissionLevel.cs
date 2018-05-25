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
                table: "FeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExactAllowedPermissionLevel",
                table: "FeatureBitDefinitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PartitionKey",
                table: "FeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowKey",
                table: "FeatureBitDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "FeatureBitDefinitions",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "FeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "ExactAllowedPermissionLevel",
                table: "FeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "PartitionKey",
                table: "FeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "RowKey",
                table: "FeatureBitDefinitions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "FeatureBitDefinitions");
        }
    }
}
