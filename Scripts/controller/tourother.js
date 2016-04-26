CKEDITOR.replace("insert-request-tour");
CKEDITOR.replace("insert-note-lichhen");
CKEDITOR.replace("insert-note-lienhe");
CKEDITOR.replace("insert-document-note");
CKEDITOR.replace("insert-program-note");
CKEDITOR.replace("insert-schedule-tour");
CKEDITOR.replace("insert-note-tourtask");
CKEDITOR.replace("insert-note-contracttour");
CKEDITOR.replace("insert-note-reviewtour");
CKEDITOR.replace("insert-note-liabilitycustomer");

$("#insert-service-tour").select2();
$("#insert-partner-tour").select2();
$("#insert-servicepartner-tour").select2();
$("#insert-method-congno1").select2();
$("#insert-partner-congno1").select2();

$('#insert-service-tour').change(function () {
    $.getJSON('/TourOtherTab/LoadPartner/' + $('#insert-service-tour').val(), function (data) {
        var items = '<option>-- Chọn đối tác --</option>';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#insert-partner-tour').html(items);
    });
});

$('#insert-partner-tour').change(function () {
    $.getJSON('/TourOtherTab/LoadServicePartner/' + $('#insert-partner-tour').val(), function (data) {
        var items = '<option>-- Chọn dịch vụ của đối tác --</option>';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#insert-servicepartner-tour').html(items);
    });
});

/** thêm mới tài liệu **/
function btnCreateFile() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-document-type").select2();
                $("#insert-tag-document").select2();
                $("#modal-insert-document").modal("show");
            }
        });
    }
}

/** thêm mới chương trình **/
function btnCreateProgram() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-tag-program").select2();
                $("#modal-insert-program").modal("show");
            }
        });
    }
}

/** thêm mới lịch hẹn **/
function btnAddLichHen() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-tour-lichhen").select2();
                $("#insert-program-lichhen").select2();
                $("#insert-task-lichhen").select2();
                $("#insert-status-lichhen").select2();
                $("#insert-service-lichhen").select2();
                $("#insert-partner-lichhen").select2();
                $("#insert-type-lichhen").select2();
                $("#insert-partner-lichhen").select2();
                //$("#insert-ngayhen-lichhen").datepicker();
                ///
                $("#insert-check-notify").click(function () {
                    if (this.checked) {
                        $("#insert-nhactruoc-lichhen").removeAttr("disabled", "disabled");
                        $("#insert-nhactruoc-lichhen").select2();
                    }
                    else {
                        $("#insert-nhactruoc-lichhen").attr("disabled", "disabled");
                    }
                });

                $("#insert-check-repeat").click(function () {
                    if (this.checked) {
                        $("#insert-laplai-lichhen").removeAttr("disabled", "disabled");
                        $("#insert-laplai-lichhen").select2();
                    }
                    else {
                        $("#insert-laplai-lichhen").attr("disabled", "disabled");
                    }
                });

                $("#modal-insert-appointment").modal("show");
            }
        });
    }
}

/** thêm mới lịch sử liên hệ **/
function btnAddLichSuLienHe() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-type-lienhe").select2();
                //$("#insert-ngay-lienhe").datepicker();
                $("#modal-insert-contacthistory").modal("show");
            }
        });
    }
}

$('#FileNameProgram').change(function () {
    var data = new FormData();
    data.append('FileName', $('#FileNameProgram')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'TourOtherTab/UploadProgram',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

$('#FileName').change(function () {
    var data = new FormData();
    data.append('FileName', $('#FileName')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'TourOtherTab/UploadFile',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/** cập nhật lịch hẹn **/
function updateAppointment(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditAppointment',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-appoinment").html(data);
            $("#edit-tour-lichhen").select2();
            $("#edit-program-lichhen").select2();
            $("#edit-task-lichhen").select2();
            $("#edit-status-lichhen").select2();
            $("#edit-service-lichhen").select2();
            $("#edit-partner-lichhen").select2();
            $("#edit-type-lichhen").select2();
            $("#edit-partner-lichhen").select2();
            //$("#edit-ngayhen-lichhen").datepicker();
            $("#edit-check-notify").click(function () {
                if (this.checked) {
                    $("#edit-nhactruoc-lichhen").removeAttr("disabled", "disabled");
                    $("#edit-nhactruoc-lichhen").select2();
                }
                else {
                    $("#edit-nhactruoc-lichhen").attr("disabled", "disabled");
                }
            });

            $("#edit-check-repeat").click(function () {
                if (this.checked) {
                    $("#edit-laplai-lichhen").removeAttr("disabled", "disabled");
                    $("#edit-laplai-lichhen").select2();
                }
                else {
                    $("#edit-laplai-lichhen").attr("disabled", "disabled");
                }
            });

            $('#edit-service-lichhen').change(function () {
                $.getJSON('/TourOtherTab/LoadPartner/' + $('#edit-service-lichhen').val(), function (data) {
                    var items = '<option value=' + 0 + '>-- Chọn đối tác --</option>';
                    $.each(data, function (i, ward) {
                        items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                    });
                    $('#edit-partner-lichhen').html(items);
                });
            });

            CKEDITOR.replace("edit-note-lichhen");
            $("#modal-edit-appointment").modal("show");
        }
    });
}

/** cập nhật lịch sử liên hệ **/
function updateContactHistory(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditContactHistory',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-contacthistory").html(data);
            //$("#edit-ngay-lienhe").datepicker();
            $("#edit-type-lienhe").select2();
            CKEDITOR.replace("edit-note-lienhe");
            $("#modal-edit-contacthistory").modal("show");
        }
    });
}

/** cập nhật nhiệm vụ **/
function updateTask(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditTask',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-task").html(data);
            $("#edit-task-type").select2();
            //$("#edit-department-tasktour").select2();
            $("#edit-priority-task").select2();
            CKEDITOR.replace("edit-note-tourtask");
            $("#modal-edit-tourtask").modal("show");

            $('#edit-department-tasktour').change(function () {
                $.getJSON('/TourManage/LoadPermission/' + $('#edit-department-tasktour').val(), function (data) {
                    var items = '<option>-- Chọn nhân viên thực hiện --</option>';
                    $.each(data, function (i, ward) {
                        items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                    });
                    $('#edit-staff-tasktour').html(items);
                });
            });
        }
    });
}

/** xóa tài liệu **/
function deleteDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteDocument',
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
        url: '/TourOtherTab/EditInfoDocument',
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
                    url: 'TourOtherTab/UploadFile',
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

function OnSuccessTourTab() {
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");

    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");

    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");

    $("#modal-insert-program").modal("hide");
    $("#modal-edit-program").modal("hide");

    $("#modal-insert-contract").modal("hide");
    $("#modal-edit-contract").modal("hide");

    $("#modal-insert-mark").modal("hide");
    $("#modal-edit-mark").modal("hide");

    $("#modal-insert-congnokh").modal("hide");
    $("#modal-edit-congnokh").modal("hide");

    $("#modal-insert-congnodt").modal("hide");
    $("#modal-edit-congnodt").modal("hide");
}

function OnFailureTourTab() {
    alert("Lỗi, vui lòng kiểm tra lại!");

    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");

    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");

    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");

    $("#modal-insert-program").modal("hide");
    $("#modal-edit-program").modal("hide");

    $("#modal-insert-contract").modal("hide");
    $("#modal-edit-contract").modal("hide");

    $("#modal-insert-mark").modal("hide");
    $("#modal-edit-mark").modal("hide");

    $("#modal-insert-congnokh").modal("hide");
    $("#modal-edit-congnokh").modal("hide");

    $("#modal-insert-congnodt").modal("hide");
    $("#modal-edit-congnodt").modal("hide");
}

/** xóa lịch hẹn **/
function deleteAppointment(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteAppointment',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#lichhen").html(data);
        }
    });
}

/** xóa lịch sử liên hệ **/
function deleteContactHistory(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteContactHistory',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#lichsulienhe").html(data);
        }
    });
}

/** xóa nhiệm vụ **/
function deleteTask(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteTask',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#nhiemvu").html(data);
        }
    });
}

/** xóa chương trình **/
function deleteProgram(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteProgram',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#chuongtrinh").html(data);
        }
    });
}

/** cập nhật chương trình **/
function updateProgram(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditInfoProgram',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-program").html(data);
            $("#edit-tag-program").select2();
            $("#edit-program-type").select2();
            CKEDITOR.replace("edit-program-note");
            $("#modal-edit-program").modal("show");
            /**** update in tab file của khách hàng ****/
            $("#btnUpdateProgram").click(function () {
                var $this = $(this);
                var $form = $("#frmUpdateProgram");
                var $parent = $form.parent();
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize()
                };

                $.ajax(options).done(function (data) {
                    $("#modal-edit-program").modal("hide");
                    alert("Lưu dữ liệu thành công!");
                    $("#chuongtrinh").html(data);
                });
                return false;
            });

            /** upload file **/
            $("#edit-program").change(function () {
                var data = new FormData();
                data.append('FileName', $('#edit-program')[0].files[0]);
                var ajaxRequest = $.ajax({
                    type: "POST",
                    url: 'TourOtherTab/UploadProgram',
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

/** thêm mới hợp đồng **/
function btnCreateContract() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-status-contracttour").select2();
                $("#insert-permission-contracttour").select2();
                $("#insert-currency-contracttour").select2();
                $("#modal-insert-contract").modal("show");
            }
        });
    }
}

/** cập nhật hợp đồng **/
function updateContract(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditContract',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-contract").html(data);
            $("#edit-status-contracttour").select2();
            $("#edit-currency-contracttour").select2();
            $("#edit-permission-contracttour").select2();
            CKEDITOR.replace("edit-note-contracttour");
            $("#modal-edit-contract").modal("show");
        }
    });
}

/** xóa hợp đồng **/
function deleteContract(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteContract',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#hopdong").html(data);
        }
    });
}

/** thêm mới đánh giá **/
function btnCreateReview() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-tour").select2();
                $("#insert-customer").select2();
                $("#insert-service1").select2();
                $("#modal-insert-mark").modal("show");

                $('#btnAddR').click(function () {
                    var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                        newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                    // manipulate the name/id values of the input inside the new element

                    newElem.find('.service').attr('id', 'insert-service' + newNum).attr('name', 'DictionaryId' + newNum);
                    newElem.find('.mark').attr('name', 'Mark' + newNum).val('');

                    // insert the new element after the last "duplicatable" input field
                    $('#entry' + num).after(newElem);
                    $("#countService").val(newNum);
                    $("#insert-service" + newNum).select2();

                    for (var i = 1; i < newNum; i++) {
                        $("#entry" + newNum + " #select2-insert-service" + i + "-container").parent().parent().parent().remove();
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
                        $('#btnAddR').attr('disabled', false).prop('value', "add section");
                    });
                    return false;

                    $('#btnAddR').attr('disabled', false);
                });
                $('#btnDel').attr('disabled', true);
            }
        });
    }
}

/** thêm mới công nợ khách hàng **/
function btnCreateLiabilityCustomer() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#modal-insert-congnokh").modal("show");
            }
        });
    }
}

/** cập nhật công nợ khách hàng **/
function updateLiabilityCustomer(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditLiabilityCustomer',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-liabilitycustomer").html(data);
            CKEDITOR.replace("edit-note-liabilitycustomer");
            $("#modal-edit-congnokh").modal("show");
        }
    });
}

/** xóa công nợ khách hàng **/
function deleteLiabilityCustomer(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteLiabilityCustomer',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#congnokh").html(data);
        }
    });
}

/** thêm mới công nợ đối tác **/
function btnCreateLiabilityPartner() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/TourManage/GetIdTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                CKEDITOR.replace("insert-note-congnodt1");
                $("#modal-insert-congnodt").modal("show");
                /*** duplicate thêm công nợ đối tác ***/
                $(function () {
                    $('#btnAddCongNo').click(function () {
                        var num = $('.clonedInputCongNo').length, // how many "duplicatable" input fields we currently have
                            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                            newElem = $('#entryCongNo' + num).clone().attr('id', 'entryCongNo' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                        // manipulate the name/id values of the input inside the new element
                        newElem.find('.congnocurrencyfirst').attr('id', 'insert-currencyfirst-congno' + newNum).attr('name', 'FirstCurrencyType' + newNum);
                        newElem.find('.congnopartner').attr('id', 'insert-partner-congno' + newNum).attr('name', 'PartnerId' + newNum);
                        newElem.find('.congnofirst').attr('name', 'FirstPayment' + newNum).val('');
                        newElem.find('.congnomethod').attr('id', 'insert-method-congno' + newNum).attr('name', 'PaymentMethod' + newNum);
                        newElem.find('.congnosecond').attr('name', 'SecondPayment' + newNum).val('');
                        newElem.find('.congnoprice').attr('name', 'ServicePrice' + newNum).val('');
                        newElem.find('.congnoremaining').attr('name', 'TotalRemaining' + newNum).val('');
                        newElem.find('.congnonote').attr('id', 'insert-note-congnodt' + newNum).attr('name', 'Note' + newNum).val('');
                        newElem.find('.collapsedt').attr('data-target', '#demo-congnodt' + newNum);
                        newElem.find('.optioncongno').attr('id', 'demo-congnodt' + newNum);
                        newElem.find('.titleoption').text('OPTION ' + newNum);

                        // insert the new element after the last "duplicatable" input field
                        $('#entryCongNo' + num).after(newElem);
                        $("#insert-partner-congno" + newNum).select2();
                        $("#insert-method-congno" + newNum).select2();
                        $("#insert-currencyfirst-congno" + newNum).select2();
                        CKEDITOR.replace("insert-note-congnodt" + newNum);
                        $("#countOptionCongNo").val(newNum);

                        for (var i = 1; i < newNum; i++) {
                            $("#entryCongNo" + newNum + " #select2-insert-currencyfirst-congno" + i + "-container").parent().parent().parent().remove();
                            $("#entryCongNo" + newNum + " #select2-insert-partner-congno" + i + "-container").parent().parent().parent().remove();
                            $("#entryCongNo" + newNum + " #select2-insert-method-congno" + i + "-container").parent().parent().parent().remove();
                            $("#entryCongNo" + newNum).find("#cke_insert-note-congnodt" + i).remove();
                        }

                        // enable the "remove" button
                        $('#btnDelCongNo').attr('disabled', false);

                    });

                    $('#btnDelCongNo').click(function () {
                        // confirmation
                        var num = $('.clonedInputCongNo').length;
                        // how many "duplicatable" input fields we currently have
                        $('#entryCongNo' + num).slideUp('slow', function () {
                            $(this).remove();
                            // if only one element remains, disable the "remove" button
                            if (num - 1 === 1)
                                $('#btnDelCongNo').attr('disabled', true);
                            // enable the "add" button
                            $('#btnAddCongNo').attr('disabled', false).prop('value', "add section");
                        });
                        return false;

                        $('#btnAddCongNo').attr('disabled', false);
                    });
                    $('#btnDelCongNo').attr('disabled', true);
                });
            }
        });
    }
}

/** cập nhật công nợ đối tác **/
function updateLiabilityPartner(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditLiabilityPartner',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-liabilitypartner").html(data);
            $("#edit-partner-congno").select2();
            $("#edit-method-congno").select2();
            $("#edit-currencyfirst-congno").select2();
            CKEDITOR.replace("edit-note-congnodt");
            $("#modal-edit-congnodt").modal("show");
        }
    });
}

/** xóa công nợ đối tác **/
function deleteLiabilityPartner(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteLiabilityPartner',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#congnodoitac").html(data);
        }
    });
}