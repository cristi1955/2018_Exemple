using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ckAzureTableMVC.Models
{
    public class StudentEntity : TableEntity
    {
        public StudentEntity(string Numele, string Prenumele)
        {
            this.PartitionKey = Numele;
            this.RowKey = Prenumele;
        }
        public StudentEntity() { }
        public string Email { get; set; }
    }
}