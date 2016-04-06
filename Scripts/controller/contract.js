$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
});

CKEDITOR.replace("insert-note");
$("#insert-tour").select2();
$("#insert-customer").select2();
$("#insert-status").select2();
$("#insert-startdate").datepicker();
$("#insert-enddate").datepicker();
$("#insert-contractdate").datepicker();

function viewFileOnline(id) {
    $("#viewfile").html("<div id='modal-view-file' class='modal fade' role='dialog'>" +
    "<div class='modal-dialog'><div class='modal-content'>" +
    "<iframe src='" + $("#txtURL" + id) + "' width='500px' height='350px' frameborder='0'></iframe>" +
    "</div></div></div>");
    $("#modal-view-file").modal("show");
}

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/ContractsManage/ContractInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#edit-data").html(data);
            CKEDITOR.replace("edit-note");
            $("#edit-tour").select2();
            $("#edit-customer").select2();
            $("#edit-status").select2();
            $("#edit-contractdate").datepicker();
            $("#edit-startdate").datepicker();
            $("#edit-enddate").datepicker();
            $("#modal-edit-contract").modal("show");
        }
    });
});

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

/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function (e) {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    var tab = $(".tab-content").find('.active').data("id");
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    switch (tab) {
        case 'thongtinchitiet':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoThongTinChiTiet',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#thongtinchitiet").html(data);
                }
            });
            break;
        case 'lichhen':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
            break;
        case 'chitiettour':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoChiTietTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chitiettour").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
            break;
        case 'congno':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoCongNo',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#congno").html(data);
                }
            });
            break;
        case 'lichsuinvoicedoitac':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoLichSuInvoiceDoiTac',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsuinvoicedoitac").html(data);
                }
            });
            break;
        case 'tailieumau':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoTaiLieuMau',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tailieumau").html(data);
                }
            });
            break;
        case 'phieuthu':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoPhieuThu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#phieuthu").html(data);
                }
            });
            break;
        case 'nhatkyxuly':
            $.ajax({
                type: "POST",
                url: '/ContactTabInfo/InfoNhatKyXuLy',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nhatkyxuly").html(data);
                }
            });
            break;
    }
});

/** click chọn từng tab -> hiển thị thông tin **/
$("#tabthongtinchitiet").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoThongTinChiTiet',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("test");
                $("#thongtinchitiet").html(data);
            }
        });
    }
});

$("#tablichhen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoLichHen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichhen").html(data);
            }
        });
    }
});

$("#tabchitiettour").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoChiTietTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#chitiettour").html(data);
            }
        });
    }
});

$("#tabtailieumau").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoTaiLieuMau',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tailieumau").html(data);
            }
        });
    }
});

$("#tablichsulienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabcongno").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoCongNo',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#congno").html(data);
            }
        });
    }
});

$("#tabphieuthu").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoPhieuThu',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#phieuthu").html(data);
            }
        });
    }
});

$("#tablichsuinvoicedoitac").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoLichSuInvoiceDoiTac',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsuinvoicedoitac").html(data);
            }
        });
    }
});

$("#tabnhatkyxuly").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 chương trình!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/ContactTabInfo/InfoNhatKyXuLy',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nhatkyxuly").html(data);
            }
        });
    }
});