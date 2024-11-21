var GetAllScalesUrl = 'Scales/ListAllScales';
var CreateScaleUrl = 'Scales/Create';
var UpdateScaleUrl = 'Scales/Edit/';
var DeleteScaleUrl = 'Scales/Delete/';
var DeleteScaleConfirmUrl = 'Scales/DeleteConfirm/';


// Open Create Scale Modal
function ShowAddScaleModal() {
  $("#addScaleModal").modal("show");
}

// Open Edit Scale Modal
function ShowEditScaleModal(id) {
  $.ajax({
    url: UpdateScaleUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#editScaleModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

// Open Delete Scale Modal
function ShowDeleteScaleModal(id) {
  $.ajax({
    url: DeleteScaleUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#deleteScaleModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function CreateScale() {
  var formData = new FormData();
  var name = $("#name").val();
  var description = $("#description").val();

  if (!name) {
    document.getElementById("nameError").textContent = "This field is required.";
    return false;
  }
 
  formData.append("name", name);
  formData.append("description", description);

  $.ajax({
    url: CreateScaleUrl,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal('addScaleModal');
        GetAllScales();
        toastr.success("Scale created successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function UpdateScale(id) {
  var formData = new FormData();
  var name = $("#name").val();
  var description = $("#description").val();

  if (!name) {
    document.getElementById("nameError").textContent = "This field is required.";
    return false;
  }
 
  formData.append("name", name);
  formData.append("description", description);

  $.ajax({
    url: UpdateScaleUrl + id,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal('editScaleModal');
        GetAllScales();
        toastr.success("Scale updated successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}  

function DeleteScaleConfirm(id)
{
  $.ajax({
    url: DeleteScaleConfirmUrl + id,
    type: "POST",
    success: function (response) {
      if (response.success) {
        CloseModal('deleteScaleModal');
        GetAllScales();
        toastr.success("Scale delete successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    }
  });
}

function previewImage() {
  var fileInput = document.getElementById("fileImage");
  var preview = document.getElementById("preview");
  var fileType = fileInput.files[0].type;
  var allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];

  if($.inArray(fileType, allowedTypes) === -1) {
    document.getElementById("fileHelp").textContent = "Invalid file format. Please upload file image."
    fileInput.value = "";
    return false;
  }


  if (fileInput.files && fileInput.files[0]) {
    const reader = new FileReader();

    reader.onload = function (e) {

      preview.src = e.target.result;
      preview.style.display = "block";
    };
    reader.readAsDataURL(fileInput.files[0]);
  }
}

function GetAllScales(){
  $.ajax({
    url: GetAllScalesUrl,
    success: function(data) {
        $('#scale-list-container').html(data);
    },
    error: function(error) {
        console.log("Error loading scales:", error);
    }
  });
}

document.getElementById("name")?.addEventListener("input", function () {
  document.getElementById("nameError").textContent = "";
})

$(document).ready(function () {
  $("#addScaleModal").on("hidden.bs.modal", function () {
    $(this).find("form")[0].reset();
    $("#preview").attr("src", "").hide();
    document.getElementById("fileHelp").textContent = "";
  });

  GetAllScales();
});
