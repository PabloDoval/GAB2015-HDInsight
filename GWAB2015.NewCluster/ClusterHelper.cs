namespace GWAB2015.NewCluster
{
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;

    public class ClusterHelper : GWAB2015.NewCluster.IClusterHelper
    {
        #region Constructor

        /// <summary>
        /// Constructor for the Cluster Helper class.
        /// </summary>
        public ClusterHelper()
        {
        }

        #endregion

        #region Cluster Methods

        /// <summary>
        /// Create a hadoop cluster
        /// </summary>
        /// <param name="clusterInfo">cluster creation info</param>
        /// <param name="configAdminAccess">true if we want to enable admin access for all nodes in the new cluster</param>
        /// <param name="adminUser">the admin username</param>
        /// <param name="adminPassword">the admin password</param>
        /// <returns>the cluster details</returns>
        public ClusterDetails CreateCluster(ClusterCreateParametersV2 clusterInfo, bool configAdminAccess = false, string adminUser = "", string adminPass = "")
        {
            var client = getClusterClient();

            if (configAdminAccess && !string.IsNullOrWhiteSpace(adminUser) && !string.IsNullOrWhiteSpace(adminPass))
            {
                clusterInfo.ConfigActions.Add(new ScriptAction(
                  "AdminAccessActionScript", // Name of the config action
                  new ClusterNodeType[] { ClusterNodeType.HeadNode, ClusterNodeType.DataNode },
                  new Uri(ConfigurationManager.AppSettings["AdminScriptLocation"]), 
                  string.Format("-adminUsername {0} -adminPassword {1}", adminUser, adminPass) 
                ));

                Console.WriteLine("Incluyendo acceso de admin en los HeadNode y DataNodes");
            }

            ClusterDetails clusterDetails = client.CreateCluster(clusterInfo);

            return clusterDetails;
        }

        private IHDInsightClient getClusterClient()
        {
            IHDInsightClient result = null;
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                 X509FindType.FindByIssuerName,
                                 "PabloDovalAzure",
                                 false);

            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];

                var creds = new HDInsightCertificateCredential(new Guid(ConfigurationManager.AppSettings["hadoopSubscription"]), cert);
                result = HDInsightClient.Connect(creds);
            }
            certStore.Close();

            return result;
        }

        /// <summary>
        /// Create a hadoop cluster
        /// </summary>
        /// <param name="clusterInfo">cluster creation info</param>
        /// <param name="configAdminAccess">true if we want to enable admin access for all nodes in the new cluster</param>
        /// <param name="adminUser">the admin username</param>
        /// <param name="adminPassword">the admin password</param>
        /// <returns>the cluster details</returns>
        public ClusterDetails CreateCluster(string jsonClusterInfo, bool configAdminAccess = false, string adminUser = "", string adminPass = "")
        {
            ClusterCreateParametersV2 parameters = JsonConvert.DeserializeObject<ClusterCreateParametersV2>(jsonClusterInfo);

            return CreateCluster(parameters, configAdminAccess, adminUser, adminPass);
        }

        /// <summary>
        /// Deletion of a HDInsight cluster
        /// </summary>
        /// <param name="clusterName">cluster name</param>
        public void DeleteCluster(string clusterName)
        {
            var client = getClusterClient();

            var clusters = client.ListClusters();

            if (clusters.Any(c => c.Name == clusterName))
            {
                client.DeleteCluster(clusterName);
            }
        }

        /// <summary>
        /// Get the requested cluster
        /// </summary>
        /// <returns></returns>
        public ClusterDetails GetCluster(string clusterName)
        {
            var client = getClusterClient();
            return client.ListClusters()
                         .Where(c => c.Name == clusterName)
                         .FirstOrDefault();
        }

        /// <summary>
        /// Get a list with the available hadoop clusters.
        /// </summary>
        /// <returns></returns>
        public List<ClusterDetails> ListClusters()
        {
            var client = getClusterClient();

            return client.ListClusters().ToList();
        }

        #endregion
    }
}
