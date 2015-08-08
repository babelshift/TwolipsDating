$(document).ready(function () {
    $('.remove-item').on('click', function (e) {
        e.preventDefault();
        var itemId = $(this).attr('data-item-id');
        var itemNumber = $(this).attr('data-item-number');
        var json = '{storeItemId:' + itemId + '}';
        postJson('/store/removeCartItem', json, function (data) {
            if (data.success) {

                var rowCount = $('#shopping-cart-table tr').length;
                if (rowCount <= 3) {
                    $('#shopping-cart-table').fadeOut('normal', function () {
                        $(this).parent().html('<h4>Your shopping cart is empty.</h4>');
                    });
                } else {
                    $('#row-item-' + itemNumber).fadeOut('normal', function () {
                        var itemPrice = $('#Items_' + itemNumber + '__IsRemoved').val(true);
                        $(this).hide();
                    });
                }
            }
        });
    });
});

function onTextQtyBlur(e, obj, itemNumber) {
    var itemQuantity = $(obj).val();
    var itemPrice = $('#Items_' + itemNumber + '__Item_DiscountedPointsCost').val();
    var totalItemPrice = itemQuantity * itemPrice;
    $('#total-cost-' + itemNumber).text(totalItemPrice);

    var subtotal = 0;

    $('.total-cost').each(function () {
        subtotal += parseInt($(this).text());
    });

    $('#subtotal').text(subtotal);
}