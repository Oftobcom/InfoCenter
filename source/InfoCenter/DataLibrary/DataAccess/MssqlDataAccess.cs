using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataLibrary.DataAccess
{
    public static class MssqlDataAccess
    {
        public static string GetConnectionString(string name = "DefaultDB")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        //public static List<T> LoadData<T>(string sql)
        //{
        //    using (IDbConnection conn = new SqlConnection(GetConnectionString()))
        //    {
        //        return conn.Query<T>(sql).ToList();
        //    }
        //}
    }
}
