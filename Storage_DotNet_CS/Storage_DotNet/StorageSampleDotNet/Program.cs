using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using Microsoft.WindowsAzure.StorageClient.Tasks;
using System.Data.Services.Common;
using System.Data.Services.Client;

namespace StorageSampleDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBlobStorage();
            TestQueueStorage();
            TestTableStorage();

            Console.WriteLine("\nPress <Enter> to end");
            Console.Read();
        }

        // Perform blob storage operations.

        static void TestBlobStorage()
        {
            string storageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
            BlobHelper BlobHelper = new BlobHelper(storageConnectionString);

            try
            {
                Separator();

                List<CloudBlobContainer> containers;
                Console.Write("List containers ");
                if (BlobHelper.ListContainers(out containers))
                {
                    Console.WriteLine("true");
                    if (containers != null)
                    {
                        foreach (CloudBlobContainer container in containers)
                        {
                            Console.Write(container.Name + " ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Create container ");
                if (BlobHelper.CreateContainer("samplecontainer1"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Delete container ");
                if (BlobHelper.DeleteContainer("samplecontainer0"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Set container ACL ");
                if (BlobHelper.SetContainerACL("samplecontainer1", "container"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string accessLevel;

                Console.Write("Get container ACL ");
                if (BlobHelper.GetContainerACL("samplecontainer1", out accessLevel))
                {
                    Console.WriteLine("true " + accessLevel);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                SortedList<string, SharedAccessPolicy> policies = new SortedList<string, SharedAccessPolicy>();

                SharedAccessPolicy policy1 = new SharedAccessPolicy()
                {
                    Permissions = SharedAccessPermissions.List | SharedAccessPermissions.Read | SharedAccessPermissions.Write | SharedAccessPermissions.Delete,
                    SharedAccessStartTime = DateTime.UtcNow,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
                };

                policies.Add("Policy1", policy1);

                policies.Add("Policy2", new SharedAccessPolicy()
                {
                    Permissions = SharedAccessPermissions.Read,
                    SharedAccessStartTime = DateTime.Parse("2010-01-01T09:38:05Z"),
                    SharedAccessExpiryTime = DateTime.Parse("2012-12-31T09:38:05Z")
                });

                Console.Write("Set container access policy ");
                if (BlobHelper.SetContainerAccessPolicy("samplecontainer1", policies))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Get container access policy ");
                if (BlobHelper.GetContainerAccessPolicy("samplecontainer1", out policies))
                {
                    Console.WriteLine("true");
                    if (policies != null)
                    {
                        foreach (KeyValuePair<string, SharedAccessPolicy> policy in policies)
                        {
                            Console.WriteLine("Policy " + policy.Key);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string signature = String.Empty;

                Console.Write("Create shared access signature ");
                if (BlobHelper.GenerateSharedAccessSignature("samplecontainer1", policy1, out signature))
                {
                    Console.WriteLine("true " + signature);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                signature = String.Empty;

                Console.Write("Create shared access signature from access policy ");
                if (BlobHelper.GenerateSharedAccessSignature("samplecontainer1", "Policy1", out signature))
                {
                    Console.WriteLine("true " + signature);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                signature = String.Empty;

                Console.Write("Create shared access signature from access policy 2 ");
                if (BlobHelper.GenerateSharedAccessSignature("samplecontainer1", "Policy2", out signature))
                {
                    Console.WriteLine("true " + signature);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                BlobContainerProperties containerProperties = new BlobContainerProperties();

                Console.Write("Get container properties ");
                if (BlobHelper.GetContainerProperties("samplecontainer1", out containerProperties))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("BlobContainerProperties.Etag: " + containerProperties.ETag);
                    Console.WriteLine("BlobContainerProperties.LastModifiedUtc: " + containerProperties.LastModifiedUtc.ToShortDateString());
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                NameValueCollection metadata = new NameValueCollection();

                metadata.Add("property1", "Value1");
                metadata.Add("property2", "Value2");
                metadata.Add("property3", "Value3");

                Console.Write("Set container metadata ");
                if (BlobHelper.SetContainerMetadata("samplecontainer1", metadata))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                metadata = new NameValueCollection();

                Console.Write("Get container metadata ");
                if (BlobHelper.GetContainerMetadata("samplecontainer1", out metadata))
                {
                    Console.WriteLine("true");
                    if (metadata != null)
                    {
                        for (int i = 0; i < metadata.Count; i++)
                        {
                            Console.WriteLine(metadata.GetKey(i) + ": " + metadata.Get(i));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                //Console.Write("Delete container container1 ");
                //if (BlobHelper.DeleteContainer("samplecontainer1"))
                //{
                //    Console.WriteLine("true");
                //}
                //else
                //{
                //    Console.WriteLine("false");
                //}

                Separator();

                List<CloudBlob> blobs;
                Console.Write("List blobs ");
                if (BlobHelper.ListBlobs("samplecontainer1", out blobs))
                {
                    Console.WriteLine("true");
                    if (blobs != null)
                    {
                        foreach (CloudBlob blob in blobs)
                        {
                            Console.WriteLine(blob.Uri.LocalPath);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put blob ");
                if (BlobHelper.PutBlob("samplecontainer1", "blob1.txt", "This is a text blob!"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put page blob ");
                if (BlobHelper.PutBlob("samplecontainer1", "pageblob1.txt", 2048))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string page0 = new string('A', 512);

                Console.Write("Put page 0 ");
                if (BlobHelper.PutPage("samplecontainer1", "pageblob1.txt", page0, 0, 512))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string page1 = new string('B', 512);

                Console.Write("Put page 1 ");
                if (BlobHelper.PutPage("samplecontainer1", "pageblob1.txt", page1, 512, 512))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                page0 = String.Empty;

                Console.Write("Get page 0 ");
                if (BlobHelper.GetPage("samplecontainer1", "pageblob1.txt", 0, 512, out page0))
                {
                    Console.WriteLine("true");
                    Console.WriteLine(page0);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                page1 = String.Empty;

                Console.Write("Get page 1 ");
                if (BlobHelper.GetPage("samplecontainer1", "pageblob1.txt", 512, 512, out page1))
                {
                    Console.WriteLine("true");
                    Console.WriteLine(page1);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                PageRange[] ranges;

                Console.Write("Get page regions ");
                if (BlobHelper.GetPageRegions("samplecontainer1", "pageblob1.txt", out ranges))
                {
                    Console.WriteLine("true");
                    if (ranges != null)
                    {
                        foreach (PageRange range in ranges)
                        {
                            Console.WriteLine(range.StartOffset.ToString() + "-" + range.EndOffset.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string content;

                Console.Write("Get blob ");
                if (BlobHelper.GetBlob("samplecontainer1", "blob1.txt", out content))
                {
                    Console.WriteLine("true");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Copy blob ");
                if (BlobHelper.CopyBlob("samplecontainer1", "blob1.txt", "samplecontainer1", "blob2.txt"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Snapshot blob ");
                if (BlobHelper.SnapshotBlob("samplecontainer1", "blob1.txt"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Delete blob ");
                if (BlobHelper.DeleteBlob("samplecontainer1", "blob2.txt"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                SortedList<string, string> properties = new SortedList<string, string>();
                properties.Add("ContentType", "text/html");

                Console.Write("Set blob properties ");
                if (BlobHelper.SetBlobProperties("samplecontainer1", "blob1.txt", properties))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Get blob properties ");
                if (BlobHelper.GetBlobProperties("samplecontainer1", "blob1.txt", out properties))
                {
                    Console.WriteLine("true");
                    foreach (KeyValuePair<string, string> item in properties)
                    {
                        Console.WriteLine(item.Key + ": " + item.Value);
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put blob If Unchanged 1 ");
                if (BlobHelper.PutBlobIfUnchanged("samplecontainer1", "blob1.txt", "This is a text blob!", properties["ETag"]))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put blob If Unchanged 2 ");
                if (BlobHelper.PutBlobIfUnchanged("samplecontainer1", "blob1.txt", "This is a text blob!", "BadETag"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put blob with MD5 ");
                if (BlobHelper.PutBlobWithMD5("samplecontainer1", "blob1.txt", "This is a text blob!"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                metadata.Clear();
                metadata.Add("property1", "Value1");
                metadata.Add("property2", "Value2");
                metadata.Add("property3", "Value3");
                metadata.Add("property4", "Value4");

                Console.Write("Set blob metadata ");
                if (BlobHelper.SetBlobMetadata("samplecontainer1", "blob1.txt", metadata))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Get blob metadata ");
                if (BlobHelper.GetBlobMetadata("samplecontainer1", "blob1.txt", out metadata))
                {
                    Console.WriteLine("true");
                    for (int i = 0; i < metadata.Count; i++)
                    {
                        Console.WriteLine(metadata.GetKey(i) + ": " + metadata.Get(i));
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string leaseAction = "acquire";
                string leaseId = null;
                Console.Write("Lease blob - acquire ");
                if (BlobHelper.LeaseBlob("samplecontainer1", "blob1.txt", leaseAction, ref leaseId))
                {
                    Console.WriteLine("true " + leaseId);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                leaseAction = "release";
                Console.Write("Lease blob - release ");
                if (BlobHelper.LeaseBlob("samplecontainer1", "blob1.txt", leaseAction, ref leaseId))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string[] blockIds = new string[3];

                Console.Write("Put block 0 ");
                if (BlobHelper.PutBlock("samplecontainer1", "largeblob1.txt", 0, blockIds, "AAAAAAAAAA"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put block 1 ");
                if (BlobHelper.PutBlock("samplecontainer1", "largeblob1.txt", 1, blockIds, "BBBBBBBBBB"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put block 2 ");
                if (BlobHelper.PutBlock("samplecontainer1", "largeblob1.txt", 2, blockIds, "CCCCCCCCCC"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                string[] getblockids = null;

                Console.Write("Get block list ");
                if (BlobHelper.GetBlockList("samplecontainer1", "largeblob1.txt", out getblockids))
                {
                    Console.WriteLine("true");
                    if (getblockids != null)
                    {
                        for (int i = 0; i < getblockids.Length; i++)
                        {
                            Console.WriteLine("Block " + i + " id = " + getblockids[i]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Put block list ");
                if (BlobHelper.PutBlockList("samplecontainer1", "largeblob1.txt", blockIds))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION " + ex.ToString());
            }
        }

        // Perform queue storage operations.

        static void TestQueueStorage()
        {
            string storageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
            QueueHelper QueueHelper = new QueueHelper(storageConnectionString);

            try
            {
                Separator();

                List<CloudQueue> queues;
                Console.Write("List queues ");
                if (QueueHelper.ListQueues(out queues))
                {
                    Console.WriteLine("true");
                    if (queues != null)
                    {
                        foreach (CloudQueue queue in queues)
                        {
                            Console.Write(queue.Name + " ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Create queue ");
                if (QueueHelper.CreateQueue("samplequeue1"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Delete queue ");
                if (QueueHelper.DeleteQueue("samplequeue0"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                NameValueCollection metadata = new NameValueCollection();
                metadata.Add("property1", "Value1");
                metadata.Add("property2", "Value2");
                metadata.Add("property3", "Value3");

                Console.Write("Set queue metadata ");
                if (QueueHelper.SetQueueMetadata("samplequeue1", metadata))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Get queue metadata ");
                if (QueueHelper.GetQueueMetadata("samplequeue1", out metadata))
                {
                    Console.WriteLine("true");
                    if (metadata != null)
                    {
                        for (int i = 0; i < metadata.Count; i++)
                        {
                            Console.WriteLine(metadata.GetKey(i) + ": " + metadata.Get(i));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                CloudQueueMessage message = null;

                Console.Write("Peek message ");
                if (QueueHelper.PeekMessage("samplequeue1", out message))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("MessageId: " + message.Id + " popReceipt=" + message.PopReceipt);
                    Console.WriteLine("POPReceipt; " + message.PopReceipt);
                    Console.WriteLine(message.AsString);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Clear messages ");
                if (QueueHelper.ClearMessages("samplequeue1"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                message = null;

                Console.Write("Peek message ");
                if (QueueHelper.PeekMessage("samplequeue1", out message))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("MessageId: " + message.Id + " popReceipt=" + message.PopReceipt);
                    Console.WriteLine("POPReceipt; " + message.PopReceipt);
                    Console.WriteLine(message.AsString);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                message = new CloudQueueMessage("<Order id=\"1001\">This is test message 1</Order>");

                Console.Write("Put message ");
                if (QueueHelper.PutMessage("samplequeue1", message))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                message = null;

                Console.Write("Get message ");
                if (QueueHelper.GetMessage("samplequeue1", out message))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("MessageId: " + message.Id + " popReceipt=" + message.PopReceipt);
                    Console.WriteLine("POPReceipt; " + message.PopReceipt);
                    Console.WriteLine(message.AsString);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Delete message ");
                if (QueueHelper.DeleteMessage("samplequeue1", message))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                message = null;

                Console.Write("Get message ");
                if (QueueHelper.GetMessage("samplequeue1", out message))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("MessageId: " + message.Id + " popReceipt=" + message.PopReceipt);
                    Console.WriteLine("POPReceipt; " + message.PopReceipt);
                    Console.WriteLine(message.AsString);
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                message = null;

                Console.Write("Get message ");
                if (QueueHelper.GetMessage("samplequeue1", out message))
                {
                    Console.WriteLine("true");
                    Console.WriteLine("MessageId: " + message.Id + " popReceipt=" + message.PopReceipt);
                    Console.WriteLine("POPReceipt; " + message.PopReceipt);
                    Console.WriteLine(message.AsString);
                }
                else
                {
                    Console.WriteLine("false");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION " + ex.ToString());
            }
        }

        // Perform table storage operations.

        static void TestTableStorage()
        {
            string storageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
            TableHelper TableHelper = new TableHelper(storageConnectionString);

            try
            {
                Separator();

                List<string> tables;

                Console.Write("List tables ");
                if (TableHelper.ListTables(out tables))
                {
                    Console.WriteLine("true");
                    if (tables != null)
                    {
                        foreach (string tableName in tables)
                        {
                            Console.Write(tableName + " ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Create table ");
                if (TableHelper.CreateTable("sampletable"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Insert entity ");
                if (TableHelper.InsertEntity("sampletable", 
                    new Contact("USA", "Pallmann") { LastName = "Pallmann", FirstName = "David", Email = "dpallmann@hotmail.com", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Insert entity ");
                if (TableHelper.InsertEntity("sampletable", 
                    new Contact("USA", "Smith") { LastName = "Smith", FirstName = "John", Email = "john.smith@hotmail.com", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Insert entity ");
                if (TableHelper.InsertEntity("sampletable", 
                    new Contact("USA", "Jones") { LastName = "Jones", FirstName = "Tom", Email = "tom.jones@hotmail.com", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Insert entity ");
                if (TableHelper.InsertEntity("sampletable",
                    new Contact("USA", "Peters") { LastName = "Peters", FirstName = "Sally", Email = "sally.peters@hotmail.com", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Update entity ");
                if (TableHelper.ReplaceUpdateEntity("sampletable", "USA", "Pallmann",
                    new Contact("USA", "Pallmann") { LastName = "Pallmann", FirstName = "David", Email = "david.pallmann@hotmail.com", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Replace Update entity ");
                if (TableHelper.ReplaceUpdateEntity("sampletable", "USA", "Peters",
                    new Contact("USA", "Peters") { LastName = "Peters", FirstName = "Sally", Country = "USA" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Merge Update entity ");
                if (TableHelper.MergeUpdateEntity("sampletable", "USA", "Peters",
                    new MiniContact("USA", "Peters") { LastName = "Peters", FirstName = "Sally", Email = "sally.peters@hotmail.com" }))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Contact contact = null;

                Console.Write("Get entity ");
                if (TableHelper.GetEntity<Contact>("sampletable", "USA", "Pallmann", out contact))
                {
                    Console.WriteLine("true");

                    if (contact != null)
                    {
                        Console.WriteLine("Contact.LastName: " + contact.LastName);
                        Console.WriteLine("Contact.FirstName: " + contact.FirstName);
                        Console.WriteLine("Contact.Email: " + contact.Email);
                        Console.WriteLine("Contact.Phone: " + contact.Phone);
                        Console.WriteLine("Contact.Country: " + contact.Country);
                    }
                    else
                    {
                        Console.WriteLine("Contact <NULL>");
                    }
                }
                else
                {
                    Console.WriteLine("false");
                }

                Separator();

                Console.Write("Query entities ");
                IEnumerable<Contact> entities = TableHelper.QueryEntities<Contact>("sampletable").Where(e => e.PartitionKey == "USA").AsTableServiceQuery<Contact>();
                if (entities != null)
                {
                    Console.WriteLine("true");
                    foreach (Contact contact1 in entities)
                    {
                        Console.WriteLine("Contact.LastName: " + contact1.LastName);
                        Console.WriteLine("Contact.FirstName: " + contact1.FirstName);
                        Console.WriteLine("Contact.Email: " + contact1.Email);
                        Console.WriteLine("Contact.Phone: " + contact1.Phone);
                        Console.WriteLine("Contact.Country: " + contact1.Country);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("<NULL>");
                }

                //Console.Write("Query entities ");
                //var query = (
                //    from Contact c
                //    in TableHelper.QueryEntities<Contact>("sampletable")
                //    where c.PartitionKey == "USA"
                //    select c
                //    ).AsTableServiceQuery<Contact>();
                //IEnumerable<Contact> entities = query.ToList();
                //if (entities != null)
                //{
                //    Console.WriteLine("true");
                //    foreach (Contact contact1 in entities)
                //    {
                //        Console.WriteLine("Contact.LastName: " + contact1.LastName);
                //        Console.WriteLine("Contact.FirstName: " + contact1.FirstName);
                //        Console.WriteLine("Contact.Email: " + contact1.Email);
                //        Console.WriteLine("Contact.Phone: " + contact1.Phone);
                //        Console.WriteLine("Contact.Country: " + contact1.Country);
                //        Console.WriteLine();
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("<NULL>");
                //}

                Separator();

                Console.Write("Delete entity ");
                if (TableHelper.DeleteEntity<Contact>("sampletable", "USA", "Smith"))
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }

                ////Separator();

                ////Console.Write("Delete table ");
                ////if (TableHelper.DeleteTable("sampletable"))
                ////{
                ////    Console.WriteLine("true");
                ////}
                ////else
                ////{
                ////    Console.WriteLine("false");
                ////}
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION " + ex.ToString());
            }
        }


        private static void Separator()
        {
            Console.WriteLine("----------------------------------------");
        }
    }

    public class Contact : TableServiceEntity
    {
        public Contact() : base() { }

        public Contact(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
    }

    public class MiniContact : TableServiceEntity
    {
        public MiniContact() : base() { }

        public MiniContact(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }

    class ListRowsContinuationToken
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}


