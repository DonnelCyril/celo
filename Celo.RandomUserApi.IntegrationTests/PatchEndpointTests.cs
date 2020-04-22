using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;
using Celo.RandomUserApi.IntegrationTests.Stubs;
using Celo.RandomUserApi.UserManagement.Model.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Celo.RandomUserApi.IntegrationTests
{
    [TestClass]
    public class PatchEndpointTests
    {
        private TestServer _server;
        private HttpClient _client;
        private readonly string _userRecordEtag = $"\"{Guid.NewGuid().ToString()}\"";

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IUserStoreFactory>(_ => new StubUserStoreFactory(
                        new UserRecord(HttpStatusCode.OK, _userRecordEtag, new User(
                            Guid.NewGuid(),
                            new UserName("Master", "Aaron", "Donnel"),
                            new ContactInfo("test@test.com", "04123412"),
                            DateTime.Now.AddYears(-5),
                            new List<Uri>()
                            ))
                    ));
                })
            );
            _client = _server.CreateClient();
        }

        [TestMethod]
        public async Task An_etag_for_the_user_record_is_required_before_it_can_be_updated()
        {
            var requestMessage = GetPatchRequest();
            var response = await _client.SendAsync(requestMessage);
            Assert.AreEqual(HttpStatusCode.PreconditionRequired, response.StatusCode);
        }

        [TestMethod]
        public async Task Can_update_a_user_record()
        {
            var requestMessage = GetPatchRequest();
            requestMessage.Headers.Add(HeaderNames.IfMatch, _userRecordEtag);
            var response = await _client.SendAsync(requestMessage);
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        private static HttpRequestMessage GetPatchRequest()
        {
            var doc = new JsonPatchDocument<UserDto>
            {
                Operations = {new Operation<UserDto>("add", "/name/firstName", null, "Donnel")}
            };
            var json = JsonConvert.SerializeObject(doc);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/api/v1/users/c679cb6d-7d16-4599-950f-21560b316ecd")
            {
                Content = content
            };
            return requestMessage;
        }
    }
}
