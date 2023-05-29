using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetPortal.Migrations
{
    /// <inheritdoc />
    public partial class DockAckREQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentAcknowledgementRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AbpUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentAcknowledgementRequestStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcknowledgedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DueDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DocumentAcknowledgementRequestStatusesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAcknowledgementRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAcknowledgementRequests_DocumentAcknowledgementRequ~",
                        column: x => x.DocumentAcknowledgementRequestStatusId,
                        principalTable: "DocumentAcknowledgementRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAcknowledgementRequests_DocumentAcknowledgementReq~1",
                        column: x => x.DocumentAcknowledgementRequestStatusesId,
                        principalTable: "DocumentAcknowledgementRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAcknowledgementRequests_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAcknowledgementRequests_DocumentAcknowledgementReq~1",
                table: "DocumentAcknowledgementRequests",
                column: "DocumentAcknowledgementRequestStatusesId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAcknowledgementRequests_DocumentAcknowledgementRequ~",
                table: "DocumentAcknowledgementRequests",
                column: "DocumentAcknowledgementRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAcknowledgementRequests_DocumentId",
                table: "DocumentAcknowledgementRequests",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAcknowledgementRequests");
        }
    }
}
