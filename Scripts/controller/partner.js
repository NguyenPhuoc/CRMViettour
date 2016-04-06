CKEDITOR.replace("insert-note-schedule");
CKEDITOR.replace("edit-note-schedule");
CKEDITOR.replace('insert-noteservicepartner1');

$("#partner-services").select2();
$("#partner-tags").select2();
$("#insert-servicepartner").select2();
$("#insert-countrypartner").select2();
$("#insert-addresspartner").select2();
$("#insert-stylepartner").select2();

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
                { type: "text" }]
});

function OnSuccessPartner()
{
    $("#modal-insert-note").modal("hide");
    $("#modal-edit-note").modal("hide");
}

function OnFailurePartner()
{
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
}

function OnFailureDocument() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
}

function OnSuccessDocument() {
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
}

//function OnFailureAppointment() {
//    alert("Lỗi, vui lòng kiểm tra lại!");
//    $("#modal-insert-appointment").modal("hide");
//    $("#modal-edit-appointment").modal("hide");
//}

//function OnSuccessAppointment() {
//    $("#modal-insert-appointment").modal("hide");
//    $("#modal-edit-appointment").modal("hide");
//}


///*** duplicate form add service partner ***/
$(function () {
    $('#btnAdd').click(function () {
        var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.servicename').attr('name', 'PartnerServiceName' + newNum).val('');
        newElem.find('.serviceprice').attr('name', 'PartnerServicePrice' + newNum).val('');
        newElem.find('.servicenote').attr('id', 'insert-noteservicepartner' + newNum).attr('name', 'PartnerServiceNote' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        CKEDITOR.replace("insert-noteservicepartner" + newNum)

        for (var i = 1; i < newNum; i++) {
            $("#entry" + newNum).find("#cke_insert-noteservicepartner" + i).remove();
            //$("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
        }

        // enable the "remove" button
        $('#btnDel').attr('disabled', false);

        // count service
        $("#countService").val(newNum);
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
        $("#countService").val(num);
    });
    //$('#btnDel').attr('disabled', true);
});

///****** Sửa thông tin ******/
$("#btnEdit").click(function () {

    $('#btnAddE').attr('disabled', false);

    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/PartnerManage/PartnerInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#edit-partner").html(data);
            //CKEDITOR.replace('edit-noteservicepartnerE1');
            $("#edit-servicepartner").select2();
            $("#edit-countrypartner").select2();
            $("#edit-addresspartner").select2();
            $("#edit-stylepartner").select2();
            for (var i = 1 ; i <= $('#countServiceE').val() ; i++) {
                CKEDITOR.replace('edit-noteservicepartnerE' + i);
            }
            $("#modal-edit-partner").modal("show");

            ///*** duplicate form add service partner ***/
            $(function () {
                $('#btnAddE').click(function () {
                    var num = $('.clonedInputE').length, // how many "duplicatable" input fields we currently have
                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                        newElem = $('#entryE' + num).clone().attr('id', 'entryE' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                    // manipulate the name/id values of the input inside the new element

                    newElem.find('.servicenameE').attr('name', 'PartnerServiceNameE' + newNum).val('');
                    newElem.find('.servicepriceE').attr('name', 'PartnerServicePriceE' + newNum).val('');
                    newElem.find('.servicenoteE').attr('id', 'insert-noteservicepartnerE' + newNum).attr('name', 'PartnerServiceNoteE' + newNum).val('');

                    // insert the new element after the last "duplicatable" input field
                    $('#entryE' + num).after(newElem);
                    CKEDITOR.replace("insert-noteservicepartnerE" + newNum)

                    for (var i = 1; i < newNum; i++) {
                        $("#entryE" + newNum).find("#cke_insert-noteservicepartnerE" + i).remove();
                        //$("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
                    }

                    // enable the "remove" button
                    $('#btnDelE').attr('disabled', false);

                    // count service
                    $("#countServiceE").val(newNum);
                });

                $('#btnDelE').click(function () {
                    // confirmation
                    var num = $('.clonedInputE').length;
                    // how many "duplicatable" input fields we currently have
                    $('#entryE' + num).slideUp('slow', function () {
                        $(this).remove();
                        // if only one element remains, disable the "remove" button
                        if (num - 1 === 1)
                            $('#btnDelE').attr('disabled', true);
                        // enable the "add" button
                        $('#btnAddE').attr('disabled', false).prop('value', "add section");
                    });
                    return false;

                    $('#btnAddE').attr('disabled', false);
                    // count service
                    $("#countServiceE").val(num);
                });

            });
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
                url: '/PartnerTabInfo/InfoThongTinChiTiet',
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
                url: '/PartnerTabInfo/InfoLichHen',
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
                url: '/PartnerTabInfo/InfoTourTuyen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tourtuyen").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
            break;
        case 'danhgia':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoDanhGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#danhgia").html(data);
                }
            });
            break;
        case 'dichvucungcap':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoDichVuCungCap',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#dichvucungcap").html(data);
                }
            });
            break;
        case 'invoice':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoInvoice',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#invoice").html(data);
                }
            });
            break;
        case 'tailieumau':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoTaiLieuMau',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tailieumau").html(data);
                }
            });
            break;
        case 'ghichu':
            $.ajax({
                type: "POST",
                url: '/PartnerTabInfo/InfoGhiChu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#ghichu").html(data);
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
            url: '/PartnerTabInfo/InfoThongTinChiTiet',
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
            url: '/PartnerTabInfo/InfoLichHen',
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
            url: '/PartnerTabInfo/InfoTourTuyen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tourtuyen").html(data);
            }
        });
    }
});

$("#tabtailieumau").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoTaiLieuMau',
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
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabdanhgia").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoDanhGia',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#danhgia").html(data);
            }
        });
    }
});

$("#tabdichvucungcap").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoDichVuCungCap',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#dichvucungcap").html(data);
            }
        });
    }
});

$("#tabghichu").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoGhiChu',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#ghichu").html(data);
            }
        });
    }
});

$("#tabinvoice").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PartnerTabInfo/InfoInvoice',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#invoice").html(data);
            }
        });
    }
});

/** upload file **/
$('#FileName').change(function () {
    var data = new FormData();
    data.append('FileName', $('#FileName')[0].files[0]);
    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'PartnerManage/UploadFile',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/** xóa tài liệu **/
function deleteDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/DeleteDocument',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#tailieumau").html(data);
        }
    });
}

/** cập nhật tài liệu **/
function updateDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/EditInfoDocument',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-doc").html(data);
            $("#edit-tag-document").select2();
            $("#edit-document-type").select2();
            CKEDITOR.replace("edit-document-note");
            $("#modal-edit-document").modal("show");

            /**** update in tab file của khách hàng ****/
            $("#btnUpdateFile").click(function () {
                var $this = $(this);
                var $form = $("#frmUpdateFile");
                var $parent = $form.parent();
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize()
                };

                $.ajax(options).done(function (data) {
                    $("#modal-edit-document").modal("hide");
                    alert("Lưu dữ liệu thành công!");
                    $("#hosolienquan").html(data);
                });
                return false;
            });

            /** upload file **/
            $("#edit-file").change(function () {
                var data = new FormData();
                data.append('FileName', $('#edit-file')[0].files[0]);
                var ajaxRequest = $.ajax({
                    type: "POST",
                    url: 'PartnerManage/UploadFile',
                    contentType: false,
                    processData: false,
                    data: data
                });

                ajaxRequest.done(function (xhr, textStatus) {
                    // Onsuccess
                });
            });
        }
    });
}

///**** filter tag ***/
$("#btnSearchTags").click(function () {
    var dataPost = {
        tags: $("#partner-tags").val()
    };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/FilterTags',
        data: '{"tags": "' + $("#partner-tags").val() + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-partner").html(data);
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
                            { type: "text" }]
            });
            $("table#tableDictionary").delegate("tr", "click", function (e) {
                $('tr').not(this).removeClass('oneselected');
                $(this).toggleClass('oneselected');
                var tab = $(".tab-content").find('.active').data("id");
                var dataPost = { id: $(this).find("input[type='checkbox']").val() };
                switch (tab) {
                    case 'thongtinchitiet':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoThongTinChiTiet',
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
                            url: '/PartnerTabInfo/InfoLichHen',
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
                            url: '/PartnerTabInfo/InfoTourTuyen',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#tourtuyen").html(data);
                            }
                        });
                        break;
                    case 'lichsulienhe':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoLichSuLienHe',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#lichsulienhe").html(data);
                            }
                        });
                        break;
                    case 'danhgia':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoDanhGia',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#danhgia").html(data);
                            }
                        });
                        break;
                    case 'dichvucungcap':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoDichVuCungCap',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#dichvucungcap").html(data);
                            }
                        });
                        break;
                    case 'invoice':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoInvoice',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#invoice").html(data);
                            }
                        });
                        break;
                    case 'tailieumau':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoTaiLieuMau',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#tailieumau").html(data);
                            }
                        });
                        break;
                    case 'ghichu':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoGhiChu',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#ghichu").html(data);
                            }
                        });
                        break;
                }
            });
        }
    });
});

///*** filter service ***/
function clickChangeService() {
    var dataPost = {
        id: $("#partner-services").val()
    };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/FilterService',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-partner").html(data);
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
                            { type: "text" }]
            });

            $("table#tableDictionary").delegate("tr", "click", function (e) {
                $('tr').not(this).removeClass('oneselected');
                $(this).toggleClass('oneselected');
                var tab = $(".tab-content").find('.active').data("id");
                var dataPost = { id: $(this).find("input[type='checkbox']").val() };
                switch (tab) {
                    case 'thongtinchitiet':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoThongTinChiTiet',
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
                            url: '/PartnerTabInfo/InfoLichHen',
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
                            url: '/PartnerTabInfo/InfoTourTuyen',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#tourtuyen").html(data);
                            }
                        });
                        break;
                    case 'lichsulienhe':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoLichSuLienHe',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#lichsulienhe").html(data);
                            }
                        });
                        break;
                    case 'danhgia':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoDanhGia',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#danhgia").html(data);
                            }
                        });
                        break;
                    case 'dichvucungcap':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoDichVuCungCap',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#dichvucungcap").html(data);
                            }
                        });
                        break;
                    case 'invoice':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoInvoice',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#invoice").html(data);
                            }
                        });
                        break;
                    case 'tailieumau':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoTaiLieuMau',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#tailieumau").html(data);
                            }
                        });
                        break;
                    case 'ghichu':
                        $.ajax({
                            type: "POST",
                            url: '/PartnerTabInfo/InfoGhiChu',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#ghichu").html(data);
                            }
                        });
                        break;
                }
            });
        }
    });
}

/** xóa ghi chú **/
function deleteNote(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/DeleteNote',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#ghichu").html(data);
        }
    });
}

/** cập nhật ghi chú **/
function updateNote(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/PartnerManage/EditInfoNote',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-note").html(data);
            CKEDITOR.replace("edit-note-partner");
            $("#modal-edit-note").modal("show");
        }
    });
}
