IF db_id('CountryService') IS NULL 
    CREATE DATABASE CountryService;
GO

CREATE TABLE CountryService.dbo.Country
(
    Iso2 nvarchar(2) NOT NULL,
    Iso3 nvarchar(3) NOT NULL,
    IsoNumber int NOT NULL,
    Name nvarchar(100) NOT NULL,
    CONSTRAINT CountryPK PRIMARY KEY  (Iso2)
); 
