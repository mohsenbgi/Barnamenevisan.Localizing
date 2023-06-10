using Barnamenevisan.Localizing.Entity;
using Microsoft.EntityFrameworkCore;

namespace Barnamenevisan.Localizing.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterLocalizing(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocalizedProperty>()
                .HasKey(e => e.Id);
        }
    }
}
