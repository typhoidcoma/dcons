#pragma strict
//---------------
// (C) 2015 Ignition Games
//----------------

// This must match the private key set in the PHP file in Server Backend/PHP/includes/server_settings.php and must
// be URL safe (no spaces or special characters)
var UNITY_PRIVATE_KEY:String = "set_this_to_something_hard_to_guess";

var url_string="http://master.ignition-games.com/unityTest/manager_worldLoader.php?pk="+UNITY_PRIVATE_KEY+"&uid=";
var user_id:String = "";

var loadingCanvas:Canvas;

class c_building
{
	var buildingType:String = "";
	var position:Vector3 = Vector3.zero;
}

class c_connObjectWorldLoader
{
	// Basic Connection Data
	var id:int = 0;
	var username:String = "";
	var serverTime:int = 0;
	
	// Game Specific Data
	var buildings:c_building[];
	var goldAmount:int = 0;
	var diamondAmount:int = 0;
}
var serverConnection:c_connObjectWorldLoader = new c_connObjectWorldLoader();

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
				case 0: // c_connObjectWorldLoader.serverTime
					serverConnection.serverTime = int.Parse(fields[1]);
				break;
				
				case 1: // c_connObjectWorldLoader.id
					serverConnection.id = int.Parse(fields[1]);
					PlayerPrefs.SetString("userID", fields[1]);
				break;
				
				case 2: // c_connObjectWorldLoader.username
					serverConnection.username = fields[1];
				break;
				
				case 3: // c_connObjectWorldLoader.buildings
					load_building_data(fields[1]);
				break;
				
				case 4: // c_connObjectWorldLoader.goldAmount
					serverConnection.goldAmount = int.Parse(fields[1]);
				break;
				
				case 5: // c_connObjectWorldLoader.diamondAmount
					serverConnection.diamondAmount = int.Parse(fields[1]);
				break;
			}
		}
		line_count+=1;  
	}
}

function load_building_data(string_data:String)
{
	var line_count:int=0;	
	var fields:String[] = string_data.Split(" "[0]);	
	var class_count:int=fields.length/2;	
	var temp_buildings:c_building[] = new c_building[class_count];
	
	for(var x:int=0; x < class_count ; x++)
	{	
		temp_buildings[x] = new c_building();
		temp_buildings[x].buildingType = fields[x+line_count];
		
		if(fields.length>1)
		{
			temp_buildings[x].position = ParseVector3String(fields[x+line_count+1]);
		}			
		line_count +=1;  
	}	
	serverConnection.buildings = temp_buildings;	
}

function buildWorld()
{
	// First lets place our ground prefab
	Instantiate(Resources.Load("theGround", GameObject), Vector3(0,0,0), Quaternion.identity);
	
	// Go through the serverConnection.buildings array and create each building
	for (var x:int=0;x<serverConnection.buildings.length;x++)
	{
		var building: GameObject = Instantiate(Resources.Load(serverConnection.buildings[x].buildingType, GameObject), serverConnection.buildings[x].position, Quaternion.identity) as GameObject;
	}
	return 1;
}

function loadSomething()
{
	Debug.Log("Variables have been loaded successfully!");
	
	// We dont want to do anything until this function is done being called
	yield buildWorld();
	
	// Now hide our loading screen
	loadingCanvas.enabled = false;
	
	// We could display these on the screen but because this is a simple example
	// were going to print how much gold and diamonds we have to the console
	Debug.Log("We have "+serverConnection.goldAmount+" gold!");
	Debug.Log("We have "+serverConnection.diamondAmount+" diamonds!");
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