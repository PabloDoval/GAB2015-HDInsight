using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;

namespace WordCount
{
    public class Spout : ISCPSpout
    {
        private Context ctx;
        
        private Random r = new Random();

        string[] sentences = new string[] 
        {
            "The rhyme of the ancient mariner",
            "The loneliness of the long distance runner",
            "Seventh son of a seventh son",
            "Don't look to the eyes of a Stanger",
            "The evil that men do"
        };

        public Spout(Context ctx)
        {
            this.ctx = ctx;

            Context.Logger.Info("Generator constructor called");

            
            Dictionary<string, List<Type>> outputSchema = new Dictionary<string, List<Type>>();
            outputSchema.Add("default", new List<Type>() { typeof(string) });
            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(null, outputSchema));
        }

        public static Spout Get(Context ctx, Dictionary<string, Object> parms)
        {
            return new Spout(ctx);
        }

        public void NextTuple(Dictionary<string, Object> parms)
        {
            Context.Logger.Info("NextTuple enter");
            
            string sentence;

            
            sentence = sentences[r.Next(0, sentences.Length - 1)];
            Context.Logger.Info("Emit: {0}", sentence);
            
            
            this.ctx.Emit(new Values(sentence));

            Context.Logger.Info("NextTuple exit");
        }

        public void Ack(long seqId, Dictionary<string, Object> parms)
        {
            
        }

        public void Fail(long seqId, Dictionary<string, Object> parms)
        {
            
        }
    }
}