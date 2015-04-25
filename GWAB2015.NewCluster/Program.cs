using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.Management.HDInsight;
using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;

namespace GWAB2015.NewCluster
{
    class Program
    {
        static void Main(string[] args)
        {
            ClusterHelper helper = new ClusterHelper();
            string clusterName = "pcgwab2015hdi02";

            var clusterScript = System.IO.File.ReadAllText("ClusterCreationScript.json").Replace("CLUSTER_NAME_PLACEHOLDER", clusterName);
            helper.CreateCluster(clusterScript, false, "admin", "Melocoton.123");

            List<ClusterDetails> clusters = helper.ListClusters();

            if (clusters.Any(c => c.Name == clusterName))
            {
                helper.DeleteCluster(clusterName);
            }
        }
    }
}
