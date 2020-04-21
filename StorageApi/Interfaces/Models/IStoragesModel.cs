using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using StorageApi.Data;

namespace StorageApi.Interfaces.Models
{
    public interface IStoragesModel
    {
        IQueryable<Contracts.Storage> GetStorages();
        Task<Contracts.Storage> GetStorage(int idParam);
        Task<int> CreateStorage(string nameParam);
        Task UpdateStorage(int idParam, string nameParam);
        Task DeleteStorage(int idParam);
        Task<int> GetQuantity(int storeIdParam, int articleIdParam);
        Task AddQuantity(int storeIdParam, int articleIdParam, int quantityToAddParam);
        Task SetQuantity(int storeIdParam, int articleIdParam, int quantityToSetParam);
    }
}