#!/usr/bin/php
<?php
if(!isset($argc)) {
    die("Arguments are not given.");
}

if($argc >= 2) {
    $caller_id = $argv[1];
    //echo $caller_id."\n";
}
else {
    die("Argument (Caller ID) is not given.");
}

$serverName = "192.168.1.3,1433";
$connectionOptions = array(
    "database" => "InfoCenterDB",
	"uid" => "sa",
	"pwd" => "sa"
);

// Establishes the connection
$conn = sqlsrv_connect($serverName, $connectionOptions);
if($conn === false) {
die(print_r(sqlsrv_errors()));
}

//$sql = "INSERT INTO [InfoCenterDB].[dbo].[AsteriskCaller] (Caller) VALUES ('777')";
//$stmt = sqlsrv_query($conn, $sql);

$sql = "INSERT INTO [InfoCenterDB].[dbo].[AsteriskCaller] (Caller) VALUES (?)";
$params = array($caller_id);
//print_r($params);
$stmt = sqlsrv_query($conn, $sql, $params);

if($stmt === false) {
    die(print_r(sqlsrv_errors()));
}

?>