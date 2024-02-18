using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task8;

class Handler : IHandler
{
    private readonly IConsumer _consumer;
    private readonly IPublisher _publisher;
    private readonly ILogger<Handler> _logger;

    public TimeSpan Timeout { get; }

    public Handler(
      TimeSpan timeout,
      IConsumer consumer,
      IPublisher publisher,
      ILogger<Handler> logger)
    {
        Timeout = timeout;

        _consumer = consumer;
        _publisher = publisher;
        _logger = logger;
    }
    
    public async Task PerformOperation(CancellationToken cancellationToken)
    {
        var data = await _consumer.ReadData();

        var addresses = new List<Address>();
        addresses.AddRange(data.Recipients);

        while (!cancellationToken.IsCancellationRequested)
        {
            var rejectedAddresses = await GetAddressForResendData(addresses.ToArray(), data.Payload, cancellationToken); 

            if(rejectedAddresses.Count > 0)
            {
                addresses.Clear();
                addresses.AddRange(rejectedAddresses);

                await Task.Delay(Timeout, cancellationToken);
            }
            else
            {
                return;
            }
        }
    }

    // Private
    private async Task<List<Address>> GetAddressForResendData(Address[] addresses, Payload payload, CancellationToken token)
    {
        var tasks = new Task<SendResult>[addresses.Length];

        for (int i = 0; i < addresses.Length; i++)
        {
            var task = _publisher.SendData(addresses[i], payload);
            tasks[i] = task;
        }

        try
        {
            await Task.WhenAll(tasks).WaitAsync(token);

            var result = new List<Address>(addresses.Length);

            for (int i = 0; i < addresses.Length; i++)
            {
                if (tasks[i].IsCompleted && tasks[i].Result == SendResult.Rejected)
                    result.Add(addresses[i]);
            }

            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(exception: ex, message: ex.Message);

            return new List<Address>();
        }
    }
}
