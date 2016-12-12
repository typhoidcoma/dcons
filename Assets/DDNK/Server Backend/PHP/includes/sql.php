<?php
// (c) 2015 Ignition Games
class sql
{
    function connect($server)
    {
        $this->type = $server["type"];
        if($this->type == "")
            $this->type = "mysql";

        $this->prefix = $server["prefix"];

        switch($server["type"])
        {
        case "mysql":
            $this->connection = @($GLOBALS["___mysqli_ston"] = mysqli_connect($server["host"],  $server["user"],  $server["pass"]));
            if(!$this->connection)
                $this->ferror("SQL_NO_CONNECT", E_USER_ERROR);
            @((bool)mysqli_query( $this->connection, "USE " . $server["db"]));
            break;

        case "pgsql":
            $str = "host={$server["host"]} port={$server["port"]} dbname={$server["db"]} user={$server["user"]} password={$server["pass"]}";
            $this->connection = @pg_connect($str);
            $status = pg_connection_status($this->connection);
            if($status !== 0)
                $this->ferror("SQL_NO_CONNECT", E_USER_ERROR);
            break;
        }

        if(defined('SQL_DEBUG') && SQL_DEBUG == true)
            $this->print_info = true;
    }

    function query($query, $debug = false)
    {
        $query = $this->addTablePrefix($query);

        $this->lquery = $query;

        switch($this->type)
        {
        case "mysql":
            $this->lresult = @mysqli_query( $this->connection, $query);
            break;

        case "pgsql":
            $query = str_replace("`", "", $query);
            $this->lresult = @pg_query($this->connection, $query);
            break;
        }

        $success = true;

        $this->info[] = Array("query"=>$query, "rows"=>$this->rows($this->lresult), "result"=>$this->lresult, "success"=>$this->lresult);

        if(!$this->lresult)
        {
            $success = false;

            if($debug == true || SQL_DEBUG === true)
                $this->ferror("SQL_QUERY_ERROR", E_USER_ERROR);
        }

        return $this->lresult;
    }

    function select($table, $ext = "", $cols = "*", $debug = false)
    {
        if($cols != "*")
            $cols = "`".implode("`, `", explode(",", $cols))."`";

        $ext = $this->extSecure($ext);

        $query = "SELECT ".$cols." FROM `".$table."` ".$ext;

        return $this->query($query, $debug);
    }

    function insert($table, $cols, $values = "", $debug = false)
    {
        if(is_array($cols))
        {
            $values = "'".implode("', '", $cols)."'";
            $cols = "`".implode("`, `", array_keys($cols))."`";
        }
        else
        {
            $values = "'".implode("', '", explode(",", $values))."'";
            $cols = "`".implode("`, `", explode(",", str_replace(" ", "", $cols)))."`";
        }

        $query = "INSERT INTO `".$table."` (".$cols.") VALUES (".$values.")";

        return $this->query($query, $debug);
    }

    function update($table, $set, $ext, $debug = false)
    {
        $ext = $this->extSecure($ext);
        if(is_array($set))
        {
            $temp = '';
            foreach($set as $key=>$value)
            {
                $temp .= "$key='$value',";
            }
            $set = substr($temp, 0, -1); // Remove extra ,
        }
        else
        {
            $set = $this->extSecure($set, true);
        }

        $query = "UPDATE `".$table."` SET ".$set." ".$ext;

        return $this->query($query, $debug);
    }

    function delete($table, $ext, $debug = false)
    {
        $ext = $this->extSecure($ext);
        
        $query = "DELETE FROM ".$table." ".$ext;

        return $this->query($query, $debug);
    }

    function addTablePrefix($query)
    {
        $s = Array("/\{PRE\}/is", 
            "/\`([^=]*?)\`\.\`([^=]*?)\`/is", 
            "/FROM \`(.*?)\`/is", 
            "/JOIN \`(.*?)\`/is", 
            "/INSERT INTO \`(.*?)\`/is", 
            "/UPDATE \`(.*?)\`/is"
            );

        $r = Array($this->prefix, 
            "`".$this->prefix."$1`.`$2`", 
            "FROM `".$this->prefix."$1`", 
            "JOIN `".$this->prefix."$1`", 
            "INSERT INTO `".$this->prefix."$1`",
            "UPDATE `".$this->prefix."$1`"
            );

        return preg_replace($s, $r, $query);
    }

    function extSecure($ext, $comma = false)
    {
        //PgSQL does not use ``
        if($this->type == "pgsql")
            return $ext;

        // My atempt to take "WHERE test=haha" into "WHERE `test`='haha'
        $str = explode(" ", $ext); // Separate by a " " (space)
        foreach($str as $key=>$value)
        {
            if(strpos($value, "=") === false) // If there is no =, skip;
                continue;

            $value = str_replace(Array("`", "'", ","), "", $value); // Take out ` and ' if they are already there...

            $explode = explode("=", $value);
            $str[$key] = "`".implode("`='", $explode)."'";
        }

        if($comma)
            $ext = implode(", ", $str);
        else
            $ext = implode(" ", $str);

        // HACK: for NOW(). we don't want it to be in 's
        $ext = str_replace("'NOW()'", "NOW()", $ext);

        return $ext;
    }

    function fetch($result = "")
    {
        if($result == "")
            $result = $this->lresult;

        switch($this->type)
        {
        case "mysql":
            return @mysqli_fetch_assoc($result);
            break;

        case "pgsql":
            return @pg_fetch_assoc($result);
            break;
        }
    }

    function rows($result = "")
    {
        if($result == "")
            $result = $this->lresult;

        switch($this->type)
        {
        case "mysql":
            return @mysqli_num_rows($result);
            break;

        case "pgsql":
            return @pg_num_rows($result);
            break;
        }
    }

    function lastId()
    {
        switch($this->type)
        {
        case "mysql":
            return @((is_null($___mysqli_res = mysqli_insert_id($GLOBALS["___mysqli_ston"]))) ? false : $___mysqli_res);
            break;

        case "pgsql":
            return 0;
            break;
        }
    }

    function error()
    {
        switch($this->type)
        {
        case "mysql":
            return ((is_object($GLOBALS["___mysqli_ston"])) ? mysqli_error($GLOBALS["___mysqli_ston"]) : (($___mysqli_res = mysqli_connect_error()) ? $___mysqli_res : false));
            break;

        case "pgsql":
            return pg_last_error($this->connection).pg_last_notice($this->connection);
            break;
        }
    }

    function split($pre, $start, &$arr2)
    {
        $arr = "";
        $arr2 = "";
        foreach($start as $key=>$value)
        {
            if(strpos($key, $pre) === 0)
                $arr[$key] = $value;
            else if($value != "")
                $arr2[$key] = $value;
        }
        return $arr;
    }

    function ferror($msg, $error)
    {
        if($this->errorClass != "")
            $this->errorClass->error($msg, "SQL", $error);
        else
            trigger_error($msg, $error);
    }

    function listQueries()
    {
        echo '<div><table class="table"><tr><td colspan="3">There have been "'.$this->numQueries().'" queries.</td></tr>
<tr><th>Num</th><th>Rows</th><th align="left">Query</th></tr>
';
        foreach($this->info as $key=>$value)
        {
            echo '<tr><td style="background-color: '.(($value["success"])?'green':'red').';">'.($key+1).'</td><td>'.$value["rows"].'</td><td>"'.$value["query"].'"</td></tr>
';
        }

        echo '</table></div>';
    }

    function numQueries()
    {
        return count($this->info);
    }

    function destruct()
    {
        if($this->print_info)
            $this->listQueries();

        if(!$this->connection)
            return;

        switch($this->type)
        {
        case "mysql":
            @((is_null($___mysqli_res = mysqli_close($this->connection))) ? false : $___mysqli_res);
            break;

        case "pgsql":
            @pg_close($this->connection);
            break;
        }
    }

    function __DESTRUCT()
    {
        //$this->destruct();
    }
}
?>
