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
                name: "BrandSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandSnapshots", x => x.BrandId);
                });

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
                name: "CharacteristicSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    CharacteristicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CharType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsUnifying = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicSnapshots", x => x.CharacteristicId);
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
                name: "ProductReadModels",
                schema: "catalog_read",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ProductReadModels", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsLockedByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeoDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SeoKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantPriceSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasPrice = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantPriceSnapshots", x => x.ProductVariantId);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasMainImage = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantSnapshots", x => x.ProductVariantId);
                });

            migrationBuilder.CreateTable(
                name: "VendorSnapshots",
                schema: "catalog_write",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                name: "ProductAttributes",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacteristicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CharType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsVariable = table.Column<bool>(type: "bit", nullable: false),
                    IsUnifying = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "catalog_write",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSuspensionReasons",
                schema: "catalog_write",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSuspensionReasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSuspensionReasons_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "catalog_write",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTags",
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
                    table.PrimaryKey("PK_ProductTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTags_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "catalog_write",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeVariableValues",
                schema: "catalog_write",
                columns: table => new
                {
                    ValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeVariableValues", x => new { x.AttributeId, x.ValueId });
                    table.ForeignKey(
                        name: "FK_ProductAttributeVariableValues_ProductAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "catalog_write",
                        principalTable: "ProductAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandSnapshots_IsActive",
                schema: "catalog_write",
                table: "BrandSnapshots",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySnapshots_IsActive",
                schema: "catalog_write",
                table: "CategorySnapshots",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicSnapshots_CategoryId",
                schema: "catalog_write",
                table: "CharacteristicSnapshots",
                column: "CategoryId");

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
                name: "IX_ProductAttributes_ProductId",
                schema: "catalog_write",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_BrandId_CategoryId",
                schema: "catalog_read",
                table: "ProductReadModels",
                columns: new[] { "BrandId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CategoryId",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CategoryId_PriceAmount",
                schema: "catalog_read",
                table: "ProductReadModels",
                columns: new[] { "CategoryId", "PriceAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CategoryId_StockStatus",
                schema: "catalog_read",
                table: "ProductReadModels",
                columns: new[] { "CategoryId", "StockStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CategoryName",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CategorySlug",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "CategorySlug");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_CreatedAt",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_DefaultProductId",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "DefaultProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_Name",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_PriceAmount",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "PriceAmount");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_PriceUpdatedAt",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "PriceUpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_Slug",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_StockStatus",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "StockStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_StockUpdatedAt",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "StockUpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_UpdatedAt",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_VendorId",
                schema: "catalog_read",
                table: "ProductReadModels",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReadModels_VendorId_StockStatus",
                schema: "catalog_read",
                table: "ProductReadModels",
                columns: new[] { "VendorId", "StockStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                schema: "catalog_write",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "catalog_write",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId_ProductStatus",
                schema: "catalog_write",
                table: "Products",
                columns: new[] { "CategoryId", "ProductStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedAt",
                schema: "catalog_write",
                table: "Products",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsDeleted",
                schema: "catalog_write",
                table: "Products",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsDeleted_ProductStatus",
                schema: "catalog_write",
                table: "Products",
                columns: new[] { "IsDeleted", "ProductStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductStatus",
                schema: "catalog_write",
                table: "Products",
                column: "ProductStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                schema: "catalog_write",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedAt",
                schema: "catalog_write",
                table: "Products",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId",
                schema: "catalog_write",
                table: "Products",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId_ProductStatus",
                schema: "catalog_write",
                table: "Products",
                columns: new[] { "VendorId", "ProductStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuspensionReasons_ProductId",
                schema: "catalog_write",
                table: "ProductSuspensionReasons",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTags_ProductId",
                schema: "catalog_write",
                table: "ProductTags",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantSnapshots_ProductId",
                schema: "catalog_write",
                table: "ProductVariantSnapshots",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "CategorySnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "CharacteristicSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "ProductAttributeVariableValues",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductReadModels",
                schema: "catalog_read");

            migrationBuilder.DropTable(
                name: "ProductSuspensionReasons",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductTags",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductVariantPriceSnapshots",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "ProductVariantSnapshots",
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
                name: "ProductAttributes",
                schema: "catalog_write");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "catalog_write");
        }
    }
}
