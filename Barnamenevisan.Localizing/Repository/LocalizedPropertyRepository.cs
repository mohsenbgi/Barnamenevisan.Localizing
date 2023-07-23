using Barnamenevisan.Localizing.Entity;
using Microsoft.EntityFrameworkCore;

namespace Barnamenevisan.Localizing.Repository
{
    public class LocalizedPropertyRepository : EfRepository<LocalizedProperty, int>, ILocalizedPropertyRepository
    {
        public LocalizedPropertyRepository(DbContext context) : base(context)
        {
        }
    }
}
