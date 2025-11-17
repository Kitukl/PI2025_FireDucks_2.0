using ConsoleApp.models;
using Task = ConsoleApp.models.Task;

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
                    case "1": UserMenu(); break;
                    case "2": LectureMenu(); break;
                    case "3": SubjectMenu(); break;
                    case "4": LessonMenu(); break;
                    case "5": SlotMenu(); break;
                    case "6": CategoryMenu(); break;
                    case "7": CommentMenu(); break;
                    case "8": TaskMenu(); break;
                    case "9": SupportMenu(); break;
                    case "0": return;

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
                    Console.Write("Ім'я: "); string name = Console.ReadLine();
                    Console.Write("Прізвище: "); string surname = Console.ReadLine();
                    Console.Write("Email: "); string email = Console.ReadLine();
                    Console.Write("Пароль: "); string pass = Console.ReadLine();

                    user.Register(name, surname, email, pass);
                    Console.WriteLine("Реєстрація успішна!");
                    break;

                case "2":
                    Console.Write("Email: "); email = Console.ReadLine();
                    Console.Write("Пароль: "); pass = Console.ReadLine();
                    bool ok = user.Login(email, pass);
                    Console.WriteLine(ok ? "Вхід успішний!" : "Невірні дані!");
                    break;

                case "3":
                    Console.Write("Новий пароль: "); string np = Console.ReadLine();
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

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void LectureMenu()
        {
            var lecture = new Lecture();

            Console.WriteLine("=== МЕНЮ ВИКЛАДАЧІВ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Показати за ID");
            Console.WriteLine("3. Додати");
            Console.WriteLine("4. Редагувати");
            Console.WriteLine("5. Видалити");
            Console.WriteLine("6. Згенерувати 20 тестових");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    foreach (var x in lecture.GetAll())
                        Console.WriteLine($"{x.Id}. {x.Name} {x.Surname}");
                    break;

                case "2":
                    Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                    var l = lecture.GetById(id);
                    Console.WriteLine(l != null ? $"{l.Id}. {l.Name} {l.Surname}" : "Не знайдено");
                    break;

                case "3":
                    Console.Write("Ім'я: "); string name = Console.ReadLine();
                    Console.Write("Прізвище: "); string sname = Console.ReadLine();
                    lecture.AddLecture(name, sname);
                    Console.WriteLine("Додано");
                    break;

                case "4":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    Console.Write("Нове ім'я: "); name = Console.ReadLine();
                    Console.Write("Нове прізвище: "); sname = Console.ReadLine();
                    lecture.EditLecture(id, name, sname);
                    Console.WriteLine("Оновлено");
                    break;

                case "5":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    lecture.RemoveLecture(id);
                    Console.WriteLine("Видалено");
                    break;

                case "6":
                    lecture.GenerateLectures();
                    Console.WriteLine("Згенеровано!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void SubjectMenu()
        {
            var subj = new Subject();
            Console.WriteLine("=== ПРЕДМЕТИ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Отримати за ID");
            Console.WriteLine("3. Додати");
            Console.WriteLine("4. Редагувати");
            Console.WriteLine("5. Видалити");
            Console.WriteLine("6. Згенерувати стандартні");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    foreach (var x in subj.GetAll())
                        Console.WriteLine($"{x.Id}. {x.Name}");
                    break;

                case "2":
                    Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                    var s = subj.GetById(id);
                    Console.WriteLine(s != null ? $"{s.Id}. {s.Name}" : "Не знайдено");
                    break;

                case "3":
                    Console.Write("Назва: "); string name = Console.ReadLine();
                    subj.createSubject(name);
                    Console.WriteLine("Додано!");
                    break;

                case "4":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    Console.Write("Нова назва: "); name = Console.ReadLine();
                    subj.EditSubject(id, name);
                    Console.WriteLine("Оновлено!");
                    break;

                case "5":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    subj.DeleteSubject(id);
                    Console.WriteLine("Видалено!");
                    break;

                case "6":
                    subj.GenerateSubjects();
                    Console.WriteLine("Готово!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void LessonMenu()
        {
            var lesson = new Lesson();
            Console.WriteLine("=== ЗАНЯТТЯ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Отримати за ID");
            Console.WriteLine("3. Створити");
            Console.WriteLine("4. Оновити");
            Console.WriteLine("5. Видалити");
            Console.WriteLine("6. Згенерувати 20 тестових");
            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    foreach (var x in lesson.GetAll())
                        Console.WriteLine($"{x.Id}. LECT:{x.LectureId} SUB:{x.SubjectId} SLOT:{x.SlotId}");
                    break;

                case "2":
                    Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                    var ls = lesson.GetById(id);
                    Console.WriteLine(ls != null ? $"{ls.Id}. {ls.LectureId}-{ls.SubjectId}-{ls.SlotId}" : "Не знайдено");
                    break;

                case "3":
                    Console.Write("Lecture ID: "); int lid = int.Parse(Console.ReadLine());
                    Console.Write("Subject ID: "); int sid = int.Parse(Console.ReadLine());
                    Console.Write("Slot ID: "); int sl = int.Parse(Console.ReadLine());
                    lesson.CreateLesson(lid, sid, sl);
                    Console.WriteLine("Створено");
                    break;

                case "4":
                    Console.Write("Lecture ID: "); lid = int.Parse(Console.ReadLine());
                    Console.Write("Subject ID: "); sid = int.Parse(Console.ReadLine());
                    Console.Write("Slot ID: "); sl = int.Parse(Console.ReadLine());
                    lesson.UpdateLesson(lid, sid, sl);
                    Console.WriteLine("Оновлено");
                    break;

                case "5":
                    Console.Write("ID уроку (Lesson.Id): ");
                    lesson.Id = int.Parse(Console.ReadLine());
                    lesson.DeleteLesson();
                    Console.WriteLine("Видалено");
                    break;

                case "6":
                    lesson.GenerateLessons();
                    Console.WriteLine("Готово");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void SlotMenu()
        {
            var slot = new LessonSlot();
            Console.WriteLine("=== СЛОТИ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Показати за ID");
            Console.WriteLine("3. Створити слот");
            Console.WriteLine("4. Згенерувати стандартні слоти");
            Console.Write("Оберіть дію: ");

            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    foreach (var s in slot.GetAll())
                        Console.WriteLine($"{s.Id} — {s.StartLesson} - {s.EndLesson}");
                    break;

                case "2":
                    Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                    var res = slot.GetById(id);
                    Console.WriteLine(res != null ? $"{res.Id}. {res.StartLesson}-{res.EndLesson}" : "Не знайдено");
                    break;

                case "3":
                    Console.Write("Початок (hh:mm): ");
                    var start = TimeOnly.Parse(Console.ReadLine());
                    Console.Write("Кінець (hh:mm): ");
                    var end = TimeOnly.Parse(Console.ReadLine());
                    slot.SetTimeSlot(start, end);
                    slot.CreateSlot();
                    Console.WriteLine("Створено!");
                    break;

                case "4":
                    slot.GenerateDefaultSlots();
                    Console.WriteLine("Готово!");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }
        
        static void CategoryMenu()
        {
            var c = new Category();

            Console.WriteLine("=== КАТЕГОРІЇ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Показати за ID");
            Console.WriteLine("3. Згенерувати тестові");
            Console.Write("Оберіть дію: ");

            var ch = Console.ReadLine();

            switch (ch)
            {
                case "1":
                    foreach (var x in c.GetAll())
                        Console.WriteLine($"{x.Id}. {x.Name}");
                    break;

                case "2":
                    Console.Write("ID: ");
                    var id = int.Parse(Console.ReadLine());
                    var res = c.GetById(id);
                    Console.WriteLine(res != null ? $"{res.Id} — {res.Name}" : "Не знайдено");
                    break;

                case "3":
                    c.GenerateCategories();
                    Console.WriteLine("Готово!");
                    break;
            }

            Console.ReadKey();
        }
        
        static void CommentMenu()
        {
            var com = new Comment();

            Console.WriteLine("=== КОМЕНТАРІ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Показати за ID");
            Console.WriteLine("3. Додати");
            Console.WriteLine("4. Редагувати");
            Console.WriteLine("5. Видалити");
            Console.Write("Оберіть дію: ");

            var ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    foreach (var c in com.GetAll())
                        Console.WriteLine($"{c.Id}. {c.Description} (Task {c.TaskId})");
                    break;

                case "2":
                    Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                    var r = com.GetById(id);
                    Console.WriteLine(r != null ? $"{r.Id}: {r.Description}" : "Не знайдено");
                    break;

                case "3":
                    Console.Write("Task ID: "); int tid = int.Parse(Console.ReadLine());
                    Console.Write("Опис: "); string desc = Console.ReadLine();
                    com.AddComment(desc, tid);
                    Console.WriteLine("OK");
                    break;

                case "4":
                    Console.Write("ID коментаря: "); int cid = int.Parse(Console.ReadLine());
                    Console.Write("Новий текст: "); string nd = Console.ReadLine();
                    com.EditComment(cid, nd);
                    Console.WriteLine("OK");
                    break;

                case "5":
                    Console.Write("ID: "); cid = int.Parse(Console.ReadLine());
                    com.DeleteComment(cid);
                    Console.WriteLine("OK");
                    break;
            }

            Console.ReadKey();
        }
        
        static void TaskMenu()
        {
            var u = new User();
            Console.WriteLine("=== ЗАВДАННЯ ===");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Показати за ID");
            Console.WriteLine("3. Додати");
            Console.WriteLine("4. Редагувати");
            Console.WriteLine("5. Видалити");
            Console.WriteLine("6. Змінити статус");
            Console.WriteLine("7. Змінити дедлайн");

            Console.Write("Оберіть дію: ");
            var ch = Console.ReadLine();
            Console.WriteLine();

            switch (ch)
            {
                case "1":
                    var temp = new Task(0,"","",DateTime.Now,"",0,0);
                    foreach (var t in temp.GetAll())
                        Console.WriteLine($"{t.Id}. {t.Title} — {t.Status}");
                    break;

                case "2":
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine());
                    var item = new Task(0,"","",DateTime.Now,"",0,0).GetById(id);
                    if (item == null) Console.WriteLine("Не знайдено");
                    else Console.WriteLine($"{item.Id}. {item.Title} ({item.Status})");
                    break;

                case "3":
                    Console.Write("User ID: "); int uid = int.Parse(Console.ReadLine());
                    Console.Write("Title: "); string title = Console.ReadLine();
                    Console.Write("Desc: "); string desc = Console.ReadLine();
                    Console.Write("Deadline yyyy-mm-dd: "); var dl = DateTime.Parse(Console.ReadLine());

                    var tsk = new Task(0, title, desc, dl, "New", uid, 1);
                    tsk.CreateTask();
                    Console.WriteLine("Створено");
                    break;

                case "4":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    Console.Write("Новий title: "); string nt = Console.ReadLine();
                    Console.Write("Новий desc: "); string newd = Console.ReadLine();
                    new Task(0,"","",DateTime.Now,"",0,0).UpdateTask(id, nt, newd);
                    Console.WriteLine("Оновлено");
                    break;

                case "5":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    new Task(0,"","",DateTime.Now,"",0,0).DeleteTask(id);
                    Console.WriteLine("Видалено");
                    break;

                case "6":
                    Console.Write("Task ID: ");
                    id = int.Parse(Console.ReadLine());
                    Console.Write("New status: ");
                    string st = Console.ReadLine();
                    var obj = new Task(0,"","",DateTime.Now,"",0,0);
                    obj.Id = id;
                    obj.ChangeStatus(st);
                    Console.WriteLine("OK");
                    break;

                case "7":
                    Console.Write("ID: "); id = int.Parse(Console.ReadLine());
                    Console.Write("New deadline yyyy-mm-dd: "); var ndl = DateTime.Parse(Console.ReadLine());
                    var o = new Task(0,"","",DateTime.Now,"",0,0);
                    o.Id = id;
                    o.SetDeadline(ndl);
                    Console.WriteLine("OK");
                    break;
            }

            Console.ReadKey();
        }

        
        static void SupportMenu()
        {
            var s = new SupportTicket();
            Console.WriteLine("=== ПІДТРИМКА ===");
            Console.Write("Email користувача: "); var email = Console.ReadLine();
            Console.Write("Ім'я: "); var name = Console.ReadLine();
            Console.Write("Тип: "); var type = Console.ReadLine();
            Console.Write("Повідомлення: "); var msg = Console.ReadLine();
            Console.Write("ID категорії: "); int cat = int.Parse(Console.ReadLine());

            s.SendContactForm(name, email, msg, type, cat);
            Console.WriteLine("Надіслано!");
            Console.ReadKey();
        }
    }
}
