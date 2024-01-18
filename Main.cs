using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class TaskManager
{

    class TaskData
    {
        public List<Task> Tasks { get; set; }
        public int TaskIdCounter { get; set; }
    }

    class Task
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int Priority {get; set;}
        public DateTime Deadline {get; set;}
        public DateTime CreationDate {get; set;}
    }

    static TaskData taskData;

    static void Main(string[] args)
    {

        ParseCommandArguments(args);
        LoadTaskDataFromFile();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(" Menedżer Zadań");
            Console.WriteLine();
            Console.WriteLine("1. Dodaj zadanie");
            Console.WriteLine("2. Usuń zadanie");
            Console.WriteLine("3. Opcje zadań");
            Console.WriteLine("4. Wyświetl zadania (-d, -p)");
            Console.WriteLine("0. Wyjdź i zapisz");
            Console.WriteLine();
            Console.Write("Wybór [1-4]: ");
            string choice = Console.ReadLine();

            if (choice.StartsWith("4 -d"))
            {
                DisplayTasks(sortedByDeadline: true);
            }
            
            else if (choice.StartsWith("4 -p"))
            {
                DisplayTasks(sortedByPriority: true);
            }
            
            else if (choice == "4")
            {
                DisplayTasks();
            }
            
            else
            {
                ProcessChoice(choice);
            }
        }
    }

    static void ParseCommandArguments(string[] args)
    {
        if (args.Contains("-d"))
        {
            taskData.Tasks = taskData.Tasks.OrderBy(t => t.Deadline).ToList();
        }
        
        else if (args.Contains("-w"))
        {
            taskData.Tasks = taskData.Tasks.OrderByDescending(t => t.Priority).ToList();
        }
    }

    static void ProcessChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                AddTask();
                break;
            case "2":
                RemoveTask();
                break;
            case "3":
                ShowUpdateTaskMenu();
                break;
            case "0":
                SaveTaskDataToFile();
                Environment.Exit(0);
                break;
            default:
                Console.Clear();
                Console.WriteLine("Niepoprawny wybór. Wybierz liczbę odpowiadającą poszczególnym funkcjom.");
                Console.WriteLine();
                break;
        }
    }

    static void LoadTaskDataFromFile()
    {
        try
        {
            string json = File.ReadAllText("tasks.json");
            taskData = JsonSerializer.Deserialize<TaskData>(json);
        }
        
        catch (FileNotFoundException)
        {
            taskData = new TaskData { Tasks = new List<Task>(), TaskIdCounter = 1 };
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void SaveTaskDataToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(taskData);
            File.WriteAllText("tasks.json", json);
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DisplayTasks(bool sortedByDeadline = false, bool sortedByPriority = false)
    {
        Console.Clear();

        Console.WriteLine();
        Console.Write(" Lista zadań: ");

        if (sortedByDeadline)
        {
            Console.WriteLine("Posortowano po Deadline");
        }
        
        else if (sortedByPriority)
        {
            Console.WriteLine("Posortowano po Stopień Ważności");
        }
        
        else
        {
            Console.WriteLine();
        }

        List<Task> displayedTasks = taskData.Tasks;

        if (sortedByDeadline)
        {
            displayedTasks = displayedTasks.OrderBy(t => t.Deadline).ToList();
        }
        
        else if (sortedByPriority)
        {
            displayedTasks = displayedTasks.OrderByDescending(t => t.Priority).ToList();
        }

        if (displayedTasks.Any())
        {
            foreach (var task in displayedTasks)
            {
                Console.WriteLine();
                Console.WriteLine($"ID: {task.Id}");
                Console.WriteLine($"Nazwa: {task.Name}");
                Console.WriteLine($"Opis: {task.Description}");
                Console.WriteLine($"Stopień Ważności: {task.Priority}");
                Console.WriteLine($"Deadline: {task.Deadline.ToString("dd-MM-yyyy")}");
                Console.WriteLine($"Data utworzenia: {task.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");
                Console.WriteLine();
            }
        }
        
        else
        {
            Console.WriteLine("Brak zaplanowanych zadań.");
            Console.WriteLine("Jesteś wolny/a :)");
            Console.WriteLine();
        }
    }
    static void AddTask()
    {
        Console.Clear();
        Console.Write("Nazwa: ");
        string name = Console.ReadLine();

        Console.Write("Opis: ");
        string description = Console.ReadLine();

        int priority;

        while (true)
        {
            Console.Write("Stopień ważnosci [1-5]: ");
            if (int.TryParse(Console.ReadLine(), out priority) && priority >= 1 && priority <= 5)
            {
                break;
            }
            
            else
            {
                Console.Clear();
                Console.WriteLine("Nieprawidłowy stopień ważności. Wprowadź liczbę od 1 do 5.");
            }
        }

        DateTime deadline;

        while (true)
        {
            Console.Write("Deadline (dd-MM-yyyy): ");
            string input = Console.ReadLine();

            if (TryParseCustomDateFormat(input, out deadline))
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid date format. Please enter the date in the format dd-MM-yyyy.");
            }
        }

        bool TryParseCustomDateFormat(string input, out DateTime result)
        {
            result = DateTime.MinValue;
            string[] dateParts = input.Split('-');

            if (dateParts.Length == 3)
            {
                int day, month, year;

                if (int.TryParse(dateParts[0], out day) &&
                    int.TryParse(dateParts[1], out month) &&
                    int.TryParse(dateParts[2], out year))
                {

                    try
                    {
                        result = new DateTime(year, month, day);
                        return true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return false;
        }

        Task newTask = new Task
        {
            Id = taskData.TaskIdCounter++,
            Name = name,
            Description = description,
            Priority = priority,
            Deadline = deadline,
            CreationDate = DateTime.Now
        };

        taskData.Tasks.Add(newTask);

        Console.Clear();
        Console.WriteLine($"Dodano nowe zadanie ID {newTask.Id}.");
        Console.WriteLine();
    }

    static void RemoveTask()
    {
        Console.Clear();

        DisplayTasks();
        Console.Write("ID zadania do usunięcia: ");
        int taskId = int.Parse(Console.ReadLine());

        Task taskToRemove = taskData.Tasks.FirstOrDefault(t => t.Id == taskId);

        if (taskToRemove != null)
        {
            taskData.Tasks.Remove(taskToRemove);
            Console.Clear();
            Console.WriteLine($"Usunięto zadanie ID: {taskId}.");
            Console.WriteLine();
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania ID: {taskId}.");
            Console.WriteLine();
        }
    }

    static void ShowUpdateTaskMenu()
    {
        Console.Clear();

        DisplayTasks();
        Console.WriteLine(" Opcje zadań");
        Console.WriteLine();
        Console.Write("Podaj ID zadania do modyfikacji: ");
        int taskId = int.Parse(Console.ReadLine());

        Task taskToUpdate = taskData.Tasks.FirstOrDefault(t => t.Id == taskId);

        if (taskToUpdate != null)
        {
            Console.Clear();
            Console.WriteLine($"ID: {taskToUpdate.Id}");
            Console.WriteLine($"Nazwa: {taskToUpdate.Name}");
            Console.WriteLine($"Opis: {taskToUpdate.Description}");
            Console.WriteLine($"Stopień Ważności: {taskToUpdate.Priority}");
            Console.WriteLine($"Deadline: {taskToUpdate.Deadline.ToString("dd-MM-yyyy")}");
            Console.WriteLine($"Data utworzenia: {taskToUpdate.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");
            Console.WriteLine();
            Console.WriteLine(" Wybierz, co chcesz zmienić:");
            Console.WriteLine("1. Zmień nazwę");
            Console.WriteLine("2. Zmień opis");
            Console.WriteLine("3. Zmień ważność");
            Console.WriteLine("4. Zmień Deadline");
            Console.WriteLine("0. Wróć");
            Console.WriteLine();
            Console.Write("Wybór [1-4]: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UpdateTaskName(taskToUpdate);
                    break;
                case "2":
                    UpdateTaskDescription(taskToUpdate);
                    break;
                case "3":
                    UpdateTaskPriority(taskToUpdate);
                    break;
                case "4":
                    UpdateTaskDeadline(taskToUpdate);
                    break;
                case "0":
                    Console.Clear();
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Niepoprawny wybór. Wybierz liczbę odpowiadającą poszczególnym funkcjom.");
                    Console.WriteLine();
                    break;
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania o ID: {taskId}.");
            Console.WriteLine();
        }
    }

    static void UpdateTaskName(Task taskToUpdate)
    {
        Console.Clear();

        DisplayTasks();
        if (taskToUpdate != null)
        {
            Console.Write("Nowa nazwa: ");
            taskToUpdate.Name = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Nazwa zadania ID: {taskToUpdate.Id} została zmieniona.");
            Console.WriteLine();
        }

        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania ID: {taskToUpdate.Id}.");
            Console.WriteLine();
        }
    }

    static void UpdateTaskDescription(Task taskToUpdate)
    {
        Console.Clear();

        DisplayTasks();
        if (taskToUpdate != null)
        {
            Console.Write("Nowy opis: ");
            taskToUpdate.Description = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Opis zadania {taskToUpdate.Id} został zmieniony.");
            Console.WriteLine();
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania ID {taskToUpdate.Id}.");
            Console.WriteLine();
        }
    }

    static void UpdateTaskPriority(Task taskToUpdate)
    {
        Console.Clear();

        DisplayTasks();
        if (taskToUpdate != null)
        {

            int updatePriority;

            while (true)
            {
                Console.Write("Nowy stopień ważności [1-5]: ");

                if (int.TryParse(Console.ReadLine(), out updatePriority) && updatePriority >= 1 && updatePriority <= 5)
                {
                    taskToUpdate.Priority = updatePriority;
                    Console.Clear();
                    Console.WriteLine($"Stopień ważności zadania ID {taskToUpdate.Id} został zmieniony.");
                    Console.WriteLine();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Nieprawidłowy stopień ważności. Wprowadź liczbę od 1 do 5.");
                }
            }
        }
        
        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania ID {taskToUpdate.Id}.");
            Console.WriteLine();
        }
    }

    static void UpdateTaskDeadline(Task taskToUpdate)
    {
        Console.Clear();

        DisplayTasks();
        if (taskToUpdate != null)
        {
            try
            {
                Console.Write("Nowy deadline (yyyy-MM-dd): ");
                taskToUpdate.Deadline = DateTime.Parse(Console.ReadLine());
                Console.Clear();
                Console.WriteLine($"Deadline zadania ID {taskToUpdate.Id} został zmieniony.");
                Console.WriteLine();
            }
            
            catch (FormatException)
            {
                Console.Clear();
                Console.WriteLine("Nieprawidłowy format daty. Wprowadź datę w formacie yyyy-MM-dd.");
                Console.WriteLine();
            }
            
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                Console.WriteLine();
            }
        }
        
        else
        {
            Console.Clear();
            Console.WriteLine($"Nie znaleziono zadania o ID: {taskToUpdate.Id}.");
            Console.WriteLine();
        }
    }
}
