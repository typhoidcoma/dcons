#pragma strict
//---------------
// (C) 2015 Ignition Games
//----------------

// This must match the private key set in the PHP file in Server Backend/PHP/includes/server_settings.php and must
// be URL safe (no spaces or special characters)
var UNITY_PRIVATE_KEY:String = "set_this_to_something_hard_to_guess";

var url_string="http://master.ignition-games.com/unityTest/manager.php?pk="+UNITY_PRIVATE_KEY+"&uid=";
var user_id:String = "";

class c_connObject
{
	// Basic Connection Data
	var id:int = 0;
	var username:String = "";
	var serverTime:int = 0;
	
	// Game Specific Data
	var lastPosition:Vector3 = Vector3.zero;
	var exampleDatabaseVariable:String = "";
}
var serverConnection:c_connObject = new c_connObject();

function Start()
{
	Debug.Log ("Logging in...");
	connectLogin();
}

function connectLogin()
{
	if(PlayerPrefs.GetString("userID") != null)
		user_id = PlayerPrefs.GetString("userID"); // The ID that matches the row in the database for our player
	else
		user_id = "0"; // This will be set later
		
	download_data();
}

function download_data() 
{
	//var headers:Hashtable = new Hashtable();
	//headers.Add("User-Agent", "EXAMPLE_USER_AGENT");
	
    var data_get:WWW = WWW(url_string + user_id.ToString());
	
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
	
	Debug.Log("Moving test cube to loaded position: "+serverConnection.lastPosition);
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