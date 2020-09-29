using Microsoft.VisualStudio.TestTools.UnitTesting;
using IIG.FileWorker;
using System;
using System.IO;

namespace labs
{
    [TestClass]
    public class Lab1_FileWorkerTests
    {
        private static string pathToLabFolder = "/Users/shepard/Study/KPI3/labs/lab1/testfiles";
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
        }

        [TestMethod]
        public void Write_MultiFiles_CreateIfNotExist_ReturnTrue()
        {
            string[] arrayExt = { "html", "jpg", "js", "txt", "xml" };
            string text = "some text\nsome text\nsome text";
            foreach (string ext in arrayExt)
            {
                Boolean result = BaseFileWorker.Write(text, pathToLabFolder + "/somefile." + ext);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void Write_MultiFiles_RewriteIfExist_ReturnTrue()
        {
            string[] arrayExt = { "html", "jpg", "js", "txt", "xml" };
            string text = "some text 1\nsome text 2\nsome text 3";
            foreach (string ext in arrayExt)
            {
                Boolean result = BaseFileWorker.Write(text, pathToLabFolder + "/somefile." + ext);

                Assert.IsTrue(result);
            }
        }


        [TestMethod]
        public void TryWrite_SingleFile_RewriteIfNotExist_ReturnTrue()
        {
            string text = "some text 11\nsome text 22\nsome text 33";

            Boolean result = BaseFileWorker.TryWrite(text, pathToLabFolder + "/somefile.sh", 2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TryWrite_SingleFile_RewriteIfExist_ReturnTrue()
        {
            string text = "some text 111\nsome text 222\nsome text 333";

            Boolean result = BaseFileWorker.TryWrite(text, pathToLabFolder + "/somefile.sh", 2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetFileName_FileExists_ReturnFileName()
        {
            string result = BaseFileWorker.GetFileName(pathToLabFolder + "/somefile.txt");

            Assert.AreEqual("somefile.txt", result);
        }

        [TestMethod]
        public void GetFileName_FileNotExists_ReturnNull()
        {
            string result = BaseFileWorker.GetFileName(pathToLabFolder + "/somefile2.txt");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GetFilePath_FileExists_ReturnFileName()
        {
            string result = BaseFileWorker.GetFullPath(pathToLabFolder + "/somefile.txt");

            Assert.AreEqual(pathToLabFolder + "/somefile.txt", result);
        }

        [TestMethod]
        public void GetFilePath_FileNotExists_ReturnNull()
        {
            string result = BaseFileWorker.GetFullPath(pathToLabFolder + "/somefile2.txt");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GetPath_FileExists_ReturnPathToFile()
        {
            string result = BaseFileWorker.GetPath(pathToLabFolder + "/somefile.txt");

            Assert.AreEqual(pathToLabFolder, result);
        }

        [TestMethod]
        public void GetPath_FileNotExists_ReturnNull()
        {
            string result = BaseFileWorker.GetPath(pathToLabFolder + "/somefile2.txt");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void MkDir_ReturnFolderPath()
        {
            string name = Lab1_FileWorkerTests.RandName("somefolder");

            // Create new folder
            string result = BaseFileWorker.MkDir(pathToLabFolder + "/" + name);

            Assert.AreEqual(pathToLabFolder + "/" + name, result);

            // Return exists folder
            result = BaseFileWorker.MkDir(pathToLabFolder + "/" + name);

            Assert.AreEqual(pathToLabFolder + "/" + name, result);
        }

        [TestMethod]
        public void ReadAll_MultiFiles_FilesExist_ReturnData()
        {
            string[] arrayExt = { "html", "jpg", "js", "txt", "xml" };
            foreach (string ext in arrayExt)
            {
                string result = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile." + ext);

                Assert.AreEqual("some text 1\nsome text 2\nsome text 3", result);
            }
        }

        [TestMethod]
        public void ReadAll_SingleFile_FileNotExists_ReturnNull()
        {
            string result = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile2.txt");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ReadLines_MultiFiles_FilesExist_ReturnData()
        {
            string[] arrayExt = { "html", "jpg", "js", "txt", "xml" };
            foreach (string ext in arrayExt)
            {
                string[] result = BaseFileWorker.ReadLines(pathToLabFolder + "/somefile." + ext);
                string[] expected = { "some text 1", "some text 2", "some text 3" };

                Assert.AreEqual(3, result.Length);
                CollectionAssert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void ReadLines_SingleFile_FileNotExists_ReturnNull()
        {
            string[] result = BaseFileWorker.ReadLines(pathToLabFolder + "/somefile2.txt");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TryCopy_InputExists_OutputNotExists_Create_ReturnTrue()
        {
            string fromData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile.txt");
            Assert.IsNotNull(fromData);

            string toData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile_new.txt");
            Assert.IsNull(toData);

            Boolean result = BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", pathToLabFolder + "/somefile_new.txt", false, 2);
            Assert.IsTrue(result);

            string finalData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile_new.txt");
            Assert.AreEqual(fromData, finalData);
        }

        [TestMethod]
        public void TryCopy_InputExists_OutputExists_NotRewriteOutput_ReturnTrue()
        {
            string fromData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile.sh");
            Assert.IsNotNull(fromData);

            string toData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile_new.txt");
            Assert.IsNotNull(toData);

            Boolean result = BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", pathToLabFolder + "/somefile_new.txt", true, 2);
            Assert.IsTrue(result);

            string finalData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile_new.txt");
            Assert.AreEqual(toData, finalData);
        }

        [TestMethod]
        public void TryCopy_InputExists_OutputExists_NotRewriteOutput_ThrowException()
        {
            string fromData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile.txt");
            Assert.IsNotNull(fromData);

            string toData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile_new.txt");
            Assert.IsNotNull(toData);

            try
            {
                BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", pathToLabFolder + "/somefile_new.txt", false, 2);
            }
            catch (Exception error)
            {
                Assert.IsNotNull(error);
            }
        }
    }
}