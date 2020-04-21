using AutoMapper;
using Contracts = StorageApi.Contracts;
using Entities = StorageApi.Data.Entities;

namespace StorageApi.Configuration.Mapping
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Entities.Article, Contracts.Article>();
            CreateMap<Entities.Storage, Contracts.Storage>();
        }
    }
}