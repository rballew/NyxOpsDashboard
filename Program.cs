using System;
using System.Collections.Generic;

List<TaskItem> tasks = new();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("NyxOpsDashboard");
    Console.WriteLine("1. Add task");
    Console.WriteLine("2. View tasks");
    Console.WriteLine("3. Complete task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Enter task title: ");
        string? title = Console.ReadLine();

        Console.Write("Enter notes: ");
        string? notes = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(title))
        {
            TaskItem newTask = new TaskItem
            {
                Id = tasks.Count + 1,
                Title = title,
                Notes = notes,
                IsComplete = false,
                CreatedAt = DateTime.Now
            };

            tasks.Add(newTask);
            Console.WriteLine("Task added.");
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
                string status = task.IsComplete ? "Complete" : "Open";

                Console.WriteLine($"{task.Id}. [{status}] {task.Title}");

                if (!string.IsNullOrWhiteSpace(task.Notes))
                {
                    Console.WriteLine($"   Notes: {task.Notes}");
                }

                Console.WriteLine($"   Created: {task.CreatedAt}");
            }
        }
    }
    else if (choice == "3")
    {
        Console.Write("Enter the task ID to complete: ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int taskId))
        {
            TaskItem? task = tasks.Find(t => t.Id == taskId);

            if (task is not null)
            {
                task.IsComplete = true;
                Console.WriteLine("Task marked complete.");
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
            TaskItem? task = tasks.Find(t => t.Id == taskId);

            if (task is not null)
            {
                tasks.Remove(task);
                Console.WriteLine("Task deleted.");
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
        Console.WriteLine("Goodbye.");
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice. Try again.");
    }
}

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string? Notes { get; set; }

    public bool IsComplete { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}