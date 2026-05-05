using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog_write");

            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.EnsureSchema(
                name: "catalog_read");

            migrationBuilder.CreateTable(
                name: "CategorySnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySnapshots", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                schema: "messaging",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                schema: "messaging",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCardReadModels",
                schema: "catalog_read",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameNormalized = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RatingAvg = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    RatingCount = table.Column<int>(type: "int", nullable: false),
                    DefaultProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    PriceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OldPriceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PriceUpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StockStatus = table.Column<int>(type: "int", nullable: false),
                    StockUpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategorySlug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductCardStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SeoDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SeoKeywords = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MainImage = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    AttributesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagsFlat = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImagesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCardReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCards",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DefaultProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductCardStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    SeoDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    SeoKeywords = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventorySnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    StockStatus = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventorySnapshots", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductPriceSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceSnapshots", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSnapshots", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "VendorSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSnapshots", x => x.VendorId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "messaging",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessages_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalSchema: "messaging",
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessages_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalSchema: "messaging",
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.CreateTable(
                name: "ProductCardAttributes",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCardAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCardAttributes_ProductCards_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "catalog_write",
                        principalTable: "ProductCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCardImages",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCardImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCardImages_ProductCards_ProductCardId",
                        column: x => x.ProductCardId,
                        principalSchema: "catalog_write",
                        principalTable: "ProductCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCardTags",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCardTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCardTags_ProductCards_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "catalog_write",
                        principalTable: "ProductCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySnapshots_IsActive",
                schema: "catalog_write",
                table: "CategorySnapshots",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                schema: "messaging",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EnqueueTime",
                schema: "messaging",
                table: "OutboxMessages",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ExpirationTime",
                schema: "messaging",
                table: "OutboxMessages",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber",
                schema: "messaging",
                table: "OutboxMessages",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_OutboxId_SequenceNumber",
                schema: "messaging",
                table: "OutboxMessages",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                schema: "messaging",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardAttributes_ProductId",
                schema: "catalog_write",
                table: "ProductCardAttributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardImages_ProductCardId",
                schema: "catalog_write",
                table: "ProductCardImages",
                column: "ProductCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_Brand",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_Brand_CategoryId",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "Brand", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_Brand_ProductCardStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "Brand", "ProductCardStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategoryId",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategoryId_PriceAmount",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "CategoryId", "PriceAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategoryId_ProductCardStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "CategoryId", "ProductCardStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategoryId_StockStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "CategoryId", "StockStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategoryName",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CategorySlug",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "CategorySlug");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_CreatedAt",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_DefaultProductId",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "DefaultProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_Name",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_PriceAmount",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "PriceAmount");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_PriceUpdatedAt",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "PriceUpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_ProductCardStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "ProductCardStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_Slug",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_StockStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "StockStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_StockUpdatedAt",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "StockUpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_UpdatedAt",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_VendorId",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_VendorId_ProductCardStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "VendorId", "ProductCardStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardReadModels_VendorId_StockStatus",
                schema: "catalog_read",
                table: "ProductCardReadModels",
                columns: new[] { "VendorId", "StockStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_CategoryId",
                schema: "catalog_write",
                table: "ProductCards",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_CategoryId_ProductCardStatus",
                schema: "catalog_write",
                table: "ProductCards",
                columns: new[] { "CategoryId", "ProductCardStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_CreatedAt",
                schema: "catalog_write",
                table: "ProductCards",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_DefaultProductId",
                schema: "catalog_write",
                table: "ProductCards",
                column: "DefaultProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_ProductCardStatus",
                schema: "catalog_write",
                table: "ProductCards",
                column: "ProductCardStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_Slug",
                schema: "catalog_write",
                table: "ProductCards",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_UpdatedAt",
                schema: "catalog_write",
                table: "ProductCards",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_VendorId",
                schema: "catalog_write",
                table: "ProductCards",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_VendorId_ProductCardStatus",
                schema: "catalog_write",
                table: "ProductCards",
                columns: new[] { "VendorId", "ProductCardStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCardTags_ProductId",
                schema: "catalog_write",
                table: "ProductCardTags",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorSnapshots_IsVerified",
                schema: "catalog_write",
                table: "VendorSnapshots",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_VendorSnapshots_Status",
                schema: "catalog_write",
                table: "VendorSnapshots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorSnapshots_UpdatedAt",
                schema: "catalog_write",
                table: "VendorSnapshots",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorySnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "ProductCardAttributes",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductCardImages",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductCardReadModels",
                schema: "catalog_read");

            migrationBuilder.DropTable(
                name: "ProductCardTags",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductInventorySnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductPriceSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "VendorSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "InboxState",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "OutboxState",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "ProductCards",
                schema: "catalog_write");
        }
    }
}
