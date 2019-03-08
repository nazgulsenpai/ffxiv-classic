-- MySQL dump 10.13  Distrib 5.7.10, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_database
-- ------------------------------------------------------
-- Server version	5.7.10-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `characters_class_exp`
--

DROP TABLE IF EXISTS `characters_class_exp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `characters_class_exp` (
  `characterId` int(10) unsigned NOT NULL,
  `pug` int(10) unsigned DEFAULT '0',
  `gla` int(10) unsigned DEFAULT '0',
  `mrd` int(10) unsigned DEFAULT '0',
  `arc` int(10) unsigned DEFAULT '0',
  `lnc` int(10) unsigned DEFAULT '0',
  `thm` int(10) unsigned DEFAULT '0',
  `cnj` int(10) unsigned DEFAULT '0',
  `crp` int(10) unsigned DEFAULT '0',
  `bsm` int(10) unsigned DEFAULT '0',
  `arm` int(10) unsigned DEFAULT '0',
  `gsm` int(10) unsigned DEFAULT '0',
  `ltw` int(10) unsigned DEFAULT '0',
  `wvr` int(10) unsigned DEFAULT '0',
  `alc` int(10) unsigned DEFAULT '0',
  `cul` int(10) unsigned DEFAULT '0',
  `min` int(10) unsigned DEFAULT '0',
  `btn` int(10) unsigned DEFAULT '0',
  `fsh` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_class_exp`
--

LOCK TABLES `characters_class_exp` WRITE;
/*!40000 ALTER TABLE `characters_class_exp` DISABLE KEYS */;

/*!40000 ALTER TABLE `characters_class_exp` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:44
