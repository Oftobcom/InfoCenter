using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;

namespace Web_API_MSSQL.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            string[] s_result = new string[1];
            string connetionString;
            SqlConnection conn;
            connetionString = @"Server=HOMA-PC\SQLEXPRESS;Database=InfoCenterDB;Trusted_Connection=True;";
            conn = new SqlConnection(connetionString);
            conn.Open();
            using (SqlCommand command = new SqlCommand("SELECT TOP 1 [Caller] FROM [dbo].[AsteriskCaller] order by [Caller] desc", conn)) {
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var my_data = reader.GetString(0);
                        s_result[0] = my_data;

                    }
                }
                else
                {
                    s_result[0] = "No rows found.";
                }
            }

            conn.Close();
            return s_result;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
