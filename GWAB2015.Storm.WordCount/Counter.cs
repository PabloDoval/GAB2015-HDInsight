using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;

namespace WordCount
{
    public class Counter : ISCPBolt
    {
        private Context ctx;

        private Dictionary<string, int> counts = new Dictionary<string, int>();

        public Counter(Context ctx)
        {
            Context.Logger.Info("Counter constructor called");
            this.ctx = ctx;

            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            inputSchema.Add("default", new List<Type>() { typeof(string) });

            Dictionary<string, List<Type>> outputSchema = new Dictionary<string, List<Type>>();
            outputSchema.Add("default", new List<Type>() { typeof(string), typeof(int) });
         
            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, outputSchema));
        }

        public static Counter Get(Context ctx, Dictionary<string, Object> parms)
        {
            return new Counter(ctx);
        }

        public void Execute(SCPTuple tuple)
        {
            Context.Logger.Info("Execute enter");

            string word = tuple.GetString(0);
            
            int count = counts.ContainsKey(word) ? counts[word] : 0;
            
            count++;
            
            counts[word] = count;

            Context.Logger.Info("Emit: {0}, count: {1}", word, count);
            
            this.ctx.Emit(Constants.DEFAULT_STREAM_ID, new List<SCPTuple> { tuple }, new Values(word, count));

            Context.Logger.Info("Execute exit");
        }
    }
}