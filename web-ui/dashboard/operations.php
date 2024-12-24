<?php
session_start();
require_once '../autoload.php';
include '../connect.php';

if (isset($_POST["operation"])) {
	if ($_POST["operation"] == "updateOrder") {
		$sql    = "UPDATE command SET " . $_POST['type'] . "=" . $_POST['set'] . " WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
	} elseif ($_POST["operation"] == "updateScreenCaptureInterval") {
		$sql    = "UPDATE command SET screenCaptureInterval=" . $_POST['value'] . " WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
	} elseif ($_POST["operation"] == "updateOrdersInterval") {
		$sql    = "UPDATE command SET ordersInterval=" . $_POST['value'] . " WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
	} elseif ($_POST["operation"] == "updateNote") {
		$sql    = "UPDATE command SET note='" . $_POST['value'] . "' WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
	} elseif ($_POST["operation"] == "interestingFiles") {
		$stmt = $conn->prepare("UPDATE command SET interestingFiles=? WHERE machineID=?");
		$stmt->bind_param("ss", $_POST['value'], $_POST["id"]);
		$stmt->execute();
	} else if ($_POST["operation"] == "removeMachine") {
		$sql    = "DELETE FROM command WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
		$sql    = "DELETE FROM specs WHERE machineID='" . $_POST['id'] . "'";
		$result = mysqli_query($conn, $sql);
		deleteDir("../machines/" . $_POST['id'], 0700);
	}
}
