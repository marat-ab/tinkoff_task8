using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task8;

public interface IHandler
{
    TimeSpan Timeout { get; }

    Task PerformOperation(CancellationToken cancellationToken);
}
