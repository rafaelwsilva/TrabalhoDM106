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
using DM106RafaelSilva.CRMClient;
using DM106RafaelSilva.br.com.correios.ws;
using System.Globalization;

namespace DM106RafaelSilva.Controllers
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private DM106RafaelSilvaContext db = new DM106RafaelSilvaContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
                return BadRequest("Order Not Found");

            if (!CheckUser(order.UserName))
                return Unauthorized();
            
            return Ok(order);
        }

        // GET: api/Orders/byemail?email={email}
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrdersByEmail(String email)
        {
            var orders = db.Orders.Where(o => o.UserName == email);
            if (orders == null)
                return BadRequest("Orders not found for this User");
            
            if (!CheckUser(orders.First().UserName))
                return Unauthorized();

            return Ok(orders.Include(order => order.OrderItems).ToList());
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // PUT: api/Orders/OrderClose?id={id}
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("orderclose")]
        public IHttpActionResult PutOrderClose(int id)
        {
            var order = db.Orders.Find(id);

            if (order == null)
                return BadRequest("Order not Found!");

            if (!CheckUser(order.UserName))
                return Unauthorized();

            if (order.Shipping != 0)
            {
                order.Status = "Fechado";
                db.Entry(order).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Orders/CalcShipping?id={id}
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("CalcShipping")]
        public IHttpActionResult PutOrderCalcShipping(int id)
        {
            String cepDestino = null;
            Order order = db.Orders.Find(id);
            if (order == null)
                return BadRequest("Order not found!");

            if (!CheckUser(order.UserName))
                return Unauthorized();

            if ((order.Status != "Novo") && (order.Shipping != 0))
                return BadRequest("Order Status invalid! Expected: 'Novo'");
            
            try
            {
                CRMRestClient crmClient = new CRMRestClient();
                Customer customer = crmClient.GetCustomerByEmail(order.UserName);
                if (customer == null)
                    return BadRequest("Customer not found!");
                cepDestino = customer.zip;
            }
            catch (Exception)
            {
                return BadRequest("CRM not available!");
            }

            List<OrderItem> items = order.OrderItems.ToList();

            if (items.Count == 0)
                return BadRequest("This Ordens has no item!");

            Random random = new Random();
            decimal comprimento = random.Next(16, 105);
            decimal largura = random.Next(11, 105);
            decimal diametro = random.Next(5, 91);
            decimal altura = random.Next(2, 105);

            double weight = random.NextDouble() * 30.0;
            decimal price = 0;
            
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado result = correios.CalcPrecoPrazo("", "", "40010", "37540000", cepDestino,
                weight.ToString(), 1, comprimento, altura, largura, diametro, "N", price, "S");

            if (result.Servicos[0].Erro.Equals("0"))
            {
                NumberFormatInfo format = new CultureInfo("pt-BR", false).NumberFormat;

                order.TotalPrice = decimal.Parse(result.Servicos[0].Valor, format) + price;
                order.TotalWeight = Convert.ToDecimal(weight);
                order.Shipping = decimal.Parse(result.Servicos[0].Valor, format);
                order.DateDelivery = DateTime.Now.AddDays(Convert.ToDouble(result.Servicos[0].PrazoEntrega));

                db.Entry(order).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest("Error Code:" + result.Servicos[0].Erro + " Error: " + result.Servicos[0].MsgErro);
            }
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            order.UserName = User.Identity.Name;
            order.Status = "Novo";
            order.TotalWeight = 0;
            order.Shipping = 0;
            order.TotalPrice = 0;
            order.DateOrder = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
                return BadRequest("Order Not Found");

            if (!CheckUser(order.UserName))
                return Unauthorized();
            
            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }

        private bool CheckUser(String userName)
        {
            Trace.TraceInformation("UserName: " + User.Identity.Name);
            return ((User.IsInRole("ADMIN")) || (User.Identity.Name == userName));
        }
    }
}