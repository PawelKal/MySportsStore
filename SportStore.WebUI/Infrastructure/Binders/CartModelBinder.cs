
using System.Web.Mvc;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //utworzenie obiektu klasy Cart
            Cart cart = null;
            //pobranie obiektu Cart z sesji
            if (controllerContext.HttpContext.Session != null)
                cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            //utworzenie obiektu jeżeli nie został znaleziony w danej sesji
            if(cart==null)
            {
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null)
                    controllerContext.HttpContext.Session[sessionKey] = cart;
            }
            return cart;
        }
    }
}