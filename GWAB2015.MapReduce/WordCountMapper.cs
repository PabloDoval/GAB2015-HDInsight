using Microsoft.Hadoop.MapReduce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GWAB2015.MapReduce
{
    public class WordCountMapper : MapperBase
    {
        public override void Map(string inputLine, MapperContext context)
        {

            var reg = new Regex(@"[A-za-z0-9_\.]*\;");
            var matches = reg.Matches(inputLine);

            foreach (Match match in matches)
            {
                context.EmitKeyValue(match.Value, "1");
            }
        }
    } 
 

}
