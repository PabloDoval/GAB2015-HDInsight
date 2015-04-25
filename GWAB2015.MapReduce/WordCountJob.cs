using Microsoft.Hadoop.MapReduce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWAB2015.MapReduce
{
    public class WordCountJob : HadoopJob<WordCountMapper, WordCountReducer>
    {
        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            var config = new HadoopJobConfiguration();
            config.InputPath = "input/CodeFiles";
            config.OutputFolder = "output/CodeFiles";
            return config;
        }
    } 

}
