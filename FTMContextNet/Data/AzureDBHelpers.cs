using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace FTMContextNet.Data;

public interface IAzureDBHelpers
{
    int GetNextId(string tableName);
    DataTable CreateDataTable(string sqlString);
    void RunCommand(string queryString);
}

public class AzureDBHelpers : IAzureDBHelpers
{

    private readonly string _connectionString;

    public AzureDBHelpers(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int GetNextId(string tableName)
    {
        using var connection = new SqlConnection(_connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "SELECT MAX(Id) from dna." + tableName + ";";

        connection.Open();

        command.Prepare();


        int nextId = Convert.ToInt32(command.ExecuteScalar());

        connection.Close();

        nextId++;

        return nextId;
    }

    public DataTable CreateDataTable(string sqlString)
    {
        DataColumnCollection columns;

        using SqlConnection con = new SqlConnection(_connectionString);

        con.Open();

        using SqlCommand command = new SqlCommand(sqlString, con);

        using (var r = command.ExecuteReader())
        {
            using (var dt = new DataTable())
            {
                dt.Load(r);
                columns = dt.Columns;
            }
        }

        con.Close();


        DataTable dataTable = new DataTable();

        while (columns.Count > 0)
        {
            DataColumn c = columns[0];
            c.Table.Columns.Remove(c);

            dataTable.Columns.Add(c);
        }
        columns = dataTable.Columns;

        return dataTable;

    }

    public void RunCommand(string queryString)
    {
        // using SqlConnection connection = new SqlConnection(_configObj.MSGGenDB01);
        
        using SqlConnection connection = new SqlConnection(_connectionString);

        var command = new SqlCommand(queryString, connection);

        try
        {
            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}