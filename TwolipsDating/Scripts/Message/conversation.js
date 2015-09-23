$(document).ready(function () {
    $('.followify').followify();

    $('#search-user-name').on('keyup', function (e) {
        if (e && e.keyCode == 13) {
            $('#search').click();
        }
    });

    $('#search').click(function (e) {
        e.preventDefault();

        var userNameInput = $('#search-user-name');
        var userName = userNameInput.val();

        if (userName != null && userName.length > 0) {
            var jsonObject = { "userName": userName };
            var json = JSON.stringify(jsonObject);

            postJson('/message/searchpeopletomessage', json, function (data) {
                $('#new-message-panel').addClass('hidden');
                $('#search-result-panel').removeClass('hidden');
                var searchResults = $('#search-result-list');
                searchResults.html('');

                if (data.result != null && data.result.length > 0) {
                    $.each(data.result, function (index, value) {
                        searchResults.append(
                            '<a href="#" class="user-to-message-link list-group-item" data-user-id=' + value.UserId + ' data-user-name="' + value.UserName + '">' +
                                '<div class="user-to-message">' +
                                    '<img src="' + value.ProfileThumbnailImagePath + '" />' +
                                    '<span class="user-to-message-name">' +
                                        value.UserName +
                                    '</span>' +
                                    '<span class="user-to-message-details">' +
                                        '<i class="fa fa-user fa-fw"></i> ' + value.Age + ' years old' +
                                        '<br />' +
                                        '<i class="fa fa-map-marker fa-fw"></i> ' + value.Location +
                                    '</span>' +
                                '</div>' +
                            '</div>'
                        );
                    });
                }
                else
                {
                    searchResults.append(
                        '<div class="list-group-item"><h5>There is no one by that name. <i class="fa fa-frown-o fa-lg fa-fw"></i></h5></div>'
                    );
                }

                $('.user-to-message-link').click(function (e) {
                    e.preventDefault();
                    var userName = $(this).attr('data-user-name');
                    var userId = $(this).attr('data-user-id');
                    userNameInput.val(userName);
                    $('#TargetApplicationUserId').val(userId);
                    $('#search-result-panel').addClass('hidden');
                    $('#new-message-panel').removeClass('hidden');
                })
            });
        }
    });
});