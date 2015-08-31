$(document).ready(function () {
    $("#purchase-result").hide();

    $('#modalGift').on('show.bs.modal', function (event) {
        $("#text-gift-count").val("");
        $("#text-charged").hide();
        var button = $(event.relatedTarget); // Button that triggered the modal
        var giftId = button.data('gift-id'); // Extract info from data-* attributes
        var giftName = button.data('gift-name');
        var giftPrice = button.data('gift-price');
        var modal = $(this);
        $('#hidden-gift-id').val(giftId);
        $('#hidden-gift-name').val(giftName);
        $('#hidden-gift-price').val(giftPrice);
        $('#span-gift-name').html(giftName);
        $('#span-gift-price').html(giftPrice);
    });
});

function onTextGiftCountBlur() {
    $("#text-charged").show();
    var giftCount = $("#text-gift-count").val();
    var giftPrice = $("#hidden-gift-price").val();
    $("#span-total-price").html(giftCount * giftPrice);
}

function onBuyGift(e, obj) {
    e.preventDefault();

    var giftId = $("#hidden-gift-id").val();
    var giftName = $("#hidden-gift-name").val();
    var giftCount = $("#text-gift-count").val();
    var purchasePointCost = $("#span-total-price").html();

    var json = '{"giftId":' + giftId + ', "giftCount":' + giftCount + '}';

    postJson('/store/buyGift', json, function (data) {
        if (data.success) {
            $("#purchase-result").removeClass("alert-success");
            $("#purchase-result").removeClass("alert-danger");
            $("#purchase-result").addClass("alert-success");
            $("#purchase-result").show();
            $("#purchase-result-message").html("You successfully purchased <strong>" + data.count + "x " + giftName + "</strong> for a total of&nbsp;&nbsp;<strong><i class=\"fa fa-money\"/></i> " + purchasePointCost + "</strong>.");
            var userPointsCount = $("#span-points-count").html();
            var userPointsCountAfterPurchase = userPointsCount - purchasePointCost;
            $("#span-points-count").html(userPointsCountAfterPurchase);
        }
        else {
            $("#purchase-result").removeClass("alert-success");
            $("#purchase-result").removeClass("alert-danger");
            $("#purchase-result").addClass("alert-danger");
            $("#purchase-result").show();
            $("#purchase-result-message").html("You cannot buy <strong>" + data.count + "x " + giftName + "</strong>.");
        }
    });
}