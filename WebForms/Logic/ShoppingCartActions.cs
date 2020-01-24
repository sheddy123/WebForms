using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms.Models;

namespace WebForms.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext db = new ProductContext();

        public const string CartSessionKey = "CartId";

        public void AddToCart(int id)
        {
            ShoppingCartId = GetCartId();
            var cartItem = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == ShoppingCartId
            && c.ProductId == id);
            if(cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            db.SaveChanges();
        }
        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }

        public string GetCartId()
        {
            if(HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();
            return db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }

        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();
            decimal? total = decimal.Zero;
            total = (decimal?)(from cartItems in db.ShoppingCartItems
                               where cartItems.CartId == ShoppingCartId
                               select (int?)cartItems.Quantity *
                               cartItems.Product.UnitPrice).Sum();
            return total ?? decimal.Zero;
        }

        public ShoppingCartActions GetCart(HttpContext context)
        {
            using(var cart = new ShoppingCartActions())
            {
                cart.ShoppingCartId = cart.GetCartId();
                return cart;
            }
        }

        public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] CartItemUpdates)
        {
            using(var db = new WebForms.Models.ProductContext())
            {
                try
                {
                    int CartItemCount = CartItemUpdates.Count();
                    List<CartItem> myCart = GetCartItems();
                    foreach(var cartItem in myCart)
                    {
                        for(int i = 0; i< CartItemCount; i++)
                        {
                            if(cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
                            {
                                if(CartItemUpdates[i].PurchaseQuantity < 1 || CartItemUpdates[i].RemoveItem == true)
                                {
                                    RemoveItem(cartId, cartItem.ProductId);
                                }
                                else
                                {
                                    UpdateItem(cartId, cartItem.ProductId, CartItemUpdates[i].PurchaseQuantity);
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
                }
            }
        }

        public void RemoveItem(string removeCartID, int removeProductID)
        {
            using(var db = new WebForms.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems where c.CartId == removeCartID && c.ProductId == removeProductID select c).FirstOrDefault();
                    if(myItem != null)
                    {
                        db.ShoppingCartItems.Remove(myItem);
                        db.SaveChanges();
                    }
                }
                catch(Exception Exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Item - " + Exp.Message.ToString(), Exp);
                }
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            using(var db = new WebForms.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems
                                  where c.CartId == updateCartID && c.ProductId == updateProductID
                                  select c).FirstOrDefault();
                }
                catch(Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                db.ShoppingCartItems.Remove(cartItem);
            } 
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();
            int? count = (from cartItems in db.ShoppingCartItems
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Quantity).Sum();
            return count ?? 0;
        }
        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}