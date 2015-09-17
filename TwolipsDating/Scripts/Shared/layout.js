function setupPopoverWithContent(elementName, contentFunction) {
    // loop through all share review links and turn them into valid popovers with share buttons
    $(elementName).each(function () {
        var popover = $(this).popover({
            content: contentFunction,
            trigger: "manual",
            html: true
        });

        var id = $(this).attr('id');

        $(this).off('click.' + id);
        $(this).on('click.' + id, function (e) {
            e.preventDefault();
            popover.popover('show');
        });

        // we remove handlers first to avoid stacking handlers
        $(document).off('click.document.' + id);
        $(document).on('click.document.' + id, function (e) {
            var isPopover = $(e.target).is(popover) || $(e.target).closest('#' + id).length > 0;
            var inPopover = $(e.target).closest('.popover').length > 0;
            if (!isPopover && !inPopover) {
                popover.popover('hide');
            }
        });
    });
}

// sets up a popover based on some html
// the popover will take html content, will not jump to the top when clicked, and will dismiss when anything other than the popover is clicked
function setupHtmlPopover(elementName, contentName) {
    var popover = $(elementName).popover({
        trigger: "manual",
        html: true,
        content: function () {
            return $(contentName).html();
        }
    });

    // we remove handlers first to avoid stacking handlers
    $(elementName).off('click.' + elementName);
    $(elementName).on('click.' + elementName, function (e) {
        e.preventDefault();
        popover.popover('show');
    });

    // we remove handlers first to avoid stacking handlers
    $(document).off('click.document.' + elementName);
    $(document).on('click.document.' + elementName, function (e) {
        var isPopover = $(e.target).is(popover) || $(e.target).closest(elementName).length > 0;
        var inPopover = $(e.target).closest('.popover').length > 0;
        if (!isPopover && !inPopover) {
            popover.popover('hide');
        }
    });

    return popover;
}

// posts to the passed url with the passed json as data and on success will execute the passed function
function postJson(url, json, successFunction) {
    $.ajax({
        data: json,
        type: 'POST',
        url: url,
        contentType: 'application/json; charset=utf-8',
        success: successFunction
    });
}

// posts to the passed url with no data and on success will execute the passed function
function post(url, successFunction) {
    $.ajax({
        type: 'POST',
        url: url,
        contentType: 'application/json; charset=utf-8',
        success: successFunction
    });
}

// gets the passed url with no data and on success will execute the passed function
function get(url, successFunction) {
    $.ajax({
        type: 'GET',
        url: url,
        success: successFunction
    });
}

$(document).ready(function () {
    if (window.canRunAds === undefined) {
        $('#adblocker-warning').removeClass('hidden');
    }

    $('.custom-tooltip').tooltip({ animation: false });

    // setup announcement/gift popovers
    var announcementPopover = setupHtmlPopover('#popover-announcements', '#popover-announcements-content');
    var giftsPopover = setupHtmlPopover('#popover-gifts', '#popover-gifts-content');

    // when the gifts popover is hidden, re-create it based on any edits made by the user
    $("#popover-gifts").on("hidden.bs.popover", function () {
        var giftsPopover = setupHtmlPopover('#popover-gifts', '#popover-gifts-content');
    });

    // when the button to remove all gift notifications is clicked, post to the server to remove all notifications and then re-create the popover with all cleared
    $(document).on("click", ".popover .remove-all-gift-notifications", function (e) {
        e.preventDefault();

        var owner = $(this);

        post('/profile/removeAllGiftNotifications',
            function (data) {
                if (data.success) {
                    var html = "<p>Nothing to see here.</p>"

                    // clear out the popover itself
                    owner.parent().parent().html(html);

                    // clear out the popover's source
                    $("#popover-gifts-container").html(html);

                    // change indicators on notification button
                    $("#span-gift-notification-count").text("0");
                    $("#gift-notification-count").val("0");
                } else {
                    alert(data.error);
                }
            });
    });

    // when the user clicks to remove a single gift notification, post to the server to remove it and on success remove that notification from the UI
    $(document).on("click", ".popover .remove-gift-notification", function (e) {
        e.preventDefault();

        var transactionId = $(this).attr("data-transaction-id");
        var owner = $(this);

        var jsonObject = {
            "giftTransactionId": transactionId
        };

        json = JSON.stringify(jsonObject);

        postJson('/profile/removeGiftNotification', json,
            function (data) {
                if (data.success) {
                    owner.parent().parent().fadeOut("normal", function () {
                        var notificationsRemaining = $(".popover .remove-gift-notification").length;
                        if (notificationsRemaining > 1) {
                            // remove the element from the popover itself
                            $(this).remove();

                            // remove the element from the popover's source
                            $("#gift-notification-" + transactionId).remove();

                            // reduce the notification count by 1
                            var giftNotificationCount = $("#gift-notification-count").val() - 1;
                            $("#gift-notification-count").val(giftNotificationCount);
                            $("#span-gift-notification-count").text(giftNotificationCount);
                        }
                        else {
                            var html = "<p>Nothing to see here.</p>"

                            // clear out the popover itself
                            $(this).parent().html(html);

                            // clear out the popover's source
                            $("#popover-gifts-container").html(html);
                        }
                    })
                } else {
                    alert(data.error);
                }
            });
    });

    $('#modalReferAFriend').on('show.bs.modal', function (e) {
        get('/account/referralcode', function (data) {
            $('#referral-code').val(data.code);
            $('#referral-link').val(data.link);
        });
    });
});