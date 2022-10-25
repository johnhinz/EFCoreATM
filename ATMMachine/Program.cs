using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ATMMachine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Customer cust = new Customer()
            //{ 
            //    Name = "Joe Smith", 
            //    Dob = new DateTime(1969, 12, 15), 
            //    Sex = "M", 
            //    SIN = "235-968-789" 
            //};

            //Account acct = new Account()
            //{
            //    Amount = 500,
            //    DateOf = DateTime.Now,
            //    Description = "Initial Deposit",
            //    Owner = cust,
            //    TransactionType = 1
            //};
            //ATMContext context = new ATMContext();
            //context.Accounts.Add(acct);
            //context.SaveChanges();

            ATMContext context = new ATMContext();

            Customer cust = context.Customers.Include(c => c.Accounts).FirstOrDefault(c => c.Id == 1);
            
            cust.Accounts.Add(new Account()
            {
                Amount = 20,
                DateOf = DateTime.Now,
                Description = "Got paid .... not enough!",
                //Owner = 1
                TransactionType = 1
            });
            context.SaveChanges();
            Console.WriteLine(GetBalance(1));

        }

        private static decimal GetBalance(int CustomerId)
        {
            ATMContext context = new ATMContext();
            var result = context.Accounts.Where(a => a.Owner.Id == CustomerId);
            decimal balance = 0;
            foreach (Account acct in result)
            {
                if (acct.TransactionType == 0)
                {
                    balance = balance - acct.Amount;
                } else if (acct.TransactionType == 1)
                {
                    balance = balance + acct.Amount;
                }
            }
            return balance;

        }

    }

    public class ATMContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=ATM_DB;Integrated Security=True;");
            builder.LogTo(message => Debug.WriteLine(message));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(p => p.Name)
                  .HasMaxLength(50);
            });
        }
    }


    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Sex { get; set; }
        public string SIN { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public DateTime DateOf { get; set; }
        public string Description { get; set; }
        // 0 = Debit, 1 = Credit
        public byte TransactionType { get; set; }
        public decimal Amount { get; set; }

        public virtual Customer Owner { get; set; }

    }




}