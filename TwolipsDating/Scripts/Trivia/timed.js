$(document).ready(function () {
    get('/trivia/startTimedQuestion', function (data) {
        var dateUtcNow = new Date(data.year, data.month, data.day, data.hours, data.minutes, data.seconds, data.milliseconds);
        interval = setInterval(tickCountDown, 1000);
    });

    setupPopoverWithContent("#share-question-link", function () {
        var shareButtonsDiv = "#share-question-popover";
        var clone = $(shareButtonsDiv).clone(true);
        var cloneUnhide = clone.removeClass('hide');
        return cloneUnhide.html();
    });

    $(".answer-link").on("click", function (e) {
        onSubmitAnswer(e, this);
    });

    $('.followify').followify();

    setupQuestionViolation();
});

var interval;

function tickCountDown() {
    var secondsLeft = $("#timer-label").text();
    secondsLeft = secondsLeft - 1;
    $("#timer-label").text(secondsLeft);

    if (secondsLeft == 0) {
        $("#alert-box").removeClass("alert-success");
        $("#alert-box").removeClass("alert-danger");
        $("#alert-box").removeClass("alert-info");
        $("#alert-box").addClass("alert-danger");
        $("#alert-box").html("<h5>You ran out of time!</h4>");
        $("#button-next").removeClass("hidden");
        $("#button-next").addClass("btn-danger");
        $(".answer-link").addClass("disabled");
        $("#button-skip").addClass("hidden");
        stopCountdown();
    }
}

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

function stopCountdown() {
    clearInterval(interval);
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

    var jsonObject = {
        "questionId": questionId,
        "answerId": answerId
    };

    var json = JSON.stringify(jsonObject);

    postJson('/trivia/submitTimedAnswer', json, function (data) {
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

            stopCountdown();

            $(".answer-link").addClass("list-group-item-danger");
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