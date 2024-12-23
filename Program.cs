﻿using PKHeX.Facade;
using PKHeX.Exporter;


// See https://aka.ms/new-console-template for more information

namespace ConsoleApplication1
{
  class Program
  {

    public static string folder;
    public static string destination;

    static void Main(string[] args)
    {

      folder = args.ElementAtOrDefault(0) == null ? "./" : args[0];
      destination = args.ElementAtOrDefault(1) == null ? "./pokemon-bank-files" : args[1];


      foreach (string file in Directory.EnumerateFiles(folder, "*POKEMON*.sav"))
      {
        var game = Game.LoadFrom(file);
        var json = new Json(game);

        json.generate(file, folder, destination);
      }

      using var watcher = new FileSystemWatcher(folder);


      watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

      watcher.Changed += OnChanged;
      watcher.Created += OnCreated;
      watcher.Deleted += OnDeleted;
      watcher.Renamed += OnRenamed;
      watcher.Error += OnError;

      watcher.Filter = "*.srm";
      watcher.IncludeSubdirectories = true;
      watcher.EnableRaisingEvents = true;

      Console.WriteLine("Press enter to exit.");
      Console.ReadLine();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {

      Console.WriteLine($"Changed: {e.FullPath}");

      var path = e.FullPath;

      var game = Game.LoadFrom(path);
      var json = new Json(game);

      json.generate(path, folder, destination);

      Console.WriteLine($"gerated json for: {e.FullPath}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
      string value = $"Created: {e.FullPath}";
      Console.WriteLine(value);


      var path = e.FullPath;

      var game = Game.LoadFrom(path);
      var json = new Json(game);

      json.generate(path, folder, destination);

      Console.WriteLine($"gerated json for: {e.FullPath}");
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
      Console.WriteLine($"Renamed:");
      Console.WriteLine($"    Old: {e.OldFullPath}");
      Console.WriteLine($"    New: {e.FullPath}");

    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
      if (ex != null)
      {
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine("Stacktrace:");
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine();
        PrintException(ex.InnerException);
      }
    }

  }
}