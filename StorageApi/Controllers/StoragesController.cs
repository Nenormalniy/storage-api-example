using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorageApi.Contracts;
using StorageApi.Data.Entities;
using StorageApi.Interfaces.Models;
using Storage = StorageApi.Contracts.Storage;

namespace StorageApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoragesController : ControllerBase
    {
        
        private readonly IStoragesModel _model;

        public StoragesController(IStoragesModel model)
        {
            _model = model;
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Storage>> Get(int id)
        {
            var foundItem = await _model.GetStorage(id);
            if (foundItem == null)
            {
                return NotFound();
            }
            return foundItem;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Storage> Get()
        {
            return _model.GetStorages();
        }

        [HttpPost]
        public async Task<ActionResult> Post(string name)
        {
            var createdId = await _model.CreateStorage(name);
            Response.Headers.Add("Location", $"/storages/{createdId}");
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, string? name)
        {
            await _model.UpdateStorage(id, name);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _model.DeleteStorage(id);
            return Ok();
        }

        [HttpGet("{storageId:int}/articles/{articleId:int}/quantity")]
        public async Task<ActionResult<int>> GetStorageArticleQuantity(int storageId, int articleId)
        {
            return await _model.GetQuantity(storageId, articleId);
        }

        [HttpPut("{storageId:int}/articles/{articleId:int}/quantity/{quantity:int}")]
        public async Task<ActionResult> IncreaseStorageArticleQuantity(int storageId, int articleId, int quantity)
        {
            await _model.AddQuantity(storageId, articleId, quantity);
            return Ok();
        }

        [HttpPatch("{storageId:int}/articles/{articleId:int}/quantity/{quantity:int}")]
        public async Task<ActionResult> SetStorageArticleQuantity(int storageId, int articleId, int quantity)
        {
            await _model.SetQuantity(storageId, articleId, quantity);
            return Ok();
        }

        [HttpDelete("{storageId:int}/articles/{articleId:int}/quantity/{quantity:int}")]
        public async Task<ActionResult> DecreaseStorageArticleQuantity(int storageId, int articleId, int quantity)
        {
            await _model.AddQuantity(storageId, articleId, quantity * -1);
            return Ok();
        }
    }
}