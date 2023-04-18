namespace JWTAuthProject.Models
{
    public interface IConnectionString
    {
        string connectionString { get; set; }
    }

    public class ConnectionString : IConnectionString
    {
        public string connectionString { get; set; }
    }
}
