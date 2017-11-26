using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Azure_TableStorage
{
    class Student:TableEntity
    {
        private int studentID_;
        private string numeStudent_;
        private string detaliiStudent_;
        private string tipStudent_;
        public void AssignRowKey()
        {
            this.RowKey = studentID.ToString();
        }
        public void AssignPartitionKey()
        {
            this.PartitionKey = tipStudent;
        }
        public int studentID
        {
            get
            {
                return studentID_;
            }

            set
            {
                studentID_ = value;
            }
        }
        public string numeStudent
        {
            get
            {
                return numeStudent_;
            }

            set
            {
                numeStudent_ = value;
            }
        }   
        public string detaliiStudent
        {
            get
            {
                return detaliiStudent_;
            }

            set
            {
                detaliiStudent_ = value;
            }
        }

        public string tipStudent
        {
            get
            {
                return tipStudent_;
            }

            set
            {
                tipStudent_ = value;
            }
        }

        public int StudentID { get => this.studentID; set => this.studentID = value; }
    }
}
