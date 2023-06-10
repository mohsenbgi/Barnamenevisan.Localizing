namespace Barnamenevisan.Localizing.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TranslateAttribute : Attribute
    {
        public string PropertyNameOfEntityIdInThisClass { get; set; }

        public string EntityName { get; set; }

        public string Key { get; set; }

        public TranslateAttribute()
        {
            PropertyNameOfEntityIdInThisClass = "Id";
        }
    }
}
