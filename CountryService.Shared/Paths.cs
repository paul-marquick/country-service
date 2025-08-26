namespace CountryService.Shared;

public static class Paths
{
    public static class WebApi
    {
        public static class Country
        {
            public const string BasePath = "country";
            public const string Lookup = "lookup";
            public const string DoesCountryNameExist = "does-country-name-exist";
        }

        public static class Log
        {
            public const string BasePath = "log";
        }

        public static class ServiceInfo
        {
            public const string BasePath = "service-info";
        }
    }
}
