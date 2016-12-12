#pragma strict
//---------------
// (C) 2015 Ignition Games
//----------------

// This must match the private key set in the PHP file in Server Backend/PHP/includes/server_settings.php and must
// be URL safe (no spaces or special characters)
var UNITY_PRIVATE_KEY:String = "set_this_to_something_hard_to_guess";

var url_string="http://master.ignition-games.com/unityTest/manager_mobile.php?pk="+UNITY_PRIVATE_KEY+"&uid=";
var user_id:String = "";
var SocialPlatformUsername:String = "";

private var data_text:String;
private var data_array:String[];

class c_connObjectMobile
{
	// Basic Connection Data
	var id:int = 0;
	var username:String = "";
	var serverTime:int = 0;
	
	// Game Specific Data
	var lastPosition:Vector3 = Vector3.zero;
	var exampleDatabaseVariable:String = "";
}
var serverConnection:c_connObjectMobile = new c_connObjectMobile();


function Start()
{
	Debug.Log ("Logging in...");
	connectSocial();
}

function connectSocial()
{
	//#if UNITY_ANDROID && !UNITY_EDITOR
	// If you have a Unity Extention for accessing the GooglePlay Store like
	// Play Games Plugin: https://github.com/playgameservices/play-games-plugin-for-unity
	// activate it here to link the Unity Social API
	//#endif
	#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
	Social.localUser.Authenticate (function (success) 
	{
        if (success)
        {
            Debug.Log ("Authenticated Social Platform user: " + Social.localUser.id);
        	user_id = Social.localUser.id;
        	SocialPlatformUsername = Social.localUser.userName;
        	
        	// Encode the username cause it could contain spaces/special chars
        	SocialPlatformUsername = WWW.EscapeURL(SocialPlatformUsername);
        
        	// Now connect us to OUR master server
       		download_data();
        }
        else
        {
            Debug.Log ("Social Platform authentication failed!");
        }
    });
	#endif
}

function download_data() 
{
	//var headers:Hashtable = new Hashtable();
	//headers.Add("User-Agent", "EXAMPLE_USER_AGENT");
	
    var data_get:WWW = WWW(url_string + user_id.ToString() +"&usrnme="+SocialPlatformUsername);
    yield data_get;
    
    if(data_get.error || data_get.text.Contains("criticalerror"))
	{
		Debug.Log(data_get.error+" "+data_get.text);
    } 
	else 
	{
		Debug.Log(data_get.text);
		split_data_into_array(data_get.text.Trim());
		
		loadSomething();
    }
}

function split_data_into_array(string_data:String)
{
	var line_count:int=0;
		
	for (var line:String in string_data.Split(";"[0])) 
	{      
		var fields:String[] = line.Split("="[0]);    
		
		if(fields.length>1)//if we dont have at leaset 1 field skip
		{
			switch(line_count)
			{
				case 0: // c_connObject.serverTime
					serverConnection.serverTime = int.Parse(fields[1]);
				break;
				
				case 1: // c_connObject.id
					serverConnection.id = int.Parse(fields[1]);
					PlayerPrefs.SetString("userID", fields[1]);
				break;
				
				case 2: // c_connObject.username
					serverConnection.username = fields[1];
				break;
				
				case 3: // c_connObject.exampleDatabaseVariable
					serverConnection.exampleDatabaseVariable = fields[1];
				break;
				
				case 4: // c_connObject.lastPosition
					serverConnection.lastPosition = ParseVector3String(fields[1]);
				break;
			}
		}
		line_count+=1;  
	}
}

function loadSomething()
{
	Debug.Log("Variables have been loaded successfully!");
	
	// In this example, we want to move a cube with the name "moverObject" to the position serverConnection.lastPosition
	// which was just downloaded from the database
	var cube:GameObject = GameObject.Find("moverObject");
	cube.transform.position = serverConnection.lastPosition; // Set the cubes starting position from our downloaded variable
}

// ==========================================
// Helper Functions
// ==========================================
function ParseVector3String (input : String) : Vector3
{
     var stringArray = input.Substring (1, input.Length-2).Split ([")("], System.StringSplitOptions.RemoveEmptyEntries);
     var v3Array : Vector3;
     for (var i = 0; i < stringArray.Length; i++)
     {
         var numbers = stringArray[i].Split(","[0]);
         v3Array = Vector3(parseFloat(numbers[0]), parseFloat(numbers[1]), parseFloat(numbers[2]));
     }
     return v3Array;
 }