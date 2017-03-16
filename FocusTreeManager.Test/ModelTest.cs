using System;
using System.Linq;
using FocusTreeManager.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FocusTreeManager.Test
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void TestAssignationModel()
        {
            //Arrange
            AssignationModel model = new AssignationModel();
            AssignationModel child = new AssignationModel();
            //Act
            model.Childrens.Add(child);
            AssignationModel clone = model.Clone() as AssignationModel;
            //Assert
            Assert.IsNotNull(clone);
            Assert.IsTrue(model.Childrens.Any());
            Assert.IsTrue(clone.Childrens.Any());
            Assert.IsTrue(clone.IsCloned);
        }

        [TestMethod]
        public void TestEventModel()
        {
        }

        [TestMethod]
        public void TestEventDescriptionModel()
        {
        }

        [TestMethod]
        public void TestEventOptionModel()
        {
        }

        [TestMethod]
        public void TestFocusModel()
        {
        }

        [TestMethod]
        public void TestMutuallyExclusiveModel()
        {
        }

        [TestMethod]
        public void TestPrerequisiteModel()
        {
        }

        [TestMethod]
        public void TestLocaleModel()
        {
        }

        [TestMethod]
        public void TestProjectModel()
        {
        }
    }
}
