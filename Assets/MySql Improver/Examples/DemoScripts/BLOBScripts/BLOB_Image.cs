using UnityEngine;
using System.Collections;
using MySql.Improver;
using MySql.Data.MySqlClient;

public class BLOB_Image : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    Texture2D texture;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(80, 100, 150, 500));
        if (GUILayout.Button("Download"))
        {
            if (!MySqlShared.ConnectionState)
                MySqlSync.OpenConnection("db.darkloftgames.com", "mysqlimp", "123456");
            if (MySqlShared.ConnectionState)
            {
                MySqlDataReader reader = MySqlSync.ExecuteReader("SELECT * FROM mysql_improver.`blob` WHERE ID='1';");
                if (reader.Read())
                {              
                    const int CHUNK_SIZE = 2 * 1024;
                    byte[] buffer = new byte[CHUNK_SIZE];
                    long bRead;
                    long fieldOffset = 0;
                    var stream = new System.IO.MemoryStream();
                    while (true)
                    {
                        bRead = reader.GetBytes(reader.GetOrdinal("BLOB"), fieldOffset, buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, (int)bRead);
                        fieldOffset += bRead;
                        if (bRead < CHUNK_SIZE)
                            break;
                    }

                    texture = new Texture2D(0,0);
                    texture.LoadImage(stream.ToArray());
                    stream.Close();
                }
                reader.Close();
            }
        }

        if (texture != null)
        {
            GUI.DrawTexture(new Rect(10, 50, texture.width, texture.height), texture);
        }
        GUILayout.EndArea();
    }
}