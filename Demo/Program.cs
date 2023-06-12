// See https://aka.ms/new-console-template for more information

using System.IO;
using System.IO.Abstractions;
using ObjectivePaths.IO;

Console.WriteLine("ObjectivePaths Demo. Copyright (C) 2023 Michael D. Corbett.");

// Create FileSystemService
var fileSystemService = new LocalFileSystemService(new FileSystem());

Console.WriteLine("Type valid directory path to list files (only top level are listed in this demo):");

// Get directory path from user.
var path = Console.ReadLine();

// Get directory as object from path:
var directory = fileSystemService.GetDirectory(path);

// List all files in the specified directory:
try
{
    Console.WriteLine($"Directory {directory.AbsolutePath}'s files:");

    foreach (var file in directory.GetFiles())
    {
        try
        {
            Console.WriteLine($"{file.Name}, (Length: {file.Length} Bytes)");
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore.
            Console.WriteLine($"Unauthorized Access Exception for: {file.AbsolutePath}");
        }
    }
}
catch (DirectoryNotFoundException ex)
{
    Console.WriteLine($"{ex.Message}");

}

{
    Console.WriteLine("Done! Happy coding :D!");
}


