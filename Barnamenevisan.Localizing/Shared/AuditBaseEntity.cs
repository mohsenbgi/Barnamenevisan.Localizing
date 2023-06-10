namespace Barnamenevisan.Localizing.Shared
{
    public abstract class AuditBaseEntity<T> : BaseEntity<T> where T : IEquatable<T>
    {
        public AuditBaseEntity()
        {
            CreatedDateOnUtc = DateTime.UtcNow;
            UpdatedDateOnUtc = DateTime.UtcNow;
        }


        public DateTime CreatedDateOnUtc { get; set; }

        public DateTime UpdatedDateOnUtc { get; set; }
    }
}
