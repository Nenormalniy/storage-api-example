using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorageApi.Contracts;
using StorageApi.Data;
using StorageApi.Interfaces.Models;

namespace StorageApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesModel _model;

        public ArticlesController(IArticlesModel model)
        {
            _model = model;
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Article>> Get(int id)
        {
            var foundItem = await _model.GetArticle(id);
            if (foundItem == null)
            {
                return NotFound();
            }
            return foundItem;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Article> Get()
        {
            return _model.GetArticles();
        }

        [HttpPost]
        public async Task<ActionResult> Post(string name)
        {
            var createdId = await _model.CreateArticle(name);
            Response.Headers.Add("Location", $"/articles/{createdId}");
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, string? name)
        {
            await _model.UpdateArticle(id, name);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _model.DeleteArticle(id);
            return Ok();
        }
    }
}