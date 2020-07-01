using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebRole1.Models;

namespace WebRole1
{
    // This class will handle communications between the cloudstorage and the blob UI
    // responsible for table creation and operations such as add blobs, get blobs, add comments, get comments
    public class BlobHelper
    {
        private CloudStorageAccount account;
        private CloudBlobClient client;
        private CloudBlobContainer container;

        private CloudTableClient cloudTableClient;
        private CloudTable table;


        public BlobHelper()
        {
            // establish communication with the cloud storage and creates a container in case it doesn't exit
            account = CloudStorageAccount.DevelopmentStorageAccount;
            client = account.CreateCloudBlobClient();
            container = client.GetContainerReference("lab4blobs");
            container.CreateIfNotExists();

            // establish communication with the cloud storage and creates a table in case it doesn't exit
            cloudTableClient = account.CreateCloudTableClient();
            table = cloudTableClient.GetTableReference("lab4comments");
            table.CreateIfNotExists();
        }


        // to upload a blob to the cloud. 
        //note that the file has to be in the same folder.
        public bool UploadBlob(string filePath, string fileName, string userId, string description)
        {
            try
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);     // the name of the image

                using (Stream file = File.OpenRead(filePath))    // image's path
                {
                    // image's meta data
                    blob.Metadata["owner"] = userId;
                    blob.Metadata["description"] = description;
                    blob.Metadata["likes"] = 0 + "";
                    blob.Metadata["dislikes"] = 0 + "";
                    blob.Metadata["downloads"] = 0 + "";
                    blob.UploadFromStream(file);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // it returns list of blobs that exist in the cloud
        public async Task<List<string>> GetBlobs()
        {
            BlobContinuationToken blobContinuationToken = null;
            List<string> uris = new List<string>();
            try
            {
                do
                {
                    var results = await container.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // Get the value of the continuation token returned by the listing call.
                    blobContinuationToken = results.ContinuationToken;

                    foreach (IListBlobItem item in results.Results)
                    {
                        uris.Add(item.Uri.Segments.Last());
                    }
                } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                return uris;
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        // it gets the description of a specific blob block(image)
        public string GetDescription(string blobName)
        {
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            string description = blockBlob.Metadata["description"];

            return description;
        }

        // to like a speicific blob block(image)
        public void Like(string blobName)
        {
            int like = GetLikes(blobName);
            like++;
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            blockBlob.Metadata["likes"] = like + "";
            blockBlob.SetMetadata();
        }

        // to dislike a speicific blob block(image)
        public void Dislike(string blobName)
        {
            int dislike = GetDislikes(blobName);
            dislike++;
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            blockBlob.Metadata["dislikes"] = dislike + "";
            blockBlob.SetMetadata();
        }

        // to get number of likes of a specific blob block(image)
        public int GetLikes(string blobName)
        {
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            int likes = int.Parse(blockBlob.Metadata["likes"]);

            return likes;
        }

        // to get number of dislikes of a specific blob block(image)
        public int GetDislikes(string blobName)
        {
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            int dislikes = int.Parse(blockBlob.Metadata["dislikes"]);

            return dislikes;
        }

        // to download a specific blob item
        public void Download(string blobName)
        {
            int downloads = GetNumOfDownloads(blobName);
            downloads++;
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            blockBlob.Metadata["downloads"] = downloads + "";
            blockBlob.SetMetadata();
        }

        // to get number of downloads of a specific blob block(image)
        public int GetNumOfDownloads(string blobName)
        {
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            int downloads = int.Parse(blockBlob.Metadata["downloads"]);

            return downloads;
        }

        // to get the owner of a specific blob block(image)
        public string getOwner(string blobName)
        {

            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.FetchAttributes();
            string owner = blockBlob.Metadata["owner"];

            return owner;
        }

        // to delete a speicifc blob block(image) and its comments
        public bool DeleteBlob(string blobName)
        {
            try
            {
                DeleteComments(blobName);

                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                blob.FetchAttributes();

                blob.Delete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // to add a comment to a specific image, it saves that in tables along with the id of the user who commented.
        public void AddComment(string comment, string blobName, string userId)
        {
            Comment c = new Comment()
            {
                CommentText = comment
            };
            c.PartitionKey = blobName;
            c.RowKey = userId;

            TableOperation insert = TableOperation.Insert(c);
            table.Execute(insert);
        }

        // to get a list of comments on a spcific image
        public List<Comment> GetComments(string blobName)
        {
            var entities = table.ExecuteQuery(new TableQuery<Comment>()).ToList();
            List<Comment> comments = new List<Comment>();

            foreach (Comment entity in entities)
            {
                if (entity.PartitionKey == blobName)
                {
                    comments.Add(entity);
                }
            }

            return comments;
        }

        // to delete the comments of a specific image
        public void DeleteComments(string blobName)
        {
            var entities = table.ExecuteQuery(new TableQuery<Comment>()).ToList();

            foreach (var entity in entities)
            {
                if (entity.PartitionKey == blobName)
                {
                    table.Execute(TableOperation.Delete(entity));
                }
            }
        }

    }
}