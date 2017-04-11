using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;
using FocusTreeManager.Helper;
using System.Configuration;
using FocusTreeManager.DataContract;

namespace FocusTreeManager.Test
{
    /// <summary>
    /// Summary description for HelperTest
    /// </summary>
    [TestClass]
    public class HelperTest
    {
        public HelperTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region CodeHelper

        [TestMethod]
        public void getLineCountTest()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Act
            int linecount = Helper.CodeHelper.GetLineCount(FileForTest);
            //Assert
            Assert.IsTrue(linecount > 0);
            Assert.AreEqual(2368, linecount);
        }

        [TestMethod]
        public void GetFirstCharIndexFromLineIndexTest()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Act
            int firstchar = Helper.CodeHelper.GetFirstCharIndexFromLineIndex(FileForTest, 28);
            //Assert
            Assert.IsTrue(firstchar > 0);
            Assert.AreEqual('c', FileForTest[firstchar]);
        }

        [TestMethod]
        public void GetLastCharIndexFromLineIndexTest()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Act
            int lastChar = Helper.CodeHelper.GetLastCharIndexFromLineIndex(FileForTest, 28);
            //Assert
            Assert.IsTrue(lastChar > 0);
            Assert.AreEqual('}', FileForTest[lastChar - 1]);
        }

        [TestMethod]
        public void getLevelOfIndent()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Arrange
            string line24To34 = string.Join("\n", FileForTest.Split('\n').Skip(23).Take(11));
            //Act
            int LevelofIdent = Helper.CodeHelper.getLevelOfIndent(line24To34);
            //Assert
            Assert.IsTrue(LevelofIdent > 0);
            Assert.AreEqual(1, LevelofIdent);
        }

        [TestMethod]
        public void getAssociatedClosingBracket()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Arrange
            string line24To35 = string.Join("\n", FileForTest.Split('\n').Skip(23).Take(12));
            //Act
            int bracketPos = Helper.CodeHelper.getAssociatedClosingBracket(line24To35, 10);
            //Assert
            Assert.IsTrue(bracketPos > 0);
            Assert.AreEqual('}', line24To35[bracketPos]);
        }

        [TestMethod]
        public void getAssociatedOpeningBracket()
        {
            string FileForTest = File.ReadAllText("usa.txt");
            //Arrange
            string line24To35 = string.Join("\n", FileForTest.Split('\n').Skip(23).Take(12));
            //Act
            int bracketPos = Helper.CodeHelper.getAssociatedOpeningBracket(line24To35, 334);
            //Assert
            Assert.IsTrue(bracketPos > 0);
            Assert.AreEqual('{', line24To35[bracketPos]);
        }

        #endregion

        #region SerializationHelper

        [TestMethod]
        public void SerializationHelperTest()
        {
            //Arrange
            string filepath = "test.xh4prj";
            string filepath2 = "testi.xh4prj";
            string filepath3 = "testj.xh4prj";
            File.Delete(filepath2);
            //Act
            Project projectOkay = SerializationHelper.Deserialize(filepath);
            Project projectBad = SerializationHelper.Deserialize("");
            SerializationHelper.Serialize(filepath2, projectOkay);
            SerializationHelper.Serialize(filepath3, projectBad);
            //Assert
            Assert.IsNotNull(projectOkay);
            Assert.IsNull(projectBad);
            //A non existent filepath should return null
            Assert.IsTrue(File.Exists(filepath2));
            Assert.IsFalse(File.Exists(filepath3));
        }

        #endregion

    }
}
