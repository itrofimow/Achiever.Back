namespace Achiever.Common
{
    public class ApiSettings
    {
        public MongoSettings MongoSettings { get; set; }

        public AuthSettings AuthSettings { get; set; }
    }

    public class MongoSettings
    {
        public string Url { get; set; }
    }

    public class AuthSettings
    {
        public string SecretKey { get; set; }
    }
}