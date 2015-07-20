function onNextQuestion(e, obj) {
    // get a random question
    get('/trivia/randomJson', function (data) {
        // show the new question
        var content = data.Content + ' <small><span class="badge">' + data.Points + ' points</span></small>';
        $('#random-question-content').html(content);
        $('#RandomQuestion_QuestionId').val(data.QuestionId);

        // show the new answers
        var answersHtml = '';
        data.Answers.forEach(function (item) {
            answersHtml += '<div class="radio"><label><input id="RandomQuestion_SelectedAnswerId" type="radio" value="' + item.AnswerId + '"'
            + 'name="RandomQuestion.SelectedAnswerId" data-val-required="The SelectedAnswerId field is required."'
            + 'data-val-number="The field SelectedAnswerId must be a number." data-val="true"></input>'
            + item.Content + '</label></div>';
        });
        $('#answers').html(answersHtml);

        // reset the OK button
        $('#result-alert').addClass("hidden");
        $('#button-next').addClass("hidden");
        $('#button-ok').removeClass("hidden");
    });
}

function onSubmitAnswer(e, obj) {
    e.preventDefault();

    //$("#button-next").addClass("hidden");
    //$("#button-ok").removeClass("hidden");

    var questionId = $("#RandomQuestion_QuestionId").val();
    var selected = $("input[type='radio'][name='RandomQuestion.SelectedAnswerId']:checked");
    var answerId = "";
    if (selected.length > 0) {
        answerId = selected.val();
    }

    var json = '{"questionId":' + questionId + ', "answerId":' + answerId + '}';

    postJson('/trivia/submitAnswer', json, function (data) {
        if (data.success) {
            if (data.isAnswerCorrect) {
                $("#button-next").removeClass("hidden");
                $("#button-ok").addClass("hidden");
                $("input[type='radio']").attr("disabled", true);
                $("#result-alert").removeClass("alert-success");
                $("#result-alert").removeClass("alert-danger");
                $("#result-alert").removeClass("hidden");
                $("#result-alert").addClass("alert-success");
                $("#result-alert").text('Correct');
            } else {
                $("#button-next").removeClass("hidden");
                $("#button-ok").addClass("hidden");
                $("input[type='radio']").attr("disabled", true);
                $("#result-alert").removeClass("alert-success");
                $("#result-alert").removeClass("alert-danger");
                $("#result-alert").removeClass("hidden");
                $("#result-alert").addClass("alert-danger");
                $("#result-alert").text('Incorrect');
            }
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

    json = '{"reviewId":' + reviewId + ', "violationTypeId":' + violationTypeId + ', "authorUserId":"' + authorUserId + '", "content":"' + content + '"}';

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
});