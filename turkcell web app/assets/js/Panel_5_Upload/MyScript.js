Dropzone.options.root2 = {
    paramName: "file", // The name that will be used to transfer the file
    maxFilesize: 1024, // MB
    addRemoveLinks: true,
    removedfile: function (file) {
        var name = file.name;
        var UToken = document.getElementById("Upload_Token").value;
        $.ajax({
            type: 'GET',
            url: '/File/DeleteFile',
            data: "FileName=" + name + "&U_Token=" + UToken,
            dataType: 'html'
        });
        var _ref;
        return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
    },
    init: function () {
        var myDropzone = this;
        var fileNames = document.getElementsByClassName("TempFilesNames");
        var fileSizes = document.getElementsByClassName("TempFilesSizes");
        //Populate any existing thumbnails
        if (fileNames) {
            for (var i = 0; i < fileNames.length; i++) {
                var mockFile = {
                    name: fileNames[i].value,
                    size: fileSizes[i].value,
                    type: 'image/jpeg',
                    status: Dropzone.ADDED,
                    url:'../assets/img/file.png'
                };

                // Call the default addedfile event handler
                myDropzone.emit("addedfile", mockFile);

                // And optionally show the thumbnail of the file:
                myDropzone.emit("thumbnail", mockFile, '../assets/img/file.png');

                myDropzone.emit("complete", mockFile);
                myDropzone.files.push(mockFile);
            }
        }
    }
};