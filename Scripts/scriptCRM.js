$(document).ready(function () {
    $("table.data-demo").delegate("tr", "click", function () {
        var value = $(this).children('td:first').text();
        $("#valueID").text(value);
    });
    loadDataJSON("http://" + "api.ilinks.xyz/api/log/108/" + 1 + "/" + $("#sort-data option:selected").val());
    $("#btnFirstPrev").addClass("disabled");
    $("#btnPrev").addClass("disabled");
});

$("#sort-data").change(function () {
    loadDataJSON("http://" + "api.ilinks.xyz/api/log/108/" + $("ul.pagination li.active").text() + "/" + $("#sort-data option:selected").val());
});

function loadDataJSON(url) {
    $.getJSON(url, function (data) {
        var html = '<tr><td hidden="hidden"></td><td></td><td><input type="text" class="form-control" /></td>'
                            + '<td><input type="text" class="form-control" /></td><td><input type="text" class="form-control" /></td>'
                            + '<td><input type="text" class="form-control" /></td><td><input type="text" class="form-control" /></td></tr>';
        $.each(JSON.parse(data), function (key, val) {
            html += '<tr><td hidden="hidden">' + val.visitIP + '</td>'
                               + '<td><input type="checkbox" id="cb' + val.visitIP + '" name="cb' + val.visitIP + '" onclick="CheckSelect()" /></td>'
                               + '<td>' + val.visitIP + '</td>'
                               + '<td>' + val.visitCountryCode + '</td>'
                               + '<td>' + val.errrorCode + '</td>'
                               + '<td>' + val.reference + '</td>'
                               + '<td>' + val.visitDate + '</td>'
                               + '</tr>';
        });

        $('#load-data-demo').html(html);
    });
}

function checkFirstLastPage(i) {
    if (i == 1) {
        $("#btnFirstPrev").addClass("disabled");
        $("#btnPrev").addClass("disabled");
        $("#btnLastNext").removeClass("disabled");
        $("#btnNext").removeClass("disabled");
    }
    else if (i == 5) {
        $("#btnFirstPrev").removeClass("disabled");
        $("#btnPrev").removeClass("disabled");
        $("#btnLastNext").addClass("disabled");
        $("#btnNext").addClass("disabled");
    }
    else {
        $("#btnFirstPrev").removeClass("disabled");
        $("#btnPrev").removeClass("disabled");
        $("#btnLastNext").removeClass("disabled");
        $("#btnNext").removeClass("disabled");
    }
}

function clickpageData(i) {
    loadDataJSON("http://" + "api.ilinks.xyz/api/log/108/" + i + "/" + $("#sort-data option:selected").val());
    $("ul.pagination li.active").removeClass("active");
    $("#page" + i).addClass("active");
    checkFirstLastPage(i);
}

function clickbtnLuuSua() {
    $("#loading").fadeIn();
    setTimeout(function () { $("#loading").fadeOut(); }, 5000);
    $("#modal-edit-customer").modal('hide');
}

function clickbtnLuuThem() {
    $("#loading").fadeIn();
    setTimeout(function () { $("#loading").fadeOut(); }, 5000);
    $("#modal-insert-customer").modal('hide');
}

function clickSortASC(i) {
    var x = $("#sortasc" + i).parent().attr('id');
    alert('ASC: ' + x);
}

function clickSortDESC(i) {
    var x = $("#sortdesc" + i).parent().attr('id');
    alert('DESC: ' + x);
}

$("#btnFirstPrev a").click(function () {
    clickpageData(1);
});

$("#btnPrev a").click(function () {
    var i = $("ul.pagination li.active").prev().find("a").text();
    clickpageData(i);
});

$("#btnLastNext a").click(function () {
    clickpageData(5);
});

$("#btnNext a").click(function () {
    var i = $("ul.pagination li.active").next().find("a").text();
    clickpageData(i);
});
