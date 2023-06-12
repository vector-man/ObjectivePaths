# ObjectivePaths
ObjectivePaths is a wrapper for .NET filesystem operations. ObjectivePaths attempts to make dealing with filesystem paths less error-prone, by treating each directory and file as an object. View Demo project to see an example.

## Examples:
    // Chaining:
    // Create FileSystemService
    var fileSystemService = new LocalFileSystemService(new FileSystem());
    
    // Get a file called "C.txt" in sub directory "A\B" of someAbsolutePath:
    var directory = fileSystemService.GetDirectory(someAbsolutePath);
    var fileTwoLevelsDown = directory.GetDirectory("A").GetDirectry("B").GetFile("C.txt");
    
    // LINQ Querying:
    var bigFilesInDirectoryB = directory.GetFiles().Where(x=> x.Length > 10737418240);
    
    // File Copying:
    var newFile = directory.GetFile("newNonExistingFile.txt");
    await fileTwoLevelsDown.CopyToAsync(newFile, overwrite: false, CancellationToken.None);

Contributors: please review LICENSE.txt before using the library or submitting a contribution. All Subissions must be released under the MIT License (https://opensource.org/license/mit/) in order to be compatible with the project's dual-license model.

Copyright (c) 2023 Michael D. Corbett (Github: @vector-man) .
