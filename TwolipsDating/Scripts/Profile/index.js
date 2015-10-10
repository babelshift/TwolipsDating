var defaultSelfSummaryText = "This user hasn't entered a self summary yet.";
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

    $('.followify').followify();

    setupNewUserGuide();

    setupProfileSetupPanel();
});

function setupProfileSetupPanel() {
    // check a cookie to see if we need to show the user the profile setup panel
    if (shouldShowProfileSetup()) {
        // show the profile setup panel
        $('#panel-profile-setup').removeClass('hidden');

        // has the user completed the step to change their banner? if so, show it as complete, otherwise, show it as incomplete
        var hasUserCompletedChangeBannerStep = $.cookie('userCompletedChangeBannerStep');
        if (hasUserCompletedChangeBannerStep) {
            completeGuideStep('#link-guide-change-banner', '#icon-guide-change-banner', 'fa-camera', 'fa-check', '#span-guide-change-banner', '#header-guide-change-banner');
        } else {
            setupProfileSetupItemClick('#link-guide-change-banner', '#icon-guide-change-banner', 'fa-camera', 'fa-check', '#span-guide-change-banner', '#header-guide-change-banner',
                function () {
                    $.cookie('userCompletedChangeBannerStep', true);
                    $('#upload-header').click();
                });
        }

        // has the user completed the step to change their profile image? if so, show it as complete, otherwise, show it as incomplete
        var hasUserCompletedChangeImageStep = $.cookie('userCompletedChangeImageStep');
        if (hasUserCompletedChangeImageStep) {
            completeGuideStep('#link-guide-change-image', '#icon-guide-change-image', 'fa-user', 'fa-check', '#span-guide-change-image', '#header-guide-change-image');
        } else {
            setupProfileSetupItemClick('#link-guide-change-image', '#icon-guide-change-image', 'fa-user', 'fa-check', '#span-guide-change-image', '#header-guide-change-image',
                function () {
                    $.cookie('userCompletedChangeImageStep', true);
                    $('#modalProfileImage').modal('show');
                });
        }

        // has the user completed the step to fill in their profile details? if so, show it as complete, otherwise, show it as incomplete
        var hasUserCompletedProfileDetilsStep = $.cookie('userCompletedProfileDetailsStep');
        if (hasUserCompletedProfileDetilsStep) {
            completeGuideStep('#link-guide-profile-details', '#icon-guide-profile-details', 'fa-edit', 'fa-check', '#span-guide-profile-details', '#header-guide-profile-details');
        } else {
            setupProfileSetupItemClick('#link-guide-profile-details', '#icon-guide-profile-details', 'fa-edit', 'fa-check', '#span-guide-profile-details', '#header-guide-profile-details',
                function () {
                    $.cookie('userCompletedProfileDetailsStep', true);
                    $('#modalWhatImLookingFor').modal('show');
                });
        }

        // when the user clicks to go back to the incomplete step of the profile setup panel, go there!
        $('#button-goto-guide-incomplete').on('click', function (e) {
            e.preventDefault();
            $('.guide-complete').addClass('hidden');
            $('.guide-incomplete').removeClass('hidden');
        });

        // when the user clicks to closet he profile setup panel, close it and set a cookie to hide it forever
        $('#button-close-profile-setup').on('click', function (e) {
            e.preventDefault();
            setShowProfile(false);
        });
    }
}

// this function is used to setup a click event and step completion event for an item in the setup profile panel
function setupProfileSetupItemClick(host, icon, iconToRemove, iconToAdd, span, header, callback) {
    $(host).on('click', function (e) {
        e.preventDefault();
        completeGuideStep(this, icon, iconToRemove, iconToAdd, span, header);

        if (callback != null) {
            callback();
        }
    });
}

function shouldShowProfileSetup() {
    // if the panel doesn't exist, this isn't the user's profile, so don't show the panel
    var panel = $('#panel-profile-setup');
    if (panel == null) {
        return false;
    }

    // if the user doesn't have a cookie indicating anything about the panel, show it to them
    var value = getShowProfileValue();
    if (value == null || value == true) {
        return true;
    } else {
        return false;
    }
}

function getShowProfileValue() {
    return $.cookie('showProfileSetup');
}

function setShowProfile(value) {
    $.cookie('showProfileSetup', value);

    if (value == false) {
        $('#panel-profile-setup').remove();
    }
}

function updateGuideProgress() {
    var percentGuideComplete = parseInt($('#header-percent-guide-complete').text());
    percentGuideComplete += 25;
    $('#header-percent-guide-complete').text(percentGuideComplete);
    $('#progress-guide-complete').attr('aria-valuenow', percentGuideComplete);
    $('#progress-guide-complete').attr('style', 'width: ' + percentGuideComplete + '%');
    $('#sr-guide-complete').text(percentGuideComplete);
    if (percentGuideComplete == 100) {
        $('.guide-complete').removeClass('hidden');
        $('.guide-incomplete').addClass('hidden');
    }
}

function completeGuideStep(host, iconControl, iconToRemove, iconToAdd, span, header) {
    $(iconControl).removeClass(iconToRemove);
    $(iconControl).addClass(iconToAdd);
    $(span).addClass('text-success');

    var text = $(host).text();
    $(host).remove();
    $(header).append(text);
    $(header).addClass('text-muted');

    updateGuideProgress();
}

function setupNewUserGuide() {
    if (shouldShowGuide()) {
        $('#alert-start-guide').removeClass('hidden');
    }

    $('#button-end-guide').on('click', function (e) {
        e.preventDefault();

        setGuideToShow(false);
    })

    // if the user has not yet gone through the user guide, set it up
    var setupPopover = function (host, popoverContent, popoverTitle, popoverPlacement, buttonEndGuide, buttonNext, popoverNextClickFunction) {
        // create the popover based on user parameters
        var popover = $(host).popover({
            trigger: "manual",
            title: popoverTitle,
            animation: false,
            placement: popoverPlacement,
            container: 'body',
            html: true,
            content: function () {
                return $(popoverContent).html();
            }
        });

        // when the popover shows, setup the button click events in the popover such as:
        // next: will go to the next guide step
        // end: will end the guide
        popover.on('shown.bs.popover', function () {
            // when the user clicks to end the guide, kill the current popover
            $(buttonEndGuide).on('click', function (e) {
                if (popover != null) { popover.popover('destroy') }
                setGuideToShow(false);
            });

            // when the user clicks the next button, go to the next popover and hide the existing popover
            $(buttonNext).on('click', function () {
                popoverNextClickFunction();
                popover.popover('hide');
            });
        });

        // we need to scroll the window into the view of the popover each time it shows so it isn't off screen
        $('html, body').scrollTop($(host).offset().top - $(window).height() / 2);

        // show the latest popover
        popover.popover('show');

        return popover;
    };

    $('#button-start-guide').on('click', function (e) {
        // get the cookie which determines if the user has already gone through the user guide
        if (shouldShowGuide()) {
            setupPopover('#button-change-profile-image', '#guide-part-1', 'This is your profile image', 'right', '.button-end-guide-1', '.button-goto-guide-part-2',
                function (previousPopover) {
                    setupPopover('#button-upload-header', '#guide-part-2', 'Change your banner background', 'right', '.button-end-guide-2', '.button-goto-guide-part-3',
                        function () {
                            setupPopover('#button-show-followers', '#guide-part-3', 'See your followers', 'left', '.button-end-guide-3', '.button-goto-guide-part-4',
                                function () {
                                    setupPopover('#button-show-following', '#guide-part-4', 'See who you are following', 'right', '.button-end-guide-4', '.button-goto-guide-part-5',
                                        function () {
                                            setupPopover('#selected-title-image', '#guide-part-5', 'Customize your displayed title', 'right', '.button-end-guide-5', '.button-goto-guide-part-6',
                                                function () {
                                                    setupPopover('#button-about', '#guide-part-6', 'Navigate your profile', 'right', '.button-end-guide-6', '.button-goto-guide-part-7',
                                                        function () {
                                                            setupPopover('#div-community-info', '#guide-part-7', 'Setup your profile details', 'left', '.button-end-guide-7', '.button-goto-guide-part-8',
                                                                function () {
                                                                    setGuideToShow(false);
                                                                });
                                                        });
                                                });
                                        });
                                });
                        });
                });
        }
    });
}

function shouldShowGuide() {
    // if the alert doesn't exist, this isn't the user's profile, so don't show the guide
    var alert = $('#alert-start-guide');
    if (alert == null) {
        return false;
    }

    // if the user doesn't have a cookie indicating anything about the guide, show it to them
    var value = getGuideToShowValue();
    if (value == null || value == true) {
        return true;
    } else {
        return false;
    }
}

function getGuideToShowValue() {
    return $.cookie('showNewUserGuide');
}

function setGuideToShow(value) {
    $.cookie('showNewUserGuide', value);

    if (value == false) {
        $('#alert-start-guide').addClass('hidden');
    }
}

// when the window has completed loading (including all images, reposition the user's banner to fix a bug where banner repositioning isn't correct with responsive windows
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
        beforeSubmit: function () {
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
    $('#form-upload-images-modal').on('change', function () {
        $('#form-upload-images-modal').submit();
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