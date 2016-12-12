#pragma strict

var theEventManager:eventQueueManager;
theEventManager = GameObject.Find("EventQueue").GetComponent(eventQueueManager);

var srvConn:World_Loader;
srvConn=GameObject.Find("WorldLoader").GetComponent(World_Loader);

function placeBuilding()
{
	// Make sure that we have enough gold and diamonds to build this building
	if((srvConn.serverConnection.goldAmount - 1) < 0 || (srvConn.serverConnection.diamondAmount - 1) < 0)
	{
		Debug.Log("Not enough Gold or Diamonds to build this building!");
		return;
	}
	
	// Create a building at a random position
	var randomPosition:Vector3 = Vector3(Random.Range(-25.0, 25.0), 1.5,  Random.Range(0, 50));
	var building: GameObject = Instantiate(Resources.Load("building1", GameObject), randomPosition, Quaternion.identity) as GameObject;
	
	// Add the create building event to the eventQueueManager to be sent to the server
	theEventManager.addUpdateQueueEvent("createBuilding", "building1", randomPosition.ToString(), "", "");
}