namespace GWAB2015.NewCluster
{
    using Microsoft.WindowsAzure.Management.HDInsight;
    using System.Collections.Generic;

    /// <summary>
    /// Hive helper interfaz
    /// </summary>
    public interface IClusterHelper
    {
        /// <summary>
        /// Create a hadoop cluster
        /// </summary>
        /// <param name="clusterInfo">cluster creation info</param>
        /// <param name="configAdminAccess">true if we want to enable admin access for all nodes in the new cluster</param>
        /// <param name="adminUser">the admin username</param>
        /// <param name="adminPassword">the admin password</param>
        /// <returns>the cluster details</returns>
        ClusterDetails CreateCluster(ClusterCreateParametersV2 clusterInfo, bool configAdminAccess = false, string adminUser = "", string adminPassword = "");

        /// <summary>
        /// Create a hadoop cluster
        /// </summary>
        /// <param name="clusterInfo">cluster creation info</param>        
        /// <param name="configAdminAccess">true if we want to enable admin access for all nodes in the new cluster</param>
        /// <param name="adminUser">the admin username</param>
        /// <param name="adminPassword">the admin password</param>
        /// <returns>the cluster details</returns>
        ClusterDetails CreateCluster(string jsonClusterInfo, bool configAdminAccess = false, string adminUser = "", string adminPassword = "");

        /// <summary>
        /// Deletion of a HDInsight cluster
        /// </summary>
        /// <param name="clusterName">cluster name</param>
        void DeleteCluster(string clusterName);

        /// <summary>
        /// Get the requested cluster
        /// </summary>
        /// <returns></returns>
        ClusterDetails GetCluster(string clusterName);

        /// <summary>
        /// Get a list with the available hadoop clusters.
        /// </summary>
        /// <returns></returns>
        List<ClusterDetails> ListClusters();
    }
}
