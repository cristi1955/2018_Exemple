using Azure_TableStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Configuration;

class Program
{
    static void Main(string[] args)
    {
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
        CloudTableClient tableStudent = cloudStorageAccount.CreateCloudTableClient();
        Console.WriteLine("Indicati Numele Tabelei pe care doriti sa o creati : ");
        string tableName = Console.ReadLine();
        CloudTable cloudTable = tableStudent.GetTableReference(tableName);
        CreateNewTable(cloudTable);
        while (true)
        {
            ScreenOptions();
            switch (Console.ReadLine())
            {
                case "INSERT":
                    if(cloudTable == null)
                    {
                        Console.WriteLine("Tabela nu a fost creata");
                        break;
                    }
                    InsertRecordToTable(cloudTable);
                    break;
                case "UPDATE":
                    if (cloudTable == null)
                    {
                        Console.WriteLine("Tabela nu a fost creata");
                        break;
                    }
                    UpdateRecordInTable(cloudTable);
                    break;
                case "DELETE":
                    if (cloudTable == null)
                    {
                        Console.WriteLine("Tabela nu a fost creata");
                        break;
                    }
                    DeleteRecordinTable(cloudTable);
                    break;
                case "DISPLAY":
                    if (cloudTable == null)
                    {
                        Console.WriteLine("Tabela nu a fost creata");
                        break;
                    }
                    DisplayTableRecords(cloudTable);
                    break;
                case "DROP tabela":
                    if (cloudTable == null)
                    {
                        Console.WriteLine("Tabela nu a fost creata");
                        break;
                    }
                    DropTable(cloudTable);
                    cloudTable = null;
                    break;
                default:
                    Console.WriteLine("Intrare eronata");
                    break;
            }
            Console.WriteLine("EXIT pentru a iesi din aplicatie. Orice pt. a continua");
            if (Console.ReadLine() == "EXIT")
            {
                return;
            }
        }

    }
    public static void CreateNewTable(CloudTable table)
    {
        if (!table.CreateIfNotExists())
        {
            Console.WriteLine("Tabela {0} deja exista", table.Name);
            return;
        }
        Console.WriteLine("Tabela {0} a fost creata", table.Name);
    }
    public static void InsertRecordToTable(CloudTable table)
    {
        Console.WriteLine("Tip student");
        string tipStudent = Console.ReadLine();
        Console.WriteLine("Student ID");
        string studentID = Console.ReadLine();
        Console.WriteLine("Nume student");
        string numeStudent = Console.ReadLine();
        Console.WriteLine("Detalii student");
        string detaliiStudent = Console.ReadLine();
        Student studentEntitate = new Student();
        studentEntitate.tipStudent= tipStudent;
        studentEntitate.studentID = Int32.Parse(studentID);
        studentEntitate.detaliiStudent = detaliiStudent;
        studentEntitate.numeStudent = numeStudent;
        studentEntitate.AssignPartitionKey();
        studentEntitate.AssignRowKey();
        Student studEntitate = RetrieveRecord(table, tipStudent, studentID);
        if (studEntitate == null)
        {
            TableOperation tabelaOperatii = TableOperation.Insert(studentEntitate);
            table.Execute(tabelaOperatii);
            Console.WriteLine("Inregistrare inclusa");
        }
        else
        {
            Console.WriteLine("Inregistrare existenta");
        }
    }
    public static void UpdateRecordInTable(CloudTable table)
    {
        Console.WriteLine("Tip student");
        string tipStudent = Console.ReadLine();
        Console.WriteLine("student ID");
        string studentID = Console.ReadLine();
        Console.WriteLine("Nume student");
        string numeStudent = Console.ReadLine();
        Console.WriteLine("Detalii student");
        string detaliiStudent = Console.ReadLine();
        Student studentEntitate = RetrieveRecord(table, tipStudent, studentID);
        if (studentEntitate != null)
        {
            studentEntitate.detaliiStudent = detaliiStudent;
            studentEntitate.numeStudent = numeStudent;
            TableOperation tabelaOperatii = TableOperation.Replace(studentEntitate);
            table.Execute(tabelaOperatii);
            Console.WriteLine("Entitate actualizata");
        }
        else
        {
            Console.WriteLine("Entitatea nu exista");
        }
    }
    public static void DeleteRecordinTable(CloudTable table)
    {
        Console.WriteLine("Tip student");
        string tipStudent = Console.ReadLine();
        Console.WriteLine("Student ID");
        string studentID = Console.ReadLine();
        Student studentEntitate = RetrieveRecord(table, tipStudent, studentID);
        if (studentEntitate != null)
        {
            TableOperation tablaOperatii = TableOperation.Delete(studentEntitate);
            table.Execute(tablaOperatii);
            Console.WriteLine("Entitate stearsa");
        }
        else
        {
            Console.WriteLine("Entitatea nu exista");
        }
    }
    public static Student RetrieveRecord(CloudTable table,string partitionKey,string rowKey)
    {
        TableOperation tablaOperatii = TableOperation.Retrieve<Student>(partitionKey, rowKey);
        TableResult tabelaRezultat = table.Execute(tablaOperatii);
        return tabelaRezultat.Result as Student;
    }
    public static void DisplayTableRecords(CloudTable table)
    {
        TableQuery<Student> tableQuery = new TableQuery<Student>();
        foreach (Student studentEntitate in table.ExecuteQuery(tableQuery))
        {
            Console.WriteLine("Student ID : {0}", studentEntitate.studentID);
            Console.WriteLine("Tip student : {0}", studentEntitate.tipStudent);
            Console.WriteLine("Nume student : {0}", studentEntitate.numeStudent);
            Console.WriteLine("Detalii student : {0}", studentEntitate.detaliiStudent);
            Console.WriteLine("=========================================");
        }
    }
    public static void DropTable(CloudTable table)
    {
        if (!table.DeleteIfExists())
        {
            Console.WriteLine("Table does not exists");
        }
    }
    public static void ScreenOptions()
    {
        Console.WriteLine("");
        Console.WriteLine("INSERT - insert entitati in AST");
        Console.WriteLine("UPDATE - actualizare entitati in AST");
        Console.WriteLine("DELETE - stergere entitati in AST");
        Console.WriteLine("DISPLAY - afisaza entitatile din AST ");
        Console.WriteLine("DROP - sterge AST");
        Console.WriteLine("Selectati optiune");
    }
}