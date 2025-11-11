using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab5_Variant12
{
   public class Program
    {
        static string filePath = "concerts.txt"; // Шлях до файлу для збереження даних

        static void Main(string[] args) 
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("==============================================");
                Console.WriteLine("     Лабораторна робота №5 (Варіант 12)");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Завдання 1: База даних концертів");
                Console.WriteLine("2. Завдання 2: Абстрактний клас і наслідування");
                Console.WriteLine("0. Вихід з програми");
                Console.WriteLine("==============================================");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ExecuteTask1();
                        break;
                    case "2":
                        ExecuteTask2();
                        break;
                    case "0":
                        Console.WriteLine("\nЗавершення програми!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\nПомилка: невірний вибір. Спробуйте ще раз.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nНатисніть Enter, щоб повернутися до головного меню...");
                    Console.ReadLine();
                }
            }
        }

        // ==============================================================
        //                      ЗАВДАННЯ 1
        // ==============================================================
        static void ExecuteTask1()
        {
            Console.Clear();
            Console.WriteLine("*** Завдання 1: База даних концертів ***\n");

            List<Concert> concerts = new List<Concert> // Початкові дані 
            {
                new Concert("Коваленко", "Рок", new DateTime(2024, 5, 5), 1200),
                new Concert("Іваненко", "Поп", new DateTime(2024, 6, 10), 950),
                new Concert("Мельник", "Джаз", new DateTime(2024, 7, 3), 800),
                new Concert("Шевченко", "Реп", new DateTime(2024, 8, 1), 700),
                new Concert("Федоренко", "Класика", new DateTime(2024, 9, 12), 1000)
            };

            bool exit = false; // Прапорець для виходу з меню
            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("A – Додати запис");
                Console.WriteLine("E – Редагувати запис");
                Console.WriteLine("D – Видалити запис");
                Console.WriteLine("S – Показати всі записи");
                Console.WriteLine("C – Обчислити результати");
                Console.WriteLine("Enter – Вихід");
                Console.Write("\nВаш вибір: ");

                string key = Console.ReadLine().ToUpper();

                switch (key)
                {
                    case "A": AddConcert(concerts); break;
                    case "E": EditConcert(concerts); break;
                    case "D": DeleteConcert(concerts); break;
                    case "S": ShowConcerts(concerts); break;
                    case "C": CalculateResults(concerts); break;
                    case "":
                        SaveToFile(concerts);
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Помилка: невірний вибір!");
                        break;
                }
            }
        }

        // ==================== Методи для Завдання 1 ====================
        static void AddConcert(List<Concert> concerts) // Додавання нового концерту
        {
            Console.WriteLine("\n--- Додавання нового концерту ---");

            string artist;
            while (true)
            {
                Console.Write("Прізвище виконавця: ");
                artist = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(artist) && artist.Length >= 4)
                    break;
                Console.WriteLine("Помилка: потрібно мінімум 4 літери. Спробуйте ще раз.");
            }

            string genre; 
            while (true)
            {
                Console.Write("Жанр: ");
                genre = Console.ReadLine(); 
                if (!string.IsNullOrWhiteSpace(genre) && genre.Length >= 2) 
                    break;
                Console.WriteLine("Помилка, спробуйте ще раз.");
            }

            DateTime date;
            while (true)
            {
                Console.Write("Дата концерту (ДД.ММ.РРРР): ");
                string dateStr = Console.ReadLine();
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out date))
                    break;
                Console.WriteLine("Помилка: введіть дату у форматі ДД.ММ.РРРР.");
            }

            int audience; 
            while (true)
            {
                Console.Write("Кількість глядачів: ");
                if (int.TryParse(Console.ReadLine(), out audience) && audience > 0) 
                    break;
                Console.WriteLine("Помилка: введіть додатне число.");
            }

            concerts.Add(new Concert(artist, genre, date, audience));
            Console.WriteLine("Концерт успішно додано!");
        }
        // Редагування існуючого концерту
        static void EditConcert(List<Concert> concerts) 
        {
            Console.Write("\nВведіть прізвище виконавця для редагування: ");
            string artist = Console.ReadLine();

            Concert concert = concerts.Find(c => c.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase)); /* Пошук концерту 
                                                                                                       за прізвищем виконавця */

            if (concert == null) 
            {
                Console.WriteLine("Запис не знайдено!");
                return;
            }

            Console.WriteLine("Редагування запису (натисніть Enter, щоб залишити старе значення):");

            Console.Write($"Жанр ({concert.Genre}): "); // Редагування жанру
            string genre = Console.ReadLine(); // Якщо введено нове значення, оновлюємо його
            if (!string.IsNullOrWhiteSpace(genre)) concert.Genre = genre; // якщо ні залишаємо старе значення

            Console.Write($"Дата ({concert.Date:dd.MM.yyyy}): ");
            string dateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateStr) &&
                DateTime.TryParseExact(dateStr, "dd.MM.yyyy", null, // Парсинг дати
                System.Globalization.DateTimeStyles.None, out DateTime d)) // Оновлення дати, якщо введено нове значення
                concert.Date = d; 

            Console.Write($"Кількість глядачів ({concert.Audience}): ");
            string audStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(audStr) && int.TryParse(audStr, out int a)) 
                concert.Audience = a; 

            Console.WriteLine("Запис оновлено!");
        }
        // Видалення концерту
        static void DeleteConcert(List<Concert> concerts)
        {
            Console.Write("\nВведіть прізвище виконавця для видалення: ");
            string artist = Console.ReadLine();
            // Пошук концерту за прізвищем виконавця
            Concert concert = concerts.Find(c => c.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase)); 

            if (concert != null)
            {
                concerts.Remove(concert);
                Console.WriteLine("Запис видалено!");
            }
            else
                Console.WriteLine("Запис не знайдено!");
        }
        // Показ усіх концертів
        static void ShowConcerts(List<Concert> concerts)
        {
            Console.WriteLine("\n--- Усі концерти ---");
            foreach (var c in concerts) 
                c.Display();
        }
        // Обчислення результатів
        static void CalculateResults(List<Concert> concerts)
        {
            Console.WriteLine("\n--- Обчислення результатів ---");

            int total = 0; // Загальна кількість глядачів
            Concert maxConcert = concerts[0]; 
            foreach (var c in concerts) 
            {
                total += c.Audience; // Підрахунок загальної кількості глядачів
                if (c.Audience > maxConcert.Audience) // Пошук концерту з максимальною кількістю глядачів
                    maxConcert = c; 
            }

            Console.WriteLine($"1️.Загальна кількість глядачів: {total}");
            Console.WriteLine("2️.Концерт з максимальною кількістю глядачів:");
            maxConcert.Display();

            int wordCount = maxConcert.CountGenreWords(); // Підрахунок кількості слів у назві жанру
            Console.WriteLine($"\n3️.Кількість слів у назві жанру \"{maxConcert.Genre}\": {wordCount}"); 
        }
        // Збереження у файл 
        static void SaveToFile(List<Concert> concerts)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8)) // Запис у файл
            {
                foreach (var c in concerts) 
                {
                    sw.WriteLine($"{c.Artist};{c.Genre};{c.Date:dd.MM.yyyy};{c.Audience}"); 
                }
            }
        }

        // ==============================================================
        //                      ЗАВДАННЯ 2
        // ==============================================================
        static void ExecuteTask2()
        {
            Console.Clear();
            Console.WriteLine("*** Завдання 2: Абстрактний клас і наслідування ***\n");
           
            List<ConcertDerived> concerts = new List<ConcertDerived>
            {
                new ConcertDerived("Коваленко", "Рок", new DateTime(2024,5,5),1200),
                new ConcertDerived("Іваненко", "Поп", new DateTime(2024,6,10),950),
                new ConcertDerived("Мельник", "Джаз", new DateTime(2024,7,3),800),
                new ConcertDerived("Шевченко", "Реп", new DateTime(2024,8,1),700),
                new ConcertDerived("Федоренко", "Класика", new DateTime(2024,9,12),1000)
            };

            Console.WriteLine("--- Усі концерти ---");
            foreach (var c in concerts)
                c.Display();

            Console.WriteLine("\n--- Результати ---");
            ConcertDerived.CalculateResults(concerts);
        }
    }

    // ==============================================================
    //                  АБСТРАКТНИЙ БАЗОВИЙ КЛАС
    // ==============================================================
    abstract class ConcertBase
    {
        public string Artist { get; set; } 
        public string Genre { get; set; }
        public DateTime Date { get; set; }
        public int Audience { get; set; }
        // Конструктор
        protected ConcertBase(string artist, string genre, DateTime date, int audience) 
        {
            Artist = artist; 
            Genre = genre;
            Date = date;
            Audience = audience;
        }
        // Абстрактний метод для відображення інформації про концерт
        public abstract void Display();
    }

    // ==============================================================
    //                   ПОХІДНИЙ КЛАС
    // ==============================================================
    class ConcertDerived : ConcertBase
    { 
        public ConcertDerived(string artist, string genre, DateTime date, int audience) 
            : base(artist, genre, date, audience) { }
        // Реалізація абстрактного методу Display
        public override void Display()
        { // Відображення інформації про концерт
            Console.WriteLine($"{Artist,-15} | {Genre,-12} | {Date:dd.MM.yyyy} | {Audience} глядачів"); 
        }
        // Підрахунок кількості слів у назві жанру
        public virtual int CountGenreWords()
        {
            return Genre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length; 
        }
        // Статичний метод для обчислення результатів
        public static void CalculateResults(List<ConcertDerived> concerts)
        {
            int total = 0; 
            ConcertDerived maxConcert = concerts[0];
            foreach (var c in concerts) /* Підрахунок загальної кількості глядачів
              і пошук концерту з максимальною кількістю глядачів */
            {
                total += c.Audience; // Загальна кількість глядачів
                if (c.Audience > maxConcert.Audience) 
                    maxConcert = c; 
            }

            Console.WriteLine($"1️.Загальна кількість глядачів: {total}");
            Console.WriteLine("2️.Концерт з максимальною кількістю глядачів:");
            maxConcert.Display();

            int wordCount = maxConcert.CountGenreWords();
            Console.WriteLine($"\n3️.Кількість слів у назві жанру \"{maxConcert.Genre}\": {wordCount}");
        }
    }

    // ==============================================================
    //                   КЛАС ЗАВДАННЯ 1 
    // ==============================================================
    public class Concert 
    { // Властивості концерту
        public string Artist { get; set; }
        public string Genre { get; set; }
        public DateTime Date { get; set; }
        public int Audience { get; set; }
        // Конструктор
        public Concert(string artist, string genre, DateTime date, int audience)
        {
            Artist = artist;
            Genre = genre;
            Date = date;
            Audience = audience;
        }
        // Метод для відображення інформації про концерт
        public void Display()
        {
            Console.WriteLine($"{Artist,-15} | {Genre,-12} | {Date:dd.MM.yyyy} | {Audience} глядачів");
        }
        // Метод для підрахунку кількості слів у назві жанру
        public virtual int CountGenreWords()
        {
            return Genre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length; 
        }
    }
}
