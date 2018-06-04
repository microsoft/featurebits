using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FeatureBitsData.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IFeatureBitDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowedUsers = table.Column<string>(maxLength: 2048, nullable: true),
                    CreatedByUser = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    ExcludedEnvironments = table.Column<string>(maxLength: 300, nullable: true),
                    LastModifiedByUser = table.Column<string>(maxLength: 100, nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(nullable: false),
                    MinimumAllowedPermissionLevel = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OnOff = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IFeatureBitDefinitions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IFeatureBitDefinitions");
        }
    }
}
