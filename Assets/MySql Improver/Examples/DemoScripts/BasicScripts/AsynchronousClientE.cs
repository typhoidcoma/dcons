using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using MySql.Improver;
using System.Threading;
using System;

public class AsynchronousClientE : MonoBehaviour
{
    public string Host;
    public string Username;
    public string Password;
    private string text = "";

    void OnEnable()
    {
        MySqlAsyncEvent.OnException += MySqlAsyncEvent_OnException;
        MySqlAsyncEvent.OnOpenConnection += MySqlAsyncEvent_OnOpenConnection;
        MySqlAsyncEvent.OnCloseConnection += MySqlAsyncEvent_OnCloseConnection;
        MySqlAsyncEvent.OnExecuteReader += MySqlAsyncEvent_OnExecuteReader;
        MySqlAsyncEvent.OnExecuteScalar += MySqlAsyncEvent_OnExecuteScalar;
        MySqlAsyncEvent.OnExecuteQuery += MySqlAsyncEvent_OnExecuteQuery;
        MySqlAsyncEvent.OnExecuteFill += MySqlAsyncEvent_OnExecuteFill;
        MySqlAsyncEvent.OnDescribeTable += MySqlAsyncEvent_OnDescribeTable;
    }

    void OnDisable()
    {
        MySqlAsyncEvent.OnException -= MySqlAsyncEvent_OnException;
        MySqlAsyncEvent.OnOpenConnection -= MySqlAsyncEvent_OnOpenConnection;
        MySqlAsyncEvent.OnCloseConnection -= MySqlAsyncEvent_OnCloseConnection;
        MySqlAsyncEvent.OnExecuteReader -= MySqlAsyncEvent_OnExecuteReader;
        MySqlAsyncEvent.OnExecuteScalar -= MySqlAsyncEvent_OnExecuteScalar;
        MySqlAsyncEvent.OnExecuteQuery -= MySqlAsyncEvent_OnExecuteQuery;
        MySqlAsyncEvent.OnDescribeTable -= MySqlAsyncEvent_OnDescribeTable;
    }

    void MySqlAsyncEvent_OnException(System.Exception ex)
    {
        text = ex.Message;
    }

    void Start()
    {
    }
    void Update()
    {

    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(330, 100, 200, 600));
        GUILayout.Label("MySql asynchronous event");

        if (GUILayout.Button("Open connection"))
        {
            MySqlAsyncEvent.OpenConnection(Host, Username, Password);
        }

        if (MySqlShared.ConnectionState)
        {
            if (GUILayout.Button("Execute Reader"))
            {
                MySqlAsyncEvent.ExecuteReader("SHOW TABLES FROM mysql_improver;");
            }

            if (GUILayout.Button("Execute Scalar"))
            {
                MySqlAsyncEvent.ExecuteScalar("SELECT ID FROM mysql_improver.testtable2;");
            }

            if (GUILayout.Button("Execute Query"))
            {
                MySqlAsyncEvent.ExecuteQuery("INSERT INTO mysql_improver.testtable2 VALUES (NULL,DEFAULT);");
            }

            if (GUILayout.Button("Get Table infos"))
            {              
                MySqlAsyncEvent.DescribeTable("mysql_improver.not_unique");                
            }
        }

        if (GUILayout.Button("Close connection"))
        {
            MySqlAsyncEvent.CloseConnection();
        }
        GUILayout.Label(text);
        GUILayout.EndArea();
    }

    void MySqlAsyncEvent_OnDescribeTable(MySqlDataReader reader)
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

    void MySqlAsyncEvent_OnCloseConnection()
    {
       text = "Connection closed!";
    }

    void MySqlAsyncEvent_OnOpenConnection()
    {
        text = "Connection opened!";
    }

    void MySqlAsyncEvent_OnExecuteQuery(int e)
    {
        if (e == -1)
        {
            text = "-1 -> False";
        }
        else
        {
            text = e + " -> True";
        }
    }

    void MySqlAsyncEvent_OnExecuteScalar(object e)
    {
        text = e.ToString();
    }

    void MySqlAsyncEvent_OnExecuteReader(MySqlDataReader e)
    {
        text = "";
        while (e.Read())
        {
            text += e.GetString(0) + "\n";
        }
        e.Close();
    }

    void MySqlAsyncEvent_OnExecuteFill(System.Data.DataSet ds)
    {
        throw new NotImplementedException();
    }
}
