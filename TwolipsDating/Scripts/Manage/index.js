$(document).ready(function () {
    var selectedGenderIdMenu = $('#SelectedGenderId');
    if (selectedGenderIdMenu != null) {
        selectedGenderIdMenu.chosen({ disable_search_threshold: 999 });
    }
});