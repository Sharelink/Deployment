CREATE DATABASE `BahamutDB` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE BahamutDB;
CREATE TABLE `Account` (
  `AccountID` bigint(20) unsigned NOT NULL AUTO_INCREMENT COMMENT 'account id,backend assign',
  `AccountName` varchar(60) NOT NULL COMMENT 'account name,user assign when regist',
  `Email` varchar(100) DEFAULT NULL,
  `Mobile` varchar(40) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `CreateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Password` longtext NOT NULL,
  `Extra` longtext COMMENT 'extra information',
  PRIMARY KEY (`AccountID`),
  UNIQUE KEY `AccountName` (`AccountName`),
  UNIQUE KEY `Email` (`Email`),
  KEY `Mobile` (`Mobile`)
) ENGINE=InnoDB AUTO_INCREMENT=147276 DEFAULT CHARSET=utf8 COMMENT='Account';
ALTER TABLE Account AUTO_INCREMENT=147258;
CREATE TABLE `App` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Appkey` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Document` longtext,
  `Description` mediumtext,
  `CreateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Appkey`),
  UNIQUE KEY `AppKey_UNIQUE` (`Appkey`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COMMENT='app table save the app information';
