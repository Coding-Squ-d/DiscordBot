namespace DiscordBot.Core
{
    public interface ISecretManager
    {
        T GetSecret<T>(string key);
        string GetDiscordBotSecret();
        EnvironmentVariableTarget SetEnvironmentScope();
        EnvironmentVariableTarget GetEnvironmentScope();
    }

    public class EnvironmentSecretManager : ISecretManager
    {
        private EnvironmentVariableTarget _scope;
        public T GetSecret<T>(string key)
        {
            string secret = Environment.GetEnvironmentVariable(key, _scope);
            return (T)Convert.ChangeType(secret, typeof(T));
        }

        public string GetDiscordBotSecret()
        {
            return Environment.GetEnvironmentVariable("DISCORD_BOT_SECRET", _scope);
        }

        public EnvironmentVariableTarget GetEnvironmentScope()
        {
            return _scope;
        }
        public EnvironmentVariableTarget SetEnvironmentScope()
        {
            if(OperatingSystem.IsWindows())
            {
                _scope = EnvironmentVariableTarget.User;
                return _scope;
            }

            _scope = EnvironmentVariableTarget.Process;
            return _scope;
        }
    }
}
