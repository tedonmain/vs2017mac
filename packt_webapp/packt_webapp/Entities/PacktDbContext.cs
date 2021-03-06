﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;

namespace packt_webapp.Entities
{
    public class PacktDbContext : DbContext
    {
        public PacktDbContext(DbContextOptions<PacktDbContext>options) : base(options)
        {
            
        }

        public DbSet<Customer> Customers { get; set; }
    }
}

