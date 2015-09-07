$(document).ready(function () {
    var selectedGenderIdMenu = $('#SelectedGenderId');
    if (selectedGenderIdMenu != null) {
        selectedGenderIdMenu.chosen({ disable_search_threshold: 20 });
    }
    $('#BirthMonth').chosen();
    $('#BirthYear').chosen();
    $('#BirthDayOfMonth').chosen();

    $('#BirthMonth').chosen().change(function () {
        var jsonObject = {
            month: $(this).val()
        };

        var json = JSON.stringify(jsonObject);

        postJson('/date/daysofmonth', json,
                function (data) {
                    if (data.success) {
                        var $el = $('#BirthDayOfMonth');
                        $el.empty();
                        $.each(data.days, function (value, key) {
                            $el.append($('<option></option')
                                .attr("value", value).text(key));
                        });
                        $el.trigger("chosen:updated");
                    }
                });
    });

    $("#f_elem_city").autocomplete({
        source: function (request, response) {
            $.getJSON(
               "https://secure.geobytes.com/AutoCompleteCity?callback=?&q=" + request.term,
               function (data) {
                   response(data);
               }
            );
        },
        minLength: 3,
        select: function (event, ui) {
            $('#SelectedLocation').val(ui.item.value);
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
    $("#f_elem_city").autocomplete("option", "delay", 100);
});