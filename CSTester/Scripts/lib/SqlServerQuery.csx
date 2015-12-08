using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CSTester.CSEngine;
using System.Windows;
using Dapper;

public class SqlServerQuery : ISqlQuery
{
    string connectionString;

    IDbConnection GetDbConnection()
    {
        IDbConnection connection = new SqlConnection(connectionString);
        connection.Open();

        return connection;
    }

    public void SetConnectionString(string connStr)
    {
        connectionString = connStr;
    }

    public IEnumerable<dynamic> Query(string sql)
    {
        IEnumerable<dynamic> list = null;

        using (var conn = GetDbConnection())
        {
            list = conn.Query(sql);
        }

        return list;
    }

    public int Execute(string sql)
    {
        int count = 0;

        using (var conn = GetDbConnection())
        {
            count = conn.Execute(sql);
        }

        return count;
    }
}