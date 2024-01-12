using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scala.StockSimulation.Web.Migrations
{
    public partial class securityStampSeeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateDelivered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTemporary = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_OrderTypes_OrderTypeId",
                        column: x => x.OrderTypeId,
                        principalTable: "OrderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ArticleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialStock = table.Column<int>(type: "int", nullable: false),
                    InitialMinimumStock = table.Column<int>(type: "int", nullable: false),
                    InitialMaximumStock = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProductStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhysicalStock = table.Column<int>(type: "int", nullable: false),
                    FictionalStock = table.Column<int>(type: "int", nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    MaximumStock = table.Column<int>(type: "int", nullable: false),
                    SoonAvailableStock = table.Column<int>(type: "int", nullable: false),
                    ReservedStock = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProductStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProductStates_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProductStates_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1fc2b645-543c-41d3-9550-2f07976a432b"), "8a5b0e48-22e8-48b0-b528-550b2c851bec", "Student", "STUDENT" },
                    { new Guid("f74f1598-1d21-42a9-a776-436e5afe7b87"), "073b577c-5a8c-4ef0-9e3b-99d4bb16724a", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Created", "Deleted", "Email", "EmailConfirmed", "Firstname", "Lastname", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "Updated", "UserName" },
                values: new object[,]
                {
                    { new Guid("1a9de52d-6226-45eb-ba39-ed5ca28e7059"), 0, "e30f7abf-59fb-4207-b53b-fb8afda27964", new DateTime(2023, 12, 21, 19, 20, 33, 295, DateTimeKind.Local).AddTicks(5883), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@howest.be", true, "Admin", "Admin", false, null, "ADMIN@HOWEST.BE", "ADMIN@HOWEST.BE", "AQAAAAEAACcQAAAAEIOJsSDwNkBMSRfpKxxL/tMYavmIikABNdgNTP56agBv2SXRgY9GN8fACdNjdB+mHQ==", null, false, "2660b159-df88-48e9-b9cc-37d3d168fc97", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@howest.be" },
                    { new Guid("7179b705-0c38-4722-af63-72dddd9e72e0"), 0, "f34c5856-7d64-4c57-a2af-5e57b9b8963b", new DateTime(2023, 12, 21, 19, 20, 33, 301, DateTimeKind.Local).AddTicks(136), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bluevos@howest.be", false, "Blue", "Vosselman", false, null, "BLUEVOS@HOWEST.BE", "BLUEVOS@HOWEST.BE", "AQAAAAEAACcQAAAAEM+RXnr8YHjzX1P+TmX5nnGrNcxPD+4dJZxPHVms9WB8GVn362mWbZsUFkWh/kfnFg==", null, false, "c7bb2e57-926d-4537-a96a-49ee51f1e7af", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bluevos@howest.be" },
                    { new Guid("73534204-bddc-46f6-9968-6682b189c246"), 0, "52ab9c8c-a604-43b3-bfed-c59bbcafb205", new DateTime(2023, 12, 21, 19, 20, 33, 301, DateTimeKind.Local).AddTicks(142), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alexver@howest.be", false, "Alekxander", "Verhaeghe", false, null, "ALEXVER@HOWEST.BE", "ALEXVER@HOWEST.BE", "AQAAAAEAACcQAAAAEPfTPuCg/E4w1jVvqIlGv+pPyyVhw8zTvmgfY0bRN1oePm590qFsxCuuF+hFwPXZSQ==", null, false, "96027029-5f5c-4825-8a0c-46b7c6a2039e", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alexver@howest.be" },
                    { new Guid("95c984ea-29ca-4461-a4b3-b6661ac8bd62"), 0, "d172981a-97d8-4d2b-84a0-821219a03ba5", new DateTime(2023, 12, 21, 19, 20, 33, 301, DateTimeKind.Local).AddTicks(123), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "quinaspe@howest.be", false, "Quinten", "Aspeslagh", false, null, "QUINASPE@HOWEST.BE", "QUINASPE@HOWEST.BE", "AQAAAAEAACcQAAAAEJ6v6OFFRFXjB6Mf/Fb6lWpo/4P1SpHGl4gRivSUcFGPr5CqKfcB9EfbJwemrbtrig==", null, false, "062f639d-5909-47ac-8fee-366f399b2dfe", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "quinaspe@howest.be" },
                    { new Guid("f7625794-3cf1-4930-ab8b-84e1d985d3b2"), 0, "c39dc94b-8d36-420f-a781-eb63396e078f", new DateTime(2023, 12, 21, 19, 20, 33, 301, DateTimeKind.Local).AddTicks(101), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "diegoetha@howest.be", false, "Diego", "Goethals", false, null, "DIEGOGOETHA@HOWEST.BE", "DIEGOGOETHA@HOWEST.BE", "AQAAAAEAACcQAAAAEFDLaht8OZnOM5xIfz0xsg2Jb21z9MvNeKGjCkpVukk/nBpdAiwBlcWfanryS8/ajw==", null, false, "6ab42615-bce1-4a59-af29-50e32dde6492", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "diegoetha@howest.be" }
                });

            migrationBuilder.InsertData(
                table: "OrderTypes",
                columns: new[] { "Id", "Created", "Deleted", "Name", "Updated" },
                values: new object[,]
                {
                    { new Guid("76a9b85d-9e0a-40fa-b863-63109835b7b6"), new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3000), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leverancier", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("86f60a20-86c9-4919-b550-20ebd650ef33"), new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3010), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Klant", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Created", "Deleted", "Name", "Updated" },
                values: new object[,]
                {
                    { new Guid("bd028f20-d7aa-4d36-8366-83020d4e4da0"), new DateTime(2023, 12, 21, 19, 20, 33, 295, DateTimeKind.Local).AddTicks(5707), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Panos", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("c26f0950-d5d5-4b79-bad7-58c65f8ac46d"), new DateTime(2023, 12, 21, 19, 20, 33, 295, DateTimeKind.Local).AddTicks(5748), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pizza Hut", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("f74f1598-1d21-42a9-a776-436e5afe7b87"), new Guid("1a9de52d-6226-45eb-ba39-ed5ca28e7059") },
                    { new Guid("1fc2b645-543c-41d3-9550-2f07976a432b"), new Guid("7179b705-0c38-4722-af63-72dddd9e72e0") },
                    { new Guid("1fc2b645-543c-41d3-9550-2f07976a432b"), new Guid("73534204-bddc-46f6-9968-6682b189c246") },
                    { new Guid("1fc2b645-543c-41d3-9550-2f07976a432b"), new Guid("95c984ea-29ca-4461-a4b3-b6661ac8bd62") },
                    { new Guid("1fc2b645-543c-41d3-9550-2f07976a432b"), new Guid("f7625794-3cf1-4930-ab8b-84e1d985d3b2") }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ArticleNumber", "Created", "Deleted", "Description", "InitialMaximumStock", "InitialMinimumStock", "InitialStock", "Name", "Price", "SupplierId", "Updated" },
                values: new object[,]
                {
                    { new Guid("03387470-8ef8-45ec-b545-dfd5b16b73a3"), "pp1", new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3037), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pizza met pepperoni", 500, 100, 150, "Pizza Pepperoni", 6.70m, new Guid("c26f0950-d5d5-4b79-bad7-58c65f8ac46d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("b5510c6d-299c-495a-b77c-7c6c20b92567"), "pm1", new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3039), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Standaard pizza", 500, 100, 160, "Pizza Margherita", 6.80m, new Guid("c26f0950-d5d5-4b79-bad7-58c65f8ac46d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("db506532-c832-487e-9524-d8115d28a45b"), "bkc1", new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3031), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Broodje met kip curry beleg", 500, 100, 150, "Broodje Kip Curry", 1.20m, new Guid("bd028f20-d7aa-4d36-8366-83020d4e4da0"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("ef8f0978-3810-42fb-9f47-f5e844bc4af0"), "brs1", new DateTime(2023, 12, 21, 19, 20, 33, 322, DateTimeKind.Local).AddTicks(3034), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Broodje met kaas en ham", 500, 100, 180, "Broodje Smos", 7.20m, new Guid("bd028f20-d7aa-4d36-8366-83020d4e4da0"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderTypeId",
                table: "Orders",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProductStates_ApplicationUserId",
                table: "UserProductStates",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProductStates_ProductId",
                table: "UserProductStates",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "UserProductStates");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "OrderTypes");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}