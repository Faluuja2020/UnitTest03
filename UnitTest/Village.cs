using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VillageOfTesting;
using VillageOfTesting.Objects;
using VillageOfTesting.OccupationActions;
using VillageOfTesting.CompleteActions;
using VillageOfTesting.Interfaces;

namespace VillageOfTesting.Tests
{
    [TestClass]
    public class VillageTests
    {
        private Village village;

        [TestInitialize]
        public void Setup()
        {
            village = new Village();
        }

        [TestMethod]
        public void TestAddWorker()
        {
            // Arrange
            village.AddWorker("John", "farmer");

            // Act
            var worker = village.Workers[0];

            // Assert
            Assert.AreEqual(1, village.Workers.Count);
            Assert.AreEqual("John", worker.Name);
            Assert.AreEqual("farmer", worker.Occupation);
        }

        [TestMethod]
        public void TestAddMultipleWorkers()
        {
            // Arrange
            village.AddWorker("John", "farmer");
            village.AddWorker("Jane", "lumberjack");

            // Act
            var worker1 = village.Workers[0];
            var worker2 = village.Workers[1];

            // Assert
            Assert.AreEqual(2, village.Workers.Count);
            Assert.AreEqual("John", worker1.Name);
            Assert.AreEqual("Jane", worker2.Name);
            Assert.AreEqual("farmer", worker1.Occupation);
            Assert.AreEqual("lumberjack", worker2.Occupation);
        }

        [TestMethod]
        public void TestMaxWorkersLimit()
        {
            // Arrange
            for (int i = 1; i <= village.MaxWorkers; i++)
            {
                village.AddWorker("Worker" + i, "farmer");
            }
            village.AddWorker("ExtraWorker", "farmer");

            // Act & Assert
            Assert.AreEqual(village.MaxWorkers, village.Workers.Count);
            Assert.IsFalse(village.Workers.Exists(w => w.Name == "ExtraWorker"));
        }

        [TestMethod]
        public void TestDayWithoutWorkers()
        {
            // Act
            village.Day();

            // Assert
            Assert.AreEqual(1, village.DaysGone);
            Assert.AreEqual(10, village.Food); // Food should remain unchanged
        }

        [TestMethod]
        public void TestDayWithWorkersAndFood()
        {
            // Arrange
            village.AddWorker("John", "farmer");
            village.Food = 10;

            // Act
            village.Day();

            // Assert
            Assert.AreEqual(1, village.DaysGone);
            Assert.AreEqual(9, village.Food); // Food decreases by 1
            Assert.IsTrue(village.Workers[0].Alive);
        }

        [TestMethod]
        public void TestDayWithWorkersAndNoFood()
        {
            // Arrange
            village.AddWorker("John", "farmer");
            village.Food = 0;

            // Act
            village.Day();
            village.Day();
            village.Day();

            // Assert
            Assert.AreEqual(3, village.DaysGone);
            Assert.IsTrue(village.Workers[0].Hungry);
            Assert.AreEqual(3, village.Workers[0].DaysHungry);
        }

        [TestMethod]
        public void TestWorkerStarvation()
        {
            // Arrange
            village.AddWorker("John", "farmer");
            village.Food = 0;

            // Act
            for (int i = 0; i < Worker.daysUntilStarvation; i++)
            {
                village.Day();
            }

            // Assert
            Assert.IsFalse(village.Workers[0].Alive);
        }

        [TestMethod]
        public void TestAddProject()
        {
            // Arrange
            village.Wood = 10;
            village.Metal = 10;

            // Act
            village.AddProject("House");

            // Assert
            Assert.AreEqual(5, village.Wood);
            Assert.AreEqual(10, village.Metal);
            Assert.AreEqual(1, village.Projects.Count);
            Assert.AreEqual("House", village.Projects[0].Name);
        }

        [TestMethod]
        public void TestAddProjectNotEnoughMaterial()
        {
            // Arrange
            village.Wood = 3;
            village.Metal = 10;

            // Act
            village.AddProject("House");

            // Assert
            Assert.AreEqual(3, village.Wood); // Wood should remain unchanged
            Assert.AreEqual(10, village.Metal); // Metal should remain unchanged
            Assert.AreEqual(0, village.Projects.Count); // No project added
        }

        [TestMethod]
        public void TestCompleteProject()
        {
            // Arrange
            village.Wood = 10;
            village.Metal = 10;
            village.AddWorker("John", "builder");
            village.AddProject("House");

            // Act
            for (int i = 0; i < 3; i++) // Assuming it takes 3 days to complete the House project
            {
                village.Day();
            }

            // Assert
            Assert.AreEqual(0, village.Projects.Count); // Project should be completed
            Assert.AreEqual(4, village.Buildings.Count); // New building should be added
        }

        [TestMethod]
        public void TestStartToEndGame()
        {
            // Arrange
            village.Food = 100;
            village.Wood = 100;
            village.Metal = 100;

            // Act
            village.AddWorker("John", "farmer");
            village.AddWorker("Jane", "builder");

            village.AddProject("Castle");
            while (village.Projects.Count > 0)
            {
                village.Day();
            }

            // Assert
            Assert.AreEqual(1, village.Buildings.Count(b => b.Name == "Castle"));
            Assert.IsTrue(village.GameOver); // Assuming game ends when Castle is built
        }
    }
}
