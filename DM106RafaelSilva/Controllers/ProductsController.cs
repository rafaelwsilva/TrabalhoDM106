using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DM106RafaelSilva.Models;
using System.Diagnostics;

namespace DM106RafaelSilva.Controllers
{
    public class ProductsController : ApiController
    {
        private DM106RafaelSilvaContext db = new DM106RafaelSilvaContext();
        private DM106RafaelSilvaContext db_check = new DM106RafaelSilvaContext();

        // GET: api/Products
        [Authorize]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        [Authorize]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            var prod = db_check.Products.Find(id);
            if (prod != null)
            {
                if (CheckModel(product))
                {
                    return BadRequest("This model already registered");
                }

                if (CheckProductCode(product))
                {
                    return BadRequest("This product code already registered");
                }
            }
            else
            {
                return NotFound();

            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (CheckModel(product))
            {
                return BadRequest("This model already registered");
            }

            if (CheckProductCode(product))
            {
                return BadRequest("This product code already registered");
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }

        private bool CheckModel(Product product)
        {
            var find_product = db_check.Products.FirstOrDefault(p => p.modelo == product.modelo);
            return (find_product != null && find_product.Id != product.Id);
        }

        private bool CheckProductCode(Product product)
        {
            var find_product = db_check.Products.FirstOrDefault(p => p.codigo == product.codigo);
            return (find_product != null && find_product.Id != product.Id);
        }
    }
}