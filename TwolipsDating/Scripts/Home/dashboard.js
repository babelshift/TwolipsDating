function onNextQuestion(e) {

    // get a random question
    get('/trivia/randomJson', function (data) {
        if (data.Success) {
            // show the new question
            var content = data.Content;
            $('#random-question-content').html(content);
            $('#RandomQuestion_QuestionId').val(data.QuestionId);

            // show the new difficulty rating
            $('#marker-very-easy').addClass('hidden');
            $('#marker-easy').addClass('hidden');
            $('#marker-average').addClass('hidden');
            $('#marker-hard').addClass('hidden');
            $('#marker-very-hard').addClass('hidden');
            $('#point-very-easy').addClass('hidden');
            $('#point-easy').addClass('hidden');
            $('#point-average').addClass('hidden');
            $('#point-hard').addClass('hidden');
            $('#point-very-hard').addClass('hidden');

            if (data.Points == 1) {
                $('#marker-very-easy').removeClass('hidden');
                $('#point-very-easy').removeClass('hidden');
            } else if (data.Points == 2) {
                $('#marker-easy').removeClass('hidden');
                $('#point-easy').removeClass('hidden');
            } else if (data.Points == 3) {
                $('#marker-average').removeClass('hidden');
                $('#point-average').removeClass('hidden');
            } else if (data.Points == 4) {
                $('#marker-hard').removeClass('hidden');
                $('#point-hard').removeClass('hidden');
            } else if (data.Points == 5) {
                $('#marker-very-hard').removeClass('hidden');
                $('#point-very-hard').removeClass('hidden');
            }

            // show the new answers
            var answersHtml = '';
            data.Answers.forEach(function (item) {
                answersHtml += '<a href="#" id="answer-' + item.AnswerId + '" class="answer-link list-group-item" data-answer-id="' + item.AnswerId + '">'
                + item.Content
                + '<span id="icon-correct-' + item.AnswerId + '" class="icon-correct pull-right hidden"><i class="fa fa-check"></i></span>'
                + '<span id="icon-incorrect-' + item.AnswerId + '" class="icon-incorrect pull-right hidden"><i class="fa fa-remove"></i></span>'
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

    var questionId = $("#RandomQuestion_QuestionId").val();
    var selectedAnswerId = $(obj).attr("data-answer-id");

    var jsonObject = {
        "questionId": questionId,
        "answerId": selectedAnswerId
    };

    var json = JSON.stringify(jsonObject);

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

    var jsonObject = {
        "reviewId": reviewId,
        "violationTypeId": violationTypeId,
        "content": content
    };

    json = JSON.stringify(jsonObject);

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

    $('.followify').followify({ fadeOut: true });

    $('#button-skip').on('click', function (e) {
        onNextQuestion(e);
    });
});