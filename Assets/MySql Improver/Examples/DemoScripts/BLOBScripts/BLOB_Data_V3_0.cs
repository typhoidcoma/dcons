using UnityEngine;
using System.Collections;
using MySql.Improver;
using MySql.Data.MySqlClient;

public class BLOB_Data_V3_0 : MonoBehaviour {

    void Start()
    {

    }

    void Update()
    {

    }

    Texture2D texture;

    /// <summary>
    /// This example is valid from the version 3.0 of MySql Improver
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(80, 100, 150, 500));
        if (GUILayout.Button("Download"))
        {
            if (!MySqlShared.ConnectionState)
                MySqlSync.OpenConnection("db.darkloftgames.com", "mysqlimp", "123456");
            if (MySqlShared.ConnectionState)
            {
                MySqlDataReader reader = MySqlSync.ExecuteReader("SELECT * FROM mysql_improver.`blob` WHERE ID='6';");
                if (reader.Read())
                {
                    texture = new Texture2D(0, 0);
                    texture.LoadImage(reader.GetBlob(1));
                    reader.Close();
                }
            }
        }

        if (texture != null)
        {
            GUI.DrawTexture(new Rect(10, 50, texture.width, texture.height), texture);
        }
        GUILayout.EndArea();
    }
}
