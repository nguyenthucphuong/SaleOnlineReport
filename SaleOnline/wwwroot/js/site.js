// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//$('.form-control').on('input', function () {
//    var keyword = $(this).val().toLowerCase();
//    $('.dropdown-menu .search-result').each(function () {
//        var text = $(this).text().toLowerCase();
//        if (text.indexOf(keyword) > -1) {
//            $(this).show();
//        } else {
//            $(this).hide();
//        }
//    });
//    $(this).dropdown('toggle');
//});
//$('.search-result').click(function (event) {
//    event.preventDefault();
//    var selectedText = $(this).text().trim();
//    $('.form-control').val(selectedText);
//    $('form').attr('action', '/danh-muc/danh-sach?filter=' + selectedText);
//    $('form').submit();
//});
//$('.form-control').on('focus', function () {
//    // Lấy đường dẫn của trang hiện tại
//    var currentUrl = window.location.href;

//    // Kiểm tra xem có query parameter keyword không
//    var hasKeyword = currentUrl.includes('?filter=');

//    // Nếu có, xóa query parameter đó và điều hướng về trang ban đầu
//    if (hasKeyword) {
//        var baseUrl = currentUrl.split('?')[0];
//        window.location.href = baseUrl;
//        // Kích hoạt sự kiện focus để giữ chuột ở ô tìm kiếm
//    }

//});