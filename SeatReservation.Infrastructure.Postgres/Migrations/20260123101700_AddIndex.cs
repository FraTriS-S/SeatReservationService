using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "event_id",
                table: "reservation_seats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_reservation_seats_event_id_seat_id",
                table: "reservation_seats",
                columns: new[] { "event_id", "seat_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reservation_seats_event_id_seat_id",
                table: "reservation_seats");

            migrationBuilder.DropColumn(
                name: "event_id",
                table: "reservation_seats");
        }
    }
}
