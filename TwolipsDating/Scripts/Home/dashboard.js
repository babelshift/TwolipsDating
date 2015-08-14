function onNextQuestion(e, obj) {

    // get a random question
    get('/trivia/randomJson', function (data) {
        if (data.Success) {
            // show the new question
            var content = data.Content;
            $('#random-question-content').html(content);
            $('#RandomQuestion_QuestionId').val(data.QuestionId);

            // show the new difficulty rating
            $('.marker-1').removeClass('current');
            $('.marker-2').removeClass('current');
            $('.marker-3').removeClass('current');
            $('.marker-4').removeClass('current');
            $('.marker-5').removeClass('current');
            if (data.Points == 1) {
                $('.marker-1').addClass('current');
            } else if (data.Points == 2) {
                $('.marker-2').addClass('current');
            } else if (data.Points == 3) {
                $('.marker-3').addClass('current');
            } else if (data.Points == 4) {
                $('.marker-4').addClass('current');
            } else if (data.Points == 5) {
                $('.marker-5').addClass('current');
            }

            // show the new answers
            var answersHtml = '';
            data.Answers.forEach(function (item) {
                answersHtml += '<a href="#" id="answer-' + item.AnswerId + '" class="answer-link list-group-item" data-answer-id="' + item.AnswerId + '">'
                + item.Content
                + '<span id="icon-correct-' + item.AnswerId + '" class="icon-correct pull-right hidden"><i class="glyphicon glyphicon-ok"></i></span>'
                + '<span id="icon-incorrect-' + item.AnswerId + '" class="icon-incorrect pull-right hidden"><i class="glyphicon glyphicon-remove"></i></span>'
                + '</a>';
            });
            $('#answers').html(answersHtml);

            // reset all click handlers on the answer links
            $(".answer-link").off('click');

            // subscribe new click handlers on new answers
            $(".answer-link").on("click", function (e) {
                onSubmitAnswer(e, this);
            });

            // reset the OK button
            $('#result-alert').addClass("hidden");
            $('#button-next').addClass("hidden");
            $('#button-skip').removeClass("hidden");
        } else {
            $('#trivia-panel').fadeOut('normal', function () {
                $(this).remove();
                $('#trivia-panel-no-questions').removeClass("hidden");
            });
        }
    });
}

function onSubmitAnswer(e, obj) {
    e.preventDefault();

    //$("#button-next").addClass("hidden");
    //$("#button-ok").removeClass("hidden");

    var questionId = $("#RandomQuestion_QuestionId").val();
    var selectedAnswerId = $(obj).attr("data-answer-id");  //var selected = $("input[type='radio'][name='RandomQuestion.SelectedAnswerId']:checked");
    //var answerId = "";
    //if (selected.length > 0) {
    //    answerId = selected.val();
    //}

    var json = '{"questionId":' + questionId + ', "answerId":' + selectedAnswerId + '}';

    postJson('/trivia/submitAnswer', json, function (data) {
        if (data.success) {
            if (data.correctAnswerId == selectedAnswerId) {
                $("#button-next").removeClass("hidden");
                $("#button-next").removeClass("btn-danger");
                $("#button-next").removeClass("btn-success");
                $("#button-next").addClass("btn-success");
                $("#button-skip").addClass("hidden");
                $("#result-alert").removeClass("alert-success");
                $("#result-alert").removeClass("alert-danger");
                $("#result-alert").removeClass("hidden");
                $("#result-alert").addClass("alert-success");
                $("#result-alert").text('Correct');

                // increase the users points by the amount the question was worth
                var questionPointsValue = parseInt($('#question-points-value').val());
                var userCurrentPointsAmount = parseInt($('#span-points-count').text());
                var userNewPointsAmount = userCurrentPointsAmount + questionPointsValue;
                $('#span-points-count').text(userNewPointsAmount);
            } else {
                $("#button-next").removeClass("hidden");
                $("#button-next").removeClass("btn-danger");
                $("#button-next").removeClass("btn-success");
                $("#button-next").addClass("btn-danger");
                $("#button-skip").addClass("hidden");
                $("#result-alert").removeClass("alert-success");
                $("#result-alert").removeClass("alert-danger");
                $("#result-alert").removeClass("hidden");
                $("#result-alert").addClass("alert-danger");
                $("#result-alert").text('Incorrect');
            }

            $(".answer-link").addClass("list-group-item-danger");
            $(".answer-link").off();
            $("#answer-" + data.correctAnswerId).removeClass("list-group-item-danger");
            $("#answer-" + data.correctAnswerId).addClass("list-group-item-success");

            $(".icon-incorrect").removeClass("hidden");
            $("#icon-incorrect-" + data.correctAnswerId).addClass("hidden");
            $("#icon-correct-" + data.correctAnswerId).removeClass("hidden");

            $("#button-skip").addClass('hidden');
        } else {
            alert(data.error);
        }
    });
}

function onAddReviewViolation(e, obj) {
    var reviewId = $('#review-id').val();
    var violationTypeId = $('#WriteReviewViolation_ViolationTypeId').val();
    var authorUserId = $('#CurrentUserId').val();
    var content = $('#WriteReviewViolation_ViolationContent').val();

    json = '{"reviewId":' + reviewId + ', "violationTypeId":' + violationTypeId + ', "content":"' + content + '"}';

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

$(document).ready(function () {
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

    setupPopoverWithContent("a[id^='share-profile-link']", function () {
        var reviewId = $(this).attr('data-review-id'); // extract the review ID
        var shareButtonsDiv = "#share-profile-buttons-popover-" + reviewId;
        var clone = $(shareButtonsDiv).clone(true);
        var cloneUnhide = clone.removeClass('hide');
        return cloneUnhide.html();
    });

    $(".answer-link").on("click", function (e) {
        onSubmitAnswer(e, this);
    });

    $('.follow-user').on('click', function (e) {
        e.preventDefault();

        var profileUserId = $(this).attr('data-user-id');
        var profileId = $(this).attr('data-profile-id');

        var json = '{"profileUserId":"' + profileUserId + '", "profileId":' + profileId + '}';

        postJson('/profile/toggleFavoriteProfile', json, function (data) {
            if (data.success) {
                toggleFavoriteProfileIcon(profileId, data.isFavorite);

                $('#user-to-follow-' + profileId).fadeOut('normal', function () {
                    $(this).remove();
                });
            } else {
                alert(data.error);
            }
        });
    });
});

function toggleFavoriteProfileIcon(profileId, isFavorite) {
    if (isFavorite) {
        $('#icon-toggle-favorite-' + profileId).removeClass("glyphicon-heart-empty");
        $('#icon-toggle-favorite-' + profileId).addClass("glyphicon-heart");
        $('#button-toggle-favorite-' + profileId).removeClass("btn-default");
        $('#button-toggle-favorite-' + profileId).addClass("btn-success");
    } else {
        $('#icon-toggle-favorite-' + profileId).removeClass("glyphicon-heart");
        $('#icon-toggle-favorite-' + profileId).addClass("glyphicon-heart-empty");
        $('#button-toggle-favorite-' + profileId).removeClass("btn-success");
        $('#button-toggle-favorite-' + profileId).addClass("btn-default");
    }
}