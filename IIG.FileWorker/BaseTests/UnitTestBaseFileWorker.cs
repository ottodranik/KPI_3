using Microsoft.VisualStudio.TestTools.UnitTesting;
using IIG.FileWorker;
using System;
using System.IO;

namespace BaseTests
{
    [TestClass]
    public class UnitTestBaseFileWorker
    {
        private static string pathToLabFolder = "/Users/shepard/Study/KPI3/IIG.FileWorker/BaseTests/testfiles";
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
        public void Write_CreateIfNotExist_ReturnTrue()
        {
            string ext = "txt";
            string text = "some text\nsome text\nsome text";
            Boolean result = BaseFileWorker.Write(text, pathToLabFolder + "/somefile." + ext);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Write_EmptyPath_ReturnFalse()
        {
            string text = "some text\nsome text\nsome text";
            Boolean result = BaseFileWorker.Write(text, "");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryWrite_EmptyPath_ReturnFalse()
        {
            string text = "some text 11\nsome text 22\nsome text 33";
            Boolean result = BaseFileWorker.TryWrite(text, "", 1);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryWrite_TriesCounterIsZero_ReturnFalse()
        {
            string ext = "txt";
            string text = "some text 11\nsome text 22\nsome text 33";
            Boolean result = BaseFileWorker.TryWrite(text, pathToLabFolder + "/somefile." + ext, 0);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryWrite_WriteSuccess_ReturnTrue()
        {
            string ext = "txt";
            string text = "some text 11\nsome text 22\nsome text 33";
            Boolean result = BaseFileWorker.TryWrite(text, pathToLabFolder + "/somefile." + ext, 2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetFileName_FileExists_ReturnFileName()
        {
            string result = BaseFileWorker.GetFileName(pathToLabFolder + "/somefile.txt");
            Assert.AreEqual("somefile.txt", result);
        }

        [TestMethod]
        public void GetFileName_FileDoesNotExist_ReturnNull()
        {
            string result = BaseFileWorker.GetFileName(pathToLabFolder + "/somefile2.txt");
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GetPath_FileExists_ReturnPathToFile()
        {
            string result = BaseFileWorker.GetPath(pathToLabFolder + "/somefile.txt");
            Assert.AreEqual(pathToLabFolder, result);
        }

        [TestMethod]
        public void GetPath_FileDoesNotExist_ReturnNull()
        {
            string result = BaseFileWorker.GetPath(pathToLabFolder + "/somefile2.txt");
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void MkDir_DirDoesNotExist_CreateFolderAndReturnPath()
        {
            string name = "somefolder2";
            string result = BaseFileWorker.MkDir(pathToLabFolder + "/" + name);
            Assert.AreEqual(pathToLabFolder + "/" + name, result);
        }

        [TestMethod]
        public void MkDir_DirExists_ReturnFolderPath()
        {
            string result = BaseFileWorker.MkDir(pathToLabFolder);
            Assert.AreEqual(pathToLabFolder, result);
        }

        [TestMethod]
        public void ReadAll_FileExist_ReturnData()
        {
            string ext = "txt";
            string result = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile." + ext);
            Assert.AreEqual("some text 11\nsome text 22\nsome text 33", result);
        }

        [TestMethod]
        public void ReadAll_FileDoesNotExist_ReturnNull()
        {
            string result = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile2.txt");
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ReadLines_FileExists_ReturnData()
        {
            string ext = "txt";
            string[] result = BaseFileWorker.ReadLines(pathToLabFolder + "/somefile." + ext);
            string[] expected = { "some text 11", "some text 22", "some text 33" };
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReadLines_FileDoesNotExists_ReturnNull()
        {
            string[] result = BaseFileWorker.ReadLines(pathToLabFolder + "/somefile2.txt");
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TryCopy_OutputDoesNotExist_ReturnFalse()
        {
            Boolean result = BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", "", false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryCopy_InputDoesNotExist_ReturnFalse()
        {
            Boolean result = BaseFileWorker.TryCopy("", pathToLabFolder + "/somefile.txt", false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryCopy_TriesCounterIsZero_ReturnFalse()
        {
            Boolean result = BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", pathToLabFolder + "/somefile2.txt", false, 0);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryCopy_InputOutputExistsTriesIsNotZero_ReturnTrue()
        {
            string fromData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile.txt");
            Boolean result = BaseFileWorker.TryCopy(pathToLabFolder + "/somefile.txt", pathToLabFolder + "/somefile2.txt", false, 2);
            Assert.IsTrue(result);
            string toData = BaseFileWorker.ReadAll(pathToLabFolder + "/somefile2.txt");
            Assert.AreEqual(fromData, toData);
        }
    }
}
