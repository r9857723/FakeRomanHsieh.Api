using System;
namespace FakeRomanHsieh.API.Dtos
{
    public class LinkDto
    {
        public String Href { get; set; }
        public String Rel { get; set; }
        public String Method { get; set; }

        public LinkDto(String href, String rel, String method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
