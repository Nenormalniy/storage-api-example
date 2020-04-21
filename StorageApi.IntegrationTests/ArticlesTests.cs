using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StorageApi.Contracts;
using StorageApi.IntegrationTests.Factories;
using Xunit;

namespace StorageApi.IntegrationTests
{
    public class ArticlesTests : IClassFixture<StorageApiWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ArticlesTests(StorageApiWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task CanCreateAndRetrieveArticle()
        {
            var articleName = $"Article {Guid.NewGuid()}";
            var postJson = $"{{\"name\": \"{articleName}\"}}";
            var postResponse = await _client.PostAsync($"/articles?name={articleName}", new StringContent(postJson, Encoding.UTF8, "application/json"));
            var responseLocation = postResponse.Headers.Location.ToString();
            
            Assert.True(!string.IsNullOrEmpty(responseLocation), "New item location missing");
            
            var getItemResponse = await _client.GetAsync(responseLocation);
            
            getItemResponse.EnsureSuccessStatusCode();
            var stringResponse = await getItemResponse.Content.ReadAsStringAsync();
            var articleItem = JsonConvert.DeserializeObject<Article>(stringResponse);
            Assert.NotNull(articleItem);
            Assert.True(articleItem.Name == articleName, "Inconsistent name");
        }
        
        [Fact]
        public async Task CanCreateAndRetrieveArticlesWithOData()
        {
            await _client.PostAsync($"/articles?name=Article {Guid.NewGuid()}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            await _client.PostAsync($"/articles?name=Article {Guid.NewGuid()}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            await _client.PostAsync($"/articles?name=Article {Guid.NewGuid()}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            await _client.PostAsync($"/articles?name=Article {Guid.NewGuid()}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            await _client.PostAsync($"/articles?name=Article {Guid.NewGuid()}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            
            var getItemsResponse = await _client.GetAsync("/articles?$select=id");
            
            getItemsResponse.EnsureSuccessStatusCode();
            var stringResponse = await getItemsResponse.Content.ReadAsStringAsync();
            var articleItems = JsonConvert.DeserializeObject<IEnumerable<Article>>(stringResponse).ToList();
            Assert.NotEmpty(articleItems);
            Assert.True(articleItems.All(x => x.Name == null), "OData query filtering not working");
        }
        
        [Fact]
        public async Task CanDeleteArticle()
        {
            var articleName = $"Article {Guid.NewGuid()}";
            var postJson = $"{{\"name\": \"{articleName}\"}}";
            var postResponse = await _client.PostAsync($"/articles?name={articleName}", new StringContent(postJson, Encoding.UTF8, "application/json"));
            var responseLocation = postResponse.Headers.Location.ToString();
            
            Assert.True(!string.IsNullOrEmpty(responseLocation), "New item location missing");
            
            var getItemResponse = await _client.GetAsync(responseLocation);
            
            getItemResponse.EnsureSuccessStatusCode();
            var stringResponse = await getItemResponse.Content.ReadAsStringAsync();
            var articleItem = JsonConvert.DeserializeObject<Article>(stringResponse);
            Assert.NotNull(articleItem);
            Assert.True(articleItem.Name == articleName, "Inconsistent name");
            
            var deleteItemResponse = await _client.DeleteAsync(responseLocation);
            getItemResponse = await _client.GetAsync(responseLocation);
            Assert.True(getItemResponse.StatusCode != HttpStatusCode.OK, "Item not deleted");
        }
    }
}