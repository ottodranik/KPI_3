using Microsoft.VisualStudio.TestTools.UnitTesting;
using IIG.DatabaseConnectionUtils;
using IIG.FileWorker;
using System;
using System.IO;
using System.Data;
using System.Linq;

namespace lab2
{
    [TestClass]
    public class Lab1_FileWorkerTests
    {
        private const string Server = @"localhost,1433";
        private const string Database = @"IIG.CoSWE.StorageDB";
        private const bool IsTrusted = false;
        private const string Login = @"sa";
        private const string Password = @"passw0RD";
        private const int ConnectionTimeout = 75;

        private static DatabaseConnection DB = new DatabaseConnection(Server, Database, IsTrusted, Login, Password, ConnectionTimeout);

        private static string pathToLabFolder = "/Users/shepard/Study/KPI3/labs/lab2/testfiles";
        private static Random rand = new Random(DateTime.Now.Second);

        static private string RandName(string prefix)
        {
            return prefix + rand.Next();
        }

        [ClassInitialize]
        public static void BeforeAllTests(TestContext testContext)
        {
            if (Directory.Exists(pathToLabFolder))
            {
                Directory.Delete(pathToLabFolder, true);
            }
            Directory.CreateDirectory(pathToLabFolder);

            // Clean DB before start and reset AI counter for FileID field
            DB.ExecSql("DELETE FROM dbo.Files DBCC CHECKIDENT (\"dbo.Files\", RESEED, 0);");
        }

        [TestMethod]
        public void Add_File_And_Save_In_DB_By_String()
        {
            // Save some string in file
            string text = "save data";
            string filename = pathToLabFolder + "/somefile.txt";
            BaseFileWorker.Write(text, filename);

            // Get byte data from file
            byte[] databytes = File.ReadAllBytes(filename);
            string hexString = "0x"+BitConverter.ToString(databytes).Replace("-", string.Empty);

            // Save file data in DB
            Boolean queryRes = DB.ExecSql("INSERT INTO dbo.Files (FileName, FileContent) VALUES ('" + filename + "', CONVERT(varbinary(1024), '"+ hexString + "', 1))");
            Assert.IsTrue(queryRes);
        }

        [TestMethod]
        public void Add_File_And_Save_In_DB_By_Procedure()
        {
            // Save some string in file
            string text = "save data 2";
            string filename = pathToLabFolder + "/somefile2.txt";
            BaseFileWorker.Write(text, filename);

            // Get byte data from file
            byte[] databytes = File.ReadAllBytes(filename);
            string hexString = "0x" + BitConverter.ToString(databytes).Replace("-", string.Empty);

            // Save file data in DB with Procedure
            Boolean queryRes = DB.ExecSql("DECLARE @res bit EXECUTE AddFile @FileName='" + filename + "', @FileContent=" + hexString + ", @Result=@res OUTPUT SELECT @res");
            Assert.IsTrue(queryRes);
        }


        [TestMethod]
        public void Get_File_From_DB_And_Check_If_Exists()
        {
            // Save some string in file
            string text = "save data 3";
            string filename = pathToLabFolder + "/somefile3.txt";
            BaseFileWorker.Write(text, filename);

            // Get byte data from file
            byte[] databytes = File.ReadAllBytes(filename);
            string hexString = "0x" + BitConverter.ToString(databytes).Replace("-", string.Empty);

            // Save files data in DB
            Boolean queryRes = DB.ExecSql("" +
                "INSERT INTO dbo.Files (FileName, FileContent) " +
                "VALUES ('" + filename + "', CONVERT(varbinary(1024), '" + hexString + "', 1))," +
                "('" + filename + "', CONVERT(varbinary(1024), '" + hexString + "', 1))"
                );
            Assert.IsTrue(queryRes);
        }

        [TestMethod]
        public void Check_If_File_Exists_And_Content_Match()
        {
            // Get last file name from list
            string filename = DB.GetStrBySql("SELECT FileName FROM dbo.Files ORDER BY FileID DESC");
            Assert.AreEqual(pathToLabFolder + "/somefile3.txt", filename);

            // Get last file name from list
            string fileContent = DB.GetStrBySql("SELECT CAST(FileContent as varchar) AS FileContent FROM dbo.Files ORDER BY FileID DESC");
            Assert.AreEqual("save data 3", fileContent);

            // Get data from file
            string fileData = BaseFileWorker.ReadAll(filename); Assert.AreEqual("save data 3", fileContent);
            Assert.AreEqual(fileData, fileContent);
        }

        [TestMethod]
        public void Get_File_Id_From_DB_By_Helper()
        {
            // Get id
            int? id = DB.GetIntBySql("SELECT FileID FROM dbo.Files WHERE FileID = 2");
            Assert.AreEqual(2, id);
        }

        [TestMethod]
        public void Get_File_Id_From_DB_By_Helper_Procedure()
        {
            int? id = DB.GetIntBySql("EXECUTE GetFile @FileID=2");
            Assert.AreEqual(2, id);
        }

        [TestMethod]
        public void Get_Files_String_List_From_DB_By_String()
        {
            string[] expectedData1 = new string[] { "1", pathToLabFolder + "/somefile.txt", "save data" };
            string[] expectedData2 = new string[] { "2", pathToLabFolder + "/somefile2.txt", "save data 2" };
            string[] expectedData3 = new string[] { "3", pathToLabFolder + "/somefile3.txt", "save data 3" };
            string[] expectedData4 = new string[] { "4", pathToLabFolder + "/somefile3.txt", "save data 3" };

            // Get several rows
            string[][] rows = DB.GetLstBySql("SELECT FileID, FileName, CAST(FileContent as varchar) as FileContent FROM dbo.Files ORDER BY FileID ASC");
            Assert.AreEqual(4, rows.Length);
            CollectionAssert.AreEqual(expectedData1, rows[0]);
            CollectionAssert.AreEqual(expectedData2, rows[1]);
            CollectionAssert.AreEqual(expectedData3, rows[2]);
            CollectionAssert.AreEqual(expectedData4, rows[3]);

            //Get one TOP row
            rows = DB.GetLstBySql("SELECT TOP(1) FileID, FileName, CAST(FileContent as varchar) as FileContent FROM dbo.Files ORDER BY FileID ASC");
            Assert.AreEqual(1, rows.Length);
            CollectionAssert.AreEqual(expectedData1, rows[0]);
        }

        [TestMethod]
        public void Get_Files_String_List_From_DB_By_Procedure()
        {
            string filename = pathToLabFolder + "/somefile2.txt";

            string[] expectedData1 = new string[] { "2", filename, "save data 2" };

            // Get one row
            string[][] rows2 = DB.GetLstBySql("EXECUTE GetFiles @FileName='" + filename + "'");
            Assert.AreEqual("2", rows2[0][0]);
            Assert.AreEqual(filename, rows2[0][1]);
        }

        [TestMethod]
        public void Get_Files_DataTable_List_From_DB_By_String()
        {
            DataTable expectedTable = new DataTable();
            expectedTable.Columns.Add("FileID");
            expectedTable.Columns.Add("FileName");
            expectedTable.Columns.Add("FileContent");
            DataRow expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 1;
            expectedTableRow["FileName"] = pathToLabFolder + "/somefile.txt";
            expectedTableRow["FileContent"] = "save data";
            expectedTable.Rows.Add(expectedTableRow);
            expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 2;
            expectedTableRow["FileName"] = pathToLabFolder + "/somefile2.txt";
            expectedTableRow["FileContent"] = "save data 2";
            expectedTable.Rows.Add(expectedTableRow);
            expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 3;
            expectedTableRow["FileName"] = pathToLabFolder + "/somefile3.txt";
            expectedTableRow["FileContent"] = "save data 3";
            expectedTable.Rows.Add(expectedTableRow);
            expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 4;
            expectedTableRow["FileName"] = pathToLabFolder + "/somefile3.txt";
            expectedTableRow["FileContent"] = "save data 3";
            expectedTable.Rows.Add(expectedTableRow);

            DataTable table = DB.GetDataTableBySql("SELECT FileID, FileName, CAST(FileContent as varchar) as FileContent FROM dbo.Files ORDER BY FileID ASC");
            Assert.AreEqual(expectedTable.Rows.Count, table.Rows.Count, "Number of records is different");
            for (int i = 0; i <= table.Rows.Count - 4; i++)
            {
                Assert.IsTrue(expectedTable.Columns.Cast<DataColumn>().Any(
                    dc1 => expectedTable.Rows[i][dc1.ColumnName].ToString() == table.Rows[i][dc1.ColumnName].ToString()
                ), "Value is different");
            }
        }

        [TestMethod]
        public void Get_Files_DataTable_List_From_DB_By_Procedure()
        {
            string filename = pathToLabFolder + "/somefile3.txt";

            DataTable expectedTable = new DataTable();
            expectedTable.Columns.Add("FileID");
            expectedTable.Columns.Add("FileName");
            expectedTable.Columns.Add("FileContent");
            DataRow expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 3;
            expectedTableRow["FileName"] = filename;
            expectedTableRow["FileContent"] = "save data 3";
            expectedTable.Rows.Add(expectedTableRow);
            expectedTableRow = expectedTable.NewRow();
            expectedTableRow["FileID"] = 4;
            expectedTableRow["FileName"] = filename;
            expectedTableRow["FileContent"] = "save data 3";
            expectedTable.Rows.Add(expectedTableRow);

            DataTable table = DB.GetDataTableBySql("EXECUTE GetFiles @FileName='" + filename + "'");
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                Assert.IsTrue(expectedTable.Columns.Cast<DataColumn>().Any(
                    dc1 => expectedTable.Rows[i][dc1.ColumnName].ToString() == table.Rows[i][dc1.ColumnName].ToString()
                ), "Value is different");
            }
        }

        [TestMethod]
        public void Delete_File_From_DB_String()
        {
            // Check that file exists
            int? id = DB.GetIntBySql("EXECUTE GetFile @FileID=1");
            Assert.AreEqual(1, id);

            // Save file data in DB
            Boolean queryRes = DB.ExecSql("DELETE FROM dbo.Files WHERE FileID = 1");
            Assert.IsTrue(queryRes);

            // Check that no file exists
            id = DB.GetIntBySql("EXECUTE GetFile @FileID=1");
            Assert.AreEqual(null, id);
        }

        [TestMethod]
        public void Delete_File_From_DB_Procedure()
        {
            // Check that file exists
            int? id = DB.GetIntBySql("EXECUTE GetFile @FileID=2");
            Assert.AreEqual(2, id);

            // Save file data in DB
            Boolean queryRes = DB.ExecSql("DECLARE @res bit EXECUTE DeleteFile @FileID=2, @Result=@res OUTPUT SELECT @res");
            Assert.IsTrue(queryRes);

            // Check that no file exists
            id = DB.GetIntBySql("EXECUTE GetFile @FileID=2");
            Assert.AreEqual(null, id);
        }
    }
}
