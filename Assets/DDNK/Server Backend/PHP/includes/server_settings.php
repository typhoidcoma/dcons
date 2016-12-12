<?php
$server["type"] = "mysql";
$server["host"] = "localhost";
$server["user"] = "db_username";
$server["pass"] = "db_passwrd";
$server["db"] = "db_database";

// The $GAME_PRIVATE_KEY is a key that is used to compare the incoming HTTP requests from a Unity game
// to make sure that the request is valid. If the key that is sent from Unity doesnt match this one,
// a connection error is thrown and no data is exchanged.
$GAME_PRIVATE_KEY = "set_this_to_something_hard_to_guess";
?>