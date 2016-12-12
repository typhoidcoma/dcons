using UnityEngine;
using System.Collections;
using MySql.Improver;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Data;

public class Query_With_Parameters : MonoBehaviour {

    public string Host;
    public string Username;
    public string Password;
    private string text = "";

    void OnEnable()
    {
        MySqlSync.OnException += MySqlSync_OnException;
    }

    void OnDisable()
    {
        MySqlSync.OnException -= MySqlSync_OnException;
    }

    void MySqlSync_OnException(System.Exception ex)
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
        GUILayout.BeginArea(new Rect(80, 100, 200, 600));
        GUILayout.Label("MySql synchronous");
        if (GUILayout.Button("Open connection"))
        {
            if (MySqlSync.OpenConnection(Host, Username, Password))
            {
                text = "Connected!";
            }
        }
        if (MySqlShared.ConnectionState)
        {
            if (GUILayout.Button("Execute Query"))
            {
                MySqlParameter par1 = new MySqlParameter("@value1","0");
                int res = MySqlSync.ExecuteQuery("INSERT INTO mysql_improver.testtable1 VALUES (@value1);", par1);
                if (res == -1)
                {
                    text = "-1 -> False";
                }
                else
                {
                    text = res + " -> True";
                }
            }
        }
        if (GUILayout.Button("Close connection"))
        {
            MySqlSync.CloseConnection();
            text = "Disconnected!";
        }
        GUILayout.Label(text);
        GUILayout.EndArea();
    }
}
