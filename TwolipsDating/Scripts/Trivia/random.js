$(document).ready(function () {
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
    var questionId = $('#QuestionId').val();
    var violationTypeId = $('#QuestionViolation_ViolationTypeId').val();

    var jsonObject = {
        "questionId": questionId,
        "violationTypeId": violationTypeId
    };

    var json = JSON.stringify(jsonObject);

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
    //$("#button-ok").removeClass("hidden");

    var questionId = $("#QuestionId").val();
    var selectedAnswerId = $(obj).attr("data-answer-id"); //$("input[type='radio'][name='SelectedAnswerId']:checked");
    //var answerId = "";
    //if (selected.length > 0) {
    //    answerId = selected.val();
    //}

    var jsonObject = {
        "questionId": questionId,
        "answerId": selectedAnswerId
    };

    var json = JSON.stringify(jsonObject);

    postJson('/trivia/submitAnswer', json, function (data) {
        if (data.success) {
            if (data.correctAnswerId == selectedAnswerId) {
                $("#button-next").removeClass("hidden");
                $("#button-next").addClass("btn-success");
                $("#button-skip").addClass("hidden");
                //$("#button-ok").addClass("hidden");
                //$("input[type='radio']").attr("disabled", true);
            } else {
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