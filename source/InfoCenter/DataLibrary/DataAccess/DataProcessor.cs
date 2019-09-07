using Dapper;
using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.DataAccess
{
    public class DataProcessor
    {
        public List<CustomerInfoModel> GetCustomerInfoByPhone(string phone)
        {
            //throw new NotImplementedException();
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(MssqlDataAccess.GetConnectionString()))
            {
                List<CustomerInfoModel> output = connection.Query<CustomerInfoModel>("dbo.rp_GetCustomerByPhone @Phone", new { Phone = phone }).ToList();
                return output;
            }
        }
    }
}
