using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CSTester.CSEngine;
using System.Windows;
using Dapper;

public class SqlQuery : ISqlQuery
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

    bool SqlParse(string sql)
    {
        bool valid = false;
        var conn = GetDbConnection();
        IDbCommand cmd = conn.CreateCommand();
        //分析但不执行
        //cmd.CommandText = "SET PARSEONLY ON";
        //编译但不执行
        cmd.CommandText = "SET NOEXEC ON";
        cmd.ExecuteNonQuery();

        try
        {
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            valid = true;
        }
        finally
        {
            //分析并执行
            //cmd.CommandText = "SET PARSEONLY OFF";
            //编译并执行
            cmd.CommandText = "SET NOEXEC OFF";
            cmd.ExecuteNonQuery();
        }

        return valid;
    }

    public IEnumerable<T> Query<T>(string sql)
    {
        IEnumerable<T> list = null;

        if (SqlParse(sql))
        {
            using (var conn = GetDbConnection())
            {
                list = conn.Query(sql);
            }
        }

        return list;
    }

    public int Execute(string sql)
    {
        int count = 0;

        if (SqlParse(sql))
        {
            using (var conn = GetDbConnection())
            {
                count = conn.Execute(sql);
            }
        }

        return count;
    }
}