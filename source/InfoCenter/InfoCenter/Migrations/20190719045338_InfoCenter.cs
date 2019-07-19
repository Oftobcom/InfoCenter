using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoCenter.Migrations
{
    public partial class InfoCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "ClientInfo");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ClientInfo",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "ClientInfo",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ClientInfo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "ClientInfo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberActiveCredits",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberActiveDeposits",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberRemittances",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberTotalCredits",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberTotalDeposits",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "ClientInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "NumberActiveCredits",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "NumberActiveDeposits",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "NumberRemittances",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "NumberTotalCredits",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "NumberTotalDeposits",
                table: "ClientInfo");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "ClientInfo");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "ClientInfo",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "ClientInfo",
                newName: "Genre");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ClientInfo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "ClientInfo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
