function onSubmitAnswer(e, obj) {
    e.preventDefault();

    $("#button-next").addClass("hidden");
    $("#button-ok").removeClass("hidden");

    var questionId = $("#QuestionId").val();
    var selected = $("input[type='radio'][name='SelectedAnswerId']:checked");
    var answerId = "";
    if (selected.length > 0) {
        answerId = selected.val();
    }

    var json = '{"questionId":' + questionId + ', "answerId":' + answerId + '}';

    postJson('/trivia/submitAnswer', json, function (data) {
        if (data.success) {
            if (data.isAnswerCorrect) {
                $("#alert-box").removeClass("alert-success");
                $("#alert-box").removeClass("alert-danger");
                $("#alert-box").removeClass("alert-info");
                $("#alert-box").addClass("alert-success");
                $("#alert-box").html("<h4>Correct!</h4>");
                $("#button-next").removeClass("hidden");
                $("#button-next").addClass("btn-success");
                $("#button-ok").addClass("hidden");
                $("input[type='radio']").attr("disabled", true);
            } else {
                $("#alert-box").removeClass("alert-success");
                $("#alert-box").removeClass("alert-danger");
                $("#alert-box").removeClass("alert-info");
                $("#alert-box").addClass("alert-danger");
                $("#alert-box").html("<h4>Incorrect!</h4>");
                $("#button-next").removeClass("hidden");
                $("#button-next").addClass("btn-danger");
                $("#button-ok").addClass("hidden");
                $("input[type='radio']").attr("disabled", true);
            }
        } else {
            alert(data.error);
        }
    });
}