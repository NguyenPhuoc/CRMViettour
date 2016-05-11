function addRow2Colum() {
    $("table.table-addrow tbody").append("<tr><td><input type='checkbox' value='0' id='cb' name='cb' checked='checked' /></td>"
        + "<td></td><td><input type='text' value='' id='txtName' class='form-control' /></td></tr>");
    $("#btnSave").removeAttr("disabled", "disabled");
    $("#btnAdd").attr("disabled", "disabled");
}

function updateRow2Colum() {
    $('tbody tr td').find("input:checkbox").each(function () {
        if (this.checked) {
            var tr = $(this).closest("tr").find("td:last");
            var text = tr.text();
            tr.text("");
            tr.append("<input type='text' id='txtName' value='" + text + "' class='form-control' />");
        }
    });
    $("#btnAdd").attr("disabled", "disabled");
    $("#btnEdit").attr("disabled", "disabled");
}

$("#btnSave").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val(),
        name: $("#txtName").val()
    };

    $.ajax({
        type: "POST",
        url: '/FlightManage/SaveData',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            window.location.href = '/FlightManage/Index';
        }
    });
})

/** Xoa du lieu **/

$("#btnRemove").on("click", function () {
    if ($("#listItemId").val() == "") {
        alert("Vui lòng chọn mục cần xóa !");
        return false;
    }
    var $this = $(this);
    var $tableWrapper = $("#tableDictionary-Wrapper");
    var $table = $("#tableDictionary");

    DeleteSelectedItem($this, $tableWrapper, $table, function (data) {

    });
    return false;
});

$("#tableDictionary").on("change", ".cbItem", function () {
    var ItemID = $(this).val();
    var currentlistItemID = $("#listItemId").val();
    var stringBranchID = "";
    if ($(this).prop('checked')) {
        currentlistItemID += ItemID + ",";
        $("#listItemId").val(currentlistItemID);
    } else {
        $("#listItemId").val(currentlistItemID.replace(ItemID + ",", ""));
    }
});

$("#tableDictionary").on("change", "#allcb", function () {
    var $this = $(this);
    var currentlistItemID = $("#listItemId").val();
    var ItemID = "";
    if ($this.prop("checked")) {
        $(".cbItem").each(function () {
            ItemID = $(this).val();
            if ($(this).parent().hasClass('text-danger') == false) {
                $(this).prop("checked", true);
                currentlistItemID += ItemID + ",";
                $("#listItemId").val(currentlistItemID);
            }
        });
    } else {
        $(".cbItem").prop("checked", false);
        $("#listItemId").val("");
    }
});

function DeleteSelectedItem(selector, tableWrapper, table, callBack) {
    if (!confirm("Bạn thực sự muốn xóa những mục đã chọn ?")) {
        return false;
    }
    var $form = selector.next("form");
    var options = {
        url: $form.attr("action"),
        type: $form.attr("method"),
        data: $form.serialize(),
    };

    tableWrapper.append("<div class='layer'></div>");

    $.ajax(options).done(function (data) {
        tableWrapper.find(".layer").remove();
        if (data.Succeed) {
            alert(data.Message);
            if (data.IsPartialView) {
                table.replaceWith(data.View);
            }
            else {
                if (data.RedirectTo != null && data.RedirectTo != "") {
                    window.location.href = data.RedirectTo;
                }
            }
        }
        else {
            alert(data.Message);
        }
    });
}