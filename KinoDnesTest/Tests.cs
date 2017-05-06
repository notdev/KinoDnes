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
            var response = controller.GetResponse("test");
            Assert.IsTrue(response.Contains("Zadejte"));
        }

        [TestMethod]
        public void Test_KnownCommand()
        {
            var controller = new MessengerBotController();
            var response = controller.GetResponse("Brno zitra");
            // More than 300 is not default message
            Assert.IsTrue(response.Length > 300);
        }
    }
}