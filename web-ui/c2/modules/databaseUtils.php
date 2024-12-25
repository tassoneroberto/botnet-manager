<?php

function updateTimestamp(mysqli $conn, string $machineID): void
{
	$stmt = $conn->prepare("UPDATE command SET last_signal=CURRENT_TIMESTAMP WHERE machineID=?");
	$stmt->bind_param("s", $machineID);
	$stmt->execute();
}

function checkPassword(mysqli $conn, string $machineID, string $password): bool
{
	$stmt = $conn->prepare("SELECT * FROM command WHERE machineID=? AND password=?");
	$stmt->bind_param("ss", $machineID, hash("sha256", mysqli_real_escape_string($conn, $password)));
	$stmt->execute();
	$stmt->store_result();
	$num_row = $stmt->num_rows;
	if ($num_row === 1) {
		return true;
	} else {
		return false;
	}
}

function getValidMachineID(mysqli $conn): string
{
	$found = false;
	while (!$found) {
		$machineID = randomAlphaNumericString(64);
		$stmt = $conn->prepare("SELECT id FROM command WHERE machineID=?");
		$stmt->bind_param("s", $machineID);
		$stmt->execute();
		$stmt->store_result();
		$num_row = $stmt->num_rows;
		if ($num_row === 0) {
			$found = true;
		}
	}
	return $machineID;
}

function randomAlphaNumericString(int $length): string
{
	$keySpace = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
	$pieces = [];
	$max = mb_strlen($keySpace, '8bit') - 1;
	for ($i = 0; $i < $length; ++$i) {
		$pieces[] = $keySpace[random_int(0, $max)];
	}
	return implode('', $pieces);
}
