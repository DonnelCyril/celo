using System;
using System.Collections.Generic;
using Celo.RandomUserApi.Infrastructure;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using Newtonsoft.Json;

namespace Celo.RandomUserApi.UserManagement.Model.User
{
    public class User
    {
        public User(Guid userId, UserName name, ContactInfo contactInfo, DateTime dateOfBirth, IEnumerable<Uri> profileImages)
        {
            name.ThrowIfNull(nameof(name));
            contactInfo.ThrowIfNull(nameof(contactInfo));
            profileImages.ThrowIfNull(nameof(profileImages));

            UserId = userId;
            Name = name;
            ContactInfo = contactInfo;
            DateOfBirth = dateOfBirth;
            ProfileImages = profileImages;
        }

        [JsonProperty("id")]
        public Guid UserId { get; }
        public UserName Name { get; }
        public ContactInfo ContactInfo { get; }
        public DateTime DateOfBirth { get; }
        public IEnumerable<Uri> ProfileImages { get; }

        [JsonProperty(PropertyName = "partitionkey")]
        private string PartitionKey { get; set; } = Constants.UserRecords.PartitionKey;

    }
}