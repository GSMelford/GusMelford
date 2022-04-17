using Bot.Api.Collection;

namespace Bot.Api.BotRequests.Interfaces
{
    public interface IParameters
    {
        public ParameterCollection BuildParameters();
    }
}