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
    public class StoragesModel : IStoragesModel
    {
        private readonly ILogger<StoragesModel> _logger;
        private readonly IMapper _mapper;
        private readonly StorageDbContext _context;

        public StoragesModel(ILogger<StoragesModel> logger, IMapper mapper, StorageDbContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public IQueryable<Contracts.Storage> GetStorages()
        {
            return _context.Storages
                .ProjectTo<Contracts.Storage>(_mapper.ConfigurationProvider);
        }

        public async Task<Contracts.Storage> GetStorage(int idParam)
        {
            using var ctx = new StorageDbContext();
            return _mapper.Map<Contracts.Storage>(await ctx.Storages
                .FirstOrDefaultAsync(x => x.Id == idParam));
        }

        public async Task<int> CreateStorage(string nameParam)
        {
            var entityToCreate = new Data.Entities.Storage
            {
                Name = nameParam
            };
            using var ctx = new StorageDbContext();
            ctx.Storages.Add(entityToCreate);
            await ctx.SaveChangesAsync();
            return entityToCreate.Id;
        }

        public async Task UpdateStorage(int idParam, string nameParam)
        {
            using var ctx = new StorageDbContext();
            var articleToUpdate = await ctx.Storages.FindAsync(idParam);
            articleToUpdate.Name = nameParam;
            ctx.Storages.Update(articleToUpdate);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteStorage(int idParam)
        {
            using var ctx = new StorageDbContext();
            var articleToDelete = await ctx.Storages.FindAsync(idParam);
            ctx.Storages.Remove(articleToDelete);
            await ctx.SaveChangesAsync();
        }

        public async Task<int> GetQuantity(int storeIdParam, int articleIdParam)
        {
            using var ctx = new StorageDbContext();
            return await ctx.StorageArticles
                .Where(x => x.StoreId == storeIdParam && x.ArticleId == articleIdParam)
                .Select(x => x.Quantity)
                .FirstOrDefaultAsync();
        }

        public async Task AddQuantity(int storeIdParam, int articleIdParam, int quantityToAddParam)
        {
            using var ctx = new StorageDbContext();
            await ctx.Database.ExecuteSqlInterpolatedAsync(
                $@"
insert into ""StorageArticles"" (""StoreId"", ""ArticleId"", ""Quantity"")
values ({storeIdParam}, {articleIdParam}, {quantityToAddParam})
on conflict (""StoreId"", ""ArticleId"") do update set ""Quantity"" = (""StorageArticles"".""Quantity"" + {quantityToAddParam});
"
                );
        }
        
        public async Task SetQuantity(int storeIdParam, int articleIdParam, int quantityToSetParam)
        {
            using var ctx = new StorageDbContext();
            await ctx.Database.ExecuteSqlInterpolatedAsync(
                $@"
insert into ""StorageArticles"" (""StoreId"", ""ArticleId"", ""Quantity"")
values ({storeIdParam}, {articleIdParam}, {quantityToSetParam})
on conflict (""StoreId"", ""ArticleId"") do update set ""Quantity"" = {quantityToSetParam};
"                
                );
        }
    }
}