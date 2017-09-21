using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using packt_webapp.Entities;

namespace packt_webapp.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private PacktDbContext _context;

        public CustomerRepository(PacktDbContext context)
        {
            _context = context;
        }

		public IQueryable<Customer> GetAll()
		{
			return _context.Customers;
		}

		public Customer GetSingle(Guid id)
		{
			return _context.Customers.FirstOrDefault(x => x.Id == id);
		}

		public void Add(Customer item)
		{
			_context.Customers.Add(item);
		}

		public void Delete(Guid id)
		{
			Customer Customer = GetSingle(id);
			_context.Customers.Remove(Customer);
		}

		public void Update(Customer item)
		{
			_context.Customers.Update(item);
		}

		public bool Save()
		{
			return _context.SaveChanges() >= 0;
		}

	}
}
