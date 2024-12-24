<?php
$conn = new mysqli(
    $_ENV['db_host'],
    $_ENV['db_username'],
    $_ENV['db_password'],
    $_ENV['db_database'],
);
$conn->set_charset($_ENV['db_charset']);
