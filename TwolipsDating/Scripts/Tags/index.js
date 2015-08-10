$(document).ready(function () {
    setupPopoverWithContent(".popover-tag", function () {
        var tagName = $(this).attr('data-tag-name');
        var contentDiv = "#popover-tag-content-" + tagName;
        var clone = $(contentDiv).clone(true);
        var cloneUnhide = clone.removeClass('hide');
        return cloneUnhide.html();
    });
});