using System;
using System.Collections.Generic;

namespace Celo.RandomUserApi.UserManagement.Model.GetUserList
{
    public class UserIndex
    {
        public UserIndex(IEnumerable<Guid> userIds, string nextIndex, string previousIndex)
        {
            UserIds = userIds;
            NextIndex = nextIndex;
            PreviousIndex = previousIndex;
        }

        public IEnumerable<Guid> UserIds { get; }

        public string NextIndex { get; }

        public string PreviousIndex { get; }
    }
}
