using System;

namespace StorageApi.Data.Entities
{
    public class StorageArticle
    {
        public Storage Store { get; set; }
        public Article Article { get; set; }
        public int StoreId { get; set; }
        public int ArticleId { get; set; }
        public int Quantity { get; set; }
    }
}