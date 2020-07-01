using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    // this class is the model class for the comments 
    // it takes the name of the Blobblock in the parameter to set it as the PK
    // the rowKey will be set as the id of the user who comments.
    public class Comment : TableEntity
    {

        public Comment()
        {
        }
        public string CommentText { get; set; }

    }
}