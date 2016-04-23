CKEDITOR.replace("insert-note-tour");
CKEDITOR.replace("insert-note-lichhen");
CKEDITOR.replace("insert-note-lienhe");
CKEDITOR.replace("insert-document-note");
CKEDITOR.replace("insert-schedule-tour");
CKEDITOR.replace("insert-note-tourtask");

$("#insert-service-tour").select2();
$("#insert-partner-tour").select2();
$("#insert-servicepartner-tour").select2();

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
        alert("Vui lòng chọn 1 khách hàng!");
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

/** thêm mới lịch hẹn **/
function btnAddLichHen() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
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
        alert("Vui lòng chọn 1 khách hàng!");
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
                    url: 'TourManage/UploadFile',
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
}

function OnFailureTourTab() {
    alert("Lỗi, vui lòng kiểm tra lại!");

    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");

    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");

    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
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