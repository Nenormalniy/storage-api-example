using System;
using System.Collections.Generic;
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
    public class StoragesTests : IClassFixture<StorageApiWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public StoragesTests(StorageApiWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task CanCreateAndRetrieveStorage()
        {
            var storageName = $"Storage {Guid.NewGuid()}";
            var postJson = $"{{\"name\": \"{storageName}\"}}";
            var postResponse = await _client.PostAsync($"/storages?name={storageName}", new StringContent(postJson, Encoding.UTF8, "application/json"));
            var responseLocation = postResponse.Headers.Location.ToString();
            
            Assert.True(!string.IsNullOrEmpty(responseLocation), "New item location missing");
            
            var getItemResponse = await _client.GetAsync(responseLocation);
            
            getItemResponse.EnsureSuccessStatusCode();
            var stringResponse = await getItemResponse.Content.ReadAsStringAsync();
            var articleItem = JsonConvert.DeserializeObject<Storage>(stringResponse);
            Assert.NotNull(articleItem);
            Assert.True(articleItem.Name == storageName, "Inconsistent name");
        }
        
        [Fact]
        public async Task CanDeleteStorage()
        {
            var storageName = $"Storage {Guid.NewGuid()}";
            var postJson = $"{{\"name\": \"{storageName}\"}}";
            var postResponse = await _client.PostAsync($"/storages?name={storageName}", new StringContent(postJson, Encoding.UTF8, "application/json"));
            var responseLocation = postResponse.Headers.Location.ToString();
            
            Assert.True(!string.IsNullOrEmpty(responseLocation), "New item location missing");
            
            var getItemResponse = await _client.GetAsync(responseLocation);
            
            getItemResponse.EnsureSuccessStatusCode();
            var stringResponse = await getItemResponse.Content.ReadAsStringAsync();
            var articleItem = JsonConvert.DeserializeObject<Storage>(stringResponse);
            Assert.NotNull(articleItem);
            Assert.True(articleItem.Name == storageName, "Inconsistent name");
            
            await _client.DeleteAsync(responseLocation);
            Assert.True(getItemResponse.StatusCode == HttpStatusCode.OK, "Item not deleted or deleted with error");
            getItemResponse = await _client.GetAsync(responseLocation);
            Assert.True(getItemResponse.StatusCode != HttpStatusCode.OK, "Item not deleted");
        }
        
        [Fact]
        public async Task CanCreateArticleAndStorageAndModifyQuantity()
        {
            var storageName = $"Storage {Guid.NewGuid()}";
            var articleName = $"Article {Guid.NewGuid()}";
            
            var postStorageResponse = await _client.PostAsync($"/storages?name={storageName}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            var postArticleResponse = await _client.PostAsync($"/articles?name={articleName}", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            
            var responseStorageLocation = postStorageResponse.Headers.Location.ToString();
            var responseArticleLocation = postArticleResponse.Headers.Location.ToString();
            
            var getStorageItemResponse = await _client.GetAsync(responseStorageLocation);
            var getArticleItemResponse = await _client.GetAsync(responseArticleLocation);
            
            getStorageItemResponse.EnsureSuccessStatusCode();
            getArticleItemResponse.EnsureSuccessStatusCode();
            
            var storageItem = JsonConvert.DeserializeObject<Storage>(await getStorageItemResponse.Content.ReadAsStringAsync());
            var articleItem = JsonConvert.DeserializeObject<Storage>(await getArticleItemResponse.Content.ReadAsStringAsync());
            
            // {storageId:int}/articles/{articleId:int}/quantity
            var currentQuantity = -1;
            var currentQuantityResponse = await _client.GetAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity");
            
            currentQuantity = int.Parse(await currentQuantityResponse.Content.ReadAsStringAsync());
            Assert.Equal(0, currentQuantity);

            var quantityToAdd = 4;
            await _client.PutAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity/{quantityToAdd}", 
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            
            currentQuantityResponse = await _client.GetAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity");
            
            currentQuantity = int.Parse(await currentQuantityResponse.Content.ReadAsStringAsync());
            Assert.Equal(quantityToAdd, currentQuantity);

            var quantityToDivide = 1;
            await _client.DeleteAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity/{quantityToDivide}");
            
            currentQuantityResponse = await _client.GetAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity");
            
            currentQuantity = int.Parse(await currentQuantityResponse.Content.ReadAsStringAsync());
            Assert.Equal(quantityToAdd - quantityToDivide, currentQuantity);

            var quantityToSet = 17556;
            await _client.PatchAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity/{quantityToSet}", 
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            
            currentQuantityResponse = await _client.GetAsync($"/storages/{storageItem.Id}/articles/{articleItem.Id}/quantity");
            
            currentQuantity = int.Parse(await currentQuantityResponse.Content.ReadAsStringAsync());
            Assert.Equal(quantityToSet, currentQuantity);
        }
    }
}