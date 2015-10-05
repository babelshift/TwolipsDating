var guessesRemaining = 0;

$(document).ready(function () {
    $('#share-score-fb').on('click', function (e) {
        var shareHref = $(this).data('share-href');
        var description = $(this).data('share-description');

        FB.ui({
            method: 'feed',
            link: shareHref,
            description: description,
        }, function (response) { });
    });

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

    $('.followify').followify();

    initGuessesRemaining();

    $('#button-reset').on('click', function (e) {
        $('.minefield-checkbox-label').removeClass('disabled');
        $('.minefield-checkbox-label').removeClass('active');
        resetGuessesRemaining();
        $('#minefield-selected-answer-count').html(guessesRemaining);
    });

    $('.minefield-checkbox').on('change', function (e) {

        var checkbox = $(this);
        var isChecked = checkbox.prop('checked');

        if (isChecked) {
            decreaseGuessesRemaining();
        } else {
            increaseGuessesRemaining();
        }

        var possibleCorrectAnswerCount = getPossibleCorrectAnswerCount();

        if (guessesRemaining <= 0) {
            $('.minefield-checkbox-label').each(function (i, e) {
                isChecked = $(e).find('input:checkbox').prop('checked');
                if (!isChecked) {
                    $(e).addClass('disabled');
                }
            });
        } else {
            $('.minefield-checkbox-label').removeClass('disabled');
        }
    });
});

function getPossibleCorrectAnswerCount() {
    return $('#minefield-possible-correct-answer-count').html();
}

function setGuessesRemaining(value) {
    guessesRemaining = value;
    $('#minefield-selected-answer-count').html(guessesRemaining);

    if (guessesRemaining == 0) {
        $('#minefield-selected-answer-count').addClass('text-danger');
    } else {
        $('#minefield-selected-answer-count').removeClass('text-danger');
    }
}

function initGuessesRemaining() {
    setGuessesRemaining(getPossibleCorrectAnswerCount());
}

function resetGuessesRemaining() {
    setGuessesRemaining(getPossibleCorrectAnswerCount());
}

function increaseGuessesRemaining() {
    setGuessesRemaining(guessesRemaining + 1);
}

function decreaseGuessesRemaining() {
    setGuessesRemaining(guessesRemaining - 1);
}

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