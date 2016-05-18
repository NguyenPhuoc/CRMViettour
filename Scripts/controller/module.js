CKEDITOR.replace("insert-note");
CKEDITOR.replace("insert-note-form");
$("#insert-function").select2();

function OnSuccess() {
    $("#modal-insert-form").modal("hide");
    $("#modal-edit-form").modal("hide");

    $("#modal-insert-function").modal("hide");
    $("#modal-edit-function").modal("hide");
}

function OnFailure() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-form").modal("hide");
    $("#modal-edit-form").modal("hide");

    $("#modal-insert-function").modal("hide");
    $("#modal-edit-function").modal("hide");
}

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
    $("#listItemId").val('');
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

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/ModuleManage/EditModule',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-module").html(data);
            CKEDITOR.replace("edit-note");
            $("#modal-edit-module").modal("show");
        }
    });
});

/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function (e) {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');

    var dataPost = { id: $(this).find("input[type='checkbox'][name='cb']").val() };
    if (dataPost.id != null) {
        $.ajax({
            type: "POST",
            url: '/ModuleManage/InfoForm',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tblForm").html(data);
            }
        });

        dataPost = { id: 0 };
        $.ajax({
            type: "POST",
            url: '/ModuleManage/InfoFunction',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tblFunction").html(data);
            }
        });

        $("#btnAddForm").attr('disabled', false)
        $("#btnAddFunction").attr('disabled', true)

    } else {
        $("#btnAddForm").attr('disabled', true)
        $("#btnAddFunction").attr('disabled', true)
    }
});

function functionForm(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ModuleManage/InfoFunction',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#tblFunction").html(data);
            $("#btnAddFunction").attr('disabled', false)
        }
    });
}

function deleteForm(id) {
    if (confirm("Bạn thực sự muốn xóa mục này ?")) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/ModuleManage/DeleteForm',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tblForm").html(data);
                $("#btnAddFunction").attr('disabled', true)
            }
        });
    }
}

function updateForm(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ModuleManage/EditForm',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-form").html(data);
            CKEDITOR.replace("edit-note-form");
            $("#modal-edit-form").modal("show");
        }
    });
}

function btnAddFunction() {
    $("#select2-insert-function-container").attr('title', '').text('');
    $.getJSON('/ModuleManage/LoadFunction', function (data) {
        var items = '';
        $.each(data, function (i, ward) {
            if (i == 0) {
                items += "<option selected value='" + ward.id + "'>" + ward.name + "</option>";
                $("#select2-insert-function-container").attr('title', ward.name).text(ward.name);
            } else {
                items += "<option value='" + ward.id + "'>" + ward.name + "</option>";
            }
        });
        $('#insert-function').html(items);
    });
    $("#modal-insert-function").modal("show");
}

function deleteFunction(id) {
    if (confirm("Bạn thực sự muốn xóa mục này ?")) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/ModuleManage/DeleteFunction',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tblFunction").html(data);
            }
        });
    }
}