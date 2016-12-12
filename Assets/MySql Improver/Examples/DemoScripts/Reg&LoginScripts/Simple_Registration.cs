using UnityEngine;
using System.Collections;
using MySql.Improver;

public class Simple_Registration : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    string username="";
    string password="";
    string email="";
    string text = "";
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(80, 100, 150, 500));
        GUILayout.Label("Registration");
        GUILayout.Label("Username:");
        username = GUILayout.TextField(username);
        GUILayout.Label("Password:");
        password = GUILayout.PasswordField(password, '*');
        GUILayout.Label("E-mail:");
        email = GUILayout.TextField(email);
        if(GUILayout.Button("Sing-in"))
        {
            if (MySqlSync.OpenConnection("db.darkloftgames.com", "mysqlimp", "123456"))
            {
                if (MySqlSync.ExecuteQuery(string.Format("INSERT INTO `mysql_improver`.`test_utenti` (`ID`, `UNAME`, `PASSWORD`, `EMAIL`) VALUES (NULL, '{0}', '{1}', '{2}');", username, password, email)) > -1)
                {
                    text = "Registration succesfully.";
                }
                else
                {
                    text = "Query error.";
                }
            }
            else
            {
                text = "Impossible open the connection.";
            }
        }
        GUILayout.Label(text);
        GUILayout.EndArea();
    }
}
