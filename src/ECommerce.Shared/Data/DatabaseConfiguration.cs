namespace ECommerce.Shared.Data;

public static class DatabaseConfiguration
{
    public static string ConnectionString { get; } = BuildConnectionString();

    private static string BuildConnectionString()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "ECommerce.sln")))
            {
                var dataDir = Path.Combine(dir.FullName, "data");
                Directory.CreateDirectory(dataDir);
                return $"Data Source={Path.Combine(dataDir, "ecommerce.db")}";
            }

            dir = dir.Parent;
        }

        return "Data Source=ecommerce.db";
    }
}
