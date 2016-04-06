$(document).ready(function () {
    $("[data-toggle='collapse']").click(function (e) {
        var $this = $(this);
        var $icon = $this.find("i");

        if ($icon.hasClass('fa-minus')) {
            $icon.removeClass('fa-minus').addClass('fa-plus');
        } else {
            $icon.removeClass('fa-plus').addClass('fa-minus');
        }
    });
});
