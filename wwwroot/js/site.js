document.addEventListener("DOMContentLoaded", function () {
    var fileInput = document.getElementById("ImageFile");
    var dropLabel = document.getElementById("file-drop-label");

    if (fileInput && dropLabel) {
        var defaultText = dropLabel.textContent;
        fileInput.addEventListener("change", function () {
            if (fileInput.files && fileInput.files.length > 0) {
                dropLabel.textContent = "Selected: " + fileInput.files[0].name;
            } else {
                dropLabel.textContent = defaultText;
            }
        });
    }
});
