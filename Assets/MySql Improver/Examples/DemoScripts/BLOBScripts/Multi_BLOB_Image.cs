using UnityEngine;
using System.Collections;
using MySql.Improver;
using MySql.Data.MySqlClient;

public class Multi_BLOB_Image : MonoBehaviour 
{
    public GameObject[] pics;

    void Start()
    {

    }
	
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(8, 15, 100, 25), "Load Images"))
        {
            LoadIMG();
        }

        if (GUI.Button(new Rect(116, 15, 100, 25), "Scene 1"))
        {
            Application.LoadLevel(0);

        }
    }

    void LoadIMG()
    {
        if (!MySqlShared.ConnectionState)
            MySqlSync.OpenConnection("db.tykonket.com", "mysqlimp", "123456");
        if (MySqlShared.ConnectionState)
        {
            MySqlDataReader reader = MySqlSync.ExecuteReader("SELECT * FROM mysql_improver.`blob`;");
            int i = 0;
            while (reader.Read() && i < 5)
            {
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(reader.GetBlob(1));
                pics[i].GetComponent<Renderer>().material.mainTexture = texture;
                i++;
            }
            reader.Close();
        }
    }
}
