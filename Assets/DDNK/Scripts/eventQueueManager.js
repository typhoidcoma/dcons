#pragma strict
var srvConn:World_Loader;
srvConn=GameObject.Find("WorldLoader").GetComponent(World_Loader);

public class eventQueueManager extends MonoBehaviour
{
	public class UpdateQueue
	{
		public var www : WWW;
		public var queue : Array;
		public var lastTransTime : float;
	
	
		public function UpdateQueue(ltt : float)
		{
			queue = new Array();
			lastTransTime = ltt;
		}
	}
	
	public var updateQueueObject : UpdateQueue = new UpdateQueue(0.0);
	
	public function OnApplicationQuit()
	{
		if(updateQueueObject.queue.Count > 0)
		{
			Debug.Log("Quitting from eventUpdateQueue class");
			CancelInvoke();
			updateEventQueue();
		}
	}
	
	public function addUpdateQueueEvent(type : String, arg1 : String, arg2 : String, arg3 : String, arg4 : String)
	{
		if(type == "createBuilding")
		{
			// arg1 = gameObject name
			// arg2 = building position

			// Strip "(Clone)" from the name so were left with our prefab name
			arg1 = arg1.Replace("(Clone)", "");
			// We want to keep a constant format for our stored positions so remove
			// all spaces. Refer to the documentation on how the game data is read from
			// the server.
			arg2 = arg2.Replace(" ", "");
			
			// Add the new string to the array
			updateQueueObject.queue.Add(type+" "+arg1+" "+arg2);
		}
		else
		{
			updateQueueObject.queue.Add(type);
		}
		
		// Make sure were not updating too often. Limit the updates to once every 10 seconds or once the queue gets >= 10 items
		//if((updateQueueObject.lastTransTime + 10) < Time.realtimeSinceStartup || updateQueueObject.queue.Count >= 10 || Time.realtimeSinceStartup < 10)
		//{
			updateEventQueue();
		//}
	}
	
	public function updateEventQueue()
	{
		// Lets print out the entire array so we can see what is being sent to the server
		Debug.Log("Updating event queue: ");
		print(updateQueueObject.queue);
		
		// Create a post data form to store our events in
		var postData = new WWWForm();
		// Set the account were working with - directly from our stored variables
		postData.AddField("uid", srvConn.serverConnection.id);
		// Add in the priate key
		postData.AddField("pk", srvConn.UNITY_PRIVATE_KEY);
		// Go thorugh each line in the queue array and add it to our postData
		for(var i : int = 0; i <updateQueueObject.queue.Count; i++)
		{
			var line : String = updateQueueObject.queue[i];
			var words : String[];
			words = line.Split(" "[0]);
			for(var w = 0; w <words.Length; w++)
			{
				postData.AddField("event["+i+"][]", words[w]);
			}
		}
		var url : String = "http://master.ignition-games.com/unityTest/eventQueue.php";
		updateQueueObject.www = new WWW(url, postData);
		updateQueueObject.lastTransTime = Time.realtimeSinceStartup;
		
		yield updateQueueObject.www;
		Debug.Log("Server return text: " + updateQueueObject.www.text);
		if(updateQueueObject.www.text.IndexOf("failed") == -1 && updateQueueObject.www.text.IndexOf("criticalfail") == -1)
		{
			Debug.Log("Event Queue updated successfully!");
			
			// Reload the newly updated data the server has sent us as a result of this update
			srvConn.SendMessage("split_data_into_array", updateQueueObject.www.text.Trim(), SendMessageOptions.DontRequireReceiver);
			
			// Clear the queue array
			updateQueueObject.queue.Clear();
		}
		else
		{
			if(updateQueueObject.www.text.IndexOf("criticalfail") != -1)
			{
				// Its up to you how you handle a failure like this. Reloading the level is one option.
				Debug.Log("Event Queue update failed!");
			}
		}
	}
}