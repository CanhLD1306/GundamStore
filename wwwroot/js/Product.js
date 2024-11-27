const GetAllProductsUrl = 'Products/ListAllProducts';
const EditProductUrl = 'Products/Edit/';

const imageModel = {
    id: 0,
    url: "",
    file: null,
    isDefault: false,
    isDeleted: false
};

let images = [];



function GetAllProducts() {
    $.ajax({
        url: GetAllProductsUrl,
        success: function(data) {
            $('#product-list-container').html(data);
        },
        error: function(error) {
            console.log("Error loading banners:", error);
        }
    });
}

function CreateNewProduct() {
    const formData = new FormData();
    const name = $("#name").val();
    const categoryId = $("#category").val();
    const scaleId = $("#scale").val();
    const brand = $("#brand").val();
    const price = $("#price").val();
    const quantity = $("#quantity").val();
    const discount = $("#discount").val();
    const description = quill.root.innerHTML;

    if(!validationAddProductForm()){
        return;
    }

    formData.append("name", name);
    formData.append("categoryId", categoryId);
    formData.append("scaleId", scaleId);
    formData.append("brand", brand);
    formData.append("price", price);
    formData.append("stockQuantity", quantity);
    formData.append("discount", discount);
    formData.append("description", description);

    for (let i = 0; i < images.length; i++) {
        formData.append(`images[${i}].Id`, images[i].id);
        formData.append(`images[${i}].Url`, images[i].url);
        formData.append(`images[${i}].File`, images[i].file);
        formData.append(`images[${i}].IsDefault`, images[i].isDefault);
        formData.append(`images[${i}].IsDeleted`, images[i].isDeleted);
    }

    $.ajax({
        url: CreateProductUrl,
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function(response) {
            if (response.success) {
                toastr.success(response.message);
                window.location.href = response.redirectUrl;
            } else {
                toastr.error(response.message);
            }
        },
        error: function(error) {
            toastr.error("An error occurred: " + error);
        }
    });
}

function EditOnClick(id) {
    $.ajax({
        url: EditProductUrl + id,
        type: "GET",
        success: function(response) {
            if (response.success) {

            } else {
                toastr.error(response.message);
            }
        },
        error: function(error) {
            toastr.error("An error occurred: " + error);
        }
    });
}

function UpdateProduct() {
    
}

function updateCarousel(images) {
    const carousel = document.getElementById('carouselExampleControls');
    const carouselInner = document.getElementById('carousel-images');

    carouselInner.innerHTML = '';

    if (images.filter(image => !image.isDeleted).length === 0) {
        carousel.style.display = 'none';
        return;
    }

    carousel.style.display = 'block';

    Array.from(images).filter(image => !image.isDeleted).forEach((image, index) => {
        const carouselItem = document.createElement('div');
        carouselItem.classList.add('carousel-item');
        if (index === 0) {
            carouselItem.classList.add('active');
        }
        

        const img = document.createElement('img');
        img.classList.add('d-block', 'w-100');
        img.src = image.url;
        img.style.height = '300px';
        img.style.objectFit = 'contain';

        const formCheckDiv = document.createElement('div');
        formCheckDiv.classList.add('form-check', 'form-check-inline', 'text-center', 'mt-4');

        const radioButton = document.createElement('input');
        radioButton.classList.add('form-check-input');
        radioButton.type = 'radio';
        radioButton.name = 'inlineRadioOptions';
        radioButton.value = `option${index + 1}`;
        radioButton.id = `inlineRadio${index + 1}`;
        radioButton.dataset.index = index;
        radioButton.checked = image.isDefault;
        radioButton.style.border =  '1.25px solid #a5adbd';
        radioButton.style.marginTop = '10%';

        radioButton.addEventListener('change', () => {
            images.forEach((img) => img.isDefault = false);
            image.isDefault = true;
        });

        const deleteButton = document.createElement('button');
        deleteButton.classList.add('ms-2');
        deleteButton.setAttribute('type', 'button');
        deleteButton.style.border =  'none';
        deleteButton.style.backgroundColor = 'transparent';

        deleteButton.addEventListener('click', function() {
            deleteImage(image);
            console.log(images);

        });

        const deleteIcon = document.createElement('i');
        deleteIcon.classList.add('ti', 'ti-trash', 'fs-7', 'text-danger');

        deleteButton.appendChild(deleteIcon);
        formCheckDiv.appendChild(radioButton);
        formCheckDiv.appendChild(deleteButton);
        carouselItem.appendChild(img);
        carouselItem.appendChild(formCheckDiv);
        carouselInner.appendChild(carouselItem);
    });
}

function deleteImage(image) {
    const inputElement = document.getElementById('formFileMultiple');

    if (image.id == 0) {
        const dataTransfer = new DataTransfer();

        Array.from(inputElement.files).forEach(file => {
            if (file !== image.file) {
                dataTransfer.items.add(file);
            }
        });

        const imageIndex = images.findIndex(img => img === image);
        if (imageIndex > -1) {
            images.splice(imageIndex, 1);
        }
    
        inputElement.files = dataTransfer.files;
        updateCarousel(images);
    }
    else {
        image.isDeleted = true;
        updateCarousel(images);
    }
}

function validationAddProductForm() {
    const defaultImageIndex = document.querySelector('input[name="inlineRadioOptions"]:checked')?.dataset.index;
    const name = document.getElementById('name').value;
    const category = document.getElementById('category').value;
    const scale = document.getElementById('scale').value;
    const files = document.getElementById('formFileMultiple').files;
    const valid = true;

    if (!name) {
        document.getElementById('nameError').textContent = 'This field is required.';
        valid = false;
    }

    if (!category) {
        document.getElementById('categoryError').textContent = 'This field is required.';
        valid = false;
    }

    if (!scale) {    
        document.getElementById('scaleError').textContent = 'This field is required.';
        valid = false;
    }
    
    if(!brand) {
        document.getElementById('brandError').textContent = 'This field is required.';
        valid = false;
    }

    if (images.length === 0) {
        document.getElementById("fileHelp").textContent = "Please upload file image.";
        valid = false;
    }

    if (defaultImageIndex === undefined) {
        document.getElementById("fileHelp").textContent = "Exactly one image must be marked as default.";
        valid = false;
    } 

    return valid;
}

document.getElementById('formFileMultiple')?.addEventListener('change', handleFilesUpdated) 

document.getElementById('price')?.addEventListener('keydown', function(event) {
    const value = event.target.value;
    if (event.key === "-") {
        event.preventDefault();
    }
    if (value.includes(".")) {
        let parts = value.split(".");
        if (parts[1].length >= 2 && event.key !== "Backspace" && event.key !== "ArrowLeft" && event.key !== "ArrowRight") {
            event.preventDefault();
        }
    }
});

document.getElementById('price')?.addEventListener('blur', function(event) {
    if(!event.target.value) {
        event.target.value = 0.01;
    }
});

document.getElementById('quantity')?.addEventListener('keydown', function(event) {
    if (event.key === "-") {
        event.preventDefault();
    }
    if (event.key === '.' || event.key === ',') {
        event.preventDefault();
    }
});

document.getElementById('quantity')?.addEventListener('blur', function(event) {
    if(!event.target.value) {
        event.target.value = 1;
    }
});

document.getElementById('discount')?.addEventListener('keydown', function(event) {
    if (event.key === "-") {
        event.preventDefault();
    }
    if (event.key === '.' || event.key === ',') {
        event.preventDefault();
    }
});

document.getElementById('discount')?.addEventListener('blur', function(event) {
    if(!event.target.value) {
        event.target.value = 0;
    }
});

document.getElementById('discount')?.addEventListener('input', function(event) {
    const value = event.target.value;
    if (value > 100) {
        event.target.value = 100;
    }
});

document.getElementById("name")?.addEventListener("input", function () {
    document.getElementById("nameError").textContent = "";
})

document.getElementById("category")?.addEventListener("input", function () {
    document.getElementById("categoryError").textContent = "";
})

document.getElementById("scale")?.addEventListener("input", function () {
    document.getElementById("scaleError").textContent = "";
})

document.getElementById("brand")?.addEventListener("input", function () {
    document.getElementById("brandError").textContent = "";
})

document.getElementById('carousel-images')?.addEventListener('change', function(event) {
    if (event.target.name === "inlineRadioOptions") {
        document.getElementById("fileHelp").textContent = "";
    }
});


function handleServerImages(serverImages) {
    serverImages.forEach(img => {
        const serverImage = {
            ...imageModel,
            id: img.id,   
            url: img.imageURL,
            isDefault: img.isDefault,
            isDeleted: img.isDeleted,
        };
        images.push(serverImage);
    });
    updateCarousel(images);
}

function handleFilesUpdated(event) {
    const files = event.target.files;
    const fileArray = Array.from(files);
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];

    for (let file of files){
        if(!allowedTypes.includes(file.type)){
            document.getElementById("fileHelp").textContent = "Some files have an invalid format. Please upload image files only.";
            event.target.value = null;
            return;
        }    
    }

    if(images.filter(img => !img.isDeleted).length + fileArray.length > 10) {
        document.getElementById("fileHelp").textContent = "You can only upload up to 10 images.";
        event.target.value = null;
        return;
    }

    fileArray.forEach((file) => {
        const newImage = { 
            ...imageModel, 
            file: file, 
            url: URL.createObjectURL(file) };
        images.push(newImage);
    });
    updateCarousel(images);
}

function UpdateProduct(id) {
    const formData = new FormData();
    const name = $("#name").val();
    const categoryId = $("#category").val();
    const scaleId = $("#scale").val();
    const brand = $("#brand").val();
    const price = $("#price").val();
    const quantity = $("#quantity").val();
    const discount = $("#discount").val();
    const description = quill.root.innerHTML;

    if(!validationAddProductForm()){
        return;
    }

    console.log(quantity);

    formData.append("name", name);
    formData.append("categoryId", categoryId);
    formData.append("scaleId", scaleId);
    formData.append("brand", brand);
    formData.append("price", price);
    formData.append("stockQuantity", quantity);
    formData.append("discount", discount);
    formData.append("description", description);

    for (let i = 0; i < images.length; i++) {
        formData.append(`images[${i}].Id`, images[i].id);
        formData.append(`images[${i}].Url`, images[i].url);
        formData.append(`images[${i}].File`, images[i].file);
        formData.append(`images[${i}].IsDefault`, images[i].isDefault);
        formData.append(`images[${i}].IsDeleted`, images[i].isDeleted);
    }

    

    $.ajax({
        url: UpdateProductUrl + "/" + id,
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function(response) {
            if (response.success) {
                toastr.success(response.message);
            } else {
                toastr.error(response.message);
            }
        },
        error: function(error) {
            toastr.error("An error occurred: " + error);
        }
    });
}

// Open Delete Banner Modal
function DeleteOnclick(id) {
    $.ajax({
      url: DeleteProductUrl + "/" + id,
      type: "GET",
      success: function (data) {
          $("#modal-Container").html(data);
          $("#deleteProductModal").modal("show");
      },
      error: function (error) {
        toastr.error("An error occurred: " + error);
      },
    });
  }

function DeleteProductConfirm(id)
{
  $.ajax({
    url: DeleteProductConfirmUrl + "/" + id,
    type: "POST",
    success: function (response) {
      if (response.success) {
        CloseModal('deleteProductModal');
        GetAllProducts();
        toastr.success("Product delete successfully.");
      } else {
        toastr.error(response.message);
      }
    },
    error: function (error) {
      toastr.error("An error occurred: " + error);
    }
  });
}