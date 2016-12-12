using UnityEngine;
using System.Collections;
using MySql.Improver;

public class Simple_Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    string username = "";
    string password = "";
    string text = "";
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(80, 100, 150, 500));
        GUILayout.Label("Login");
        GUILayout.Label("Username:");
        username = GUILayout.TextField(username);
        GUILayout.Label("Password:");
        password = GUILayout.PasswordField(password, '*');
        if (GUILayout.Button("Log-in"))
        {
            if (MySqlSync.OpenConnection("db.darkloftgames.com", "mysqlimp", "123456"))
            {
                if (MySqlSync.ExecuteScalar(string.Format("SELECT ID FROM `mysql_improver`.`test_utenti` WHERE UNAME='{0}' AND PASSWORD='{1}';", username, password)) != null)
                {
                    text = "Login succesfully.";
                }
                else
                {
                    text = "Username or password wrong.";
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
