using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faker;
using MySqlConnector;

namespace PTMK_Task
{
    public class Employee
    {
        /*
         * Класс, на основе которого реализуется таблица в БД,
         * поля которого служат в качестве столбцов этой самой таблицы.
         */ 

        [Key]
        public int Id { get; set; }     // номер работника, в таблице является первычным ключом

        public string? Fio { get; set; }        // ФИО работника

        public DateOnly Dob { get; set; }       // дата рождения

        public string? Gender { get; set; }     // пол


        public static void DbAdd(Employee employee)
        {
            /*
             * Данный метод получает в качестве параметра экземпляр типпа 'Employee',
             * производит подключение к созданной БД и добавляет экземпляр в таблицу.
             */

            using (var db = new ApplicationContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Employee.Add(employee);
                        db.SaveChanges();
                        transaction.Commit();
                        Console.WriteLine("Работник добавлен в БД.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred: " + ex.Message);

                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        else
                        {
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        public static int GetAge(DateOnly dob)
        {
            /*
             *  Метод возвращает возвраст сотрудника,
             *  в качестве параметра получает его дату рождения.
             */

            var now = DateTime.Now;     // текущая дата

            /*
             * Производится проверка даты рождения с помощью условного оператора
             * с текущей датой для получения точного возраста сотрудника.
             */

            if (now.Month < dob.Month)
            {
                return now.Year - dob.Year - 1;
            }
            else if (now.Month == dob.Month && now.Day < dob.Day)
            {
                return now.Year - dob.Year - 1;
            }
            else
            {
                return now.Year - dob.Year;
            }
        }

        public static void BatchSend(List<Employee> employees)
        {
            /*
             * В данном методе реализована пакетная отправка данных в БД.
             * В качестве параметра метод получает список типа 'Employee'.
             */

            using (var db = new ApplicationContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Employee.AddRange(employees);
                        db.SaveChanges();
                        transaction.Commit();
                        Console.WriteLine("Пакетная отправка в БД успешно завершена.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        else
                        {
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        public static List<Employee> EmplsGenerator(int count)
        {
            /*
             * Метод используется для генирации данных сотрудников 
             * с целью дальнейшего заполнения большого количества строк в БД.
             * В качестве параметра получает количество необходимых строк,
             * возвращает список сотрудников.
             */ 

            List<Employee> employees = new List<Employee>();
            Random random = new Random();

            // цикл генерации 1000000 записей сотрудников
            for (int i = 0; i < count; i++)
            {
                string fio = Faker.Name.Last() + " " + Faker.Name.First() + " " + Faker.Name.Middle();
                DateOnly dob = DobGenerator();
                string gender = GenderGenerator();

                employees.Add(new Employee
                {
                    Fio = fio,
                    Dob = dob,
                    Gender = gender
                });
            }

            // цикл генерации 100 записей с заданными условиями по заданию
            for (int i = 0; i < 100; i++)
            {
                string fio = Faker.Name.Last().StartsWith("F") + " " + Faker.Name.First() + " " + Faker.Name.Middle();
                DateOnly dob = DobGenerator();

                employees.Add(new Employee
                {
                    Fio = fio,
                    Dob = dob,
                    Gender = "Male"
                });
            }
            Console.WriteLine("Генерация записей успешно завершена.");
            return employees;
        }

        public static void DbShow()
        {
            /*
             * Метод используется для вывода всех строк справочника сотрудников,
             * отсортированными по ФИО. Производится подключение к БД,
             * производится сортировка и вывод информации.
             */

            using (var db = new ApplicationContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var employees = db.Employee.OrderBy(e => e.Fio).ToList();

                        foreach (Employee employee in employees)
                        {
                            var age = Employee.GetAge(employee.Dob);
                            Console.WriteLine($"Имя: {employee.Fio}\nДата рождения: {employee.Dob}\n" +
                                $"Пол: {employee.Gender}\nВозраст: {age}\n");
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred: " + ex.Message);

                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        else
                        {
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        public static string GenderGenerator()
        {
            /*
             * Метод, реализующий генерацию пола сотрудников.
             */

            Random random = new Random();
            int gender = random.Next(2);
            if (gender == 1)
            {
                return "Male";
            }
            else
            {
                return "Female";
            }
        }

        public static DateOnly DobGenerator()
        {
            /*
             * Метод, реализующий генерацию даты рождения сотрудников.
             */

            Random random = new Random();
            int year = random.Next(1950, 2005);
            int month = random.Next(1, 12);
            int day = random.Next(1, 28);

            return new DateOnly(year, month, day);
        }

        public static void ResultShow()
        {
            /*
             * С помощью данного метода получаем результат выборки по заданным условиям.
             * Также измеряется и выводится затраченное время.
             * Список сотрудников из БД сохраняется в коллецию,
             * в которой происходит фильтрация по заданным условиям.
             */

            using (var db = new ApplicationContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();     

                        var employees = db.Employee.ToList().
                                                    Where(e => e.Fio.StartsWith("F") && e.Gender.Equals("Male"));

                        foreach (Employee employee in employees)
                        {
                            var age = Employee.GetAge(employee.Dob);
                            Console.WriteLine($"Имя: {employee.Fio}\nДата рождения: {employee.Dob}\n" +
                                $"Пол: {employee.Gender}\n");
                        }
                        transaction.Commit();

                        sw.Stop();
                        TimeSpan time = sw.Elapsed;
                        Console.WriteLine($"Времени потрачено: {time.TotalSeconds}с.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred: " + ex.Message);

                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        else
                        {
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        public static void ResultOptimized()
        {
            /*
             * Оптимизированный метод, выводящий также выборку и улучшенное время,
             * которое было затрачено в процессе. В методе делается запрос напрямую в БД
             * по необходиым условиям, без сохранения в коллекцию.
             */

            string connectionString = "Server=localhost;Database=ptmk;Uid=root;Pwd=***;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                string query = "SELECT fio, dob, gender FROM employee WHERE fio LIKE 'F%' and gender = 'Male'";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@value", "desired_value");

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       string fio = reader.GetString(0);
                       DateOnly dob = reader.GetDateOnly(1);
                       string gender = reader.GetString(2);
                       Console.WriteLine($"Имя: {fio}\nДата рождения: {dob}\nПол: {gender}\n");
                    }
                }

                sw.Stop();
                TimeSpan time = sw.Elapsed;
                Console.WriteLine($"Времени потрачено после оптимизации: {time.TotalSeconds}c.");
            }
        }
    }
}
