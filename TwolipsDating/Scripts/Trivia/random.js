$(document).ready(function () {
    setupPopoverWithContent("#share-question-link", function () {
        var shareButtonsDiv = "#share-question-popover";
        var clone = $(shareButtonsDiv).clone(true);
        var cloneUnhide = clone.removeClass('hide');
        return cloneUnhide.html();
    });

    $(".answer-link").on("click", function (e) {
        onSubmitAnswer(e, this);
    });

    setupQuestionViolation();
});

function setupQuestionViolation() {
    $('#violation-error').hide();
    $('#violation-success').hide();

    $('#modalQuestionViolation').on('hide.bs.modal', function (event) {
        $('#violation-error').hide();
        $('#violation-success').hide();
        $('#button-violation-submit').show();
    });
}

function onAddQuestionViolation(e, obj) {
    var reviewId = $('#QuestionId').val();
    var violationTypeId = $('#QuestionViolation_ViolationTypeId').val();

    var json = '{"questionId":' + reviewId + ', "violationTypeId":' + violationTypeId + '}';

    postJson('/violation/addQuestionViolation', json, function (data) {
        if (data.success) {
            $('#violation-success').show();
            $('#button-violation-submit').hide();
        } else {
            $('#violation-error').show();
            $('#violation-error-text').text(data.error);
        }
    });
}

function onSubmitAnswer(e, obj) {
    e.preventDefault();

    //$("#button-next").addClass("hidden");
    $("#button-ok").removeClass("hidden");

    var questionId = $("#QuestionId").val();
    var selectedAnswerId = $(obj).attr("data-answer-id"); //$("input[type='radio'][name='SelectedAnswerId']:checked");
    //var answerId = "";
    //if (selected.length > 0) {
    //    answerId = selected.val();
    //}

    var json = '{"questionId":' + questionId + ', "answerId":' + selectedAnswerId + '}';

    postJson('/trivia/submitAnswer', json, function (data) {
        if (data.success) {
            if (data.correctAnswerId == selectedAnswerId) {
                $("#alert-box").removeClass("alert-success");
                $("#alert-box").removeClass("alert-danger");
                $("#alert-box").removeClass("alert-info");
                $("#alert-box").addClass("alert-success");
                $("#alert-box").html("<h4>Correct!</h4>");
                $("#button-next").removeClass("hidden");
                $("#button-next").addClass("btn-success");
                $("#button-skip").addClass("hidden");
                //$("#button-ok").addClass("hidden");
                //$("input[type='radio']").attr("disabled", true);
            } else {
                $("#alert-box").removeClass("alert-success");
                $("#alert-box").removeClass("alert-danger");
                $("#alert-box").removeClass("alert-info");
                $("#alert-box").addClass("alert-danger");
                $("#alert-box").html("<h4>Incorrect!</h4>");
                $("#button-next").removeClass("hidden");
                $("#button-next").addClass("btn-danger");
                $("#button-skip").addClass("hidden");
                //$("#button-ok").addClass("hidden");
                //$("input[type='radio']").attr("disabled", true);
            }

            $(".answer-link").addClass("list-group-item-danger");
            $(".answer-link").off();
            $("#answer-" + data.correctAnswerId).removeClass("list-group-item-danger");
            $("#answer-" + data.correctAnswerId).addClass("list-group-item-success");

            $(".icon-incorrect").removeClass("hidden");
            $("#icon-incorrect-" + data.correctAnswerId).addClass("hidden");
            $("#icon-correct-" + data.correctAnswerId).removeClass("hidden");

            $("#button-skip").hide();
        } else {
            alert(data.error);
        }
    });
}