using System.Web;

namespace Xania.AspNet.TagHelpers
{
    public class TagAttribute
    {
        private string _value;

        public TagAttribute()
        {
        }

        public TagAttribute(string name, string value)
        {
            Name = name;
            RawValue = HttpUtility.HtmlEncode(value);

            _value = value;
        }
        
        public string Name { get; set; }

        public string RawValue { get; set; }

        public string Value
        {
            get { return _value ?? (_value = HttpUtility.HtmlDecode(RawValue)); }
        }


    }
}