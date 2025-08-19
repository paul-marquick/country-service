using CountryService.DataAccess.ListQuery;

namespace CountryService.DataAccess.Models.Country;

public static class CountryMetaData
{
    public static DataType GetDataType(string propertyName)
    {
        switch (propertyName)
        {
            case "Iso2":
            case "Iso3":
            case "Name":
            case "CallingCode":
                return DataType.Text;

            case "IsoNumber":
                return DataType.Numeric;

            default:
                throw new ArgumentException($"Unknown property name: {propertyName}.");
        }
    }

    public static readonly string[] SortableProperties = ["Iso2", "Iso3", "IsoNumber", "Name"];

    public static readonly string[] FilterableProperties = ["Iso2", "Iso3", "IsoNumber", "Name", "CallingCode"];     
}
