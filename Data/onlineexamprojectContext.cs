using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using onlineexamproject.Models;

namespace onlineexamproject.Data
{
    public class onlineexamprojectContext : DbContext
    {
        public onlineexamprojectContext (DbContextOptions<onlineexamprojectContext> options)
            : base(options)
        {
        }

        public DbSet<onlineexamproject.Models.Course> Course { get; set; } = default!;
        public DbSet<onlineexamproject.Models.Exam> Exam { get; set; } = default!;
        public DbSet<onlineexamproject.Models.Questions> Questions { get; set; } = default!;
        public DbSet<onlineexamproject.Models.Result> Result { get; set; } = default!;
    }
}
