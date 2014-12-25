using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JT76.Common.ObjectExtensions;
using JT76.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JT76.Tests.Common.Services
{
    [TestClass]
    public class FileServiceTests
    {
        private const DirectoryFolders TestFolder = DirectoryFolders.Jt76Test;
        private const int NDaysToHoldDirectoryFiles = 7;
        private const int NMaxDirectoryFolderFiles = 10;
        private readonly string _strExecutingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        public void SaveToDirectoryFileTest()
        {
            var fileService = new FileService();
            string strGuid = Guid.NewGuid().ToString();

            //save to directory folder
            fileService.SaveTextToDirectoryFile(TestFolder, strGuid);

            bool bMessageSaved = false;
            string[] fileLines = fileService.LoadTextFromDirectoryFile(TestFolder, "", DateTime.Now);
            if (fileLines.Any(item => item.Contains(strGuid)))
            {
                bMessageSaved = true;
            }
            Assert.IsTrue(bMessageSaved);
        }

        [TestMethod]
        public void LoadTextFromDirectoryFileTest()
        {
            var fileService = new FileService();
            string strGuid = Guid.NewGuid().ToString();

            //save to directory folder
            fileService.SaveTextToDirectoryFile(TestFolder, strGuid);

            //ensure date identifier is working
            bool bMessageFound = false;
            string[] fileLines = fileService.LoadTextFromDirectoryFile(TestFolder, "", DateTime.Now);
            if (fileLines.Any(item => item.Contains(strGuid)))
            {
                bMessageFound = true;
            }
            Assert.IsTrue(bMessageFound);

            //esnure file name load is working
            bMessageFound = false;
            string strEnum = TestFolder.ToNameString();
            string strFileName = "\\" + strEnum + "_" + DateTime.Now.ToString("M-dd-yyyy") + ".txt";
            fileLines = fileService.LoadTextFromDirectoryFile(TestFolder, strFileName);
            if (fileLines.Any(item => item.Contains(strGuid)))
            {
                bMessageFound = true;
            }
            Assert.IsTrue(bMessageFound);
        }

        [TestMethod]
        //NOTE: Running this test method will delete all but the newest nTestNumberOfFilesToHold in each directory
        public void DeleteOldFilesInFolderTest()
        {
            var fileService = new FileService();

            int nFolderCount = Directory.GetFiles(GetDirectoryFolderLocation(TestFolder)).Count();
            while (nFolderCount <= NMaxDirectoryFolderFiles*2)
            {
                string strEnum = TestFolder.ToNameString();
                string strFileName = strEnum + "_" + DateTime.Now.Ticks + ".txt";

                //create the file
                string strFileAndPathName = GetDirectoryFolderLocation(TestFolder) + "\\" + strFileName;
                File.AppendAllText(strFileAndPathName, "This is a new file created for deletion testing");

                //ensure created file exists and is unique
                string[] fileLines = fileService.LoadTextFromDirectoryFile(TestFolder, strFileName);
                Assert.AreEqual(1, fileLines.Count());

                nFolderCount++;
            }

            //delete all files but nTestNumberOfFilesToHold, ordered by LastWriteTime
            bool result = fileService.DeleteOldFilesInFolder(TestFolder, NMaxDirectoryFolderFiles);
            Assert.IsTrue(result);

            nFolderCount = Directory.GetFiles(GetDirectoryFolderLocation(TestFolder)).Count();
            Assert.AreEqual(nFolderCount, NMaxDirectoryFolderFiles);
        }

        [TestMethod]
        //NOTE: Running this test method will delete all files older than nDaysToTestAgainst in each directory
        public void DeleteFilesByDaysTest()
        {
            var fileService = new FileService();

            string strEnum = TestFolder.ToNameString();
            string strFileName = strEnum + "_" +
                                 DateTime.Now.AddDays(NDaysToHoldDirectoryFiles*-1).ToString("M-dd-yyyy") + ".txt";

            //create the file
            string strFileAndPathName = GetDirectoryFolderLocation(TestFolder) + "\\" + strFileName;
            File.AppendAllText(strFileAndPathName, "This is for deletion testing");

            //ensure created file exists
            fileService.LoadTextFromDirectoryFile(TestFolder, strFileName);

            //set the last write time on this file to 1 day older than days to delete against
            foreach (
                FileInfo file in
                    new DirectoryInfo(GetDirectoryFolderLocation(TestFolder)).GetFiles()
                        .Where(
                            x =>
                                x.Name.Contains("_" +
                                                DateTime.Now.AddDays(NDaysToHoldDirectoryFiles*-1).ToString("M-dd-yyyy") +
                                                ".txt")))
                file.LastWriteTime = DateTime.Now.AddDays((NDaysToHoldDirectoryFiles + 1)*-1);

            //delete files by days
            bool result = fileService.DeleteFilesByDays(TestFolder, NDaysToHoldDirectoryFiles);
            Assert.IsTrue(result);

            try
            {
                //try to load the file
                fileService.LoadTextFromDirectoryFile(TestFolder, strFileName);
                Assert.Fail();
            }
            catch (FileNotFoundException)
            {
                //expected file not found exception
            }
        }

        private string GetDirectoryFolderLocation(DirectoryFolders directory)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strEnum = TestFolder.ToNameString();
            return _strExecutingDirectory + "\\App_Data\\" + strEnum;
        }
    }
}