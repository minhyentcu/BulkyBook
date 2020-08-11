using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _context;

        private static string _connectionString = "";
        public SP_Call(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;

        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void Execute(string produreName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            sqlCon.Execute(produreName, parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<T> List<T>(string produreName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            return sqlCon.Query<T>(produreName, parameters, commandType: CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string produreName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            var result = SqlMapper.QueryMultiple(sqlCon, produreName, parameters, commandType: CommandType.StoredProcedure);
            var itemOne = result.Read<T1>().ToList();
            var itemTwo = result.Read<T2>().ToList();

            if (itemOne != null && itemTwo != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(itemOne, itemTwo);
            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        public T OneRecord<T>(string produreName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            var value = sqlCon.Query<T>(produreName, parameters, commandType: CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T Single<T>(string produreName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(produreName, parameters, commandType: CommandType.StoredProcedure),typeof(T)) ;
        }
    }
}
