## Script para la creación de un cluster de HDInsight 

## Definición de Parametros

$subscriptionName = "Plain Concepts Labs - Data"
$storageAccountName = "pcgwab2015storage"
$containerName = "pcgwab2015hdi01"

$clusterName = "pcgwab2015hdi01"
$location = "North Europe"
$clusterNodes = 4
$clusterVersion = "3.2"
$clusterUser = "admin"
$clusterPassword = "abcDEF!123"

$sqlServer = "pcgwab2015.database.windows.net"
$sqlServerShort = "pcgwab2015"
$sqlDatabase = "GWAB2015metastore"
$sqlUser = "Plain"
$sqlPassword = "abcDEF!123"

## Obtenemos la cuenta de storage
Select-AzureSubscription $subscriptionName
$storageAccountKey = Get-AzureStorageKey $storageAccountName | %{ $_.Primary }

## Configuración Personalizada de Hive
$configvalues = new-object 'Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects.AzureHDInsightHiveConfiguration'
$configvalues.Configuration = @{ “hive.auto.convert.join”=”true”} 
$configvalues.Configuration.Add( "hive.metastore.client.socket.timeout", "1800")
$configvalues.Configuration.Add( "hive.execution.engine", "tez")

## Configuración Personalizada de MapReduce
$mapredconfigvalues = new-object 'Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects.AzureHDInsightMapReduceConfiguration'
$mapredconfigvalues.Configuration = @{ “hive.auto.convert.join”=”true”} 

## Configuración Personalizada de YARN
$yarnConfigValues = @{"yarn.nodemanager.resource.memory-mb"="6144";"yarn.scheduler.minimum-allocation-mb"="2048";}

## Obtenemos Credenciales desde los literales de las contraseñas
$clusterSecurePasswd = ConvertTo-SecureString $clusterPassword -AsPlainText -Force
$clusterCreds = New-Object System.Management.Automation.PSCredential ($clusterUser, $clusterSecurePasswd)

$credSecureSqlPasswd = ConvertTo-SecureString $sqlPassword -AsPlainText -Force
$credsSql = New-Object System.Management.Automation.PSCredential ($sqlUser, $credSecureSqlPasswd)

## Recreamos el metastore si ya existe
$ctx = New-AzureSqlDatabaseServerContext  -ServerName $sqlServerShort -Credential $credsSql
$PerformanceLevel = Get-AzureSqlDatabaseServiceObjective -Context $ctx -ServiceObjectiveName "S1"
$dblist = Get-AzureSqlDatabase $ctx
foreach ($db in $dblist) { if ($db.Name -eq $sqlDatabase) { Remove-AzureSqlDatabase $ctx –DatabaseName  $sqlDatabase } }
New-AzureSqlDatabase $ctx –DatabaseName $sqlDatabase -Edition Standard -ServiceObjective $PerformanceLevel -MaxSizeGB 10

## Eliminamos el cluster si existe
$clusterList = Get-AzureHDInsightCluster
foreach ($cluster in $clusterList) { if ($cluster.Name -eq $clusterName) { Remove-AzureHDInsightCluster -Name $clusterName } }

## Creación del Cluster
New-AzureHDInsightClusterConfig -ClusterSizeInNodes $clusterNodes | 
Set-AzureHDInsightDefaultStorage –StorageAccountName “$storageAccountName.blob.core.windows.net”  -StorageAccountKey $storageAccountKey -StorageContainerName $containerName |
Add-AzureHDInsightMetastore -SqlAzureServerName $sqlServer -DatabaseName $sqlDatabase -Credential $credsSql -MetastoreType HiveMetastore |
Add-AzureHDInsightConfigValues -Hive $configvalues | 
Add-AzureHDInsightConfigValues -Yarn $yarnConfigValues |
Add-AzureHDInsightConfigValues -MapReduce $mapredconfigvalues |
New-AzureHDInsightCluster -Debug -Name $clusterName -Location $location -Credential $clusterCreds -Version $version
