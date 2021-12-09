using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Contracts
{
    public record OrderSubmissionRejected
    {
        public string OrderId { get; init; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
    }
}