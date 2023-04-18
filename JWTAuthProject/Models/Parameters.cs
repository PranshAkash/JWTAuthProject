using Dapper;

namespace JWTAuthProject.Models
{
    public class Parameters
    {
        public DynamicParameters dynamicParameters { get; set; }
        public List<arg> arguments { get; set; }
        public string preparedQuery { get; set; }
    }
    public class arg
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class InMemoryFile
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
