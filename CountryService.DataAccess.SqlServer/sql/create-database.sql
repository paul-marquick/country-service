CREATE DATABASE CountryService;
GO

CREATE TABLE CountryService.dbo.Country
(
    Iso2 nvarchar(2) NOT NULL,
    Iso3 nvarchar(3) NOT NULL,
    IsoNumber int NOT NULL,
    Name nvarchar(100) NOT NULL,
    CONSTRAINT PK_Country_Iso2 PRIMARY KEY  (Iso2)
); 

CREATE UNIQUE INDEX UI_Country_Iso3 ON CountryService.dbo.Country (Iso3);
CREATE UNIQUE INDEX UI_Country_IsoNumber ON CountryService.dbo.Country (IsoNumber);
CREATE UNIQUE INDEX UI_Country_Name ON CountryService.dbo.Country (Name);
