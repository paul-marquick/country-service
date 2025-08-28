namespace CountryService.Shared;

public static class Constants
{
    public const string CountryServiceConnectionStringName = "CountryService";
    public const int DefaultOffset = 0;
    public const int DefaultLimit = 10;
    public static readonly int[] ValidLimits = [10, 20, 50, 100];
    public const ushort ToastTimeout = 5000; // ms
}
