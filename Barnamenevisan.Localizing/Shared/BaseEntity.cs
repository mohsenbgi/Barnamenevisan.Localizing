using System.ComponentModel.DataAnnotations;

namespace Barnamenevisan.Localizing.Shared
{
    public abstract class BaseEntity<T> where T : IEquatable<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
