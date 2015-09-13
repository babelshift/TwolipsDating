$(document).ready(function () {
    $('#modalQuestionViolation').on('hide.bs.modal', function (event) {
        $('#violation-error').addClass('hidden');
        $('#violation-success').addClass('hidden');
        $('#button-violation-submit').removeClass('hidden');
    });

    $('#modalQuestionViolation').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var questionId = button.data('question-id'); // Extract info from data-* attributes
        var modal = $(this);
        modal.find('#selected-question-id').val(questionId);
    });
});

function onAddQuestionViolation(e, obj) {
    var questionId = $('#selected-question-id').val();
    var violationTypeId = $('#QuestionViolation_ViolationTypeId').val();
    
    var jsonObject = {
        "questionId": questionId,
        "violationTypeId": violationTypeId
    };

    var json = JSON.stringify(jsonObject);

    postJson('/violation/addQuestionViolation', json, function (data) {
        if (data.success) {
            $('#violation-success').removeClass('hidden');
            $('#button-violation-submit').addClass('hidden');
        } else {
            $('#violation-error').removeClass('hidden');
            $('#violation-error-text').text(data.error);
        }
    });
}