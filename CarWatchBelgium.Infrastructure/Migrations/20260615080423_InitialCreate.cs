using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWatchBelgium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    FallbackHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Make = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Price = table.Column<int>(type: "INTEGER", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    MileageKm = table.Column<int>(type: "INTEGER", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    FuelType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Transmission = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    PowerHp = table.Column<int>(type: "INTEGER", nullable: true),
                    SellerType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false, defaultValue: "BE"),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    FirstSeenAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastSeenAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastPriceChangeAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    CapturedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceHistories_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedSearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Make = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false, defaultValue: "BE"),
                    PriceMin = table.Column<int>(type: "INTEGER", nullable: true),
                    PriceMax = table.Column<int>(type: "INTEGER", nullable: true),
                    YearMin = table.Column<int>(type: "INTEGER", nullable: true),
                    YearMax = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxMileageKm = table.Column<int>(type: "INTEGER", nullable: true),
                    FuelType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Transmission = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    MinPowerHp = table.Column<int>(type: "INTEGER", nullable: true),
                    SellerType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    RadiusKm = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiredKeywords = table.Column<string>(type: "TEXT", nullable: false),
                    ExcludedKeywords = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastMatchedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserListingStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSeen = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsIgnored = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeenAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    FavoritedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserListingStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserListingStates_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserListingStates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedSearchMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SavedSearchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchedAtUtc = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearchMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearchMatches_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedSearchMatches_SavedSearches_SavedSearchId",
                        column: x => x.SavedSearchId,
                        principalTable: "SavedSearches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_FirstSeenAtUtc",
                table: "Listings",
                column: "FirstSeenAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_IsAvailable",
                table: "Listings",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_LastSeenAtUtc",
                table: "Listings",
                column: "LastSeenAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_MileageKm",
                table: "Listings",
                column: "MileageKm");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Price",
                table: "Listings",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Source_ExternalId",
                table: "Listings",
                columns: new[] { "Source", "ExternalId" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [ExternalId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Source_FallbackHash",
                table: "Listings",
                columns: new[] { "Source", "FallbackHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Year",
                table: "Listings",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistories_ListingId_CapturedAtUtc",
                table: "PriceHistories",
                columns: new[] { "ListingId", "CapturedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_IsActive",
                table: "SavedSearches",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_UserId_Name",
                table: "SavedSearches",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearchMatches_ListingId",
                table: "SavedSearchMatches",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearchMatches_MatchedAtUtc",
                table: "SavedSearchMatches",
                column: "MatchedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearchMatches_SavedSearchId_ListingId",
                table: "SavedSearchMatches",
                columns: new[] { "SavedSearchId", "ListingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserListingStates_IsFavorite",
                table: "UserListingStates",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_UserListingStates_IsIgnored",
                table: "UserListingStates",
                column: "IsIgnored");

            migrationBuilder.CreateIndex(
                name: "IX_UserListingStates_IsSeen",
                table: "UserListingStates",
                column: "IsSeen");

            migrationBuilder.CreateIndex(
                name: "IX_UserListingStates_ListingId",
                table: "UserListingStates",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserListingStates_UserId_ListingId",
                table: "UserListingStates",
                columns: new[] { "UserId", "ListingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceHistories");

            migrationBuilder.DropTable(
                name: "SavedSearchMatches");

            migrationBuilder.DropTable(
                name: "UserListingStates");

            migrationBuilder.DropTable(
                name: "SavedSearches");

            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
