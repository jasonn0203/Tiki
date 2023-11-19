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
        //private string connectionString = "Data Source=JASON11;Initial Catalog=Tiki;Integrated Security=True;MultipleActiveResultSets=True";
        private string connectionString = "Data Source = SQL8004.site4now.net; Initial Catalog = db_aa181a_tiki; User Id = db_aa181a_tiki_admin; Password=quanhn2002";

        public Dictionary<string, object> GetRow(SqlDataReader reader)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = xulydulieu(reader.GetValue(i).ToString());
            }
            return row;
        }

        public ArrayList Get(string sql)
        {

            //List<Dictionary<string, object>>
            /*List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

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

            return dataList;*/


            ArrayList dataList = new ArrayList();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(xulydulieu(reader.GetValue(i).ToString()));
                    }
                    dataList.Add(row);
                }
            }

            connection.Close();
            return dataList;


        }

        public string xulydulieu(string text)
        {

            text = text.TrimEnd('0');

            text = text.Replace(",", "đ");


            text = text.Replace("\"", "&34;");
            text = text.Replace("'", "&39;");

            return text;
        }

    }

}