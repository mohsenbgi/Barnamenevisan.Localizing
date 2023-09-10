using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Barnamenevisan.Localizing.Shared
{
    public abstract class BaseEntity<T> where T : IEquatable<T>
    {
        [Key]
        public T Id { get; set; }

        public bool IsDeleted { get; set; }

        public TEntity DeepCopy<TEntity>() where TEntity : class
        {
            return JsonConvert.DeserializeObject<TEntity>(
                JsonConvert.SerializeObject(this, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            );
        }
    }
}
