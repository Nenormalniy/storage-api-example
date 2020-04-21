using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using StorageApi.Data;

namespace StorageApi.Interfaces.Models
{
    public interface IArticlesModel
    {
        IQueryable<Contracts.Article> GetArticles();
        Task<Contracts.Article> GetArticle(int idParam);
        Task<int> CreateArticle(string nameParam);
        Task UpdateArticle(int idParam, string nameParam);
        Task DeleteArticle(int idParam);
    }
}