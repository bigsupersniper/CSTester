using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using CSTester;
using CSTester.CSEngine;

public class CSEngineDialog : ScriptBase
{
	void OpenSqlQueryDialog()
	{
		var func = new Function();
		func.Name = MethodBase.GetCurrentMethod().Name;

		if (Functions.Count(i => i.Name == func.Name) <= 0)
		{
			var json = new JObject();
			json.Add("connectionString", "Data Source=192.168.1.26;Initial Catalog=SF_AuthentDB;Integrated Security=False;User Id=sa;Password=jihe#123456;");
			
			func.Json = json;
			func.Invoker = () =>
			{
				try
				{
                    if (SqlQueryWindow.Opened)
                    {
                        TextPrinter.WriteLine("SqlQueryWindow alreday Opened");
                        return;
                    }
					string connectionString = func.Json.Value<string>("connectionString");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var query = new SqlQuery();
                        query.SetConnectionString(connectionString);

                        var dialog = new SqlQueryWindow(query);
                        dialog.Show();
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

    void OpenImageView()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            var json = new JObject();
            func.Json = json;
            func.Invoker = () =>
            {
                try
                {
                    if (!ImageView.Opened)
                    {
                        var view = new ImageView();
                        view.Show();
                    }
                    else
                    {
                        TextPrinter.WriteLine("ImageView alreday Opened");
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