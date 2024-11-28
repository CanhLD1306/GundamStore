function GetAllBanners() {
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