CREATE TABLE `country_service`.`country` (
  `iso_2` VARCHAR(2) NOT NULL,
  `iso_3` VARCHAR(3) NOT NULL,
  `iso_number` INT NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `calling_code` VARCHAR(100) NULL,
  PRIMARY KEY (`iso_2`),
  UNIQUE INDEX `iso_3_UNIQUE` (`iso_3` ASC) VISIBLE,
  UNIQUE INDEX `iso_number_UNIQUE` (`iso_number` ASC) VISIBLE,
  UNIQUE INDEX `name_UNIQUE` (`name` ASC) VISIBLE);
