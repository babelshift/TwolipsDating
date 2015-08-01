$(document).ready(function () {
    
});

function onTextQtyBlur(e, obj, itemNumber) {
    var itemQuantity = $(obj).val();
    var itemPrice = $('#Items_' + itemNumber + '__Item_PointsCost').val();
    var totalItemPrice = itemQuantity * itemPrice;
    $('#total-cost-' + itemNumber).text(totalItemPrice);

    var subtotal = 0;

    $('.total-cost').each(function () {
        subtotal += parseInt($(this).text());
    });

    $('#subtotal').text(subtotal);
}