var defaultSelfSummaryText = "This user hasn't entered a self summary yet.";
var defaultSummaryOfDoingText = "This user hasn't entered what they're doing yet.";
var defaultSummaryOfGoingText = "This user hasn't entered where they're going yet.";

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

            var jsonObject = {
                "titleId": titleId
            };

            var json = JSON.stringify(jsonObject);

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

    setupTextAreaForEdit('#edit-self-summary', '#self-summary', '#text-edit-self-summary', '#button-edit-self-summary', '#button-save-self-summary', '#button-cancel-self-summary', defaultSelfSummaryText);
    setupTextAreaForEdit('#edit-summary-of-doing', '#summary-of-doing', '#text-edit-summary-of-doing', '#button-edit-summary-of-doing', '#button-save-summary-of-doing', '#button-cancel-summary-of-doing', defaultSummaryOfDoingText);
    setupTextAreaForEdit('#edit-summary-of-going', '#summary-of-going', '#text-edit-summary-of-going', '#button-edit-summary-of-going', '#button-save-summary-of-going', '#button-cancel-summary-of-going', defaultSummaryOfGoingText);

    setupTextAreaForPost('#button-save-self-summary', '#text-edit-self-summary', 'selfSummary', '/profile/saveSelfSummary', '#button-edit-self-summary', '#edit-self-summary', '#self-summary', defaultSelfSummaryText);
    setupTextAreaForPost('#button-save-summary-of-doing', '#text-edit-summary-of-doing', 'summaryOfDoing', '/profile/saveSummaryOfDoing', '#button-edit-summary-of-doing', '#edit-summary-of-doing', '#summary-of-doing', defaultSummaryOfDoingText);
    setupTextAreaForPost('#button-save-summary-of-going', '#text-edit-summary-of-going', 'summaryOfGoing', '/profile/saveSummaryOfGoing', '#button-edit-summary-of-going', '#edit-summary-of-going', '#summary-of-going', defaultSummaryOfGoingText);

    $('#modalWhatImLookingFor').on('shown.bs.modal', function (e) {
        $('#LookingForTypeId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
        $('#LookingForLocationId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
    });

    $('#modalMyDetails').on('shown.bs.modal', function (e) {
        $('#RelationshipStatusId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
        $('#SelectedLanguages').chosen();
    });

    var minAgeTextBox = $('#looking-for-age-min');
    var maxAgeTextBox = $('#looking-for-age-max');
    minAgeTextBox.on('change', function (e) {
        var value = parseInt(minAgeTextBox.val());
        if (isNaN(value)) {
            minAgeTextBox.val(18);
        } else {
            if (value < 18) {
                minAgeTextBox.val(18);
            } else if (value > 99) {
                minAgeTextBox.val(99);
            }
        }
    });
    maxAgeTextBox.on('change', function (e) {
        var value = parseInt(maxAgeTextBox.val());
        if (isNaN(value)) {
            maxAgeTextBox.val(18);
        } else {
            if (value < 18) {
                maxAgeTextBox.val(18);
            } else if (value > 99) {
                maxAgeTextBox.val(99);
            }
        }
    });

    // when the user uploads a banner image, immediately submit the upload banner form
    $('#upload-header').on('change', function () {
        $('#upload-header-form').submit();
    });

    // when the user click sto submit the "upload banner image" form, submit and then callback to hide the save button and update the banner image
    $('#upload-header-form').ajaxForm({
        success: function (data) {
            if (data.success) {
                $('#profile-banner-background').css('background', 'url(' + data.bannerImagePath + ')');
                $('#profile-banner-background').css('background-size', 'cover');
                $('#profile-banner-background').backgroundDraggable();
                $('#profile-banner-background').backgroundDraggable({
                    done: function () {
                        var backgroundPosition = $('#profile-banner-background').css('background-position');
                        var split = backgroundPosition.split(" ");
                        var x = split[0].replace("px", "");
                        var y = split[1].replace("px", "");
                        $('#BannerPositionX').val(parseInt(x));
                        $('#BannerPositionY').val(parseInt(y));
                    }
                });
                $('#save-header').removeClass('hidden');
            } else {
            }
        }
    });

    // when the user clicks to reposition the banner, enable dragging and show the button to save the position
    $('#reposition-header').on('click', function (e) {
        e.preventDefault();

        $('#profile-banner-background').backgroundDraggable();
        $('#profile-banner-background').backgroundDraggable({
            done: function () {
                var backgroundPosition = $('#profile-banner-background').css('background-position');
                var split = backgroundPosition.split(" ");
                var x = parseInt(split[0].replace("px", ""));
                var y = parseInt(split[1].replace("px", ""));

                var containerHeight = $('#profile-banner-background').height();

                var percentY = parseInt((Math.abs(y) / containerHeight) * 100);
                if (percentY > 100) {
                    percentY = 100;
                }

                $('#BannerPositionX').val(x);
                $('#BannerPositionY').val(percentY);
            }
        });
        $('#save-header').removeClass('hidden');
        $('#cancel-header').removeClass('hidden');
    });

    $('#cancel-header').on('click', function () {
        $('#save-header').addClass('hidden');
        $('#cancel-header').addClass('hidden');
        $('#profile-banner-background').backgroundDraggable('disable');
    });

    // when the user clicks to submit the "save banner position" form, submit and then callback to hide the button and disable draggong
    $('#save-header-form').ajaxForm(function () {
        $('#save-header').addClass('hidden');
        $('#cancel-header').addClass('hidden');
        $('#profile-banner-background').backgroundDraggable('disable');
    });
});

function setupTextAreaForPost(buttonSave, textArea, paramName, postPath, buttonEdit, container, textDisplay, defaultText) {
    $(buttonSave).on('click', function (e) {
        var text = $(textArea).val();

        var jsonObject = {
        };

        jsonObject[paramName] = text;

        var json = JSON.stringify(jsonObject);

        postJson(postPath, json, function (data) {
            if (data.success) {

                text = htmlEscape(text);

                $(buttonEdit).removeClass('hidden');
                $(container).addClass('hidden');
                $(textDisplay).removeClass('hidden');

                if (text == null || text.length == 0) {
                    $(textDisplay).text(defaultText);
                } else {
                    $(textDisplay).html(text);
                }
                $(textArea).val(text);
            }
        });
    });
}

function setupTextAreaForEdit(editContainer, textDisplay, textArea, buttonEdit, buttonSave, buttonCancel, defaultText) {
    $(buttonEdit).on('click', function (e) {
        e.preventDefault();

        $(buttonEdit).addClass('hidden');
        $(editContainer).removeClass('hidden');
        $(textDisplay).addClass('hidden');
    });

    $(buttonCancel).on('click', function (e) {
        e.preventDefault();

        $(buttonEdit).removeClass('hidden');
        $(editContainer).addClass('hidden');
        $(textDisplay).removeClass('hidden');

        var currentSelfSummary = $(textDisplay).text();

        if (currentSelfSummary == defaultText) {
            $(textArea).val('');
        } else {
            $(textArea).val(currentSelfSummary);
        }
    });
}

function onWriteReview(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var rating = $("#WriteReview_RatingValue").val();
    var reviewContent = $("#WriteReview_ReviewContent").val();

    if (reviewContent != null && reviewContent.length > 0) {

        var jsonObject = {
            "profileUserId": profileUserId,
            "rating": rating,
            "reviewContent": reviewContent
        };

        var json = JSON.stringify(jsonObject);

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

    var jsonObject = {
        "profileUserId": profileUserId,
        "giftId": giftId,
        "inventoryItemId": inventoryItemId
    };

    var json = JSON.stringify(jsonObject);

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

        var jsonObject = {
            "profileUserId": profileUserId,
            "messageBody": messageBody
        };

        var json = JSON.stringify(jsonObject);

        postJson('/profile/sendMessage', json, function (data) {
            if (data.success) {
                $('#message-send-log').show();
                $('#message-success').show();
                $('#SendMessage_MessageBody').val('');
                $("#message-send-log tr:last").after("<tr><td>" + htmlEscape(messageBody) + "</td></tr>");
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

    var jsonObject = {
        "reviewId": reviewId,
        "violationTypeId": violationTypeId,
        "content": content
    }

    var json = JSON.stringify(jsonObject);

    postJson('/violation/addReviewViolation', json, function (data) {
        if (data.success) {
            $('#violation-success').show();
            $('#button-violation-submit').hide();
            $('#violation-review-body').hide();
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
    $('#form-upload-images').on('change', function () {
        $('#form-upload-images').submit();
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

    var jsonObject = {
        "id": userImageId,
        "fileName": fileName,
        "profileUserId": profileUserId
    };

    json = JSON.stringify(jsonObject);

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

    var jsonObject = {
        "profileUserId": profileUserId,
        "profileId": profileId
    };

    var json = JSON.stringify(jsonObject);

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
        $(".button-toggle-favorite-icon").removeClass("fa-user-plus");
        $(".button-toggle-favorite-icon").addClass("fa-check");
        $(".button-toggle-favorite").removeClass("btn-primary");
        $(".button-toggle-favorite").addClass("btn-success");
        $(".button-toggle-favorite").attr("title", "Stop following updates");
        $(".button-toggle-favorite-text").text("Following");
    } else {
        $(".button-toggle-favorite-icon").removeClass("fa-check");
        $(".button-toggle-favorite-icon").addClass("fa-user-plus");
        $(".button-toggle-favorite").removeClass("btn-success");
        $(".button-toggle-favorite").addClass("btn-primary");
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

    var jsonObject = {
        "profileUserId": profileUserId
    };

    var json = JSON.stringify(jsonObject);

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

    var jsonObject = {
        "id": tagId,
        "profileId": profileId,
        "suggestAction": suggestAction
    };

    var json = JSON.stringify(jsonObject);

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

function htmlEscape(str) {
    return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
}