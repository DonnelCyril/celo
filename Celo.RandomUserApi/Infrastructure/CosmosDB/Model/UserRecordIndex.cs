using System;
using System.Collections.Generic;

namespace Celo.RandomUserApi.Infrastructure.CosmosDB.Model
{
    public class UserRecordIndex
    {
        public UserRecordIndex(IEnumerable<Guid> userIds, string nextOffset)
        {
            UserIds = userIds ?? new List<Guid>();
            NextOffset = nextOffset ?? string.Empty;
        }

        public IEnumerable<Guid> UserIds { get; }
        public string NextOffset { get; }
    }
}