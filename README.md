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
    var bigFilesInDirectory = directory.GetFiles().Where(x=> x.Length > 10737418240);
    
    // File Copying:
    var newFile = directory.GetFile("newNonExistingFile.txt");
    await fileTwoLevelsDown.CopyToAsync(newFile, overwrite: false, CancellationToken.None);
    
# License 
ObjectivePaths is dual-licensed. It is free to use in open-source projects that are compatible with the license
AGPL. For closed source projects, a  commercial license must be obtained from the Author, Michael D. Corbett. Due to the nature of dual-licensing, all contributions to to
the official project's repositories must be released under the MIT License:  (https://opensource.org/license/mit/).

Contributors to this project are welcome to add their attribution information to the project under the Contributors section below:

# Contributors 
<add your credit here>
    
# ObjectivePaths Copyright 
Copyright (c) 2023 Michael D. Corbett (Github: @vector-man).
