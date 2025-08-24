// using CountryService.DataAccess.ListQuery;
// using Microsoft.Extensions.Logging;

// namespace CountryService.DataAccess.Models.Country;

// public class CountryMetaData(ILogger<CountryMetaData> logger)
// {
//     public DataType GetDataType(string propertyName)
//     {
//         logger.LogDebug($"GetDataType, propertyName: {propertyName}.");

//         switch (propertyName.ToLower())
//         {
//             case "iso2":
//             case "iso3":
//             case "name":
//             case "callingcode":
//                 return DataType.Text;

//             case "isonumber":
//                 return DataType.Numeric;

//             default:
//                 throw new ArgumentException($"Unknown property name: {propertyName}.");
//         }
//     }

//     public static readonly string[] SortableProperties = ["Iso2", "Iso3", "IsoNumber", "Name"];

//     public static readonly string[] FilterableProperties = ["Iso2", "Iso3", "IsoNumber", "Name", "CallingCode"];

//     public const string DefaultSortPropertyName = "Name";
//     public const string DefaultSortDirection = SortDirection.Ascending;   
// }
