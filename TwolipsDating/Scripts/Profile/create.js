$(document).ready(function () {
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