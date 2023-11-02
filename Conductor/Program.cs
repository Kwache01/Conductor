using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        FileExplorer fileExplorer = new FileExplorer();
        fileExplorer.Run();
    }
}

class FileExplorer
{
    private Stack<string> pathStack;
    private DriveInfo[] drives;
    private string currentPath;
    private int selectedOption;

    public FileExplorer()
    {
        pathStack = new Stack<string>();
        drives = DriveInfo.GetDrives();
        currentPath = "";
        selectedOption = 0;
    }

    public void Run()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            DisplayDrives();

            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                if (pathStack.Count > 0)
                {
                    currentPath = pathStack.Pop();
                }
                else
                {
                    break;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (currentPath == "")
                {
                    int driveIndex = selectedOption - 1;
                    if (driveIndex >= 0 && driveIndex < drives.Length)
                    {
                        currentPath = drives[driveIndex].RootDirectory.FullName;
                        pathStack.Push(currentPath);
                    }
                }
                else
                {
                    int selectedIndex = selectedOption - 1;
                    if (selectedIndex >= 0 && selectedIndex < Directory.GetFileSystemEntries(currentPath).Length)
                    {
                        string selectedEntry = Directory.GetFileSystemEntries(currentPath)[selectedIndex];
                        if (Directory.Exists(selectedEntry))
                        {
                            currentPath = selectedEntry;
                            pathStack.Push(currentPath);
                        }
                        else
                        {
                            OpenFile(selectedEntry);
                        }
                    }
                }
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedOption = Math.Max(0, selectedOption - 1);
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (currentPath == "")
                {
                    selectedOption = Math.Min(drives.Length, selectedOption + 1);
                }
                else
                {
                    selectedOption = Math.Min(Directory.GetFileSystemEntries(currentPath).Length, selectedOption + 1);
                }
            }
            else if (keyInfo.Key == ConsoleKey.A && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                if (currentPath != "")
                {
                    CreateFile();
                }
            }
            else if (keyInfo.Key == ConsoleKey.D && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                if (currentPath != "")
                {
                    CreateDirectory();
                }
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (currentPath != "")
                {
                    DeleteEntry();
                }
            }
        } while (true);
    }

    private void DisplayDrives()
    {
        Console.Clear();
        Console.WriteLine("Выберите диск:");
        for (int i = 0; i < drives.Length; i++)
        {
            if (i == selectedOption - 1)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine($"{i + 1}. {drives[i].Name} ({drives[i].DriveFormat}) - {drives[i].TotalSize / 1024 / 1024 / 1024} ГБ, свободно {drives[i].AvailableFreeSpace / 1024 / 1024 / 1024} ГБ");
            Console.ResetColor();
        }

        if (currentPath != "")
        {
            Console.WriteLine();
            Console.WriteLine("Esc - Назад");
            Console.WriteLine("Ctrl+A - Добавить файл");
            Console.WriteLine("Ctrl+D - Добавить директорию");
            Console.WriteLine("Delete - Удалить");
            Console.WriteLine();
            Console.WriteLine("Выберите папку или файл:");
            string[] entries = Directory.GetFileSystemEntries(currentPath);
            for (int i = 0; i < entries.Length; i++)
            {
                if (i == selectedOption - 1)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"{i + 1}. {Path.GetFileName(entries[i])}");
                Console.ResetColor();
            }
        }
    }

    private void OpenFile(string filePath)
    {
        try
        {
            Console.WriteLine($"Открытие файла: {filePath}");
            System.Diagnostics.Process.Start(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при открытии файла: {ex.Message}");
        }
    }

    private void CreateFile()
    {
        Console.Write("Введите имя файла: ");
        string fileName = Console.ReadLine();
        string filePath = Path.Combine(currentPath, fileName);

        try
        {
            File.Create(filePath).Close();
            Console.WriteLine($"Файл {fileName} создан.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        }
    }

    private void CreateDirectory()
    {
        Console.Write("Введите имя директории: ");
        string directoryName = Console.ReadLine();
        string directoryPath = Path.Combine(currentPath, directoryName);

        try
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Директория {directoryName} создана.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании директории: {ex.Message}");
        }
    }

    private void DeleteEntry()
    {
        int selectedIndex = selectedOption - 1;
        string selectedEntry = Directory.GetFileSystemEntries(currentPath)[selectedIndex];
        bool isDirectory = Directory.Exists(selectedEntry);

        try
        {
            if (isDirectory)
            {
                Directory.Delete(selectedEntry, true);
                Console.WriteLine($"Директория {selectedEntry} удалена.");
            }
            else
            {
                File.Delete(selectedEntry);
                Console.WriteLine($"Файл {selectedEntry} удален.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении: {ex.Message}");
        }
    }
}
