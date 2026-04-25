using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

const string FileName = "tasks.json";

List<TaskItem> tasks = LoadTasks();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("NyxOpsDashboard");
    Console.WriteLine("1. Add task");
    Console.WriteLine("2. View tasks");
    Console.WriteLine("3. Complete task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Search tasks");
    Console.WriteLine("6. Edit task");
    Console.WriteLine("7. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Enter task title: ");
        string? title = Console.ReadLine();

        Console.Write("Enter notes: ");
        string? notes = Console.ReadLine();

        Console.Write("Enter priority (Low, Medium, High, Urgent): ");
        string? priority = Console.ReadLine();

        priority = NormalizePriority(priority);

        Console.Write("Enter due date (MM/DD/YYYY) or leave blank: ");
        string? dueDateInput = Console.ReadLine();

        DateTime? dueDate = ParseDueDate(dueDateInput);

        if (!string.IsNullOrWhiteSpace(title))
        {
            TaskItem newTask = new TaskItem
            {
                Id = tasks.Count == 0 ? 1 : tasks.Max(task => task.Id) + 1,
                Title = title,
                Notes = notes,
                Priority = priority,
                DueDate = dueDate,
                IsComplete = false,
                CreatedAt = DateTime.Now
            };

            tasks.Add(newTask);
            SaveTasks(tasks);

            Console.WriteLine("Task added and saved.");
        }
        else
        {
            Console.WriteLine("Task title cannot be empty.");
        }
    }
    else if (choice == "2")
    {
        Console.WriteLine();
        Console.WriteLine("Tasks:");

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks yet.");
        }
        else
        {
            foreach (TaskItem task in tasks)
            {
                PrintTask(task);
            }
        }
    }
    else if (choice == "3")
    {
        Console.Write("Enter the task ID to complete: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskId))
        {
            TaskItem? task = tasks.Find(task => task.Id == taskId);

            if (task is not null)
            {
                task.IsComplete = true;
                SaveTasks(tasks);

                Console.WriteLine("Task marked complete and saved.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
        }
    }
    else if (choice == "4")
    {
        Console.Write("Enter the task ID to delete: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskId))
        {
            TaskItem? task = tasks.Find(task => task.Id == taskId);

            if (task is not null)
            {
                tasks.Remove(task);
                SaveTasks(tasks);

                Console.WriteLine("Task deleted and saved.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
        }
    }
    else if (choice == "5")
    {
        Console.Write("Enter search keyword: ");
        string? keyword = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Search keyword cannot be empty.");
        }
        else
        {
            List<TaskItem> results = tasks
                .Where(task =>
                    task.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(task.Notes) &&
                     task.Notes.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    task.Priority.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine();
            Console.WriteLine("Search Results:");

            if (results.Count == 0)
            {
                Console.WriteLine("No matching tasks found.");
            }
            else
            {
                foreach (TaskItem task in results)
                {
                    PrintTask(task);
                }
            }
        }
    }
    else if (choice == "6")
    {
        Console.Write("Enter the task ID to edit: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskId))
        {
            TaskItem? task = tasks.Find(task => task.Id == taskId);

            if (task is null)
            {
                Console.WriteLine("Task not found.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Current task:");
                PrintTask(task);

                Console.Write("New title or leave blank to keep current: ");
                string? newTitle = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newTitle))
                {
                    task.Title = newTitle.Trim();
                }

                Console.Write("New notes, leave blank to keep current, or type clear: ");
                string? newNotes = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newNotes))
                {
                    if (newNotes.Trim().Equals("clear", StringComparison.OrdinalIgnoreCase))
                    {
                        task.Notes = null;
                    }
                    else
                    {
                        task.Notes = newNotes.Trim();
                    }
                }

                Console.Write("New priority (Low, Medium, High, Urgent) or leave blank to keep current: ");
                string? newPriority = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newPriority))
                {
                    task.Priority = NormalizePriority(newPriority);
                }

                Console.Write("New due date (MM/DD/YYYY), leave blank to keep current, or type clear: ");
                string? newDueDateInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newDueDateInput))
                {
                    if (newDueDateInput.Trim().Equals("clear", StringComparison.OrdinalIgnoreCase))
                    {
                        task.DueDate = null;
                    }
                    else
                    {
                        DateTime? newDueDate = ParseDueDate(newDueDateInput);

                        if (newDueDate.HasValue)
                        {
                            task.DueDate = newDueDate;
                        }
                    }
                }

                SaveTasks(tasks);
                Console.WriteLine("Task updated and saved.");
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
        }
    }
    else if (choice == "7")
    {
        Console.WriteLine("Goodbye.");
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice. Try again.");
    }
}

static List<TaskItem> LoadTasks()
{
    if (!File.Exists(FileName))
    {
        return new List<TaskItem>();
    }

    string json = File.ReadAllText(FileName);

    if (string.IsNullOrWhiteSpace(json))
    {
        return new List<TaskItem>();
    }

    List<TaskItem>? savedTasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

    if (savedTasks is null)
    {
        return new List<TaskItem>();
    }

    foreach (TaskItem task in savedTasks)
    {
        task.Priority = NormalizePriority(task.Priority);
    }

    return savedTasks;
}

static void SaveTasks(List<TaskItem> tasks)
{
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    string json = JsonSerializer.Serialize(tasks, options);

    File.WriteAllText(FileName, json);
}

static DateTime? ParseDueDate(string? input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return null;
    }

    if (DateTime.TryParse(input, out DateTime dueDate))
    {
        return dueDate;
    }

    Console.WriteLine("Invalid due date. No due date was saved.");
    return null;
}

static string NormalizePriority(string? priority)
{
    if (string.IsNullOrWhiteSpace(priority))
    {
        return "Medium";
    }

    string cleanedPriority = priority.Trim().ToLower();

    if (cleanedPriority == "low")
    {
        return "Low";
    }

    if (cleanedPriority == "medium")
    {
        return "Medium";
    }

    if (cleanedPriority == "high")
    {
        return "High";
    }

    if (cleanedPriority == "urgent")
    {
        return "Urgent";
    }

    return "Medium";
}

static void PrintTask(TaskItem task)
{
    string status = task.IsComplete ? "Complete" : "Open";

    Console.WriteLine($"{task.Id}. [{status}] [{task.Priority}] {task.Title}");

    if (task.DueDate.HasValue)
    {
        Console.WriteLine($"   Due: {task.DueDate.Value.ToShortDateString()}");
    }

    if (!string.IsNullOrWhiteSpace(task.Notes))
    {
        Console.WriteLine($"   Notes: {task.Notes}");
    }

    Console.WriteLine($"   Created: {task.CreatedAt}");
}

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string? Notes { get; set; }

    public string Priority { get; set; } = "Medium";

    public DateTime? DueDate { get; set; }

    public bool IsComplete { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}