using System;
using System.Data;
using System.Data.SqlClient;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable table = new DataTable();
            SqlConnection connection = new SqlConnection("Data Source=******;Initial Catalog=*****;User Id=******;Password=****;Connect Timeout=300;MultipleActiveResultSets=True;");
            connection.Open();
            try
            {
                SqlCommand command = new SqlCommand("****<stored procedure name/Sql inline query>****", connection);
                command.CommandTimeout = 0;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@batch", 500);
                command.Parameters.AddWithValue("@iteration", 0);
                table.Load(command.ExecuteReader());
                //RiverOx.Utility.Convert.SqlToJson(DataTable, "PrimaryKeyName of parent table")
                var b = (RiverOx.Utility.Convert.SqlToJson(table, "_id"));
            }
            catch (Exception ex)
            {

            }
            connection.Close();

        }
      
    }
}
