$(document).ready(function () {

    

    var profileUserId = $('#ProfileUserId').val();

    // only bother setting up the select title popover if the content is visible
    // the content will only be visible if the user is viewing their own profile
    if ($('#popover-titles-content').length) {
        // setup the popover for the selected title
        setupHtmlPopover('#selected-title', '#popover-titles-content');

        // when the title to select a popover is hidden, recreate it based on any changes that were made from the user's interaction
        $("#selected-title").on("hidden.bs.popover", function () {
            setupHtmlPopover('#selected-title', '#popover-titles-content');
        });

        // whenever a link is clicked to select a title, perform an AJAX call to change to the user's selection
        $(document).on("click", "[class^=select-title-link]", function (e) {
            e.preventDefault();

            // get the values from the element's attributes
            var titleId = $(this).attr("data-title-id");
            var titleName = $(this).attr("data-title-name");

            var json = "{\"titleId\":" + titleId + "}";

            postJson('/profile/setSelectedTitle', json,
                function (data) {
                    if (data.success) {
                        // change profile's displayed selected title
                        $("#selected-title").text(titleName);

                        // remove all existing checkmarks which indicated the selected title
                        $('[class^="selected-title-item-check"]').remove();

                        // prepend the checkmark before the now selected link
                        var html = '<i class="selected-title-item-check-' + titleId + ' glyphicon glyphicon-ok"></i>';
                        $(".select-title-link-" + titleId).before(html);
                    } else {
                        alert(data.error);
                    }
                });
        });
    }

    toggleFavoriteIcon();
    toggleIgnoredIcon();
    selectUnitedStates();
    initializeStarRating();
    setupFileUploadText();
    setupReportViolation();
    setupMessageSend();
    setupGiftSend();
    setupReviewWrite();

    // loop through all share review links and turn them into valid popovers with share buttons
    setupPopoverWithContent("a[id^='share-profile-link']", function () {
        var reviewId = $(this).attr('data-review-id'); // extract the review ID
        var shareButtonsDiv = "#share-profile-buttons-popover-" + reviewId;
        var clone = $(shareButtonsDiv).clone(true);
        var cloneUnhide = clone.removeClass('hide');
        return cloneUnhide.html();
    });
});

function onWriteReview(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var rating = $("#WriteReview_RatingValue").val();
    var reviewContent = $("#WriteReview_ReviewContent").val();

    if (reviewContent != null && reviewContent.length > 0) {

        var json = '{"profileUserId":"' + profileUserId + '", "rating":' + rating + ', "reviewContent":"' + reviewContent + '"}';

        postJson('/profile/writeReview', json, function (data) {
            if (data.success) {
                $('#review-success').show();
                $('#WriteReview_ReviewContent').val('');
                $('#button-review-write').hide();
            } else {
                $('#review-error').show();
                $('#review-error-text').text(data.error);
            }
        });
    }
}

function onSendGift(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var giftId = $('#SendGift_GiftId').val();
    var inventoryItemId = $('#SendGift_InventoryItemId').val();
    var giftName = $("#selected-gift-name").val();

    var json = '{"profileUserId":"' + profileUserId + '", "giftId":' + giftId + ', "inventoryItemId":' + inventoryItemId + '}';

    postJson('/profile/sendGift', json, function (data) {
        if (data.success) {
            $('#gift-send-log').show();
            $('#gift-success').show();
            var messageBody = $('#SendMessage_MessageBody').val('');
            $("#gift-count-" + inventoryItemId).text(data.giftCount);
            $("#gift-send-log tr:last").after("<tr><td>" + giftName + " sent successfully.</td></tr>");

            if (data.giftCount == 0) {
                $("#gift-image-wrapper-" + giftId).remove();
                clearSelectedGift();
            }
        } else {
            $('#gift-error').show();
            $('#gift-error-text').text(data.error);
        }
    });
}

function onSendMessage(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var messageBody = $('#SendMessage_MessageBody').val();

    if (messageBody != null && messageBody.length > 0) {
        var json = '{"profileUserId":"' + profileUserId + '", "messageBody":"' + messageBody + '"}';

        postJson('/profile/sendMessage', json, function (data) {
            if (data.success) {
                $('#message-send-log').show();
                $('#message-success').show();
                $('#SendMessage_MessageBody').val('');
                $("#message-send-log tr:last").after("<tr><td>" + messageBody + "</td></tr>");
            } else {
                $('#message-error').show();
                $('#message-error-text').text(data.error);
            }
        });
    }
}

function onAddReviewViolation(e, obj) {
    var reviewId = $('#review-id').val();
    var violationTypeId = $('#WriteReviewViolation_ViolationTypeId').val();
    var content = $('#WriteReviewViolation_ViolationContent').val();

    var json = '{"reviewId":' + reviewId + ', "violationTypeId":' + violationTypeId + ', "content":"' + content + '"}';

    postJson('/violation/addReviewViolation', json, function (data) {
        if (data.success) {
            $('#violation-success').show();
            $('#button-violation-submit').hide();
        } else {
            $('#violation-error').show();
            $('#violation-error-text').text(data.error);
        }
    });
}

function toggleFavoriteIcon() {
    var isFavoritedByCurrentUser = $('#IsFavoritedByCurrentUser').val();

    if (isFavoritedByCurrentUser == 'True') {
        toggleFavoriteProfileIcon(true);
    } else {
        toggleFavoriteProfileIcon(false);
    }
}

function toggleIgnoredIcon() {
    var isIgnoredByCurrentUser = $('#IsIgnoredByCurrentUser').val();
    if (isIgnoredByCurrentUser == 'True') {
        toggleIgnoredUserIcon(true);
    } else {
        toggleIgnoredUserIcon(false);
    }
}

function initializeStarRating() {
    // initialize the star rating plugin
    $("#star").raty({
        score: 3,
        hints: ['awful', 'bad', 'average', 'good', 'great'],
        click: function (score, evt) {
            $("#WriteReview_RatingValue").val(score);
        }
    });
}

function setupFileUploadText() {
    // handle file selection text updating
    $('.btn-file :file').on('fileselect', function (event, numFiles, label) {
        $('#form-upload-images').submit();

        /*
        var input = $(this).parents('.input-group').find(':text'),
            log = numFiles > 1 ? numFiles + ' files selected' : label;

        if (input.length) {
            input.val(log);
        } else {
            if (log) alert(log);
        }
        */
    });
}

function setupReportViolation() {
    $('#violation-error').hide();
    $('#violation-success').hide();

    $('#modalReviewViolation').on('hide.bs.modal', function (event) {
        $('#violation-error').hide();
        $('#violation-success').hide();
        $('#button-violation-submit').show();
        $('#WriteReviewViolation_ViolationContent').val('');
    });

    $('#modalReviewViolation').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var recipient = button.data('review-author'); // Extract info from data-* attributes
        var reviewId = button.data('review-id');
        var modal = $(this);
        modal.find('#review-author-name').text(recipient)
        modal.find('#review-id').val(reviewId)
    });
}

function setupMessageSend() {
    $('#message-send-log').hide();
    $('#message-error').hide();
    $('#message-success').hide();

    $('#modalMessage').on('hide.bs.modal', function (event) {
        $('#message-error').hide();
        $('#message-success').hide();
        $('#button-message-send').show();
        $('#SendMessage_MessageBody').val('');
    });
}

function setupGiftSend() {
    $('#gift-send-log').hide();
    $('#gift-error').hide();
    $('#gift-success').hide();

    $('#modalGift').on('hide.bs.modal', function (event) {
        $('#gift-error').hide();
        $('#gift-success').hide();
        $('#button-gift-send').show();
    });
}

function setupReviewWrite() {
    $('#review-error').hide();
    $('#review-success').hide();

    $('#modalReview').on('hide.bs.modal', function (event) {
        $('#review-error').hide();
        $('#review-success').hide();
        $('#button-review-write').show();
        $('#WriteReview_ReviewContent').val('');
    });
}

// handle file selection text updating
$(document).on('change', '.btn-file :file', function () {
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
    input.trigger('fileselect', [numFiles, label]);
});

function clearSelectedGift() {
    var hiddenSelectedInventoryItemId = $("#SendGift_InventoryItemId");
    hiddenSelectedInventoryItemId.val('');

    var hiddenSelectedGiftId = $("#SendGift_GiftId");
    hiddenSelectedGiftId.val('');

    var hiddenSelectedGiftName = $("#selected-gift-name");
    hiddenSelectedGiftName.val('');
}

function onSelectGiftClick(e, obj) {
    e.preventDefault();

    var inventoryItemId = $(obj).attr("id");
    var hiddenSelectedInventoryItemId = $("#SendGift_InventoryItemId");
    hiddenSelectedInventoryItemId.val(inventoryItemId);

    var giftId = $(obj).attr("data-gift-id");
    var hiddenSelectedGiftId = $("#SendGift_GiftId");
    hiddenSelectedGiftId.val(giftId);

    var giftName = $(obj).attr("data-gift-name");
    var hiddenSelectedGiftName = $("#selected-gift-name");
    hiddenSelectedGiftName.val(giftName);

    $("div[id^='gift-image-wrapper']").css("background-color", "transparent");
    $(obj).parent().css("background-color", "#eeeeee");
}

function onSelectProfileImageClick(e, obj) {
    e.preventDefault();
    var imageId = $(obj).attr("id");
    var hiddenProfileImage = $("#ChangeImage_UserImageId");
    hiddenProfileImage.val(imageId);
}

function onDeleteImage(e, obj, userImageId, fileName, profileUserId) {
    json = '{"id":' + userImageId + ', "fileName":"' + fileName + '", "profileUserId":"' + profileUserId + '"}';

    postJson('/profile/deleteImage', json, function (data) {
        if (data.success) {
            var div = $("#" + userImageId + "-image-div");
            div.fadeOut(300, function () {
                div.remove();
            });
        } else {
            alert(data.error);
        }
    });
}

function onToggleFavoriteProfile(e, obj, profileUserId, profileId) {
    e.preventDefault();

    var json = '{"profileUserId":"' + profileUserId + '", "profileId":' + profileId + '}';

    postJson('/profile/toggleFavoriteProfile', json, function (data) {
        if (data.success) {
            toggleFavoriteProfileIcon(data.isFavorite);
        } else {
            alert(data.error);
        }
    });
}

function toggleFavoriteProfileIcon(isFavorite) {
    if (isFavorite) {
        $(".button-toggle-favorite-icon").removeClass("glyphicon-heart");
        $(".button-toggle-favorite-icon").addClass("glyphicon-ok");
        $(".button-toggle-favorite").removeClass("btn-default");
        $(".button-toggle-favorite").addClass("btn-success");
        $(".button-toggle-favorite").attr("title", "Stop following updates");
        $(".button-toggle-favorite-text").text("Following");
    } else {
        $(".button-toggle-favorite-icon").removeClass("glyphicon-ok");
        $(".button-toggle-favorite-icon").addClass("glyphicon-heart");
        $(".button-toggle-favorite").removeClass("btn-success");
        $(".button-toggle-favorite").addClass("btn-default");
        $(".button-toggle-favorite").attr("title", "Start following updates");
        $(".button-toggle-favorite-text").text("Follow");
    }
}

function toggleIgnoredUserIcon(isIgnored) {
    if (isIgnored) {
        $(".button-toggle-ignored-icon").removeClass("glyphicon-volume-up");
        $(".button-toggle-ignored-icon").addClass("glyphicon-volume-off");
        $(".button-toggle-ignored").removeClass("btn-default");
        $(".button-toggle-ignored").addClass("btn-danger");
        $(".button-toggle-ignored").attr("title", "Not receiving updates and messages from this user");
        $(".button-toggle-ignored-text").text("Ignoring");
    } else {
        $(".button-toggle-ignored-icon").removeClass("glyphicon-volume-off");
        $(".button-toggle-ignored-icon").addClass("glyphicon-volume-up");
        $(".button-toggle-ignored").removeClass("btn-danger");
        $(".button-toggle-ignored").addClass("btn-default");
        $(".button-toggle-ignored").attr("title", "Receiving updates and messages from this user");
        $(".button-toggle-ignored-text").text("Ignore");
    }
}

function onToggleIgnoredUser(e, obj, profileUserId) {
    e.preventDefault();

    var json = '{"profileUserId":"' + profileUserId + '"}';

    postJson('/profile/toggleIgnoredUser', json, function (data) {
        if (data.success) {
            toggleIgnoredUserIcon(data.isIgnored);
        } else {
            alert(data.error);
        }
    });
}

function onSuggestTag(e, obj, tagId) {
    e.preventDefault();
    var profileId = $("#ProfileId").val();

    var didUserSuggest = $("#" + tagId + "-tag-hidden").val();

    var suggestAction = '';

    if (didUserSuggest === "true") {
        suggestAction = 'remove';
    } else {
        suggestAction = 'add';
    }

    var json = '{"id":' + tagId + ', "profileId":' + profileId + ', "suggestAction":"' + suggestAction + '"}';

    postJson('/profile/suggestTag', json, function (data) {
        if (data.success) {
            var tagGlyphClassRemove = "";
            var tagButtonClassRemove = "";
            var tagGlyphClassAdd = "";
            var tagButtonClassAdd = "";

            if (data.suggestAction == "remove") {
                tagGlyphClassRemove = "glyphicon-minus";
                tagGlyphClassAdd = "glyphicon-plus";
                tagButtonClassRemove = "btn-primary";
                tagButtonClassAdd = "btn-default";
                $("#" + data.tagId + "-tag-hidden").val(false);

            } else if (data.suggestAction == "add") {
                tagGlyphClassRemove = "glyphicon-plus";
                tagGlyphClassAdd = "glyphicon-minus";
                tagButtonClassRemove = "btn-default";
                tagButtonClassAdd = "btn-primary";
                $("#" + data.tagId + "-tag-hidden").val(true);
            }

            var tagGlyph = $("#" + data.tagId + "-tag-glyph");
            tagGlyph.removeClass(tagGlyphClassRemove);
            tagGlyph.addClass(tagGlyphClassAdd);

            var tagButton = $("#" + data.tagId + "-tag-button");
            tagButton.removeClass(tagButtonClassRemove);
            tagButton.addClass(tagButtonClassAdd);

            var tagCount = $("#" + data.tagId + "-tag-count");
            tagCount.text(data.tagCount);
        }
        else {
            alert(data.error);
        }
    });
}