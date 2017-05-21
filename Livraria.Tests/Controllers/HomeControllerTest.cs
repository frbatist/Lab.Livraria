using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab.Livraria.Controllers;
using System.Web.Mvc;

namespace Livraria.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {            
            HomeController controller = new HomeController();
            
            ViewResult result = controller.Index() as ViewResult;
            
            Assert.IsNotNull(result);
        }
    }
}
