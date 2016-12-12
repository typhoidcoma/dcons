using UnityEngine;
using System.Collections;
using MySql.Improver;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Data;

public class SynchronousClient : MonoBehaviour
{
    public string Host;
    public string Username;
    public string Password;
    private string text = "dcons_db";

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

	void Start () 
    {
	}

	void Update () {

	}

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(80, 100, 200, 600));
        GUILayout.Label("MySql synchronous");

        if(GUILayout.Button("Open connection"))
        {
            if (MySqlSync.OpenConnection(Host, Username, Password))
            {
                text = "Connected!";
            }
        }

        if (MySqlShared.ConnectionState)
        {
            if (GUILayout.Button("Execute Reader"))
            {
                text = "";
                MySqlDataReader reader = MySqlSync.ExecuteReader("SELECT * FROM dcons_db.dcons_icons LIMIT 10;");
                while (reader.Read())
                {
                    if(reader != null)
                        text += reader.GetString(0) + " -> " + reader.GetString(1) + "\n";
                }
                reader.Close();
            }

            if (GUILayout.Button("Execute Scalar"))
            {
                text = MySqlSync.ExecuteScalar("SELECT * FROM dcons_db.dcons_icons;").ToString();
            }

            if (GUILayout.Button("Execute Query"))
            {
                int res = MySqlSync.ExecuteQuery("INSERT INTO dcons_db.dcons_icons  VALUES (NULL);");
                if (res == -1)
                {
                    text = "-1 -> False";
                }
                else
                {
                    text = res + " -> True";
                }
            }

            if (GUILayout.Button("Get Table infos"))
            {
                text = "";
                MySqlDataReader reader = MySqlSync.DescribeTable("dcons_db.dcons_icons");
                while(reader.Read())
                {
                    text += reader.GetString(0) + " ";
                    text += "-> " + reader.GetString(1) + " ";
                    text += "-> " + reader.GetString(2) + " ";
                    text += "-> " + reader.GetString(3) + " ";
                    text += "-> " + reader.GetValue(4).ToString() +" ";
                    text += "-> " + reader.GetString(5) + "\n";
                }
                reader.Close();
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