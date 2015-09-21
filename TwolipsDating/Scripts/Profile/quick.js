$(document).ready(function () {
    setupMessageSend();
});

function setupMessageSend() {
    $('#message-send-log').hide();
    $('#message-error').hide();
    $('#message-success').hide();

    $('#modalMessage').on('hide.bs.modal', function (event) {
        $('#message-error').hide();
        $('#message-success').hide();
        $('#button-message-send').show();
        $('#SendMessage_MessageBody').val('');
    });

    $('#button-message-send').on('click', function (e) {
        onSendMessage(e);
    });

    $('.followify').followify();
}

function onSendMessage(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var messageBody = $('#SendMessage_MessageBody').val();

    if (messageBody != null && messageBody.length > 0) {

        var jsonObject = {
            "profileUserId": profileUserId,
            "messageBody": messageBody
        };

        var json = JSON.stringify(jsonObject);

        postJson('/profile/sendMessage', json, function (data) {
            if (data.success) {
                $('#message-send-log').show();
                $('#message-success').show();
                $('#SendMessage_MessageBody').val('');
                $("#message-send-log tr:last").after("<tr><td>" + htmlEscape(messageBody) + "</td></tr>");
            } else {
                $('#message-error').show();
                $('#message-error-text').text(data.error);
            }
        });
    }
}