using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC;
using MVC.Controllers;
using System.Threading.Tasks;

namespace MVC.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public async void Index()
        {

            WorkbookController controller = new WorkbookController();
            ViewResult result = await controller.Index() as ViewResult;
            Assert.IsNotNull(result);
           
        }

    }
}
