namespace HBase.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.HBase.Client;
    using org.apache.hadoop.hbase.rest.protobuf.generated;


    class Program
    {
        static void Main(string[] args)
        {
            string clusterURL = "https://tekhbase.azurehdinsight.net/";
            string hadoopUsername = "plain";
            string hadoopUserPassword = "abcDEF!123";

            string hbaseTableName = "temazos";

            ClusterCredentials creds = new ClusterCredentials(new Uri(clusterURL), hadoopUsername, hadoopUserPassword);
            HBaseClient hbaseClient = new HBaseClient(creds);

            var version = hbaseClient.GetVersion();
            Console.WriteLine("La version de este cluster de HBase es: " + version);

            TableSchema testTableSchema = new TableSchema();
            testTableSchema.name = hbaseTableName;
            testTableSchema.columns.Add(new ColumnSchema() { name = "d" });
            testTableSchema.columns.Add(new ColumnSchema() { name = "f" });
            hbaseClient.CreateTable(testTableSchema);

            string testKey = "rhyme";
            string testValue = "The rhyme of the ancient mariner... (up the Irons!)";
            CellSet cellSet = new CellSet();
            CellSet.Row cellSetRow = new CellSet.Row { key = Encoding.UTF8.GetBytes(testKey) };
            cellSet.rows.Add(cellSetRow);

            Cell value = new Cell { column = Encoding.UTF8.GetBytes("d:therhyme"), data = Encoding.UTF8.GetBytes(testValue) };
            cellSetRow.values.Add(value);
            hbaseClient.StoreCells(hbaseTableName, cellSet);

            cellSet = hbaseClient.GetCells(hbaseTableName, testKey);
            Console.WriteLine("La entrada con la clave '" + testKey + "' es: " + Encoding.UTF8.GetString(cellSet.rows[0].values[0].data));

            Scanner scanSettings = new Scanner()
            {
                batch = 10,
                startRow = BitConverter.GetBytes(25),
                endRow = BitConverter.GetBytes(35)
            };

            ScannerInformation scannerInfo = hbaseClient.CreateScanner(hbaseTableName, scanSettings);
            CellSet next = null;
            Console.WriteLine("Resultados del scan:");

            while ((next = hbaseClient.ScannerGetNext(scannerInfo)) != null)
            {
                foreach (CellSet.Row row in next.rows)
                {
                    Console.WriteLine(row.key + " : " + Encoding.UTF8.GetString(row.values[0].data));
                }
            }

              Console.ReadLine();
        }
    }
}
