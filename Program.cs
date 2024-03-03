using PTMK_Task;

class Programm
{
    static void Main()
    {
        int number;     // номер режима работы

        do
        {
            Console.WriteLine("Введите номер режима работы приложения или введите 0 для выхода: ");

            if (int.TryParse(Console.ReadLine(), out number))
            {
                switch (number) {
                    
                    // создание БД с таблицей справочника сотрудников
                    case 1:
                        using (var db = new ApplicationContext())
                        {     
                          Console.WriteLine("Создана таблица в БД справочника сотрудников.");
                        }
                        break;

                    // создание записи сотрудника
                    case 2:
                        try
                        {
                            string? input;

                            do
                            {
                                Console.WriteLine("Введите ФИО работника: ");
                                input = Console.ReadLine();

                                Employee employee = new Employee
                                {
                                    Fio = input
                                };

                                Console.WriteLine("Введите дату рождения: ");
                                input = Console.ReadLine();
                                employee.Dob = input != null ? DateOnly.Parse(input) : default;

                                Console.WriteLine("Введите пол: ");
                                employee.Gender = Console.ReadLine();

                                Employee.DbAdd(employee);

                                Console.WriteLine("Чтобы выйти из режима введите 'exit' или любой символ, чтобы продолжить.");
                                input = Console.ReadLine();

                            } while (!input.Equals("exit"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + " Неправильный формат ввода.");
                            continue;
                        }
                        break;

                    // вывод всех строк справочника сотрудников
                    case 3:
                        Employee.DbShow();
                        break;

                    /*
                     * Генерация заданного количества строк записей для сотрудников
                     * и пакетная отправка их в БД.
                     */
                        
                    case 4:
                        List<Employee> employees = Employee.EmplsGenerator(1000000);
                        Employee.BatchSend(employees);
                        break;

                    // результат выборки со временем по заданному критерию
                    case 5:
                        Employee.ResultShow();
                        break;

                    // оптимизированный резьтут выборки со временем
                    case 6:
                        Employee.ResultOptimized();
                        break;
                }
            } else
            {
                Console.WriteLine("Вы ввели неверный формат номера.");
                Main();
            }
        } while (number != 0);
    }
}