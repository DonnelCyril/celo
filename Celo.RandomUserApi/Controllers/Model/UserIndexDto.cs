using System;
using System.Collections.Generic;

namespace Celo.RandomUserApi.Controllers.Model
{
    public class UserIndexDto
    {
        public UserIndexDto(IEnumerable<Guid> userIds, string nextIndex, string previousIndex)
        {
            UserIds = userIds;
            NextPage = ! string.IsNullOrWhiteSpace(nextIndex) ? new Uri($"/page?{nextIndex}") : null;
            PreviousPage = ! string.IsNullOrWhiteSpace(nextIndex) ? new Uri($"/page?{previousIndex}") : null;
        }


        public IEnumerable<Guid> UserIds { get; set; }

        public Uri NextPage { get; }

        public Uri PreviousPage { get; set; }
    }
}
