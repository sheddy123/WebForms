using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms.Models;

namespace WebForms.Logic
{
    public class AddProducts
    {
        public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
        {
            var myProduct = new Product();
            myProduct.ProductName = ProductName;
            myProduct.Description = ProductDesc;
            myProduct.UnitPrice = Convert.ToDouble(ProductPrice);
            myProduct.ImagePath = ProductImagePath;
            myProduct.CategoryID = Convert.ToInt32(ProductCategory);
            using (ProductContext db = new ProductContext())
            {
                db.Products.Add(myProduct);
                db.SaveChanges();
            }
            return true;
        }
    }
}