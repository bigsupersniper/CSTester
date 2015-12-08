using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using CSTester;
using CSTester.CSEngine;

public class CSDialogScript : ScriptContextBase
{
	IMethodContext OpenSqlQueryDialog()
	{
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("connectionString", "");

        context.Execute = () =>
        {
            try
            {
                if (SqlQueryWindow.Opened)
                {
                    TextPrinter.WriteLine("SqlQueryWindow alreday Opened");
                    return;
                }
                string connectionString = context.Parameters.Value<string>("connectionString");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var query = new SqlServerQuery();
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

        return context;
    }

    IMethodContext OpenImageView()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Execute = () =>
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

        return context;
    }
}