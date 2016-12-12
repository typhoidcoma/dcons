using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using MySql.Improver;
using System.Threading;

public class AsynchronousClientCB : MonoBehaviour 
{
    public string Host;
    public string Username;
    public string Password;
    private string text = "";

    void OnEnable()
    {
        MySqlAsyncCallBack.OnException += MySqlAsyncCallBack_OnException;
    }

    void OnDisable()
    {
        MySqlAsyncCallBack.OnException -= MySqlAsyncCallBack_OnException;
    }

    void MySqlAsyncCallBack_OnException(System.Exception ex)
    {
        text = ex.Message;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(580, 100, 200, 600));
        GUILayout.Label("MySql asynchronous callback");

        if (GUILayout.Button("Open connection"))
        {
            MySqlAsyncCallBack.OpenConnection(OpConnCB, Host, Username, Password);
        }

        if (MySqlShared.ConnectionState)
        {
            if (GUILayout.Button("Execute Reader"))
            {
                MySqlAsyncCallBack.ExecuteReader(ExReader, "SHOW DATABASES;");
            }

            if (GUILayout.Button("Execute Scalar"))
            {
                MySqlAsyncCallBack.ExecuteScalar(ExScalar,"SELECT TIME FROM mysql_improver.testtable2;");
            }

            if (GUILayout.Button("Execute Query"))
            {
                MySqlAsyncCallBack.ExecuteQuery(ExQuery, "INSERT INTO mysql_improver.testtable2 VALUES (NULL,CURRENT_TIMESTAMP);");
            }

            if (GUILayout.Button("Get Table infos"))
            {
                MySqlAsyncCallBack.DescribeTable(DeTable, "mysql_improver.testtable2");
            }
        }

        if (GUILayout.Button("Close connection"))
        {
            MySqlAsyncCallBack.CloseConnection(ClConnCB);
        }
        GUILayout.Label(text);
        GUILayout.EndArea();
    }

    void OpConnCB()
    {
        text = ("Connection opened!");
    }

    void ClConnCB()
    {
        text = ("Connection closed!");
    }

    void ExReader(MySqlDataReader reader)
    {
        text = "";
        while (reader.Read())
        {
            text += (reader.GetString(0)) + "\n";
        }
        reader.Close();
    }

     void ExScalar(object res)
    {
        text = (res).ToString();
    }

    void ExQuery(int res)
     {
         if (res == -1)
         {
             text = ("-1 -> False");
         }
         else
         {
             text = (res + " -> True");
         }
     }

    void DeTable(MySqlDataReader reader)
    {
        text = "";
        while (reader.Read())
        {
            text += reader.GetString(0) + " ";
            text += "-> " + reader.GetString(1) + " ";
            text += "-> " + reader.GetString(2) + " ";
            text += "-> " + reader.GetString(3) + " ";
            text += "-> " + reader.GetValue(4).ToString() + " ";
            text += "-> " + reader.GetString(5) + "\n";
        }
        reader.Close();
    }
}
