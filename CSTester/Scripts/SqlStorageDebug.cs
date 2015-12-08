using System;
using System.Collections.Generic;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.Common;
using CSTester.CSEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SqlStorageDebug : ScriptBase
{
	string ConnectionString = "Data Source=192.168.1.26;Initial Catalog=SF_AuthentDB;Integrated Security=False;User Id=sa;Password=jihe#123456;";
	DbProviderFactory dbProviderFactory;
	
	void SetConnectionString()
	{
		var func = new Function();
		func.Name = MethodBase.GetCurrentMethod().Name;

		if (Functions.Count(i => i.Name == func.Name) <= 0)
		{
			var json = new JObject();
			json.Add("ConnectionString", ConnectionString);
			
			func.Json = json;
			func.Invoker = () =>
			{
				ConnectionString = json.Value<String>("ConnectionString");
				TextPrinter.WriteLine("SetConnectionString Success !");
				TextPrinter.WriteLine("Current ConnectionString " + ConnectionString);
			};

			Functions.Add(func);
		}
	}
	
	IDbConnection GetDbConnection()
	{
		dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
		IDbConnection connection = dbProviderFactory.CreateConnection();
		connection.ConnectionString = ConnectionString;
		connection.Open();
		
		return connection;
	}
	
	string CreateQuerySql(string top , string tableName , string where , string orderby , bool desc)
	{
		string sql = "select ";
		if(!string.IsNullOrEmpty(top))
		{
			sql += " top (" + top + ") ";
		}
		
		sql += " * from " + tableName ;
		
		if(!string.IsNullOrEmpty(where))
		{
			sql += " where " + where;
		}
		if(!string.IsNullOrEmpty(orderby))
		{
			sql += " order by " + orderby ;
			if(desc)
			{
				sql += " desc ";
			}
		}
		
		return sql;
	}
	
	void GetAllTable()
	{
		var func = new Function();
		func.Name = MethodBase.GetCurrentMethod().Name;

		if (Functions.Count(i => i.Name == func.Name) <= 0)
		{
			var json = new JObject();
			func.Json = json;
			func.Invoker = async () =>
			{
				try
				{
					string sql = "select name from sys.sysobjects where type = 'u'";
					using(var conn = GetDbConnection())
					{
						var list = conn.Query(sql);
						TextPrinter.WriteLine(" Get " + conn.Database + " all tables\r\n");
						foreach (var item in list)
						{
							Console.WriteLine("  " + item.name);
						}
						Console.WriteLine();
					}
				}
				catch (Exception e)
				{
					TextPrinter.WriteLine(e);
				}
			};

			Functions.Add(func);
		}		
	}
	
	void Query()
	{
		var func = new Function();
		func.Name = MethodBase.GetCurrentMethod().Name;

		if (Functions.Count(i => i.Name == func.Name) <= 0)
		{
			var json = new JObject();
			json.Add("top", 10);
			json.Add("tableName", "");
			json.Add("where", "");
			json.Add("orderby", "");
			json.Add("desc", true);
			
			func.Json = json;
			func.Invoker = async () =>
			{
				try
				{
					int top = func.Json.Value<int>("top");
					string tableName = func.Json.Value<string>("tableName");
					string where = func.Json.Value<string>("where");
					string orderby = func.Json.Value<string>("orderby");
					bool desc = func.Json.Value<bool>("desc");
					
					string sql = CreateQuerySql(top > 0 ? top + "" : "" , tableName , where , orderby , desc);
					using(var conn = GetDbConnection())
					{
						var list = conn.Query(sql);
						TextPrinter.WriteLine("exec sql query : " + sql);
						Console.WriteLine(JArray.FromObject(list));
					}
				}
				catch (Exception e)
				{
					TextPrinter.WriteLine(e);
				}
			};

			Functions.Add(func);
		}
	}
}