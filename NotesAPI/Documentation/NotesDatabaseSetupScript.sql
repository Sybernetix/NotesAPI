/*Create the database*/
CREATE SCHEMA `notedb` ;

/*Create users table*/
CREATE TABLE `notedb`.`users` (
  `userID` INT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(100) NOT NULL,
  `password` VARCHAR(100) NOT NULL,
  `accountCreated` DATETIME NOT NULL,
  `lastLogin` DATETIME NULL,
  PRIMARY KEY (`userID`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC) VISIBLE);
  
/*Create tokens table*/  
CREATE TABLE `notedb`.`tokens` (
  `userID` INT NOT NULL,
  `token` VARCHAR(100) NOT NULL);
  
/*Create projects table*/
CREATE TABLE `notedb`.`projects` (
  `projectID` INT NOT NULL AUTO_INCREMENT,
  `projectName` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`projectID`),
  UNIQUE INDEX `projectName_UNIQUE` (`projectName` ASC) VISIBLE);
 
/*Create attributes table*/ 
CREATE TABLE `notedb`.`attributes` (
  `attributeID` INT NOT NULL AUTO_INCREMENT,
  `attributeName` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`attributeID`),
  UNIQUE INDEX `attributeName_UNIQUE` (`attributeName` ASC) VISIBLE);
  
/*Create attributetonote table*/
CREATE TABLE `notedb`.`attributetonote` (
  `noteID` INT NOT NULL,
  `attributeID` INT NOT NULL);
 
/*Create notes table*/ 
CREATE TABLE `notedb`.`notes` (
  `noteID` INT NOT NULL AUTO_INCREMENT,
  `projectID` INT NULL,
  `noteBody` VARCHAR(1000) NULL,
  `noteCreated` DATETIME NOT NULL,
  `lastEdit` DATETIME NOT NULL,
  PRIMARY KEY (`noteID`));

