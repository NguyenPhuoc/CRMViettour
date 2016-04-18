/***INSERT***/
$("#insert-task-type").select2();
$("#insert-task-status").select2();
$("#insert-customer-task").select2();
$("#insert-tour-task").select2();
$("#insert-task-priority").select2();
CKEDITOR.replace("insert-assign-note1")

/*** datepicker ***/
//$("#ngaycapmst").datepicker();
//$("#ngaytldn").datepicker();
//$("#ngaysinh").datepicker();
//$("#ngaysinh-contact").datepicker();
//$("#insert-ngaycap").datepicker();
//$("#insert-ngaycap-passport").datepicker();
//$("#insert-ngayhethan-passport").datepicker();

///*** visa ***/
//$("#ngaycapvisa1").datepicker();
//$("#ngayhethanvisa1").datepicker();
$("#countryvisa1").select2();

$('.dataTable').dataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
});

function radCompanyClick() {
    $('#detail-company').show(); $('#detail-personal').hide();
}

function radPersonalClick() {
    $('#detail-company').hide(); $('#detail-personal').show();
}


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
                url: '/CustomerTabInfo/InfoThongTinChiTiet',
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
                url: '/CustomerTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
            break;
        case 'tourtuyen':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoTourTuyen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tourtuyen").html(data);
                }
            });
            break;
        case 'nguoilienhe':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoNguoiLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nguoilienhe").html(data);
                }
            });
            break;
        case 'visa':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoVisa',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#visa").html(data);
                }
            });
            break;
        case 'hosolienquan':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoHoSoLienQuan',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hosolienquan").html(data);
                }
            });
            break;
        case 'phanhoikhachhang':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#phanhoikhachhang").html(data);
                }
            });
            break;
        case 'email':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoEmail',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#email").html(data);
                }
            });
            break;
        case 'sms':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoSMS',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#sms").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
            break;
        case 'capnhatthaydoi':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoCapNhatThayDoi',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#capnhatthaydoi").html(data);
                }
            });
            break;
    }
});

/** click chọn từng tab -> hiển thị thông tin **/
$("#tabthongtinchitiet").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoThongTinChiTiet',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#thongtinchitiet").html(data);
            }
        });
    }
});

$("#tablichhen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoLichHen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichhen").html(data);
            }
        });
    }
});

$("#tabtourtuyen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoTourTuyen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tourtuyen").html(data);
            }
        });
    }
});

$("#tabvisa").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoVisa',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#visa").html(data);
            }
        });
    }
});

$("#tabhosolienquan").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoHoSoLienQuan',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#hosolienquan").html(data);
            }
        });
    }
});

$("#tabemail").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoEmail',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#email").html(data);
            }
        });
    }
});

$("#tabsms").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoSMS',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#sms").html(data);
            }
        });
    }
});

$("#tablichsulienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabcapnhatthaydoi").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoCapNhatThayDoi',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#capnhatthaydoi").html(data);
            }
        });
    }
});

$("#tabphanhoikhachhang").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#phanhoikhachhang").html(data);
            }
        });
    }
});

$("#tabnguoilienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoNguoiLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nguoilienhe").html(data);
            }
        });
    }
});


///****** Sửa thông tin ******/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox'].cbItem:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/TaskManage/TaskInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-task").html(data);
            $("#edit-task-type").select2();
            $("#edit-task-status").select2();
            $("#edit-customer-task").select2();
            $("#edit-task-priority").select2();
            $("#edit-tour-task").select2();
            CKEDITOR.replace("edit-note");
            $("#modal-edit-task").modal("show");

            $("#edit-check-notify").click(function () {
                if (this.checked) {
                    $("#edit-ngaynhac").removeAttr("disabled", "disabled");
                }
                else {
                    $("#edit-ngaynhac").attr("disabled", "disabled");
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Error: " + errorThrown);
        }

    });
});



/** success ajax form **/
function OnSuccessCustomerFile() {
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
}

/** failure ajax form **/
function OnFailureCustomerFile() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
}
$(function () {
    $('#btnAddAssign').click(function () {
        var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.assigncustomer').attr('id', 'insert-assign-customer' + newNum).attr('name', 'Customer' + newNum).val('');
        newElem.find('.assignrole').attr('name', 'Role' + newNum).val('');
        newElem.find('.assignnote').attr('id', 'insert-assign-note' + newNum).attr('name', 'Note' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        CKEDITOR.replace("insert-assign-note" + newNum)

        for (var i = 1; i < newNum; i++) {
            $("#entry" + newNum).find("#cke_insert-assign-note" + i).remove();
            //$("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
        }

        // enable the "remove" button
        $('#btnDel').attr('disabled', false);

        // count service
        $("#countAssign").val(newNum);
    });

    $('#btnDel').click(function () {
        // confirmation
        var num = $('.clonedInput').length;
        // how many "duplicatable" input fields we currently have
        $('#entry' + num).slideUp('slow', function () {
            $(this).remove();
            // if only one element remains, disable the "remove" button
            if (num - 1 === 1)
                $('#btnDel').attr('disabled', true);
            // enable the "add" button
            $('#btnAdd').attr('disabled', false).prop('value', "add section");
        });
        return false;

        $('#btnAdd').attr('disabled', false);
        // count service
        $("#countAssign").val(num);
    });
    //$('#btnDel').attr('disabled', true);
});

$("#btnAssign").click(function () {
    var dataPost = {
        id: $("input[type='checkbox'].cbItem:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/TaskManage/GetIdTask',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
        }
    });

});

