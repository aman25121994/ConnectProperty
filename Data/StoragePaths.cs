namespace PropertyConnect.Data
{
    /// <summary>
    /// Works out a data folder that survives both app restarts AND redeploys.
    ///
    /// On Azure App Service, the %HOME% (Windows) / $HOME (Linux) environment
    /// variable points at persistent storage that is NOT touched when you
    /// redeploy your code — only the site's wwwroot/app folder gets replaced.
    /// So the database and uploaded photos are stored there instead of next
    /// to the app itself.
    ///
    /// Outside of Azure (e.g. running locally, or in plain IIS on your own
    /// server) this just falls back to a "data" folder next to the app,
    /// which is fine there since nothing is wiping that folder on its own.
    /// </summary>
    public static class StoragePaths
    {
        public static string DataDirectory { get; }
        public static string UploadsDirectory { get; }
        public static string DatabasePath { get; }
        public static string ConnectionString { get; }

        static StoragePaths()
        {
            // Azure App Service (Windows) sets HOME to something like D:\home
            // Azure App Service (Linux) sets HOME to /home
            var azureHome = Environment.GetEnvironmentVariable("HOME");
            var isAzure = !string.IsNullOrEmpty(azureHome)
                && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

            DataDirectory = isAzure
                ? Path.Combine(azureHome!, "data")
                : Path.Combine(AppContext.BaseDirectory, "data");

            UploadsDirectory = Path.Combine(DataDirectory, "uploads");
            DatabasePath = Path.Combine(DataDirectory, "propertyconnect.db");

            Directory.CreateDirectory(UploadsDirectory);

            ConnectionString = $"Data Source={DatabasePath}";
        }
    }
}
