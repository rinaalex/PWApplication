using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataLayer.EfCode;
using ServiceLayer.Transfers;
using ServiceLayer.Transfers.Concrete;
using ServiceLayer.Transfers.QueryObjects;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PwWebApp.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly PwContext context;

        public TransactionsController(PwContext context)
        {
            this.context = context;
        }

        // GET: api/<controller>
        //[HttpGet]
        //public IQueryable<TransactionListDto> Get()
        //{
        //    ListTransactionService service = new ListTransactionService(context);
        //    SortFilterOptions optrions = new SortFilterOptions();
        //    optrions.OrderByOptions = OrderByOptions.ByDateTimeDesc;
        //    int userId = 3;            
        //    var transactions = service.GetList(userId, optrions);
        //    return transactions;
        //}

        [HttpGet]
        public IEnumerable<TransactionListDto> Get()
        {
            ListTransactionService service = new ListTransactionService(context);
            SortFilterOptions optrions = new SortFilterOptions();
            optrions.OrderByOptions = OrderByOptions.ByDateTimeDesc;
            int userId = 3;
            var transactions = service.GetList(userId, optrions);

            string s = null; 
            foreach(var tr in transactions)
            {
                s += tr.ToString() + ";";
            }

            //TransactionListDto dto = new TransactionListDto
            //{
            //    TransferId = 1,
            //    Correspondent = "1",
            //    Amount = 2,
            //    Timestamp = DateTime.Now,
            //    Type = "2"
            //};
            //List<TransactionListDto> tr = new List<TransactionListDto>
            //{
            //    dto
            //};
            int c = transactions.Count();
            return transactions;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
