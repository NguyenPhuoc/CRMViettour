function DeleteImport() {
    $("#fileImport").val('');
    $("#import-data").html('');
}
$(".dataTable").dataTable();

$("table#tableDictionary").delegate("tr", "click", function () {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');

    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    $.ajax({
        type: "POST",
        url: '/ExchangeRates/InfoTyGia',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#table-document").html(data);
        }
    });
});
