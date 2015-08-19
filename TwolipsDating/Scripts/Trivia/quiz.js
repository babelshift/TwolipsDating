$(document).ready(function () {
    $('#violation-error').hide();
    $('#violation-success').hide();

    $('#modalQuestionViolation').on('hide.bs.modal', function (event) {
        $('#violation-error').hide();
        $('#violation-success').hide();
        $('#button-violation-submit').show();
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

    var json = '{"questionId":' + questionId + ', "violationTypeId":' + violationTypeId + '}';

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