using System;
using System.Collections.Generic;
using System.Text;

namespace intro.Models
{
    public sealed class RequestDeviceList
    {
        public RequestDeviceList(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }


}
