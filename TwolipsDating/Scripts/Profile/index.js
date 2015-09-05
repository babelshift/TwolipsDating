﻿var defaultSelfSummaryText = "This user hasn't entered a self summary yet.";
var defaultSummaryOfDoingText = "This user hasn't entered what they're doing yet.";
var defaultSummaryOfGoingText = "This user hasn't entered where they're going yet.";

$(document).ready(function () {
    var profileUserId = $('#ProfileUserId').val();

    setupPopoverSelectTitle();
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

    setupProfileDetailsEditControls();

    setupEditBanner();
});

$(window).load(function () {
    repositionBannerCoverToLimits();
});

function setupEditBanner() {
    // when the user uploads a banner image, immediately submit the upload banner form
    $('#upload-header').on('change', function () {
        $('#upload-header-camera').addClass('hidden');
        $('#upload-header-spinner').removeClass('hidden');
        $('#upload-header-form').submit();
    });

    $('#upload-header-mobile').on('change', function () {
        $('#upload-header-camera-mobile').addClass('hidden');
        $('#upload-header-spinner-mobile').removeClass('hidden');
        $('#upload-header-form-mobile').submit();
    })

    var profileBanner = $('#profile-banner-background');
    var saveBannerButton = $('#save-header');

    // when the user click sto submit the "upload banner image" form, submit and then callback to hide the save button and update the banner image
    $('#upload-header-form').ajaxForm({
        success: function (data) {
            if (data.success) {
                $('#upload-header-camera').removeClass('hidden');
                $('#upload-header-spinner').addClass('hidden');
                profileBanner.css('background', 'url(' + data.bannerImagePath + ')');
                profileBanner.css('background-size', 'cover');
                profileBanner.backgroundDraggable();
                profileBanner.backgroundDraggable({
                    done: function () {
                        var backgroundPosition = profileBanner.css('background-position');
                        var split = backgroundPosition.split(" ");
                        var x = split[0].replace("px", "");
                        var y = split[1].replace("px", "");
                        $('#BannerPositionX').val(parseInt(x));
                        $('#BannerPositionY').val(parseInt(y));
                    }
                });
                saveBannerButton.removeClass('hidden');
            }
        }
    });

    $('#upload-header-form-mobile').ajaxForm({
        success: function (data) {
            if (data.success) {
                $('#upload-header-camera-mobile').removeClass('hidden');
                $('#upload-header-spinner-mobile').addClass('hidden');
                profileBanner.css('background', 'url(' + data.bannerImagePath + ')');
                profileBanner.css('background-size', 'cover');
            }
        }
    });

    var beginMoveBanner = function (e) {
        e.preventDefault();

        profileBanner.backgroundDraggable();
        profileBanner.backgroundDraggable({
            done: function () {
                var backgroundPosition = profileBanner.css('background-position');
                var split = backgroundPosition.split(" ");
                var x = parseInt(split[0].replace("px", ""));
                var y = parseInt(split[1].replace("px", ""));

                $('#BannerPositionX').val(x);
                $('#BannerPositionY').val(y);
            }
        });
    };

    // when the user clicks to reposition the banner, enable dragging and show the button to save the position
    $('#reposition-header').on('click', function (e) {
        beginMoveBanner(e);

        saveBannerButton.removeClass('hidden');
        $('#cancel-header').removeClass('hidden');
    });

    $('#cancel-header').on('click', function () {
        saveBannerButton.addClass('hidden');
        $('#cancel-header').addClass('hidden');
        profileBanner.backgroundDraggable('disable');
    });

    // when the user clicks to submit the "save banner position" form, submit and then callback to hide the button and disable draggong
    $('#save-header-form').ajaxForm({
        beforeSubmit: function() {
            $('#save-header-spinner').removeClass('hidden');
            $('#save-header-check').addClass('hidden');
        },
        success: function () {
            saveBannerButton.addClass('hidden');
            $('#cancel-header').addClass('hidden');
            profileBanner.backgroundDraggable('disable');
            $('#save-header-spinner').addClass('hidden');
            $('#save-header-check').removeClass('hidden');
        }
    });
}

function setupProfileDetailsEditControls() {
    $('#modalWhatImLookingFor').on('shown.bs.modal', function (e) {
        $('#LookingForTypeId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
        $('#LookingForLocationId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
    });

    $('#modalMyDetails').on('shown.bs.modal', function (e) {
        $('#RelationshipStatusId').chosen({ allow_single_deselect: true, disable_search_threshold: 10 });
        $('#SelectedLanguages').chosen();
    });

    var limitEnteredAge = function (textBox) {
        var value = parseInt(textBox.val());
        if (isNaN(value)) {
            textBox.val(18);
        } else {
            if (value < 18) {
                textBox.val(18);
            } else if (value > 99) {
                textBox.val(99);
            }
        }
    }

    var minAgeTextBox = $('#looking-for-age-min');
    var maxAgeTextBox = $('#looking-for-age-max');
    minAgeTextBox.on('change', function (e) {
        limitEnteredAge(minAgeTextBox);
    });
    maxAgeTextBox.on('change', function (e) {
        limitEnteredAge(maxAgeTextBox);
    });
}

function setupPopoverSelectTitle() {
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
}

function repositionBannerCoverToLimits() {
    var $el = $('#profile-banner-background');

    var pos = $el.css('background-position').match(/(-?\d+).*?\s(-?\d+)/) || [],
    xPos = parseInt(pos[1]) || 0,
    yPos = parseInt(pos[2]) || 0;

    var imageDimensions = getBackgroundImageDimensions($el);

    if (imageDimensions != null) {
        xPos = limit($el.innerWidth() - imageDimensions.width, 0, xPos, true);
        yPos = limit($el.innerHeight() - imageDimensions.height, 0, yPos, true);

        $el.css('background-position', xPos + 'px ' + yPos + 'px');
    }
}

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
    var buttonToggleFavoriteIcon = $(".button-toggle-favorite-icon");
    var buttonToggleFavorite = $(".button-toggle-favorite");

    if (isFavorite) {
        buttonToggleFavoriteIcon.removeClass("fa-user-plus");
        buttonToggleFavoriteIcon.addClass("fa-check");
        buttonToggleFavorite.removeClass("btn-primary");
        buttonToggleFavorite.addClass("btn-success");
        buttonToggleFavorite.attr("title", "Stop following updates");
        $(".button-toggle-favorite-text").text("Following");
    } else {
        buttonToggleFavoriteIcon.removeClass("fa-check");
        buttonToggleFavoriteIcon.addClass("fa-user-plus");
        buttonToggleFavorite.removeClass("btn-success");
        buttonToggleFavorite.addClass("btn-primary");
        buttonToggleFavorite.attr("title", "Start following updates");
        $(".button-toggle-favorite-text").text("Follow");
    }
}

function toggleIgnoredUserIcon(isIgnored) {
    var buttonToggleIgnoredIcon = $(".button-toggle-ignored-icon");
    var buttonToggleIgnored = $(".button-toggle-ignored");

    if (isIgnored) {
        buttonToggleIgnoredIcon.removeClass("glyphicon-volume-up");
        buttonToggleIgnoredIcon.addClass("glyphicon-volume-off");
        buttonToggleIgnored.removeClass("btn-default");
        buttonToggleIgnored.addClass("btn-danger");
        buttonToggleIgnored.attr("title", "Not receiving updates and messages from this user");
        $(".button-toggle-ignored-text").text("Ignoring");
    } else {
        buttonToggleIgnoredIcon.removeClass("glyphicon-volume-off");
        buttonToggleIgnoredIcon.addClass("glyphicon-volume-up");
        buttonToggleIgnored.removeClass("btn-danger");
        buttonToggleIgnored.addClass("btn-default");
        buttonToggleIgnored.attr("title", "Receiving updates and messages from this user");
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

var bannerImage = new Image();

var getBackgroundImageDimensions = function ($el) {
    var bgSrc = ($el.css('background-image').match(/^url\(['"]?(.*?)['"]?\)$/i) || [])[1];
    if (!bgSrc) return;

    var imageDimensions = { width: 0, height: 0 }

    if (bannerImage.src == '') {
        bannerImage.src = bgSrc;
    }

    if ($el.css('background-size') == "cover") {
        var elementWidth = $el.innerWidth(),
            elementHeight = $el.innerHeight(),
            elementAspectRatio = elementWidth / elementHeight;
        imageAspectRatio = bannerImage.width / bannerImage.height,
        scale = 1;

        if (imageAspectRatio >= elementAspectRatio) {
            scale = elementHeight / bannerImage.height;
        } else {
            scale = elementWidth / bannerImage.width;
        }

        imageDimensions.width = bannerImage.width * scale;
        imageDimensions.height = bannerImage.height * scale;
    } else {
        imageDimensions.width = bannerImage.width;
        imageDimensions.height = bannerImage.height;
    }

    return imageDimensions;
};

// Helper function to guarantee a value between low and hi unless bool is false
var limit = function (low, hi, value, bool) {
    if (arguments.length === 3 || bool) {
        if (value < low) return low;
        if (value > hi) return hi;
    }
    return value;
};