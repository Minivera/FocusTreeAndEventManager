using System.Collections.Generic;
using System.IO;
using System.Linq;
using FocusTreeManager.CodeStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FocusTreeManager.Model.TabModels;
using FocusTreeManager.Parsers;
using FocusTreeManager.DataContract;

namespace FocusTreeManager.Test
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void TestFocusParser()
        {
            FocusGridModel model = TestScriptToTree();
            TestTreeToScript(model);
            TestParseAllTrees();
        }

        private FocusGridModel TestScriptToTree()
        {
            //Act
            FocusGridModel model = FocusTreeParser.CreateTreeFromScript("usa.txt", File.ReadAllText("usa.txt"));
            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.FociList.Any());
            Assert.AreEqual(model.TAG, "USA");
            Assert.AreEqual(model.VisibleName, "usa_focus");
            return model;
        }

        private void TestTreeToScript(FocusGridModel model)
        {
            //Arrange
            FociGridContainer container = new FociGridContainer(model);
            List<FociGridContainer> list = new List<FociGridContainer> { container };
            Script tester = new Script();
            //Act
            Dictionary<string, string> files = FocusTreeParser.ParseAllTrees(list);
            string filecontent = files.FirstOrDefault().Value;
            string fileName = files.FirstOrDefault().Key;            
            //Assert
            Assert.IsNotNull(filecontent);
            Assert.IsNotNull(fileName);
            Assert.AreEqual(fileName, "usa_focus");
            //Test if we can process the script
            tester.Analyse(filecontent);
            Assert.IsFalse(tester.Logger.hasErrors());
            Assert.IsNotNull(tester.FindAssignation("tag"));
        }

        private void TestParseAllTrees()
        {
            //Arrange
            Script tester = new Script();
            foreach (string file in Directory.EnumerateFiles("national_focus", "*.txt"))
            {
                string contents = File.ReadAllText(file);
                //Act
                FocusGridModel model = FocusTreeParser.CreateTreeFromScript(file, contents);
                FociGridContainer container = new FociGridContainer(model);
                List<FociGridContainer> list = new List<FociGridContainer> { container };
                Dictionary<string, string> files = FocusTreeParser.ParseAllTrees(list);
                string filecontent = files.FirstOrDefault().Value;
                string fileName = files.FirstOrDefault().Key;
                //Assert
                Assert.IsNotNull(model);
                Assert.IsTrue(model.FociList.Any());
                Assert.IsNotNull(fileName);
                Assert.IsNotNull(filecontent);
                tester.Analyse(filecontent);
                Assert.IsFalse(tester.Logger.hasErrors());
            }
        }

        [TestMethod]
        public void TestLocalizationParser()
        {
            LocalisationModel model = TestScriptToLocale();
            TestLocaleToScript(model);
        }

        private LocalisationModel TestScriptToLocale()
        {
            //Act
            LocalisationModel model = LocalisationParser.CreateLocaleFromFile("focus_l_english.yml");
            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.LocalisationMap.Any());
            Assert.AreEqual(model.LanguageName, "l_english");
            Assert.AreEqual(model.VisibleName, "focus_l_english");
            return model;
        }

        private void TestLocaleToScript(LocalisationModel model)
        {
            //Arrange
            LocalisationContainer container = new LocalisationContainer(model);
            List<LocalisationContainer> list = new List<LocalisationContainer> { container };
            //Act
            Dictionary<string, string> files = LocalisationParser.ParseEverything(list);
            string filecontent = files.FirstOrDefault().Value;
            string fileName = files.FirstOrDefault().Key;
            //Assert
            Assert.IsNotNull(filecontent);
            Assert.IsNotNull(fileName);
            Assert.AreEqual(fileName, "focus_l_english");
        }

        [TestMethod]
        public void TestEventParser()
        {
            EventTabModel model = TestScriptToEvent();
            TestEventToScript(model);
            TestParseAllEvents();
        }

        private EventTabModel TestScriptToEvent()
        {
            //Arrange
            string eventScript = File.ReadAllText("Baltic.txt");
            //Act
            EventTabModel model = EventParser.CreateEventFromScript("Baltic.txt", eventScript);
            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(model.EventList.Any());
            Assert.AreEqual(model.EventNamespace, "baltic");
            Assert.AreEqual(model.VisibleName, "Baltic");
            return model;
        }

        private void TestEventToScript(EventTabModel model)
        {
            //Arrange
            EventContainer container = new EventContainer(model);
            List<EventContainer> list = new List<EventContainer> { container };
            //Act
            Dictionary<string, string> files = EventParser.ParseAllEvents(list);
            string filecontent = files.FirstOrDefault().Value;
            string fileName = files.FirstOrDefault().Key;
            //Assert
            Assert.IsNotNull(filecontent);
            Assert.IsNotNull(fileName);
            Assert.AreEqual(fileName, "Baltic");
        }

        private void TestParseAllEvents()
        {
            //Arrange
            Script tester = new Script();
            foreach (string file in Directory.EnumerateFiles("events", "*.txt"))
            {
                string contents = File.ReadAllText(file);
                //Act
                EventTabModel model = EventParser.CreateEventFromScript(file, contents);
                EventContainer container = new EventContainer(model);
                List<EventContainer> list = new List<EventContainer> { container };
                Dictionary<string, string> files = EventParser.ParseAllEvents(list);
                string filecontent = files.FirstOrDefault().Value;
                string fileName = files.FirstOrDefault().Key;
                //Assert
                Assert.IsNotNull(model);
                Assert.IsTrue(model.EventList.Any());
                Assert.IsNotNull(fileName);
                Assert.IsNotNull(filecontent);
                tester.Analyse(filecontent);
                Assert.IsFalse(tester.Logger.hasErrors());
            }
        }

        [TestMethod]
        public void TestScriptParser()
        {
            ScriptModel model = TestScriptToScript();
            TestScriptToScript(model);
        }

        private ScriptModel TestScriptToScript()
        {
            //Act
            ScriptModel model = ScriptParser.CreateScriptFromFile("AFG - Afghanistan.txt");
            //Assert
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.InternalScript);
            Assert.AreEqual(model.VisibleName, "AFG - Afghanistan");
            return model;
        }

        private void TestScriptToScript(ScriptModel model)
        {
            //Arrange
            ScriptContainer container = new ScriptContainer()
            {
                InternalScript = model.InternalScript,
                FileInfo = model.FileInfo,
                ContainerID = model.VisibleName
            };
            List<ScriptContainer> list = new List<ScriptContainer> { container };
            //Act
            Dictionary<string, string> files = ScriptParser.ParseEverything(list);
            string filecontent = files.FirstOrDefault().Value;
            string fileName = files.FirstOrDefault().Key;
            //Assert
            Assert.IsNotNull(filecontent);
            Assert.IsNotNull(fileName);
            Assert.AreEqual(fileName, "AFG - Afghanistan");
        }
    }
}
