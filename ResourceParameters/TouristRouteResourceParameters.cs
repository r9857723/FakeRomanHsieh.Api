using System;
using System.Text.RegularExpressions;

namespace FakeRomanHsieh.API.ResourceParameters
{
    public class TouristRouteResourceParameters
    {
        public String OrderBy { get; set; }
        public String Keyword { get; set; }
        public String RatingOperator { get; set; }
        public int? RatingValue { get; set; }

        private String _rating;
        public String Rating {
            get {
                return _rating;
            }
            set {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }
                }
                _rating = value;
            }
        }

        public string Fields { get; set; }
    }
}
