const GetAllBannersUrl = 'Banners/ListAllBanners';
const CreateBannerUrl = 'Banners/Create';
const UpdateBannerUrl = 'Banners/Edit/';
const DeleteBannerUrl = 'Banners/Delete/';
const DeleteBannerConfirmUrl = 'Banners/DeleteConfirm/';


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
  const formData = new FormData();
  const fileImage = $("#fileImage")[0].files[0];
  const description = $("#description").val();

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
        CloseModal('addBannerModal');
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
  const formData = new FormData();
  const description = $("#description").val();

  formData.append("description", description);

  $.ajax({
    url: UpdateBannerUrl + id,
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (response) {
      if (response.success) {
        CloseModal('editBannerModal');
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

function DeleteBannerConfirm(id)
{
  $.ajax({
    url: DeleteBannerConfirmUrl + id,
    type: "POST",
    success: function (response) {
      if (response.success) {
        CloseModal('deleteBannerModal');
        GetAllBanners();
        toastr.success("Banner delete successfully.");
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
  const fileInput = document.getElementById("fileImage");
  const preview = document.getElementById("preview");
  const fileType = fileInput.files[0].type;
  const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];

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
    type: "GET",
    success: function(data) {
        $('#banner-list-container').html(data);
    },
    error: function(error) {
        console.log("Error loading banners:", error);
    }
  });
}

