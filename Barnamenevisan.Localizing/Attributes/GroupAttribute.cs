namespace Barnamenevisan.Localizing.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GroupAttribute : Attribute
    {
        public string GroupName { get; set; }

        public GroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
