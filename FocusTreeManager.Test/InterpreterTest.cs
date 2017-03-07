using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FocusTreeManager.CodeStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FocusTreeManager.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class InterpreterTest
    {
        public InterpreterTest()
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

        [TestMethod]
        public void GoodScriptTest()
        {
            //Arrange
            Script script = new Script();
            //Act
            script.Analyse(File.ReadAllText("usa.txt"));
            //Test code value
            CodeValue value = script.FindValue("tag");
            string valueString = value.Parse();
            //Test Assignation
            Assignation assign = script.FindAssignation("modifier");
            string assignString = assign.Parse();
            //Test Extraction
            ICodeStruct extracted = script.Extract("default");
            string extractedString = extracted.Parse();
            //Test find all focuses
            List<ICodeStruct> assigns = script.FindAllValuesOfType<CodeBlock>("focus");
            //Test try parse
            string tag = Script.TryParse(script, "tag");
            //Assert
            Assert.IsTrue(script.Code.Any());
            Assert.IsFalse(ErrorLogger.Instance.hasErrors());
            Assert.IsTrue(valueString == "USA");
            Assert.IsTrue(!string.IsNullOrEmpty(assignString));
            Assert.IsTrue(extractedString.Contains("yes"));
            //Should not find a tag after extraction
            Assert.IsNull(script.FindValue("default"));
            Assert.IsTrue(assigns.Any());
            Assert.IsTrue(tag == "USA");
            //Test broken parse 
            Assert.IsNull(Script.TryParse(script, "default", false));
            try
            {
                Script.TryParse(script, "default");
                Assert.Fail("Exception was to be lifted by mandatory parse");
            }
            catch (Exception)
            {
                Console.WriteLine(@"Exception was lifted by missing mandatory parse");
            }
        }

        [TestMethod]
        public void BadScriptTest()
        {
            //Arrange
            string code = @"focus = {
                                id = USA_ww1_political_effort
                                icon = GFX_goal_generic_demand_territory
                                completion_reward = { add_political_power = 120 }
                                ai_will_do = { factor = 8000 }
                                cost = 10
                                x = 3
                                y  0
                             }";
            Script script = new Script();
            //Act
            try
            {
                script.Analyse(code);
                Assert.Fail("Exception was to be lifted by broken script");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void TestAllVanilla()
        {
            //Arrange
            Script script = new Script();
            //Act
            foreach (string file in Directory.EnumerateFiles("national_focus", "*.txt"))
            {
                string contents = File.ReadAllText(file);
                script.Analyse(contents);
                //Assert
                Assert.IsTrue(script.Code.Any());
                Assert.IsFalse(ErrorLogger.Instance.hasErrors());
            }
        }
    }
}
