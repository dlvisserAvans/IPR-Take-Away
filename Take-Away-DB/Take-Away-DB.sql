-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server versie:                10.5.5-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Versie:              11.0.0.5919
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Databasestructuur van takeaway wordt geschreven
CREATE DATABASE IF NOT EXISTS `takeaway` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `takeaway`;

-- Structuur van  tabel takeaway.products wordt geschreven
CREATE TABLE IF NOT EXISTS `products` (
  `productname` varchar(20) NOT NULL,
  `productprice` int(2) DEFAULT NULL,
  `producttype` enum('Burger','Fries','Pizza','Dessert','Icecream','Milkshake','Snack') DEFAULT NULL,
  PRIMARY KEY (`productname`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumpen data van tabel takeaway.products: ~5 rows (ongeveer)
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` (`productname`, `productprice`, `producttype`) VALUES
	('BigMac', 5, 'Burger'),
	('Frikandel', 2, 'Snack'),
	('Ijsje', 2, 'Icecream'),
	('Milkshake Vanille', 2, 'Milkshake'),
	('Patatje Met', 3, 'Fries');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;

-- Structuur van  tabel takeaway.restaurants wordt geschreven
CREATE TABLE IF NOT EXISTS `restaurants` (
  `restaurantname` varchar(20) NOT NULL,
  `address` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`restaurantname`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumpen data van tabel takeaway.restaurants: ~2 rows (ongeveer)
/*!40000 ALTER TABLE `restaurants` DISABLE KEYS */;
INSERT INTO `restaurants` (`restaurantname`, `address`) VALUES
	('Het Hoekje', 'Het Hoekje 5'),
	('MacDonalds', 'De grote hoek 19');
/*!40000 ALTER TABLE `restaurants` ENABLE KEYS */;

-- Structuur van  tabel takeaway.restaurants_products wordt geschreven
CREATE TABLE IF NOT EXISTS `restaurants_products` (
  `restaurantname` varchar(20) DEFAULT NULL,
  `productname` varchar(20) DEFAULT NULL,
  KEY `restaurantname` (`restaurantname`),
  KEY `productname` (`productname`),
  CONSTRAINT `restaurants_products_ibfk_1` FOREIGN KEY (`restaurantname`) REFERENCES `restaurants` (`restaurantname`),
  CONSTRAINT `restaurants_products_ibfk_2` FOREIGN KEY (`productname`) REFERENCES `products` (`productname`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumpen data van tabel takeaway.restaurants_products: ~4 rows (ongeveer)
/*!40000 ALTER TABLE `restaurants_products` DISABLE KEYS */;
INSERT INTO `restaurants_products` (`restaurantname`, `productname`) VALUES
	('MacDonalds', 'BigMac'),
	('MacDonalds', 'MilkShake Vanille'),
	('Het Hoekje', 'Patatje Met'),
	('Het Hoekje', 'Ijsje'),
	('Het Hoekje', 'Frikandel');
/*!40000 ALTER TABLE `restaurants_products` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
