using System.Linq;
using KinoDnes.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinoDnesTest
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Test_UnknownCommand()
        {
            var controller = new MessengerBotController();
            var response = controller.GetResponses("test").ToList();
            Assert.IsTrue(response.Any(r => r.Contains("Zadejte")));
        }

        [TestMethod]
        public void Test_KnownCommand()
        {
            var controller = new MessengerBotController();
            var response = controller.GetResponses("Brno zitra").ToList();
            // More than 300 is not default message
            Assert.IsTrue(response.Any(r => r.Length > 300));
        }
    }
}