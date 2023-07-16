using Barnamenevisan.Localizing.Entity;
using Microsoft.EntityFrameworkCore;

namespace Barnamenevisan.Localizing.Repository
{
    public class LocalizedPropertyRepository : EfRepository<LocalizedProperty, ulong>, ILocalizedPropertyRepository
    {
        public LocalizedPropertyRepository(DbContext context) : base(context)
        {
        }
    }
}
