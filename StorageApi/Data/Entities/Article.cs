using System.ComponentModel.DataAnnotations;

namespace StorageApi.Data.Entities
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}