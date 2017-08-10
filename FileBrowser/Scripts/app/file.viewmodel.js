function FileViewModel() {
    var self = this;

    // Default Values
    self.Directory = ko.observable('~\\');
    self.Filter = ko.observable('');
    self.TotalFiles = ko.observable(0);
    self.TotalFolders = ko.observable(0);
    self.TotalSize = ko.observable(0);
    self.Files = ko.observableArray([]);

    /***************************************************************
    * GetFiles
    * author:  john greaux
    * date:    05/17/2017
    * desc:    Client-side ajax call to the webapi to populate the
    *          file browser table
    **************************************************************/
    self.GetFiles = function () {
        // Clear any existing data from the model
        self.Files.removeAll();

        $.ajax({
            url: "api/file/getfiles",
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            data: { path: self.Directory, filter: self.Filter }, 
            success: function (data) {
                if (data['success'])
                {
                    // Update file, folder, and size totals
                    self.TotalFiles(data.model.TotalFiles);
                    self.TotalFolders(data.model.TotalFolders);
                    self.TotalSize(data.model.TotalSize);

                    // Iterate through returned files and add to table
                    for (var i = 0; i < data.model.Files.length; i++) {
                        if (data.model.Files[i].Type === "File Folder") {
                            self.Files.push(new file(data.model.Files[i].Path, data.model.Files[i].Name, data.model.Files[i].LastModified, data.model.Files[i].Type, data.model.Files[i].Size));
                        }
                        else {
                            self.Files.push(new file(data.model.Files[i].Path, data.model.Files[i].Name, data.model.Files[i].LastModified, data.model.Files[i].Type, data.model.Files[i].Size));
                        }
                    }
                }
                else {
                    alert(data['exception']);
                }
            }
        });
    };

    /***************************************************************
    * GetFiles
    * author:  john greaux
    * date:    05/17/2017
    * desc:    Manages click event for folders and files. Folder 
    *          click changes directory and file click downloads
    *          the file
    **************************************************************/
    self.FolderClick = function(file) {
        // If the selected row is a folder, modify and the directory and search again
        if (file.type() === 'File Folder') {
            self.Directory(file.path());
            self.GetFiles();
        }
        // Selected row is a file, initiate file download
        else {
            $.ajax({
                url: "api/file/download",
                type: "GET",
                data: { path: file.path(), name: file.name() },
                success: function (data) {
                    // Create a temporary link to add the downloaded data to
                    var link = document.createElement('a');
                    link.download = file.name();
                    link.href = 'data:,' + data;

                    // Call the link click event to download
                    link.click();
                },
                error: function (err) {
                    console.log('fail' + err['Message']);
                }
            });
        }
    }

    /***************************************************************
    * Upload
    * author:  john greaux
    * date:    05/18/2017
    * desc:    Loads the file from the input form into FormData to
    *          be sent to the webapi for uploading
    **************************************************************/
    self.Upload = function (file) {
        // Build form data
        var data = new FormData();
        data.append("Path", self.Directory());
        data.append("File", file);

        $.ajax({
            url: "api/file/upload",
            type: "POST",
            dataType: "json",
            contentType: false,
            processData: false,
            data: data,
            success: function (data) {
                if (data['success']) {
                    alert("File uploaded successfully!");

                    // Refresh the file browser
                    self.GetFiles();
                }
                else {
                    alert("File upload failed! " + data['exception']);
                }
            }
        });
    }

    /***************************************************************
    * Delete
    * author:  john greaux
    * date:    05/17/2017
    * desc:    Ajax call to delete the selected file
    **************************************************************/
    self.Delete = function (file) {
        if (confirm("Are you sure you want to delete this file?")) {
            var data = new FormData();
            data.append("Path", file.path());
            $.ajax({
                url: "api/file/delete",
                type: "DELETE",
                dataType: "json",
                contentType: false,
                processData: false,
                data: data,
                success: function (data) {
                    if (data['success']) {
                        alert("File successfully deleted!");

                        // Refresh the file browser
                        self.GetFiles();
                    }
                    else {
                        alert("File delete failed! " + data['exception']);
                    }
                }
            });
        }
    }

    /***************************************************************
    * Copy
    * author:  john greaux
    * date:    05/19/2017
    * desc:    Uses the path input from the user to copy the 
    *           selected file to the new location
    **************************************************************/
    self.Copy = function (file) {
        // Get path to copy to
        var newPath = prompt("Enter the directory path to copy " + file.name() + " to:");

        $.ajax({
            url: "api/file/copy",
            type: "GET",
            dataType: "json",
            data: { currentPath: file.path(), newPath: newPath, fileName: file.name() },
            success: function (data) {
                if (data['success']) {
                    alert("File copied successfully!");

                    // Refresh file browser
                    self.GetFiles();
                }
                else {
                    alert("File copy failed! " + data['exception']);
                }
            }
        });
    }

    /***************************************************************
    * Move
    * author:  john greaux
    * date:    05/19/2017
    * desc:    Uses the path input from the user to move the 
    *           selected file to the new location
    **************************************************************/
    self.Move = function (file) {
        // Get new path
        var newPath = prompt("Enter the directory path to move " + file.name() + " to:");

        $.ajax({
            url: "api/file/move",
            type: "GET",
            dataType: "json",
            data: { currentPath: file.path(), newPath: newPath, fileName: file.name() },
            success: function (data) {
                if (data['success']) {
                    alert("File moved successfully!");

                    // Refresh file browser
                    self.GetFiles();
                }
                else {
                    alert("File move failed! " + data['exception']);
                }
            }
        });
    }

    /***************************************************************
    * DirectoryUp
    * author:  john greaux
    * date:    05/17/2017
    * desc:    Reformats the directory search input to move the
    *          directory up one level
    **************************************************************/
    self.DirectoryUp = function () {
        var newPath;
        var paths = self.Directory().split(/[/\\]+/);
        
        // Reconstruct path
        for (var i = 0; i < paths.length - 1; i++) {
            if (i === 0) {
                newPath = paths[0] + '\\';
            }
            else if (i === paths.length - 2) {
                newPath = newPath + paths[i];
            }
            else {
                newPath = newPath + paths[i] + '\\';
            }
        }

        // Update directory and refresh table
        self.Directory(newPath);
        self.GetFiles();
    }
}

ko.applyBindings(new FileViewModel());