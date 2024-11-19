// Open Create Banner Modal
function ShowAddBannerModal() {
  $("#addBannerModal").modal("show");
}

// Open Edit Banner Modal
function ShowEditBannerModal() {
  $("#editBannerModal").modal("show");
}

// Open Delete Banner Modal
function ShowDeleteBannerModal() {
  $("#deleteBannerModal").modal("show");
}

function CloseModal() {
  $("#addBannerModal").modal("hide");
  $("#editBannerModal").modal("hide");
  $("#deleteBannerModal").modal("hide");
}

function createBanner() {
  // Prevent the default form submission
  event.preventDefault();

  // Create a FormData object to handle the file and other form data
  var formData = new FormData();

  // Get the file and description from the form fields
  var fileImage = $("#fileImage")[0].files[0];
  var description = $("#description").val();

  // Append the file and description to the FormData object
  formData.append("fileImage", fileImage);
  formData.append("description", description);

  $.ajax({
    url: '@Url.Action("Create", "Banners", new { area = "Admin" })', // Replace with the correct controller and action
    type: "POST",
    data: formData,
    contentType: false, // Important to set this to false for file uploads
    processData: false, // Prevent jQuery from processing the data
    success: function (response) {
      // Handle the success response
      if (response.Success) {
        alert(response.Message);
        // Close the modal and reset the form
        ClosePopup();
        location.reload();
      } else {
        alert("Error: " + response.Message);
      }
    },
    error: function (xhr, status, error) {
      // Handle errors if any
      alert("An error occurred: " + error);
    },
  });
}

$(document).ready(function () {
  // Clear form fields when modal is hidden
  $("#addBannerModal").on("hidden.bs.modal", function () {
    $(this).find("form")[0].reset();
  });
});
