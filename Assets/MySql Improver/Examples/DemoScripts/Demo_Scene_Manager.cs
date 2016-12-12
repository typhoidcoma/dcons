using UnityEngine;
using System.Collections;

public class Demo_Scene_Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Label(new Rect(8, 8, 100, 25), "Examples :");
        if(GUI.Button(new Rect(8, 40, 100, 25),"MySqlClient"))
        {
            gameObject.GetComponent<SynchronousClient>().enabled = true;
            gameObject.GetComponent<AsynchronousClientCB>().enabled = true;
            gameObject.GetComponent<AsynchronousClientE>().enabled = true;
            gameObject.GetComponent<Simple_Login>().enabled = false;
            gameObject.GetComponent<Simple_Registration>().enabled = false;
            gameObject.GetComponent<BLOB_Image>().enabled = false;
            gameObject.GetComponent<BLOB_Data_V3_0>().enabled = false;
        }

        if (GUI.Button(new Rect(116, 40, 100, 25), "Login"))
        {
            gameObject.GetComponent<SynchronousClient>().enabled = false;
            gameObject.GetComponent<AsynchronousClientCB>().enabled = false;
            gameObject.GetComponent<AsynchronousClientE>().enabled = false;
            gameObject.GetComponent<Simple_Login>().enabled = true;
            gameObject.GetComponent<Simple_Registration>().enabled = false;
            gameObject.GetComponent<BLOB_Image>().enabled = false;
            gameObject.GetComponent<BLOB_Data_V3_0>().enabled = false;

        }

        if (GUI.Button(new Rect(224, 40, 100, 25), "Registration"))
        {
            gameObject.GetComponent<SynchronousClient>().enabled = false;
            gameObject.GetComponent<AsynchronousClientCB>().enabled = false;
            gameObject.GetComponent<AsynchronousClientE>().enabled = false;
            gameObject.GetComponent<Simple_Login>().enabled = false;
            gameObject.GetComponent<Simple_Registration>().enabled = true;
            gameObject.GetComponent<BLOB_Image>().enabled = false;
            gameObject.GetComponent<BLOB_Data_V3_0>().enabled = false;
        }

        if (GUI.Button(new Rect(332, 40, 100, 25), "BLOB Image"))
        {
            gameObject.GetComponent<SynchronousClient>().enabled = false;
            gameObject.GetComponent<AsynchronousClientCB>().enabled = false;
            gameObject.GetComponent<AsynchronousClientE>().enabled = false;
            gameObject.GetComponent<Simple_Login>().enabled = false;
            gameObject.GetComponent<Simple_Registration>().enabled = false;
            gameObject.GetComponent<BLOB_Image>().enabled = true;
            gameObject.GetComponent<BLOB_Data_V3_0>().enabled = false;
        }

        if (GUI.Button(new Rect(440, 40, 100, 25), "BLOB V3.0"))
        {
            gameObject.GetComponent<SynchronousClient>().enabled = false;
            gameObject.GetComponent<AsynchronousClientCB>().enabled = false;
            gameObject.GetComponent<AsynchronousClientE>().enabled = false;
            gameObject.GetComponent<Simple_Login>().enabled = false;
            gameObject.GetComponent<Simple_Registration>().enabled = false;
            gameObject.GetComponent<BLOB_Image>().enabled = false;
            gameObject.GetComponent<BLOB_Data_V3_0>().enabled = true;
        }

        if (GUI.Button(new Rect(548, 40, 100, 25), "Scene 2"))
        {
            Application.LoadLevel(1);
        }
    }
}
