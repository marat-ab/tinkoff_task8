using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task8;

public record Event(IReadOnlyCollection<Address> Recipients, Payload Payload);
