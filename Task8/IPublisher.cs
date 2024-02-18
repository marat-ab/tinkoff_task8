using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task8;

public interface IPublisher
{
    Task<SendResult> SendData(Address address, Payload payload);
}