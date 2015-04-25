using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;

namespace WordCount
{
    [Active(true)]
    class Program : TopologyDescriptor
    {
        static void Main(string[] args)
        {
        }

        public ITopologyBuilder GetTopologyBuilder()
        {
            TopologyBuilder topologyBuilder = new TopologyBuilder("WordCount");

            topologyBuilder.SetSpout("sentences",
                                     Spout.Get,
                                     new Dictionary<string, List<string>>()
                                        {
                                            {Constants.DEFAULT_STREAM_ID, new List<string>(){"sentence"}}
                                    }, 
                                    1);
            
            
            topologyBuilder.SetBolt("splitter",
                                    Splitter.Get,
                                    new Dictionary<string, List<string>>()
                                    {
                                        {Constants.DEFAULT_STREAM_ID, new List<string>(){"word"}}
                                    },
                                    1).shuffleGrouping("sentences");

            topologyBuilder.SetBolt("counter",
                                    Counter.Get,
                                    new Dictionary<string, List<string>>()
                                    {
                                        {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                                    },
                                    1).fieldsGrouping("splitter", new List<int>() { 0 });

            
            topologyBuilder.SetTopologyConfig(new Dictionary<string, string>()
                                             {
                                                {"topology.kryo.register","[\"[B\"]"}
                                             });

            return topologyBuilder;
        }
    }
}

