$(document).ready(function () {
    $("#purchase-result").hide();
});

function onBuyTitle(e, obj, titleId) {
    e.preventDefault();

    var jsonObject = {
        "titleId": titleId
    };

    var json = JSON.stringify(jsonObject);

    postJson('/store/buyTitle', json, function (data) {
        if (data.success) {
            var buyButton = $("#button-buy-" + titleId);
            buyButton.removeClass("btn-primary");
            buyButton.addClass("btn-success");
            buyButton.prop("disabled", true);
            buyButton.html("Already purchased");
            $("#table-row-" + titleId).addClass("success");
        }
        else {
            $("#purchase-result").removeClass("alert-success");
            $("#purchase-result").removeClass("alert-danger");
            $("#purchase-result").addClass("alert-danger");
            $("#purchase-result").show();
            $("#purchase-result-message").html("You cannot buy that title.");
        }
    });
}