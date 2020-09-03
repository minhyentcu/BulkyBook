using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkyBook.DataAccess.Migrations
{
    public partial class addProcForCoverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create PROC usp_GetCoverTypes" +
                " AS " +
                " BEGIN " +
                " SELECT * FROM dbo.CoverTypes " +
                " END ");
            migrationBuilder.Sql(@"Create PROC usp_GetCoverType" +
                " @Id int " +
                " AS " +
                " BEGIN " +
                " SELECT * FROM dbo.CoverTypes WHERE (Id=@Id) " +
                " END ");

            migrationBuilder.Sql(@"Create PROC usp_UpdateCoverType" +
                " @Id int ," +
                " @Name varchar(100) " +
                " AS " +
                " BEGIN " +
                " UPDATE dbo.CoverTypes " +
                " SET Name = @Name "+
                " Where Id= @Id "+
                " END ");
            migrationBuilder.Sql(@"Create PROC usp_DeleteCoverType" +
               " @Id int " +
               " AS " +
               " BEGIN " +
               " DELETE FROM dbo.CoverTypes " +
               " Where Id= @Id " +
               "END");
            migrationBuilder.Sql(@"Create PROC usp_CreateCoverType" +
               " @Name varchar(100) " +
               " AS " +
               " BEGIN " +
               " INSERT INTO dbo.CoverTypes(Name) " +
               " VALUES(@Name) " +
               " END ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Drop PROCEDURE usp_GetCoverTypes");
            migrationBuilder.Sql(@"Drop PROCEDURE usp_GetCoverType");
            migrationBuilder.Sql(@"Drop PROCEDURE usp_UpdateCoverType");
            migrationBuilder.Sql(@"Drop PROCEDURE usp_DeleteCoverType");
            migrationBuilder.Sql(@"Drop PROCEDURE usp_CreateCoverType");
        }
    }
}
