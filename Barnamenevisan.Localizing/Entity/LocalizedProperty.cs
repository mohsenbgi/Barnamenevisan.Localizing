using Barnamenevisan.Localizing.Shared;

namespace Barnamenevisan.Localizing.Entity
{
    public class LocalizedProperty : BaseEntity<int>
    {
        public object EntityId { get; set; }

        public string EntityName { get; set; }

        public string CultureName { get; set; }

        public string Key { get; set; }

        public string? Value { get; set; }
    }
}
