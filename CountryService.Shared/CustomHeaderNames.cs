namespace CountryService.Shared;

/// <summary>
/// Provides a collection of custom header names used for HTTP requests or responses.
/// </summary>
/// <remarks>This class defines constants representing the names of custom headers that can be used to convey
/// additional metadata, such as counts or totals, in HTTP communications. These constants are intended to standardize
/// the use of specific header names across the application.</remarks>
public static class CustomHeaderNames
{
    public const string Count = "Count";  
    public const string Total = "Total";    
}
