using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models
{
    public class IncludeField
    {
        public string Name { get; set; }
        public List<string> Filters { get; set; }

        public bool HasNameAndFilter(string name, string filter = null) =>
            Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
            (string.IsNullOrEmpty(filter) || Filters.Any(f => f.Equals(filter, StringComparison.InvariantCultureIgnoreCase)));

        public static List<IncludeField> ParseList(string input)
        {
            var output = new List<IncludeField>();

            if (string.IsNullOrEmpty(input))
            {
                return output;
            }

            var tokens = input.Split(',');

            if (tokens.Length == 1 && tokens[0] == "")
            {
                return output;
            }

            foreach (var token in tokens)
            {
                try
                {
                    var pieces = token.Split(':');
                    output.Add(new IncludeField()
                    {
                        Name = pieces[0],
                        Filters = pieces.Length > 1 ? pieces.Skip(1).ToList() : new List<string>()
                    });
                }
                catch (Exception)
                {
                    break;
                }
            }

            return output;
        }
    }
}
