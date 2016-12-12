<?php
// (c) 2015 Ignition Games
function createNewUser($user, $usrnme)
{
	global $sql;
	
	$arr = Array("data_username"=>$usrnme,
	"user_createdate"=>date('Y-m-d H:i:s'),
	"data_lastPosition"=>'(2,1.5,5)');
	
	$newUser = $sql->insert("accounts", $arr);
	$newUser = $sql->lastID();

	// Update the new account so that data_id can be sent to the game
	$sql->update("accounts", "`data_id`=".$newUser, "WHERE user_id='".$newUser."'");
	return $newUser;
}

function createNewMobileUser($user, $usrnme)
{
	global $sql;
	
	$arr = Array("data_username"=>$usrnme,
	"user_createdate"=>date('Y-m-d H:i:s'),
	"data_lastPosition"=>'(2,1.5,5)',
	"data_social"=>$user);
	
	$newUser = $sql->insert("accountsMobile", $arr);
	$newUser = $sql->lastID();

	return $newUser;
}

function createNewUserWorldLoader($user, $usrnme)
{
	global $sql;
	
	$goldRand = rand(1, 500); // Get a random amount of gold between 1 and 500
	$diamondRand = rand(1, 50); // Get a random amount of diamonds between 1 and 50
	
	$arr = Array("data_username"=>$usrnme,
	"user_createdate"=>date('Y-m-d H:i:s'),
	"data_buildings"=>'building1 (0,1.5,15)',
	"data_gold"=>$goldRand,
	"data_diamond"=>$diamondRand);
	
	$newUser = $sql->insert("accountsWorldLoader", $arr);
	$newUser = $sql->lastID();

	// Update the new account so that data_id can be sent to the game
	$sql->update("accountsWorldLoader", "`data_id`=".$newUser, "WHERE user_id='".$newUser."'");
	return $newUser;
}

function resendPlayerData($row)
{
	// First, send some non-database variables
	echo "server_time=".time().';';

	// Next, go through all returned columns in our player's database row and look for only
	// columns tagged with "data_"
	foreach($row as $r)
	{
		if(strpos(key($row), "data_") !== false)
		{
			$line = $r.";";			
			echo key($row)."=".$line;
		}
		next($row);
	}
}
?>