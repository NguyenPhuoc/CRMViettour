CKEDITOR.replace("insert-note-company");
CKEDITOR.replace("insert-note-personal");
CKEDITOR.replace("insert-note-contact");

/*** visa-passport ***/
$("#country-insert-cmnd").select2();
$("#country-insert-passport").select2();

/*** doanh nghiệp ***/
$("#insert-address-company").select2();
$("#customer-nhomkh-company").select2();
$("#customer-nguonden-company").select2();
$("#edit-customer-company").select2();

/*** cá nhân ***/
$("#insert-address-personal").select2();
$("#customer-nghenghiep-personal").select2();
$("#customer-nguonden-personal").select2();
$("#customer-nhomkh-personal").select2();
$("#customer-quydanh").select2();

/*** người liên hệ ***/
$("#customer-contact").select2();
$("#customer-quydanh-contact").select2();
$("#insert-address-contact").select2();
$("#country-insert-profilevisa").select2();

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

///*** duplicate form visa ***/
$(function () {
    $('#btnAdd').click(function () {
        var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.visacard').attr('name', 'VisaNumber' + newNum).val('');
        newElem.find('.ngaycapvisa').attr('id', 'ngaycapvisa' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
        newElem.find('.ngayhethanvisa').attr('id', 'ngayhethanvisa' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
        newElem.find('.countryvisa').attr('id', 'countryvisa' + newNum).attr('name', 'TagsId' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        //$("#ngaycapvisa" + newNum).datepicker();
        //$("#ngayhethanvisa" + newNum).datepicker();
        $("#countryvisa" + newNum).select2();

        for (var i = 1; i < newNum; i++) {
            $("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
        }

        // enable the "remove" button
        $('#btnDel').attr('disabled', false);

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
    });
    $('#btnDel').attr('disabled', true);
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

/** upload file **/
//$('#FileName').change(function () {
//    var data = new FormData();
//    data.append('FileName', $('#FileName')[0].files[0]);
//    alert('aaaaaaaaaaaaaaaaaaa');
//    var ajaxRequest = $.ajax({
//        type: "POST",
//        url: 'CustomersManage/UploadFile',
//        contentType: false,
//        processData: false,
//        data: data
//    });

//    ajaxRequest.done(function (xhr, textStatus) {
//        // Onsuccess
//    });
//});

/** xóa tài liệu **/
function deleteDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/DeleteDocument',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#hosolienquan").html(data);
        }
    });
}

/** cập nhật tài liệu **/
function updateDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/EditInfoDocument',
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
                    url: 'CustomersManage/UploadFile',
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

/**** insert in tab visa của khách hàng ****/
$("#btnInsertVisa").click(function () {
    var $this = $(this);
    var $form = $("#frmInsertVisa");
    var $parent = $form.parent();
    var options = {
        url: $form.attr("action"),
        type: $form.attr("method"),
        data: $form.serialize()
    };

    $.ajax(options).done(function (data) {
        $("#modal-insert-visa").modal("hide");
        alert("Lưu dữ liệu thành công!");
        $("#visa").html(data);
    });
    return false;
})

/** xóa visa **/
function deleteVisa(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/DeleteVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#visa").html(data);
        }
    });
}

/** cập nhật visa **/
function updateVisa(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/EditInfoVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-visa").html(data);
            $("#edit-country-visa").select2();
            $("#edit-type-visa").select2();
            $("#edit-status-visa").select2();
            //$("#edit-createdate-visa").datepicker();
            //$("#edit-expiredate-visa").datepicker();
            $("#modal-edit-visa").modal("show");

            /**** update in tab visa của khách hàng ****/
            $("#btnUpdateVisa").click(function () {
                var $this = $(this);
                var $form = $("#frmUpdateVisa");
                var $parent = $form.parent();
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize()
                };

                $.ajax(options).done(function (data) {
                    $("#modal-edit-visa").modal("hide");
                    alert("Lưu dữ liệu thành công!");
                    $("#visa").html(data);
                });
                return false;
            })
        }
    });
}

///****** Sửa thông tin ******/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/CustomersManage/CustomerInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-customer").html(data);


            /*** visa-passport ***/
            $("#country-edit-cmnd").select2();
            $("#country-edit-passport").select2();

            /*** doanh nghiệp ***/
            $("#edit-address-company").select2();
            $("#edit-nhomkh-company").select2();
            $("#edit-nguonden-company").select2();
            $("#edit-company").select2();
            $("#edit-customer-career").select2();

            /*** cá nhân ***/
            $("#edit-personal-quydanh").select2();
            $("#edit-address-personal").select2();
            $("#edit-nghenghiep-personal").select2();
            $("#edit-nhomkh-personal").select2();
            $("#edit-nguonden-personal").select2();

            /*** người liên hệ ***/
            $("#edit-customer-contact").select2();
            $("#edit-quydanh-contact").select2();
            $("#edit-address-contact").select2();

            /*** datepicker ***/
            //$("#edit-ngaycapmst").datepicker();
            //$("#edit-ngaytldn").datepicker();
            //$("#edit-ngaysinh").datepicker();
            //$("#edit-ngaysinh-contact").datepicker();
            //$("#edit-ngaycap").datepicker();
            //$("#edit-ngaycap-passport").datepicker();
            //$("#edit-ngayhethan-passport").datepicker();

            ///*** visa ***/
            //$("#ngaycapvisaE1").datepicker();
            //$("#ngayhethanvisaE1").datepicker();
            //$("#countryvisaE1").select2();

            ///*** modal ***/
            CKEDITOR.replace("edit-note-company");
            CKEDITOR.replace("edit-note-personal");
            CKEDITOR.replace("edit-note-contact");
            $("#modal-edit-customer").modal("show");

            ///*** duplicate form visa (edit) ***/
            $(function () {
                $('#btnAddE').click(function () {
                    var num = $('.clonedInputE').length, // how many "duplicatable" input fields we currently have
                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                        newElem = $('#entryE' + num).clone().attr('id', 'entryE' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                    // manipulate the name/id values of the input inside the new element

                    newElem.find('.visacardE').attr('name', 'VisaNumber' + newNum).val('');
                    newElem.find('.ngaycapvisaE').attr('id', 'ngaycapvisaE' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
                    newElem.find('.ngayhethanvisaE').attr('id', 'ngayhethanvisaE' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
                    newElem.find('.countryvisaE').attr('id', 'countryvisaE' + newNum).attr('name', 'TagsId' + newNum).val('');

                    // insert the new element after the last "duplicatable" input field
                    $('#entryE' + num).after(newElem);
                    //$("#ngaycapvisaE" + newNum).datepicker();
                    //$("#ngayhethanvisaE" + newNum).datepicker();
                    $("#countryvisaE" + newNum).select2();

                    for (var i = 1; i < newNum; i++) {
                        $("#entryE" + newNum + " #select2-countryvisaE" + i + "-container").parent().parent().parent().remove();
                    }

                    // enable the "remove" button
                    $('#btnDelE').attr('disabled', false);

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
                });
                $('#btnDelE').attr('disabled', true);
            });
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

$("#btnExport").click(function () {
    $("#exportForm").submit();
});
