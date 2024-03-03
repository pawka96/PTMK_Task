using System;
using Microsoft.EntityFrameworkCore;
using System.Data.Sql;

namespace PTMK_Task
{
    public class ApplicationContext : DbContext
    {
        /*
         * Класс, используемый для реализации БД.
         */

        public DbSet<Employee> Employee { get; set; } = null!;  // реализация таблицы, содержащей список сотрудников

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
             * Метод, который осущесвляет подключение к БД с заданными параметрами
             */

            optionsBuilder.UseMySql("server=localhost;user=root;password=***;database=ptmk;",
                new MySqlServerVersion(new Version(8, 1, 0)));
        }
    }
}
