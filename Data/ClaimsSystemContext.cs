using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClaimsSystem.Models;

//claimsystemcontext
    public class ClaimsSystemContext : DbContext
    {
        public ClaimsSystemContext (DbContextOptions<ClaimsSystemContext> options)
            : base(options)
        {
        }

        public DbSet<ClaimsSystem.Models.User> User { get; set; } = default!;

public DbSet<ClaimsSystem.Models.Claim> Claim { get; set; } = default!;
    }
