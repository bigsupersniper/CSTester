using System;
using System.Collections.Generic;
using System.Configuration;
using CSTester;
using CSTester.CSEngine;

public class SqlQueryAnalyzer : ScriptBase
{
	string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB";
	
	void OpenDialog()
	{
		var func = new Function();
		func.Name = MethodBase.GetCurrentMethod().Name;

		if (Functions.Count(i => i.Name == func.Name) <= 0)
		{
			var json = new JObject();
			json.Add("connectionString", ConnectionString);
			
			func.Json = json;
			func.Invoker = () =>
			{
				try
				{
					string connectionString = func.Json.Value<string>("connectionString");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var query = new SqlQuery();
                        query.SetConnectionString(connectionString);

                        var dialog = new SqlQueryWindow(query);
                        dialog.ShowDialog();
                    }
                    else
                    {
                        TextPrinter.WriteLine("connectionString not set");
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