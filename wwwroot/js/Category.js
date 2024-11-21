var GetAllCategoriesUrl = 'Categories/ListAllCategories';
var CreateCategoryUrl = 'Categories/Create';
var UpdateCategoryUrl = 'Categories/Edit/';
var DeleteCategoryUrl = 'Categories/Delete/';
var DeleteCategoryConfirmUrl = 'Categories/DeleteConfirm/';


// Open Create Category Modal
function ShowAddCategoryModal() {
  $("#addCategoryModal").modal("show");
}

// Open Edit Category Modal
function ShowEditCategoryModal(id) {
  $.ajax({
    url: UpdateCategoryUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#editCategoryModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

// Open Delete Category Modal
function ShowDeleteCategoryModal(id) {
  $.ajax({
    url: DeleteCategoryUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#deleteCategoryModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function CreateCategory() {
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
    url: CreateCategoryUrl,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal('addCategoryModal');
        GetAllCategories();
        toastr.success("Category created successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function UpdateCategory(id) {
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
    url: UpdateCategoryUrl + id,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal('editCategoryModal');
        GetAllCategories();
        toastr.success("Category updated successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}  

function DeleteCategoryConfirm(id)
{
  $.ajax({
    url: DeleteCategoryConfirmUrl + id,
    type: "POST",
    success: function (response) {
      if (response.success) {
        CloseModal('deleteCategoryModal');
        GetAllCategories();
        toastr.success("Category delete successfully.");
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

function GetAllCategories(){
  $.ajax({
    url: GetAllCategoriesUrl,
    success: function(data) {
        $('#category-list-container').html(data);
    },
    error: function(error) {
        console.log("Error loading categories:", error);
    }
  });
}

document.getElementById("name")?.addEventListener("input", function () {
  document.getElementById("nameError").textContent = "";
})

$(document).ready(function () {
  $("#addCategoryModal").on("hidden.bs.modal", function () {
    $(this).find("form")[0].reset();
    $("#preview").attr("src", "").hide();
    document.getElementById("fileHelp").textContent = "";
  });

  GetAllCategories();
});
