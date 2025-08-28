using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotPot23API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMasters",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMasters", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantMasters",
                columns: table => new
                {
                    RestaurantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RestaurantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CuisineType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AveragePreparationTime = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantMasters", x => x.RestaurantID);
                    table.ForeignKey(
                        name: "FK_RestaurantMasters_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddressMasters",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Landmark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddressMasters", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_UserAddressMasters_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    HashKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                    table.ForeignKey(
                        name: "FK_User_UserMaster",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryMasters",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantID = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMasters", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_CategoryMasters_RestaurantMasters_RestaurantID",
                        column: x => x.RestaurantID,
                        principalTable: "RestaurantMasters",
                        principalColumn: "RestaurantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderTransactions",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RestaurantID = table.Column<int>(type: "int", nullable: false),
                    ShippingAddressID = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTransactions", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_OrderTransactions_RestaurantMasters_RestaurantID",
                        column: x => x.RestaurantID,
                        principalTable: "RestaurantMasters",
                        principalColumn: "RestaurantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTransactions_UserAddressMasters_ShippingAddressID",
                        column: x => x.ShippingAddressID,
                        principalTable: "UserAddressMasters",
                        principalColumn: "AddressID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTransactions_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    MenuItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailabilityTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsVeg = table.Column<bool>(type: "bit", nullable: false),
                    TasteInfo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NutritionalInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InStock = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.MenuItemID);
                    table.ForeignKey(
                        name: "FK_MenuItems_CategoryMasters_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "CategoryMasters",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuItems_RestaurantMasters_RestaurantID",
                        column: x => x.RestaurantID,
                        principalTable: "RestaurantMasters",
                        principalColumn: "RestaurantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryStatusTransactions",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusUpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStatusTransactions", x => x.StatusID);
                    table.ForeignKey(
                        name: "FK_DeliveryStatusTransactions_OrderTransactions_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTransactions",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryStatusTransactions_UserMasters_StatusUpdatedBy",
                        column: x => x.StatusUpdatedBy,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_OrderTransactions_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTransactions",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartTransactions",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MenuItemID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartTransactions", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_CartTransactions_MenuItems_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartTransactions_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountMasters",
                columns: table => new
                {
                    DiscountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemID = table.Column<int>(type: "int", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountMasters", x => x.DiscountID);
                    table.ForeignKey(
                        name: "FK_DiscountMasters_MenuItems_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemDetails",
                columns: table => new
                {
                    OrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    MenuItemID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtOrder = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDetails", x => x.OrderItemID);
                    table.ForeignKey(
                        name: "FK_OrderItemDetails_MenuItems_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemDetails_OrderTransactions_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTransactions",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTransactions",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MenuItemID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTransactions", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_ReviewTransactions_MenuItems_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewTransactions_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartTransactions_MenuItemID",
                table: "CartTransactions",
                column: "MenuItemID");

            migrationBuilder.CreateIndex(
                name: "IX_CartTransactions_UserID_MenuItemID",
                table: "CartTransactions",
                columns: new[] { "UserID", "MenuItemID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryMasters_RestaurantID",
                table: "CategoryMasters",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStatusTransactions_OrderID",
                table: "DeliveryStatusTransactions",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStatusTransactions_StatusUpdatedBy",
                table: "DeliveryStatusTransactions",
                column: "StatusUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountMasters_MenuItemID",
                table: "DiscountMasters",
                column: "MenuItemID");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryID",
                table: "MenuItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_RestaurantID",
                table: "MenuItems",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetails_MenuItemID",
                table: "OrderItemDetails",
                column: "MenuItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetails_OrderID",
                table: "OrderItemDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransactions_RestaurantID",
                table: "OrderTransactions",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransactions_ShippingAddressID",
                table: "OrderTransactions",
                column: "ShippingAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransactions_UserID",
                table: "OrderTransactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderID",
                table: "PaymentTransactions",
                column: "OrderID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantMasters_UserID",
                table: "RestaurantMasters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTransactions_MenuItemID",
                table: "ReviewTransactions",
                column: "MenuItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTransactions_UserID",
                table: "ReviewTransactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddressMasters_UserID",
                table: "UserAddressMasters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserID",
                table: "Users",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartTransactions");

            migrationBuilder.DropTable(
                name: "DeliveryStatusTransactions");

            migrationBuilder.DropTable(
                name: "DiscountMasters");

            migrationBuilder.DropTable(
                name: "OrderItemDetails");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "ReviewTransactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "OrderTransactions");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "UserAddressMasters");

            migrationBuilder.DropTable(
                name: "CategoryMasters");

            migrationBuilder.DropTable(
                name: "RestaurantMasters");

            migrationBuilder.DropTable(
                name: "UserMasters");
        }
    }
}
