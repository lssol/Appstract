namespace appcrawl.Options
{
    public class MongoOptions
    {
        public const string Key = "Mongo";
        
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}