using System;
using ConsoleApp.models;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ГОЛОВНЕ МЕНЮ ===");
                Console.WriteLine("1. Користувачі");
                Console.WriteLine("2. Викладачі");
                Console.WriteLine("3. Предмети");
                Console.WriteLine("4. Заняття");
                Console.WriteLine("5. Слоти занять");
                Console.WriteLine("6. Категорії");
                Console.WriteLine("7. Коментарі");
                Console.WriteLine("8. Завдання");
                Console.WriteLine("9. Підтримка");
                Console.WriteLine("0. Вихід");
                Console.Write("\nОберіть дію: ");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        UserMenu();
                        break;
                    case "2":
                        LectureMenu();
                        break;
                    case "3":
                        SubjectMenu();
                        break;
                    case "4":
                        LessonMenu();
                        break;
                    case "5":
                        SlotMenu();
                        break;
                    case "6":
                        CategoryMenu();
                        break;
                    case "7":
                        CommentMenu();
                        break;
                    case "8":
                        TaskMenu();
                        break;
                    case "9":
                        SupportMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Натисніть будь-яку клавішу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void UserMenu()
        {
            var user = new User();

            Console.WriteLine("=== МЕНЮ КОРИСТУВАЧА ===");
            Console.WriteLine("1. Реєстрація");
            Console.WriteLine("2. Вхід");
            Console.WriteLine("3. Змінити пароль");
            Console.WriteLine("4. Налаштувати сповіщення");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Ім'я: ");
                    string name = Console.ReadLine();
                    Console.Write("Прізвище: ");
                    string surname = Console.ReadLine();
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Пароль: ");
                    string pass = Console.ReadLine();
                    user.Register(name, surname, email, pass);
                    Console.WriteLine("Реєстрація успішна!");
                    break;

                case "2":
                    Console.Write("Email: ");
                    email = Console.ReadLine();
                    Console.Write("Пароль: ");
                    pass = Console.ReadLine();
                    bool ok = user.Login(email, pass);
                    Console.WriteLine(ok ? "Вхід успішний!" : "Невірні дані!");
                    break;

                case "3":
                    Console.Write("Новий пароль: ");
                    string np = Console.ReadLine();
                    bool done = user.ResetPassword(np);
                    Console.WriteLine(done ? "Пароль змінено!" : "Помилка!");
                    break;

                case "4":
                    Console.Write("Увімкнути сповіщення (true/false): ");
                    bool notif = bool.Parse(Console.ReadLine() ?? "false");
                    Console.Write("Днів до дедлайну: ");
                    int days = int.Parse(Console.ReadLine() ?? "0");
                    user.SetNotificationPreferences(notif, days);
                    Console.WriteLine("Збережено!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
            Console.ReadKey();
        }

        static void LectureMenu()
        {
            var lecture = new Lecture();
            Console.WriteLine("=== МЕНЮ ВИКЛАДАЧІВ ===");
            Console.WriteLine("1. Додати");
            Console.WriteLine("2. Редагувати");
            Console.WriteLine("3. Видалити");
            Console.WriteLine("4. Згенерувати тестові");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Ім'я: ");
                    string name = Console.ReadLine();
                    Console.Write("Прізвище: ");
                    string surname = Console.ReadLine();
                    lecture.AddLecture(name, surname);
                    Console.WriteLine("Викладача додано");
                    break;
                case "2":
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine());
                    Console.Write("Нове ім'я: ");
                    name = Console.ReadLine();
                    Console.Write("Нове прізвище: ");
                    surname = Console.ReadLine();
                    lecture.EditLecture(id, name, surname);
                    Console.WriteLine("Змінено");
                    break;
                case "3":
                    Console.Write("ID: ");
                    id = int.Parse(Console.ReadLine());
                    lecture.RemoveLecture(id);
                    Console.WriteLine("Видалено");
                    break;
                case "4":
                    lecture.GenerateLectures();
                    Console.WriteLine("Згенеровано 20 викладачів");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
            Console.ReadKey();
        }
        
        static void SubjectMenu()
        {
            var subj = new Subject();
            Console.WriteLine("=== МЕНЮ ПРЕДМЕТІВ ===");
            Console.WriteLine("1. Додати предмет");
            Console.WriteLine("2. Редагувати предмет");
            Console.WriteLine("3. Видалити предмет");
            Console.WriteLine("4. Згенерувати стандартні предмети");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Назва: ");
                    string name = Console.ReadLine();
                    subj.createSubject(name);
                    Console.WriteLine("Додано!");
                    break;
                case "2":
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine());
                    Console.Write("Нова назва: ");
                    name = Console.ReadLine();
                    subj.EditSubject(id, name);
                    Console.WriteLine("Оновлено!");
                    break;
                case "3":
                    Console.Write("ID: ");
                    id = int.Parse(Console.ReadLine());
                    subj.DeleteSubject(id);
                    Console.WriteLine("Видалено!");
                    break;
                case "4":
                    subj.GenerateSubjects();
                    Console.WriteLine("Згенеровано стандартні предмети");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
            Console.ReadKey();
        }

        static void LessonMenu()
        {
            var lesson = new Lesson();
            Console.WriteLine("=== МЕНЮ ЗАНЯТЬ ===");
            Console.WriteLine("1. Створити");
            Console.WriteLine("2. Оновити");
            Console.WriteLine("3. Видалити");
            Console.WriteLine("4. Згенерувати тестові");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Lecture ID: ");
                    int lid = int.Parse(Console.ReadLine());
                    Console.Write("Subject ID: ");
                    int sid = int.Parse(Console.ReadLine());
                    Console.Write("Slot ID: ");
                    int sl = int.Parse(Console.ReadLine());
                    lesson.CreateLesson(lid, sid, sl);
                    Console.WriteLine("Заняття створено!");
                    break;
                case "2":
                    Console.Write("Lecture ID: ");
                    lid = int.Parse(Console.ReadLine());
                    Console.Write("Subject ID: ");
                    sid = int.Parse(Console.ReadLine());
                    Console.Write("Slot ID: ");
                    sl = int.Parse(Console.ReadLine());
                    lesson.UpdateLesson(lid, sid, sl);
                    Console.WriteLine("Оновлено!");
                    break;
                case "3":
                    lesson.DeleteLesson();
                    Console.WriteLine("Видалено!");
                    break;
                case "4":
                    lesson.GenerateLessons();
                    Console.WriteLine("Згенеровано 20 занять!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
            Console.ReadKey();
        }

        static void SlotMenu()
        {
            var slot = new LessonSlot();
            Console.WriteLine("=== МЕНЮ СЛОТІВ ===");
            Console.WriteLine("1. Створити слот");
            Console.WriteLine("2. Згенерувати стандартні");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Початок (hh:mm): ");
                    var start = TimeOnly.Parse(Console.ReadLine());
                    Console.Write("Кінець (hh:mm): ");
                    var end = TimeOnly.Parse(Console.ReadLine());
                    slot.SetTimeSlot(start, end);
                    slot.CreateSlot();
                    Console.WriteLine("Створено слот!");
                    break;
                case "2":
                    slot.GenerateDefaultSlots();
                    Console.WriteLine("Згенеровано стандартні слоти!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
            Console.ReadKey();
        }

        static void CategoryMenu()
        {
            var c = new Category();
            c.GenerateCategories();
            Console.WriteLine("✅ Категорії згенеровано!");
            Console.ReadKey();
        }

        static void CommentMenu()
        {
            var com = new Comment();
            Console.WriteLine("=== КОМЕНТАРІ ===");
            Console.WriteLine("1. Додати коментар");
            Console.WriteLine("2. Редагувати коментар");
            Console.WriteLine("3. Видалити коментар");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    Console.Write("Task ID: ");
                    int tid = int.Parse(Console.ReadLine());
                    Console.Write("Опис: ");
                    string desc = Console.ReadLine();
                    com.AddComment(desc, tid);
                    Console.WriteLine("Додано!");
                    break;
                case "2":
                    Console.Write("ID коментаря: ");
                    int cid = int.Parse(Console.ReadLine());
                    Console.Write("Новий текст: ");
                    string nd = Console.ReadLine();
                    com.EditComment(cid, nd);
                    Console.WriteLine("Оновлено!");
                    break;
                case "3":
                    Console.Write("ID: ");
                    cid = int.Parse(Console.ReadLine());
                    com.DeleteComment(cid);
                    Console.WriteLine("Видалено!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void TaskMenu()
        {
            Console.WriteLine("=== ЗАВДАННЯ ===");
            Console.WriteLine("Тут можна реалізувати CRUD Task та фільтрацію.");
            Console.ReadKey();
        }

        static void SupportMenu()
        {
            var s = new SupportTicket();
            Console.Write("Email користувача: ");
            var email = Console.ReadLine();
            Console.Write("Ім'я: ");
            var name = Console.ReadLine();
            Console.Write("Тип: ");
            var type = Console.ReadLine();
            Console.Write("Повідомлення: ");
            var msg = Console.ReadLine();
            Console.Write("ID категорії: ");
            int cat = int.Parse(Console.ReadLine());
            s.SendContactForm(name, email, msg, type, cat);
            Console.WriteLine("Відправлено!");
            Console.ReadKey();
        }
    }
}