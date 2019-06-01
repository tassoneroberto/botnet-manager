<?php
$username = "admin";
$password = "admin";
$host = "localhost";
$database = "db_name";
$conn = new mysqli($host, $username, $password,$database);
$conn->set_charset("utf8")
?>