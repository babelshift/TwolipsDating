(function ($) {
    $.fn.followify = function (options) {
        var settings = $.extend({
            fadeOut: false
        }, options);

        return this.on('click', function (e) {
            e.preventDefault();

            var button = $(this);

            var profileUserId = button.attr('data-user-id');
            var profileId = button.attr('data-profile-id');

            var jsonObject = {
                "profileUserId": profileUserId,
                "profileId": profileId
            };

            var json = JSON.stringify(jsonObject);

            postJson('/profile/toggleFavoriteProfile', json, function (data) {
                if (data.success) {
                    if (data.isFavorite) {
                        button.removeClass('btn-default');
                        button.addClass('btn-success');
                        button.find('i:first').removeClass('fa-user-plus');
                        button.find('i:first').addClass('fa-check');
                        button.find('span:first').html('Following');
                    } else {
                        button.removeClass("btn-success");
                        button.addClass("btn-default");
                        button.find('i:first').removeClass('fa-check');
                        button.find('i:first').addClass('fa-user-plus');
                        button.find('span:first').html('Follow');
                    }

                    if (settings.fadeOut) {
                        $('#user-to-follow-' + profileId).fadeOut('normal', function () {
                            $(this).remove();
                        });
                    }
                } else {
                    alert(data.error);
                }
            });
        });
    }
}(jQuery));