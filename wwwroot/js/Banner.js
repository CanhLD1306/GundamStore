var GetAllBannersUrl = 'Banners/ListAllBanners';
var CreateBannerUrl = 'Banners/Create';
var UpdateBannerUrl = 'Banners/Edit/';
var DeleteBannerUrl = 'Banners/Delete/';
var DeleteConfirmUrl = 'Banners/DeleteConfirm/';


// Open Create Banner Modal
function ShowAddBannerModal() {
  $("#addBannerModal").modal("show");
}

// Open Edit Banner Modal
function ShowEditBannerModal(id) {
  $.ajax({
    url: UpdateBannerUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#editBannerModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

// Open Delete Banner Modal
function ShowDeleteBannerModal(id) {
  $.ajax({
    url: DeleteBannerUrl + id,
    type: "GET",
    success: function (data) {
        $("#modal-Container").html(data);
        $("#deleteBannerModal").modal("show");
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function CreateBanner() {
  var formData = new FormData();
  var fileImage = $("#fileImage")[0].files[0];
  var description = $("#description").val();

  if (!fileImage) {
    document.getElementById("fileHelp").textContent = "Please upload file image.";
    return false;
  }
 
  formData.append("fileImage", fileImage);
  formData.append("description", description);

  $.ajax({
    url: CreateBannerUrl,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal();
        GetAllBanners();
        toastr.success("Banner created successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function UpdateBanner(id) {
  var formData = new FormData();
  var description = $("#description").val();

  formData.append("description", description);

  $.ajax({
    url: UpdateBannerUrl + id,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal();
        GetAllBanners();
        toastr.success("Banner updated successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    },
  });
}  

function DeleteConfirm(id)
{
  $.ajax({
    url: DeleteConfirmUrl + id,
    type: "POST",
    success: function (response) {
      if (response.success) {
        CloseModal();
        GetAllBanners();
        toastr.success("Banner delete successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    }
  });
}

function CloseModal() {
  $("#addBannerModal").modal("hide");
  $("#editBannerModal").modal("hide");
  $("#deleteBannerModal").modal("hide");
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

function GetAllBanners(){
  $.ajax({
    url: GetAllBannersUrl,
    success: function(data) {
        $('#banner-list-container').html(data);
    },
    error: function(error) {
        console.log("Error loading banners:", error);
    }
  });
}

$(document).ready(function () {
  $("#addBannerModal").on("hidden.bs.modal", function () {
    $(this).find("form")[0].reset();
    $("#preview").attr("src", "").hide();
    document.getElementById("fileHelp").textContent = "";
  });

  GetAllBanners();
});
