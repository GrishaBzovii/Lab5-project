using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lab5_Variant12
{
    class Program
    {
        static string filePath = "concerts.txt"; // файл для збереження даних
        // Головний метод
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            bool run = true; // головний цикл
            while (run)
            {
                Console.Clear();
                Console.WriteLine("============== Лабораторна робота №5 ==============");
                Console.WriteLine("1 — Завдання 1 (звичайний клас)");
                Console.WriteLine("2 — Завдання 2 (abstract + похідний клас)");
                Console.WriteLine("0 — Вихід");
                Console.WriteLine("===================================================");
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": Task1.Execute(filePath); break;
                    case "2": Task2.Execute(filePath); break;
                    case "0": run = false; break;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    // ==========================
    //  ЗАВДАННЯ 1 БАЗОВИЙ КЛАС
    // ==========================
    public class Artist1
    {
        public string Surname { get; set; }
        public string Genre { get; set; }

        public Artist1(string s, string g)
        {
            Surname = s;
            Genre = g;
        }

        // У базовому класі реалізація методу індивідуального завдання
        public virtual int CountGenreWords()
        {
            return Genre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
    // ==========================
    //  ПОХІДНИЙ КЛАС 1 ЗАВДАННЯ
    // ==========================
    public class Concert1 : Artist1
    {
        public DateTime Date { get; set; }
        public int Audience { get; set; }
        
        public Concert1(string s, string g, DateTime d, int a)
            : base(s, g)
        {
            Date = d;
            Audience = a;
        }
        // Вивід на екран
        public void Display()
        {
            Console.WriteLine($"{Surname,-12} | {Genre,-12} | {Date:dd.MM.yyyy} | {Audience} глядачів");
        }

        // Для збереження у файл
        public string ToFileLine()
        {
            return $"{Surname};{Genre};{Date:dd.MM.yyyy};{Audience}";
        }
        // Зчитування з файлу
        public static Concert1 FromFileLine(string line)
        {
            // формат: Surname;Genre;dd.MM.yyyy;Audience
            var parts = line.Split(';');
            if (parts.Length != 4) throw new FormatException("Невірний формат рядка у файлі.");
            DateTime d = DateTime.ParseExact(parts[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
            int a = int.Parse(parts[3]);
            return new Concert1(parts[0], parts[1], d, a);
        }
    }
    // ==========================
    //  ЛОГІКА ЗАВДАННЯ 1
    // ==========================
    public static class Task1
    {
        public static void Execute(string filePath)
        {
            Console.Clear();
            Console.WriteLine("*** Завдання 1 (звичайний клас) ***\n");

            List<Concert1> concerts = LoadOrCreateDefault1(filePath);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("A – Додати запис");
                Console.WriteLine("E – Редагувати запис");
                Console.WriteLine("D – Видалити запис");
                Console.WriteLine("S – Показати всі записи");
                Console.WriteLine("C – Обчислити результати");
                Console.WriteLine("Enter – Вихід (збереження)");
                Console.Write("\nВаш вибір: ");
                string key = Console.ReadLine().ToUpper();

                switch (key)
                {
                    case "A": Add(concerts); break;
                    case "E": Edit(concerts); break;
                    case "D": Delete(concerts); break;
                    case "S": ShowAll(concerts); break;
                    case "C": Calculate(concerts); break;
                    case "":
                        SaveToFile1(concerts, filePath);
                        Console.WriteLine("Дані збережено у файлі.");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Помилка: невірний вибір!");
                        break;
                }
            }
        }
        // Завантаження з файлу або створення дефолтного списку
        static List<Concert1> LoadOrCreateDefault1(string filePath)
        {
            var list = new List<Concert1>();
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    foreach (var l in lines)
                    {
                        if (string.IsNullOrWhiteSpace(l)) continue;
                        list.Add(Concert1.FromFileLine(l));
                    }
                }
            }
            catch
            {
                // якщо файл пошкоджений — ігноруємо і створимо дефолт
                list.Clear();
            }

            if (list.Count < 5)
            {
                // початковий масив (не менше 5 записів)
                list = new List<Concert1>
                {
                    new Concert1("Коваленко","Рок", new DateTime(2024,5,5),1200),
                    new Concert1("Іваненко","Поп", new DateTime(2024,6,10),950),
                    new Concert1("Мельник","Джаз", new DateTime(2024,7,3),800),
                    new Concert1("Шевченко","Реп", new DateTime(2024,8,1),700),
                    new Concert1("Федоренко","Класика", new DateTime(2024,9,12),1000)
                };
            }

            return list;
        }
        // Додавання нового запису
        static void Add(List<Concert1> concerts)
        {
            Console.WriteLine("\n--- Додавання ---");
            string s = ReadNonEmpty("Прізвище виконавця (мінімум 2 літери): ", minLen: 2);
            string g = ReadNonEmpty("Жанр (мінімум 2 літери): ", minLen: 2);

            DateTime d = ReadDate("Дата концерту (ДД.ММ.РРРР): ");
            int a = ReadPositiveInt("Кількість глядачів: ");

            concerts.Add(new Concert1(s, g, d, a));
            Console.WriteLine("Запис успішно додано.");
        }
        // Редагування існуючого запису
        static void Edit(List<Concert1> concerts)
        {
            Console.Write("\nВведіть прізвище виконавця для редагування: ");
            string name = Console.ReadLine();
            var idx = concerts.FindIndex(c => c.Surname.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx == -1)
            {
                Console.WriteLine("Запис не знайдено.");
                return;
            }

            var c = concerts[idx];
            Console.WriteLine("Натисніть Enter щоб залишити старе значення.");

            Console.Write($"Прізвище ({c.Surname}): ");
            string s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s) && s.Length >= 2) c.Surname = s;

            Console.Write($"Жанр ({c.Genre}): ");
            string g = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(g) && g.Length >= 2) c.Genre = g;

            Console.Write($"Дата ({c.Date:dd.MM.yyyy}): ");
            string ds = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ds))
            {
                if (DateTime.TryParseExact(ds, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime d)) c.Date = d;
                else Console.WriteLine("Невірний формат дати — залишено старе.");
            }

            Console.Write($"Кількість глядачів ({c.Audience}): ");
            string asStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(asStr) && int.TryParse(asStr, out int an) && an > 0) c.Audience = an;

            Console.WriteLine("Запис оновлено.");
        }
        // Видалення запису
        static void Delete(List<Concert1> concerts)
        {
            Console.Write("\nПрізвище для видалення: ");
            string name = Console.ReadLine();
            var idx = concerts.FindIndex(c => c.Surname.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx == -1)
            {
                Console.WriteLine("Запис не знайдено.");
                return;
            }
            concerts.RemoveAt(idx);
            Console.WriteLine("Запис успішно видалено.");
        }
        // Показ усіх записів
        static void ShowAll(List<Concert1> concerts)
        {
            Console.WriteLine("\nУсі концерти:");
            foreach (var c in concerts) c.Display();
        }
        // Обчислення результатів
        static void Calculate(List<Concert1> concerts)
        {
            Console.WriteLine("\n--- Обчислення результатів ---");
            if (concerts.Count == 0)
            {
                Console.WriteLine("Немає концертів.");
                return;
            }
            int total = 0;
            Concert1 max = concerts[0];
            foreach (var c in concerts)
            {
                total += c.Audience;
                if (c.Audience > max.Audience) max = c;
            }

            Console.WriteLine($"1. Загальна кількість глядачів: {total}");
            Console.WriteLine("2. Концерт з макс. глядачами:");
            max.Display();
            Console.WriteLine($"3. Кількість слів у жанрі \"{max.Genre}\" = {max.CountGenreWords()}");
        }
        // Збереження у файл
        static void SaveToFile1(List<Concert1> concerts, string filePath)
        {
            try
            {
                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (var c in concerts) sw.WriteLine(c.ToFileLine());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка запису у файл: " + ex.Message);
            }
        }

        // --- допоміжні методи для вводу ---
        static string ReadNonEmpty(string prompt, int minLen = 1)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s) && s.Length >= minLen) return s;
                Console.WriteLine($"Помилка: потрібно мінімум {minLen} символів.");
            }
        }
        // Читання дати з консолі
        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (DateTime.TryParseExact(s, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime d)) return d;
                Console.WriteLine("Помилка: введіть дату у форматі ДД.MM.YYYY.");
            }
        }
        // Читання додатного цілого числа
        static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v > 0) return v;
                Console.WriteLine("Помилка: введіть додатне число.");
            }
        }
    }

    // ==========================
    //  ЗАВДАННЯ 2 АБСТРАКТНИЙ КЛАС
    // ==========================
    public abstract class Artist2 
    {
        public string Surname { get; set; }
        public string Genre { get; set; }

        protected Artist2(string s, string g)
        {
            Surname = s;
            Genre = g;
        }

        // тут метод тільки оголошується а реалізується в похідному класі
        public abstract int CountGenreWords();
    }
    // ==========================
    //  ПОХІДНИЙ КЛАС 2 ЗАВДАННЯ
    // ==========================
    public class Concert2 : Artist2
    {
        public DateTime Date { get; set; }
        public int Audience { get; set; }

        public Concert2(string s, string g, DateTime d, int a)
            : base(s, g)
        {
            Date = d;
            Audience = a;
        }

        // Тепер індивідуальне завдання реалізується тут в похідному класі 
        public override int CountGenreWords()
        {
            return Genre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        // Вивід на екран
        public void Display()
        {
            Console.WriteLine($"{Surname,-12} | {Genre,-12} | {Date:dd.MM.yyyy} | {Audience} глядачів");
        }
        // Для збереження у файл
        public string ToFileLine()
        {
            return $"{Surname};{Genre};{Date:dd.MM.yyyy};{Audience}";
        }
        // Зчитування з файлу
        public static Concert2 FromFileLine(string line)
        {
            var parts = line.Split(';');
            if (parts.Length != 4) throw new FormatException("Невірний формат рядка у файлі.");
            DateTime d = DateTime.ParseExact(parts[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
            int a = int.Parse(parts[3]);
            return new Concert2(parts[0], parts[1], d, a);
        }
    }
    // ==========================
    //  ЛОГІКА ЗАВДАННЯ 2
    // ==========================
    public static class Task2
    {
        public static void Execute(string filePath)
        {
            Console.Clear();
            Console.WriteLine("*** Завдання 2 (abstract + похідний) ***\n");

            List<Concert2> concerts = LoadOrCreateDefault2(filePath);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("A – Додати запис");
                Console.WriteLine("E – Редагувати запис");
                Console.WriteLine("D – Видалити запис");
                Console.WriteLine("S – Показати всі записи");
                Console.WriteLine("C – Обчислити результати");
                Console.WriteLine("Enter – Вихід (збереження)");
                Console.Write("\nВаш вибір: ");
                string key = Console.ReadLine().ToUpper();

                switch (key)
                {
                    case "A": Add(concerts); break;
                    case "E": Edit(concerts); break;
                    case "D": Delete(concerts); break;
                    case "S": ShowAll(concerts); break;
                    case "C": Calculate(concerts); break;
                    case "":
                        SaveToFile2(concerts, filePath);
                        Console.WriteLine("Дані збережено у файлі.");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Помилка: невірний вибір!");
                        break;
                }
            }
        }
        // Завантаження з файлу або створення дефолтного списку
        static List<Concert2> LoadOrCreateDefault2(string filePath)
        {
            var list = new List<Concert2>();
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    foreach (var l in lines)
                    {
                        if (string.IsNullOrWhiteSpace(l)) continue;
                        list.Add(Concert2.FromFileLine(l));
                    }
                }
            }
            catch
            {
                list.Clear();
            }

            if (list.Count < 5)
            {
                list = new List<Concert2>
                {
                    new Concert2("Коваленко","Рок", new DateTime(2024,5,5),1200),
                    new Concert2("Іваненко","Поп", new DateTime(2024,6,10),950),
                    new Concert2("Мельник","Джаз", new DateTime(2024,7,3),800),
                    new Concert2("Шевченко","Реп", new DateTime(2024,8,1),700),
                    new Concert2("Федоренко","Класика", new DateTime(2024,9,12),1000)
                };
            }

            return list;
        }
        // Додавання нового запису
        static void Add(List<Concert2> concerts)
        {
            Console.WriteLine("\n--- Додавання ---");
            string s = ReadNonEmpty("Прізвище виконавця (мінімум 2 літери): ", minLen: 2);
            string g = ReadNonEmpty("Жанр (мінімум 2 літери): ", minLen: 2);
            DateTime d = ReadDate("Дата концерту (ДД.ММ.РРРР): ");
            int a = ReadPositiveInt("Кількість глядачів: ");

            concerts.Add(new Concert2(s, g, d, a));
            Console.WriteLine("Запис успішно додано.");
        }
        // Редагування існуючого запису
        static void Edit(List<Concert2> concerts)
        {
            Console.Write("\nВведіть прізвище виконавця для редагування: ");
            string name = Console.ReadLine();
            var idx = concerts.FindIndex(c => c.Surname.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx == -1)
            {
                Console.WriteLine("Запис не знайдено.");
                return;
            }

            var c = concerts[idx];
            Console.WriteLine("Натисніть Enter щоб залишити старе значення.");

            Console.Write($"Прізвище ({c.Surname}): ");
            string s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s) && s.Length >= 2) c.Surname = s;

            Console.Write($"Жанр ({c.Genre}): ");
            string g = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(g) && g.Length >= 2) c.Genre = g;

            Console.Write($"Дата ({c.Date:dd.MM.yyyy}): ");
            string ds = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ds))
            {
                if (DateTime.TryParseExact(ds, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime d)) c.Date = d;
                else Console.WriteLine("Невірний формат дати — залишено старе.");
            }

            Console.Write($"Кількість глядачів ({c.Audience}): ");
            string asStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(asStr) && int.TryParse(asStr, out int an) && an > 0) c.Audience = an;

            Console.WriteLine("Запис оновлено.");
        }
        // Видалення запису
        static void Delete(List<Concert2> concerts)
        {
            Console.Write("\nПрізвище для видалення: ");
            string name = Console.ReadLine();
            var idx = concerts.FindIndex(c => c.Surname.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx == -1)
            {
                Console.WriteLine("Запис не знайдено.");
                return;
            }
            concerts.RemoveAt(idx);
            Console.WriteLine("Запис успішно видалено.");
        }
        // Показ усіх записів
        static void ShowAll(List<Concert2> concerts)
        {
            Console.WriteLine("\nУсі концерти:");
            foreach (var c in concerts) c.Display();
        }
        // Обчислення результатів
        static void Calculate(List<Concert2> concerts)
        {
            Console.WriteLine("\n--- Обчислення результатів ---");
            if (concerts.Count == 0)
            {
                Console.WriteLine("Немає концертів.");
                return;
            }
            int total = 0;
            Concert2 max = concerts[0];
            foreach (var c in concerts)
            {
                total += c.Audience;
                if (c.Audience > max.Audience) max = c;
            }

            Console.WriteLine($"1. Загальна кількість глядачів: {total}");
            Console.WriteLine("2. Концерт з макс. глядачами:");
            max.Display();
            Console.WriteLine($"3. Кількість слів у жанрі \"{max.Genre}\" = {max.CountGenreWords()}");
        }
        // Збереження у файл
        static void SaveToFile2(List<Concert2> concerts, string filePath)
        {
            try
            {
                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (var c in concerts) sw.WriteLine(c.ToFileLine());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка запису у файл: " + ex.Message);
            }
        }

        // --- допоміжні методи t2 ---
        static string ReadNonEmpty(string prompt, int minLen = 1)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s) && s.Length >= minLen) return s;
                Console.WriteLine($"Помилка: потрібно мінімум {minLen} символів.");
            }
        }
        // Читання дати з консолі
        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (DateTime.TryParseExact(s, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime d)) return d;
                Console.WriteLine("Помилка: введіть дату у форматі ДД.MM.YYYY.");
            }
        }
        // Читання додатного цілого числа
        static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v > 0) return v;
                Console.WriteLine("Помилка: введіть додатне число.");
            }
        }
    }
}
