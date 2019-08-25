using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApiMsql.Classes;
using WebApiMsql.Models;

namespace Web_API_MSSQL.Classes
{
    public class DataAccess
    {
        public List<CustomerInfo> GetCustomerInfo(string phone)
        {
            //throw new NotImplementedException();
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.ConnVal("CustomerInfo")))
            {
                var output = connection.Query<CustomerInfo>("dbo.rp_GetInfoByPhone @Phone", new { Phone = phone }).ToList();
                return output;
            }
        }
    }
}