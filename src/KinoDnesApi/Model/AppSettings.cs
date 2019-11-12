using StackExchange.Redis;

namespace KinoDnesApi.Model
{
    public class AppSettings
    {
        public string REDIS_URL { get; set; }

        public ConfigurationOptions GetRedisSettings()
        {
            REDIS_URL = REDIS_URL.Replace("redis://", "");
            var split = REDIS_URL.Split("@");
            var usernamePassword = split[0].Split(":");
            var hostPort = split[1];

            var config = new ConfigurationOptions
            {
                EndPoints = {hostPort},
                Password = usernamePassword[1]
            };
            return config;
        }
    }
}