<?php
// (c) 2015 Ignition Games
require("includes/server_settings.php");
require("includes/sql.php");
$sql = new sql;
$sql->connect($server);

require("includes/include-functions.php");

//if($_SERVER['HTTP_USER_AGENT'] != "EXAMPLE_USER_AGENT")
//	exit("UA");

$user = $_REQUEST["uid"];
$event = $_REQUEST["event"];

// Check to see if we have a profile setup
$sql->select("accountsWorldLoader", "WHERE `user_id`='".$user."'");
$row = $sql->fetch();

// We either are working with the correct login details straight from the profiles table or the one we just created
if($row["user_id"] != "")
{
		$auth = $row["user_id"];
		$totalEvents = count($event);
		$totalEventsSuccess = 0;
		$eventsFailed = "";
		
		// Set our current variables
		$upd = Array();
		$data_buildings = $row["data_buildings"];
		$data_gold = $row["data_gold"];
		$data_diamond = $row["data_diamond"];
		
		$eID = 0;
		if(count($event) > 0)
		foreach($event as $r)
		{
			if($r[0] == "createBuilding")
			{
				$data_buildings .= " ".$r[1]." ".$r[2]; // Append our new building to the current building string
				$data_gold--; // Decrease gold by 1
				$data_diamond--; // Decrease diamonds by 1
			}
			$eID++;
		}

		// Update account info
		$upd["data_buildings"] = $data_buildings;
		$upd["data_gold"] = $data_gold;
		$upd["data_diamond"] = $data_diamond;
		
		if($sql->update("accountsWorldLoader", $upd, "WHERE `user_id`='".$row["user_id"]."'"))
		{
			$sql->select("accountsWorldLoader", "WHERE `user_id`='".$row["user_id"]."'");
			$row = $sql->fetch();
			
			echo resendPlayerData($row);
			return;
		}
		else
		{
			exit("failed ".$eID);
		}
}
else
{
	exit("criticalfail");
}