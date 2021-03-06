﻿$(document).ready(function () {
    $('.followify').followify();

    $('.store-item-wrapper').on({
        mouseenter: function () {
            $('.store-item', this).attr('style', "border: 1px solid #ddd; box-shadow: 0 2px 2px rgba(0, 0, 0, 0.15); padding: 5px");
            $('.store-item-details', this).hide();
            $('.store-item-buy', this).removeClass('hidden');
        },
        mouseleave: function () {
            $('.store-item', this).attr('style', "border: 1px solid transparent; padding: 5px");
            $('.store-item-details', this).show();
            $('.store-item-buy', this).addClass('hidden');
        }
    });
    
    $('.store-item-buy-button').on('click', function () {
        var itemId = $(this).attr('data-item-id');
        var itemTypeId = $(this).attr('data-item-type-id');

        var jsonObject = {
            "storeItemId": itemId,
            "storeItemTypeId": itemTypeId
        };

        var json = JSON.stringify(jsonObject);

        postJson('/store/addToCart', json, function (data) {
            if (data.success) {
                var currentShoppingCartCount = parseInt($('#shopping-cart-count').text());
                $('#shopping-cart-count').text(++currentShoppingCartCount);
                $('.custom-tooltip').tooltip('hide');
                $('#modal-buy').modal('show');
            }
            else {
            }
        });
    });
});

