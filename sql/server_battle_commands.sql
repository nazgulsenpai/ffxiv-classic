-- MySQL dump 10.13  Distrib 5.7.11, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_server
-- ------------------------------------------------------
-- Server version	5.7.11

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
-- Table structure for table `server_battle_commands`
--

DROP TABLE IF EXISTS `server_battle_commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battle_commands` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `classJob` tinyint(3) unsigned NOT NULL,
  `lvl` tinyint(3) unsigned NOT NULL,
  `requirements` smallint(5) unsigned NOT NULL,
  `mainTarget` tinyint(3) unsigned NOT NULL,
  `validTarget` tinyint(3) unsigned NOT NULL,
  `aoeType` tinyint(3) unsigned NOT NULL,
  `aoeRange` float NOT NULL DEFAULT '0',
  `aoeMinRange` float NOT NULL DEFAULT '0',
  `aoeConeAngle` float NOT NULL DEFAULT '0',
  `aoeRotateAngle` float NOT NULL DEFAULT '0',
  `aoeTarget` tinyint(3) NOT NULL,
  `basePotency` smallint(5) unsigned NOT NULL,
  `numHits` tinyint(3) unsigned NOT NULL,
  `positionBonus` tinyint(3) unsigned NOT NULL,
  `procRequirement` tinyint(3) unsigned NOT NULL,
  `range` int(10) unsigned NOT NULL,
  `minRange` int(10) unsigned NOT NULL DEFAULT '0',
  `bestRange` int(10) unsigned NOT NULL DEFAULT '0',
  `rangeHeight` int(10) unsigned NOT NULL DEFAULT '10',
  `rangeWidth` int(10) unsigned NOT NULL DEFAULT '2',
  `statusId` int(10) NOT NULL,
  `statusDuration` int(10) unsigned NOT NULL,
  `statusChance` float NOT NULL DEFAULT '0.5',
  `castType` tinyint(3) unsigned NOT NULL,
  `castTime` int(10) unsigned NOT NULL,
  `recastTime` int(10) unsigned NOT NULL,
  `mpCost` smallint(5) unsigned NOT NULL,
  `tpCost` smallint(5) unsigned NOT NULL,
  `animationType` tinyint(3) unsigned NOT NULL,
  `effectAnimation` smallint(5) unsigned NOT NULL,
  `modelAnimation` smallint(5) unsigned NOT NULL,
  `animationDuration` smallint(5) unsigned NOT NULL,
  `battleAnimation` int(10) unsigned NOT NULL DEFAULT '0',
  `validUser` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `comboId1` int(11) NOT NULL DEFAULT '0',
  `comboId2` int(11) NOT NULL DEFAULT '0',
  `comboStep` tinyint(4) NOT NULL DEFAULT '0',
  `accuracyMod` float NOT NULL DEFAULT '1',
  `worldMasterTextId` smallint(5) unsigned NOT NULL DEFAULT '0',
  `commandType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `actionType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `actionProperty` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `isRanged` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battle_commands`
--

LOCK TABLES `server_battle_commands` WRITE;
/*!40000 ALTER TABLE `server_battle_commands` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battle_commands` VALUES (23456,'breath_of_the_lion',0,1,0,1,32,2,12,0,0.5,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,1,3,318771200,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23457,'voice_of_the_lion',0,1,0,1,32,3,20,0,0,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,2,3,318775296,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23458,'breath_of_the_dragon',0,1,0,1,32,2,12,0,0.666667,0.25,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,3,3,318779392,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23459,'voice_of_the_dragon',0,1,0,1,32,1,22,15,2,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,4,3,318783488,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23460,'breath_of_the_ram',0,1,0,1,32,2,12,0,0.666667,-0.25,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,5,3,318787584,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23461,'voice_of_the_ram',0,1,0,1,32,1,10,0,2,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,6,3,318791680,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23462,'dissent_of_the_bat',0,1,0,1,32,2,16,0,0.66667,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,7,3,318795776,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23463,'chaotic_chorus',0,1,0,1,32,1,16,0,2,0,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,8,3,318799872,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (23464,'the_scorpions_sting',0,1,0,1,32,2,12,0,0.5,1,1,100,1,0,0,0,0,0,10,2,0,0,0,11,3500,10,0,0,19,0,9,3,318803968,0,0,0,3,0,30301,2,2,9,1);
INSERT INTO `server_battle_commands` VALUES (27100,'second_wind',2,6,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,0,0,0,0,0,45,0,0,14,519,2,3,234889735,0,0,0,0,0,30320,3,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27101,'blindside',2,14,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223237,60,1,0,0,60,0,0,14,635,2,3,234889851,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27102,'taunt',2,42,4,32,32,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,223073,5,1,0,0,60,0,0,14,517,2,3,234889733,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27103,'featherfoot',2,2,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223075,30,1,0,0,60,0,0,14,535,2,3,234889751,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27104,'fists_of_fire',2,34,4,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223209,4294967295,1,0,0,10,0,0,14,684,2,3,234889900,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27105,'fists_of_earth',2,22,4,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223210,4294967295,1,0,0,10,0,0,14,685,2,3,234889901,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27106,'hundred_fists',15,50,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223244,15,1,0,0,900,0,0,14,712,2,3,234889928,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27107,'spinning_heel',15,35,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223245,20,1,0,0,120,0,0,14,718,2,3,234889934,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27108,'shoulder_tackle',15,30,0,32,32,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,223015,10,0.75,0,0,60,0,0,18,1048,205,3,302830616,0,0,0,0,0,30301,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27109,'fists_of_wind',15,40,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223211,4294967295,1,0,0,10,0,0,14,720,2,3,234889936,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27110,'pummel',2,1,1,32,32,0,0,0,0,0,0,100,1,1,0,5,0,0,10,2,0,0,0,0,0,10,0,1000,18,1027,1,3,301995011,0,27111,27113,1,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27111,'concussive_blow',2,10,1,32,32,0,0,0,0,0,0,100,1,4,0,5,0,0,10,2,223007,30,0,0,0,30,0,1500,18,20,3,3,302002196,0,27112,0,2,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27112,'simian_thrash',2,50,4,32,32,0,0,0,0,0,0,100,9,0,0,5,0,0,10,2,0,0,0,0,0,80,0,2000,18,1003,202,3,302818283,0,0,0,3,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27113,'aura_pulse',2,38,4,32,32,1,8,0,0,0,1,100,1,0,0,5,0,0,10,2,223003,30,0,0,0,40,0,1500,18,66,203,3,302821442,0,0,0,2,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27114,'pounce',2,4,4,32,32,0,0,0,0,0,0,100,1,2,0,5,0,0,10,2,223015,10,0,0,0,20,0,1500,18,8,3,3,302002184,0,27115,27117,1,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27115,'demolish',2,30,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,30,0,1500,18,1028,2,3,301999108,0,27116,0,2,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27116,'howling_fist',2,46,4,32,32,0,0,0,0,0,0,100,1,4,0,5,0,0,10,2,0,0,0,0,0,80,0,3000,18,1029,2,3,301999109,0,0,0,3,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27117,'sucker_punch',2,26,1,32,32,0,0,0,0,0,0,100,1,4,0,5,0,0,10,2,0,0,0,0,0,15,0,1000,18,73,3,3,302002249,0,0,0,3,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27118,'dragon_kick',15,45,0,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,223013,10,0,0,0,60,0,2000,18,1041,204,3,302826513,0,0,0,0,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27119,'haymaker',2,18,4,32,32,0,0,0,0,0,0,100,1,0,2,5,0,0,10,2,223015,10,0.75,0,0,5,0,250,18,23,201,3,302813207,0,0,0,0,0,30301,2,1,3,0);
INSERT INTO `server_battle_commands` VALUES (27140,'sentinel',3,22,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223062,15,1,0,0,90,0,0,14,526,2,3,234889742,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27141,'aegis_boon',3,6,8,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223058,30,1,0,0,60,0,0,14,583,21,3,234967623,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27142,'rampart',3,2,3,1,5,0,8,0,0,0,1,100,1,0,0,0,0,0,10,2,223064,60,1,0,0,120,0,0,14,536,2,3,234889752,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27143,'tempered_will',3,42,8,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223068,20,1,0,0,180,0,0,14,515,2,3,234889731,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27144,'outmaneuver',3,34,8,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223236,30,1,0,0,90,0,0,14,512,21,3,234967552,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27145,'flash',3,14,3,32,32,0,8,0,0,0,0,100,1,0,0,15,0,0,10,2,223007,10,0,0,0,30,0,0,14,696,2,3,234889912,0,0,0,0,0,30101,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27146,'cover',16,30,0,12,12,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,223173,15,1,0,0,60,0,0,14,725,2,3,234889941,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27147,'divine_veil',16,35,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223248,20,1,0,0,60,0,0,14,713,2,3,234889929,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27148,'hallowed_ground',16,50,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223249,20,1,0,0,900,0,0,14,709,2,3,234889925,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27149,'holy_succor',16,40,0,9,9,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,0,0,0,3,2000,10,100,0,1,701,1,3,16782013,0,0,0,0,0,30328,3,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27150,'fast_blade',3,1,1,32,32,0,0,0,0,0,0,100,1,1,0,5,0,0,10,2,0,0,0,0,0,10,0,1000,18,1023,1,3,301995007,0,27151,27152,1,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27151,'flat_blade',3,26,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,10,0,1500,18,1024,2,3,301999104,0,0,0,2,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27152,'savage_blade',3,10,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,30,0,1000,18,1025,1,3,301995009,0,27153,0,2,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27153,'goring_blade',3,50,8,32,32,0,0,0,0,0,0,100,1,2,0,5,0,0,10,2,223206,30,0,0,0,60,0,3000,18,1026,301,3,303223810,0,0,0,3,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27154,'riot_blade',3,30,8,32,32,0,0,0,0,0,0,100,1,2,0,15,0,0,10,2,223038,30,0,0,0,80,0,2000,18,75,2,3,301998155,0,27155,0,1,0,30301,2,1,1,1);
INSERT INTO `server_battle_commands` VALUES (27155,'rage_of_halone',3,46,8,32,32,0,0,0,0,0,0,100,5,0,0,5,0,0,10,2,0,0,0,0,0,20,0,1500,18,1008,302,3,303227888,0,0,0,2,-40,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27156,'shield_bash',3,18,17,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,223015,5,0.75,0,0,30,0,250,18,5,26,3,302096389,0,0,0,0,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27157,'war_drum',3,38,24,32,32,1,8,0,0,0,1,100,1,0,4,5,0,0,10,2,0,0,0,0,0,60,0,500,14,502,21,3,234967542,0,0,0,0,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27158,'phalanx',3,4,8,32,32,0,0,0,0,0,0,100,1,0,4,5,0,0,10,2,0,0,0,0,0,5,0,250,18,32,1,3,301994016,0,27159,0,1,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27159,'spirits_within',16,45,0,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,60,0,3000,18,1044,304,3,303236116,0,0,0,2,50,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27180,'provoke',4,14,3,32,32,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,223034,30,0,0,0,30,0,0,14,600,2,3,234889816,0,0,0,0,0,30101,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27181,'foresight',4,2,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223083,30,1,0,0,60,0,0,14,545,2,3,234889761,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27182,'bloodbath',4,6,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223081,30,1,0,0,60,0,0,14,581,2,3,234889797,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27183,'berserk',4,22,32,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223207,4294967295,1,0,0,10,0,0,14,682,2,3,234889898,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27184,'rampage',4,34,32,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223208,4294967295,1,0,0,10,0,0,14,546,2,3,234889762,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27185,'enduring_march',4,42,32,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223078,20,1,0,0,180,0,0,14,539,2,3,234889755,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27186,'vengeance',17,30,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223250,15,1,0,0,150,0,0,14,714,2,3,234889930,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27187,'antagonize',17,35,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223251,15,1,0,0,120,0,0,14,715,2,3,234889931,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27188,'collusion',17,40,0,12,12,0,0,0,0,0,0,100,1,0,0,15,0,0,10,2,223097,15,1,0,0,90,0,0,14,711,2,3,234889927,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27189,'mighty_strikes',17,50,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223252,15,1,0,0,900,0,0,14,716,2,3,234889932,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27190,'heavy_swing',4,1,1,32,32,0,0,0,0,0,0,100,1,1,0,5,0,0,10,2,0,0,0,0,0,10,0,1000,18,14,1,3,301993998,0,27191,0,1,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27191,'skull_sunder',4,10,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,30,0,1500,18,43,1,3,301994027,0,27192,0,2,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27192,'steel_cyclone',17,45,0,32,32,1,8,0,0,0,1,100,1,0,0,5,0,0,10,2,223015,3,0,0,0,30,0,2000,18,1040,404,3,303645712,0,0,0,3,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27193,'brutal_swing',4,4,1,32,32,0,0,0,0,0,0,100,1,4,0,5,0,0,10,2,0,0,0,0,0,20,0,1500,18,15,3,3,302002191,0,27194,0,1,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27194,'maim',4,26,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,0,0,0,0,0,30,0,1500,18,88,1,3,301994072,0,27195,0,2,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27195,'godsbane',4,50,32,32,32,0,0,0,0,0,0,100,3,0,0,5,0,0,10,2,0,0,0,0,0,60,0,3000,18,1014,402,3,303637494,0,0,0,3,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27196,'path_of_the_storm',4,38,32,32,32,0,0,0,0,0,0,100,1,2,0,5,0,0,10,2,228021,30,0,0,0,30,0,1500,18,44,401,3,303632428,0,27197,0,1,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27197,'whirlwind',4,46,32,32,32,1,8,0,0,0,1,100,1,0,0,5,0,0,10,2,0,0,0,0,0,80,0,3000,18,1015,403,3,303641591,0,0,0,2,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27198,'fracture',4,18,32,32,32,0,0,0,0,0,0,100,1,0,3,5,0,0,10,2,223013,8,0.75,0,0,40,0,500,18,42,3,3,302002218,0,0,0,0,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27199,'overpower',4,30,1,32,32,2,8,0,0.5,0,1,100,1,0,3,8,0,0,10,2,0,0,0,0,0,5,0,250,18,89,1,3,301994073,0,0,0,0,0,30301,2,1,1,0);
INSERT INTO `server_battle_commands` VALUES (27220,'hawks_eye',7,6,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223106,15,1,0,0,90,0,0,14,516,2,3,234889732,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27221,'quelling_strike',7,22,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223104,30,1,0,0,60,0,0,14,614,2,3,234889830,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27222,'decoy',7,2,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223108,60,1,0,0,90,100,0,14,565,2,3,234889781,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27223,'chameleon',7,42,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,0,0,0,0,0,180,0,0,14,504,2,3,234889720,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27224,'barrage',7,34,64,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223220,60,1,0,0,90,0,0,14,683,2,3,234889899,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27225,'raging_strike',7,14,64,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223221,4294967295,1,0,0,10,0,0,14,632,2,3,234889848,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27226,'swiftsong',7,26,64,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,223224,180,1,0,0,10,100,0,1,150,1,3,16781462,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27227,'battle_voice',18,50,0,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,223029,60,1,0,0,900,0,0,14,721,2,3,234889937,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27228,'heavy_shot',7,1,1,32,32,0,0,0,0,0,0,100,1,0,0,20,0,8,10,2,0,0,0,0,0,10,0,1000,18,1036,4,3,302007308,0,27229,27231,1,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27229,'leaden_arrow',7,10,1,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228021,30,0.75,0,0,30,0,1500,18,1035,4,3,302007307,0,27230,0,2,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27230,'wide_volley',7,50,64,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,0,0,80,0,2000,18,18,703,3,304869394,0,0,0,3,-20,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27231,'quick_nock',7,38,64,32,32,2,10,0,0.5,0,1,100,1,0,0,10,0,0,10,2,0,0,0,0,0,180,0,1000,18,1017,702,3,304866297,0,27232,0,2,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27232,'rain_of_death',18,45,0,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,223015,5,0.75,0,0,30,0,3000,18,1037,704,3,304874509,0,0,0,3,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27233,'piercing_arrow',7,4,1,32,32,0,0,0,0,0,0,100,1,0,0,20,0,8,10,2,0,0,0,0,0,20,0,1000,18,1038,1,3,301995022,0,27234,27236,1,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27234,'gloom_arrow',7,30,1,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223007,30,0,0,0,10,0,1000,18,1039,4,3,302007311,0,27235,0,2,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27235,'bloodletter',7,46,64,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223241,30,0.75,0,0,80,0,1500,18,53,701,3,304861237,0,0,0,3,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27236,'shadowbind',7,18,64,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228011,15,0.9,0,0,40,0,250,18,17,4,3,302006289,0,0,0,2,0,30301,2,1,4,1);
INSERT INTO `server_battle_commands` VALUES (27237,'ballad_of_magi',18,30,0,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,223254,180,1,8,3000,10,100,0,1,709,1,3,16782021,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27238,'paeon_of_war',18,40,0,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,223255,180,1,8,3000,10,50,1000,1,710,1,3,16782022,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27239,'minuet_of_rigor',18,35,0,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,223256,180,1,8,3000,10,100,0,1,711,1,3,16782023,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27258,'refill',7,1,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,32613,3,0,0,0);
INSERT INTO `server_battle_commands` VALUES (27259,'light_shot',7,1,0,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,0,0,0,0,0,17,0,1,3,285216768,0,0,0,0,0,30301,3,1,0,1);
INSERT INTO `server_battle_commands` VALUES (27260,'invigorate',8,14,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223094,30,1,0,0,90,0,0,14,575,2,3,234889791,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27261,'power_surge',8,34,128,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223212,60,1,0,0,10,0,0,14,686,2,3,234889902,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27262,'life_surge',8,22,128,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223215,180,1,0,0,15,0,250,14,687,2,3,234889903,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27263,'dread_spike',8,42,128,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223218,30,1,0,0,120,0,0,14,686,2,3,234889902,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27264,'blood_for_blood',8,6,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223219,60,1,0,0,60,0,0,14,689,2,3,234889905,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27265,'keen_flurry',8,26,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223091,30,1,0,0,90,0,0,14,569,2,3,234889785,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27266,'jump',19,30,0,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,0,0,60,0,0,18,1045,804,3,305284117,0,0,0,0,0,30301,3,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27267,'elusive_jump',19,40,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,0,0,0,0,0,180,0,0,18,1046,806,3,305292310,0,0,0,0,0,30101,3,0,0,0);
INSERT INTO `server_battle_commands` VALUES (27268,'dragonfire_dive',19,50,0,32,32,1,4,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,0,0,900,0,0,18,1045,804,3,305284117,0,0,0,0,0,30301,3,2,5,0);
INSERT INTO `server_battle_commands` VALUES (27269,'true_thrust',8,1,1,32,32,0,0,0,0,0,0,100,1,1,0,5,0,0,10,2,0,0,0,0,0,10,0,1000,18,1030,2,3,301999110,0,27270,27273,1,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27270,'leg_sweep',8,30,1,32,32,2,8,0,0.5,0,1,100,1,0,0,5,0,0,10,2,223015,8,0,0,0,30,0,1000,18,37,1,3,301994021,0,27271,0,2,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27271,'doom_spike',8,46,128,32,32,3,5,0,0,0,1,100,1,0,0,5,0,0,10,2,0,0,0,0,0,60,0,3000,18,83,801,3,305270867,0,0,0,3,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27272,'disembowel',19,35,0,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,223005,15,0.75,0,0,30,0,750,18,1042,2,3,301999122,0,0,0,0,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27273,'heavy_thrust',8,10,1,32,32,0,0,0,0,0,0,100,1,0,0,5,0,0,10,2,223015,4,0.75,0,0,20,0,1500,18,1031,1,3,301995015,0,0,0,0,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27274,'vorpal_thrust',8,2,1,32,32,0,0,0,0,0,0,100,1,2,0,5,0,0,10,2,0,0,0,0,0,20,0,1500,18,1032,2,3,301999112,0,27275,0,1,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27275,'impulse_drive',8,18,1,32,32,0,0,0,0,0,0,100,1,4,0,5,0,0,10,2,0,0,0,0,0,30,0,1500,18,1033,2,3,301999113,0,27276,27277,2,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27276,'chaos_thrust',8,50,128,32,32,0,0,0,0,0,0,100,6,0,0,5,0,0,10,2,0,0,0,0,0,80,0,3000,18,40,802,3,305274920,0,0,0,3,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27277,'ring_of_talons',19,45,0,32,32,1,8,0,0,0,1,100,1,0,0,5,0,0,10,2,0,0,0,0,0,60,0,2000,18,1009,803,3,305279985,0,0,0,3,0,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27278,'feint',8,4,1,32,32,0,0,0,0,0,0,100,1,0,1,5,0,0,10,2,0,0,0,0,0,10,0,250,18,39,2,3,301998119,0,27272,0,1,100,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27279,'full_thrust',8,38,128,32,32,0,0,0,0,0,0,100,1,0,1,5,0,0,10,2,0,0,0,0,0,30,0,250,18,1034,801,3,305271818,0,0,0,0,50,30301,2,1,2,0);
INSERT INTO `server_battle_commands` VALUES (27300,'dark_seal',22,14,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223229,30,1,0,0,90,0,0,14,518,2,3,234889734,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27301,'resonance',22,22,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223230,30,1,0,0,90,0,0,14,669,2,3,234889885,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27302,'excruciate',22,38,256,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223231,30,1,0,0,90,0,0,14,694,2,3,234889910,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27303,'necrogenesis',22,6,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223232,30,1,0,0,90,0,0,14,695,2,3,234889911,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27304,'parsimony',22,2,256,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223233,30,1,0,0,90,0,0,14,568,2,3,234889784,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27305,'convert',26,30,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,0,0,0,0,0,450,0,0,14,724,2,3,234889940,0,0,0,0,0,30101,3,0,0,0);
INSERT INTO `server_battle_commands` VALUES (27306,'sleep',22,42,256,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228001,60,0.9,2,3000,0,75,0,1,651,1,3,16781963,0,0,0,0,0,30328,4,4,12,0);
INSERT INTO `server_battle_commands` VALUES (27307,'sanguine_rite',22,30,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223234,20,1,0,0,60,120,0,1,152,1,3,16781464,0,0,0,0,0,30328,4,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27308,'blizzard',22,4,256,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228021,30,0.75,2,3000,10,90,0,1,502,1,3,16781814,0,0,0,0,0,30301,4,2,6,0);
INSERT INTO `server_battle_commands` VALUES (27309,'blizzara',22,26,256,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,228011,30,0.75,0,0,40,150,0,1,506,1,3,16781818,0,0,0,0,0,30301,4,2,6,0);
INSERT INTO `server_battle_commands` VALUES (27310,'fire',22,10,3,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,3000,8,105,0,1,501,1,3,16781813,0,27311,0,1,0,30301,4,2,5,0);
INSERT INTO `server_battle_commands` VALUES (27311,'fira',22,34,3,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,5000,16,180,0,1,504,1,3,16781816,0,27312,0,2,0,30301,4,2,5,0);
INSERT INTO `server_battle_commands` VALUES (27312,'firaga',22,50,256,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,8000,7,255,0,1,700,1,3,16782012,0,0,0,3,0,30301,4,2,5,0);
INSERT INTO `server_battle_commands` VALUES (27313,'thunder',22,1,3,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,2000,6,75,0,1,503,1,3,16781815,0,27314,0,1,0,30301,4,2,9,0);
INSERT INTO `server_battle_commands` VALUES (27314,'thundara',22,18,256,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223015,4,0.75,0,0,30,135,0,1,508,1,3,16781820,0,27315,27316,2,0,30301,4,2,9,0);
INSERT INTO `server_battle_commands` VALUES (27315,'thundaga',22,46,256,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,5000,45,195,0,1,509,1,3,16781821,0,0,0,3,0,30301,4,2,9,0);
INSERT INTO `server_battle_commands` VALUES (27316,'burst',26,50,0,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,4000,900,90,0,1,705,1,3,16782017,0,0,0,3,0,30301,4,2,9,0);
INSERT INTO `server_battle_commands` VALUES (27317,'sleepga',26,45,0,32,32,1,8,0,0,0,0,100,1,0,0,20,0,0,10,2,228001,60,0.9,2,4000,0,100,0,1,704,1,3,16782016,0,0,0,0,0,30328,4,4,12,0);
INSERT INTO `server_battle_commands` VALUES (27318,'flare',26,40,0,1,32,1,8,0,0,0,1,100,1,0,0,20,0,0,10,2,223262,30,0.75,2,8000,120,200,0,1,706,1,3,16782018,0,0,0,0,0,30301,4,2,5,0);
INSERT INTO `server_battle_commands` VALUES (27319,'freeze',26,35,0,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,2,5000,120,120,0,1,707,1,3,16782019,0,0,0,0,0,30301,4,2,6,0);
INSERT INTO `server_battle_commands` VALUES (27340,'sacred_prism',23,34,3,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223225,60,1,0,0,90,0,0,14,690,2,3,234889906,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27341,'shroud_of_saints',23,38,512,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223226,20,1,0,0,180,0,0,14,691,2,3,234889907,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27342,'cleric_stance',23,10,512,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223227,4294967295,1,0,0,30,0,0,14,692,2,3,234889908,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27343,'blissful_mind',23,14,512,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223228,4294967295,1,0,0,30,0,0,14,693,2,3,234889909,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27344,'presence_of_mind',27,30,0,1,1,0,0,0,0,0,0,100,1,0,0,0,0,0,10,2,223116,30,1,0,0,300,0,0,14,722,2,3,234889938,0,0,0,0,0,30328,3,4,0,0);
INSERT INTO `server_battle_commands` VALUES (27345,'benediction',27,50,0,1,5,1,20,0,0,0,1,100,1,0,0,0,0,0,10,2,0,0,0,0,0,900,0,0,14,723,2,3,234889939,0,0,0,0,0,30320,3,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27346,'cure',23,2,3,9,9,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,2000,5,40,0,1,101,1,3,16781413,0,0,0,0,0,30320,4,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27347,'cura',23,30,512,9,9,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,2000,5,100,0,1,103,1,3,16781415,0,0,0,0,0,30320,4,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27348,'curaga',23,46,512,5,5,1,15,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,3000,10,150,0,1,146,1,3,16781458,0,0,0,0,0,30320,4,3,13,0);
INSERT INTO `server_battle_commands` VALUES (27349,'raise',23,18,512,130,130,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,10000,300,150,0,1,148,1,3,16781460,0,0,0,0,0,30101,4,4,11,0);
INSERT INTO `server_battle_commands` VALUES (27350,'stoneskin',23,26,3,9,9,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223133,300,1,3,3000,30,50,0,1,133,1,3,16781445,0,0,0,0,0,30328,4,4,8,0);
INSERT INTO `server_battle_commands` VALUES (27351,'protect',23,6,3,5,5,1,20,0,0,0,0,100,1,0,0,20,0,0,10,2,223129,300,1,3,3000,30,80,0,1,1085,1,3,16782397,0,0,0,0,0,30328,4,4,11,0);
INSERT INTO `server_battle_commands` VALUES (27352,'repose',23,50,0,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228001,60,0.9,3,3000,0,80,0,1,151,1,3,16781463,0,0,0,0,0,30328,4,4,10,0);
INSERT INTO `server_battle_commands` VALUES (27353,'aero',23,4,3,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223235,20,0.75,3,3000,6,75,0,1,510,1,3,16781822,0,27354,0,1,0,30301,4,2,7,0);
INSERT INTO `server_battle_commands` VALUES (27354,'aerora',23,42,512,32,32,1,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,4000,20,150,0,1,511,1,3,16781823,0,0,0,2,0,30301,4,2,7,0);
INSERT INTO `server_battle_commands` VALUES (27355,'stone',23,1,3,32,32,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223243,10,0.75,3,2000,6,75,0,1,513,1,3,16781825,0,27356,0,1,0,30301,4,2,8,0);
INSERT INTO `server_battle_commands` VALUES (27356,'stonera',23,22,512,32,32,1,0,0,0,0,0,100,1,0,0,20,0,0,10,2,228021,30,0.75,3,3000,30,150,0,1,514,1,3,16781826,0,0,0,2,0,30301,4,2,8,0);
INSERT INTO `server_battle_commands` VALUES (27357,'esuna',27,40,0,9,9,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,0,0,0,3,2000,10,40,0,1,702,1,3,16782014,0,0,0,0,0,30329,4,0,13,0);
INSERT INTO `server_battle_commands` VALUES (27358,'regen',27,35,0,9,9,0,0,0,0,0,0,100,1,0,0,20,0,0,10,2,223180,45,1,3,2000,5,20,0,1,703,1,3,16782015,0,0,0,0,0,30328,4,4,13,0);
INSERT INTO `server_battle_commands` VALUES (27359,'holy',27,45,0,1,32,1,8,0,0,0,1,100,1,0,0,0,0,0,10,2,228011,10,0.9,0,0,300,100,0,1,708,1,3,16782020,0,0,0,0,0,30301,4,2,11,0);
/*!40000 ALTER TABLE `server_battle_commands` ENABLE KEYS */;
UNLOCK TABLES;
commit;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-07-02  0:22:43
