using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshBoard.Data.Migrations
{
    public partial class CreateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationPeriod",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false, defaultValue: ""),
                    Summary = table.Column<string>(),
                    Description = table.Column<string>(nullable: false, defaultValue: ""),
                    UserApproved = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(),
                    TwoFactorEnabled = table.Column<bool>(),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(),
                    AccessFailedCount = table.Column<int>(),
                    PuzzleProgress = table.Column<int>(),
                    Privilege = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(),
                    Title = table.Column<string>(),
                    Content = table.Column<string>(),
                    Mode = table.Column<int>(nullable: false, defaultValueSql: "1")
                        .Annotation("Sqlite:Autoincrement", true),
                    Targets = table.Column<string>(nullable: true),
                    HasPushed = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Problem",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(),
                    Content = table.Column<string>(),
                    Script = table.Column<string>(nullable: true),
                    Level = table.Column<int>(),
                    Answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleRecord",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(),
                    ProblemId = table.Column<int>(),
                    UserId = table.Column<string>(),
                    Result = table.Column<int>(),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadStatus",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(),
                    NotificationId = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDataType",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(),
                    Description = table.Column<string>(nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDataType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPeriodDataType",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false, defaultValue: ""),
                    Description = table.Column<string>(nullable: false, defaultValue: ""),
                    UserVisible = table.Column<bool>(nullable: false, defaultValue: true),
                    UserEditable = table.Column<bool>(nullable: false, defaultValue: true),
                    PeriodId = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPeriodDataType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPeriodDataType_ApplicationPeriod_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "ApplicationPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    UserId = table.Column<string>(),
                    PeriodId = table.Column<int>(),
                    IsSuccessful = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Application_ApplicationPeriod_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "ApplicationPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Application_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128),
                    ProviderKey = table.Column<string>(maxLength: 128),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(),
                    RoleId = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(),
                    LoginProvider = table.Column<string>(maxLength: 128),
                    Name = table.Column<string>(maxLength: 128),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    UserId = table.Column<string>(),
                    DataTypeId = table.Column<int>(),
                    Value = table.Column<string>(nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => new { x.UserId, x.DataTypeId });
                    table.ForeignKey(
                        name: "FK_UserData_UserDataType_DataTypeId",
                        column: x => x.DataTypeId,
                        principalTable: "UserDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPeriodData",
                columns: table => new
                {
                    ApplicationId = table.Column<string>(),
                    DataTypeId = table.Column<int>(),
                    Value = table.Column<string>(nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPeriodData", x => new { x.ApplicationId, x.DataTypeId });
                    table.ForeignKey(
                        name: "FK_ApplicationPeriodData_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationPeriodData_ApplicationPeriodDataType_DataTypeId",
                        column: x => x.DataTypeId,
                        principalTable: "ApplicationPeriodDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_PeriodId",
                table: "Application",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPeriodData_DataTypeId",
                table: "ApplicationPeriodData",
                column: "DataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPeriodDataType_PeriodId",
                table: "ApplicationPeriodDataType",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_DataTypeId",
                table: "UserData",
                column: "DataTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationPeriodData");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Problem");

            migrationBuilder.DropTable(
                name: "PuzzleRecord");

            migrationBuilder.DropTable(
                name: "ReadStatus");

            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "ApplicationPeriodDataType");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "UserDataType");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ApplicationPeriod");
        }
    }
}
