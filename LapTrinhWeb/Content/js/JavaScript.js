$(document).ready(function () {
    $('.btn-add-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        var productName = $(this).closest('.product-card').find('.title').text();

        // Hiển thị thông báo
        alert('Đã thêm "' + productName + '" vào giỏ hàng!');