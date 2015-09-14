(function ($) {
    $.fn.followify = function (options) {
        var settings = $.extend({
            fadeOut: true
        }, options);

        return this.on('click', function (e) {
            e.preventDefault();

            var profileUserId = $(this).attr('data-user-id');
            var profileId = $(this).attr('data-profile-id');

            var jsonObject = {
                "profileUserId": profileUserId,
                "profileId": profileId
            };

            var json = JSON.stringify(jsonObject);

            postJson('/profile/toggleFavoriteProfile', json, function (data) {
                if (data.success) {
                    toggleIcon(profileId, data.isFavorite);

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
    var toggleIcon = function (profileId, isFavorite) {
        if (isFavorite) {
            $('#button-toggle-favorite-' + profileId).removeClass("btn-default");
            $('#button-toggle-favorite-' + profileId).addClass("btn-success");
            $('#icon-toggle-favorite-' + profileId).removeClass('fa-user-plus');
            $('#icon-toggle-favorite-' + profileId).addClass('fa-check');
        } else {
            $('#button-toggle-favorite-' + profileId).removeClass("btn-success");
            $('#button-toggle-favorite-' + profileId).addClass("btn-default");
            $('#icon-toggle-favorite-' + profileId).removeClass('fa-check');
            $('#icon-toggle-favorite-' + profileId).addClass('fa-user-plus');
        }
    }
}(jQuery));