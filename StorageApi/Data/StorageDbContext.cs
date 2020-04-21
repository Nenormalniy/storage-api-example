using Microsoft.EntityFrameworkCore;
using StorageApi.Data.Entities;

namespace StorageApi.Data
{
    public class StorageDbContext : DbContext
    {
        readonly string _connectionString;
        
        public StorageDbContext() : base()
        {
            _connectionString = Startup.ConnectionString;
        }

        public StorageDbContext (string connectionString) : base()
        {
            _connectionString = connectionString;
        }
        
        public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!string.IsNullOrEmpty(_connectionString))
                optionsBuilder.UseNpgsql(_connectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StorageArticle>().HasKey(table => new {
                table.StoreId, table.ArticleId
            });
        }
        
        public DbSet<Article> Articles { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageArticle> StorageArticles { get; set; }
    }
}