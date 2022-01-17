using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;

namespace Wordleans.Kernel.Stats
{
    /// <summary>
    /// https://dotnet.github.io/orleans/docs/streaming/streams_programming_APIs.html#explicit-and-implicit-subscriptionsa-nameexplicit-and-implicit-subscriptionsa
    /// </summary>
    [ImplicitStreamSubscription(StatsDefaults.Namespace)]
    public class StatsGrain : Grain, IStats
    {
        private readonly ILogger<StatsGrain> _logger;
        private long _winCount; 
        private long _lostCount; 

        public StatsGrain(ILogger<StatsGrain> logger)
        {
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = base.GetStreamProvider(StatsDefaults.StatsProvider);
            var stream = streamProvider.GetStream<GameEndedMessage>(this.GetPrimaryKey(), StatsDefaults.Namespace);
            await stream.SubscribeAsync(OnNextAsync);
        }

        private Task OnNextAsync(GameEndedMessage msg, StreamSequenceToken token)
        {
            if (msg.Result.HasWon)
            {
                _winCount++;
            }
            else
            {
                _lostCount++;
            }

            return Task.CompletedTask;
        }

        public Task<long> GetWins()
        {
            return Task.FromResult(_winCount);
        }

        public Task<long> GetLosses()
        {
            return Task.FromResult(_lostCount);
        }
    }
}
