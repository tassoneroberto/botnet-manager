<?php
// Load installed libraries
require_once(__DIR__ . "/vendor/autoload.php");
// Load modules
require_once(__DIR__ . "/modules/constants.php");
require_once(__DIR__ . "/modules/debug.php");
require_once(__DIR__ . "/modules/ioUtils.php");
// Import environment variables
$dotenv = Dotenv\Dotenv::createImmutable(__DIR__, '.env.local');
$dotenv->safeLoad();
