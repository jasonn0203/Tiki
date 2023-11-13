using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Tiki.Models
{
    public class DataModel
    {
        private string connectionString = "Data Source=JASON11;Initial Catalog=Tiki;Integrated Security=True;MultipleActiveResultSets=True";

        public Dictionary<string, object> GetRow(SqlDataReader reader)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = xulydulieu(reader.GetValue(i).ToString());
            }
            return row;
        }

        public List<Dictionary<string, object>> Get(string sql)
        {
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = GetRow(reader);
                            dataList.Add(row);
                        }
                    }

                    connection.Close();
                }
            }

            return dataList;
        }

        public string xulydulieu(string text)
        {
            string s = text.Replace(",", "&44;");
            s = s.Replace("\"", "&34;");
            s = s.Replace("'", "&39;");
            return s;
        }
    }

}