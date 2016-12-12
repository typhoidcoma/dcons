<?php
// (c) 2015 Ignition Games
require("includes/server_settings.php");
require("includes/sql.php");
$sql = new sql;
$sql->connect($server);

require("includes/include-functions.php");

$cmd = $_REQUEST["cmd"];

// NOTE: For extra security, you should set the User-Agent sent from Unity to something unique to your game that isnt easily guessable
// and comment in the code below. It will check to make sure that the only requests coming to this script are in fact from your game
// and not from something like a web browser.
//if($_SERVER['HTTP_USER_AGENT'] != "EXAMPLE_USER_AGENT")
//	exit("UA");

// Check to make sure the received private key matches the official server version.
$unity_private_key = $_REQUEST["pk"];
if($unity_private_key != $GAME_PRIVATE_KEY)
	exit("criticalerror - key mismatch");

// If the $cmd variable is blank, we are logging in
if($cmd == "")
{
	$user = $_REQUEST["uid"];
	$usrnme = $_REQUEST["usrnme"];
	
	// Check to see if we have an account setup with the sent Social userID
	$sql->select("accountsMobile", "WHERE `data_social`='".$user."'");
	$row = $sql->fetch();
	
	// Our database row returned back blank
	if($row["user_id"] == "")
	{
		$username = $usrnme;
		$newUser = createNewMobileUser($user, $username);

		$sql->select("accountsMobile", "WHERE `user_id`='".$newUser."'");
		$row = $sql->fetch();
	}
	
	// We either are working with the correct login details straight from the accounts table or the one we just created
	if($row["user_id"] != "")
	{
			$auth = $row["user_id"];
			
			// Update account info
			$upd = Array("user_ip"=>$_SERVER['REMOTE_ADDR'], "last_login"=>date('Y-m-d H:i:s'));
			$sql->update("accountsMobile", $upd, "WHERE `user_id`='".$auth."'");
			
			// Next send the player data from the database
			echo resendPlayerData($row);
			return;
	}
	else
		exit("criticalerror"); // return back critical error
}
?>