using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiscordBot.Tests
{
    public class SecretManagerTests
    {
        [Fact]
        public void GetEnvironmentScope_IsSame()
        {
            ISecretManager manager = new EnvironmentSecretManager();
            var scope = manager.SetEnvironmentScope();
            Assert.Equal(scope, manager.GetEnvironmentScope());
        }

        [Fact]
        public void GetDiscordBotSecret_IsSame()
        {
            ISecretManager manager = new EnvironmentSecretManager();
            Environment.SetEnvironmentVariable("DISCORD_BOT_SECRET", "test", EnvironmentVariableTarget.Process);
            var secret = manager.GetDiscordBotSecret();
            Assert.Equal("test", secret);
        }
    }
}