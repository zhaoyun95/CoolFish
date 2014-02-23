using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoolFishTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mock = new Moq.Mock<CoolFishNS.Bots.CoolFishBot.CoolFishBot>();

           mock.Object.StartBot();
        }
    }
}
