using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {

            // przygotowanie - utworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // przygotowanie - utworzenie nowego koszyka
            Cart target = new Cart();

            // działanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] result = target.Lines.ToArray();
            // asercje
            Assert.IsTrue(result.Length == 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);

        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {

            // przygotowanie - utworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // przygotowanie - utworzenie nowego koszyka
            Cart target = new Cart();

            // działanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] result = target.Lines.ToArray();

            // asercje
            Assert.IsTrue(result.Length == 2);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);

        }

        [TestMethod]
        public void Can_Remove_Line()
        {

            // przygotowanie - utworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            // przygotowanie - utworzenie nowego koszyka
            Cart target = new Cart();
            // przygotowanie - dodanie kilku produktów do koszyka
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // działanie
            target.RemoveLine(p2);

            // asercje
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {

            // przygotowanie - utworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // przygotowanie - utworzenie nowego koszyka
            Cart target = new Cart();

            // działanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // asercje
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {

            // przygotowanie - utworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // przygotowanie - utworzenie nowego koszyka
            Cart target = new Cart();

            // przygotowanie - dodanie kilku produktów do koszyka
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            // działanie - czyszczenie koszyka
            target.Clear();

            // asercje
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
              {
                new Product{ProductID=1, Name="P1",Category="Jab"}
              });
            //przygotowanie - utworzenie koszyka
            Cart cart = new Cart();
            //przygotowanie - utworzenie kontrolera
            CartController target = new CartController(mock.Object);

            //działanie- dodanie produktu do koszyka
            target.AddToCart(cart, 1, null);
            //assercja
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.Name, "P1");
        }
        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //przygotowanie - tworzenie imitacji
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
              {
                new Product{ProductID=1, Name="P1",Category="Jabłka"}
              }.AsQueryable());
            //przygotowanie - utworzenie koszyka
            Cart cart = new Cart();

            CartController target = new CartController(mock.Object);
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //assercje
            Assert.AreEqual(result.RouteValues["action"],"Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }
        [TestMethod]
        public void Can_View_Cart_Contest()
        {
            //przygotowanie 
            Cart cart = new Cart();
            CartController target = new CartController(null);
            //działanie
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            //asercje
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
