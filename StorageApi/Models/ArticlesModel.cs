using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorageApi.Data;
using StorageApi.Interfaces.Models;
using Contracts = StorageApi.Contracts;

namespace StorageApi.Models
{
    public class ArticlesModel : IArticlesModel
    {
        private readonly ILogger<ArticlesModel> _logger;
        private readonly IMapper _mapper;
        private readonly StorageDbContext _context;

        public ArticlesModel(ILogger<ArticlesModel> logger, IMapper mapper, StorageDbContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public IQueryable<Contracts.Article> GetArticles()
        {
            return _context.Articles
                .ProjectTo<Contracts.Article>(_mapper.ConfigurationProvider);
        }

        public async Task<Contracts.Article> GetArticle(int idParam)
        {
            using var ctx = new StorageDbContext();
            return _mapper.Map<Contracts.Article>(await ctx.Articles
                .FirstOrDefaultAsync(x => x.Id == idParam));
        }

        public async Task<int> CreateArticle(string nameParam)
        {
            var entityToCreate = new Data.Entities.Article
            {
                Name = nameParam
            };
            using var ctx = new StorageDbContext();
            ctx.Articles.Add(entityToCreate);
            await ctx.SaveChangesAsync();
            return entityToCreate.Id;
        }

        public async Task UpdateArticle(int idParam, string nameParam)
        {
            using var ctx = new StorageDbContext();
            var articleToUpdate = await ctx.Articles.FindAsync(idParam);
            articleToUpdate.Name = nameParam;
            ctx.Articles.Update(articleToUpdate);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteArticle(int idParam)
        {
            using var ctx = new StorageDbContext();
            var articleToDelete = await ctx.Articles.FindAsync(idParam);
            ctx.Articles.Remove(articleToDelete);
            await ctx.SaveChangesAsync();
        }
        
        
    }
}