$(document).ready(function () {
    setupMessageSend();

    $('.followify').followify();

    $('#select-search').chosen();

    $('#modalMessage').on('show.bs.modal', function(event) {
        var button = $(event.relatedTarget);
        var userName = button.data('user-name');
        var userId = button.data('user-id');

        $('#ProfileUserId').val(userId);

        var modal = $(this);
        modal.find('.modal-title-user-name').html(userName);
        modal.find('#link-view-conversation').prop('href', '/message/conversation/' + userId);
    });
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
}

function onSendMessage(e, obj) {
    var profileUserId = $('#ProfileUserId').val();
    var messageBody = $('#modal-message-body').val();

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
                $('#modal-message-body').val('');
                $("#message-send-log tr:last").after("<tr><td>" + htmlEscape(messageBody) + "</td></tr>");
            } else {
                $('#message-error').show();
                $('#message-error-text').text(data.error);
            }
        });
    }
}