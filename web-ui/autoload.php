<?php
require_once(__DIR__ . "/vendor/autoload.php");
$dotenv = Dotenv\Dotenv::createImmutable(__DIR__, '.env.local');
$dotenv->safeLoad();
