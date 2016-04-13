
function OnSuccessAppointment() {
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");

    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");
}

function OnFailureAppointment() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");

    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");
}

/** xóa lịch hẹn **/
function deleteAppointment(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ContractOtherTab/DeleteAppointment',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#lichhen").html(data);
        }
    });
}

/** cập nhật lịch hẹn **/
function updateAppointment(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ContractOtherTab/EditAppointment',
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
            CKEDITOR.replace("edit-note-lichhen");
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
                $.getJSON('/ContractOtherTab/LoadPartner/' + $('#edit-service-lichhen').val(), function (data) {
                    var items = '<option value=' + 0 + '>-- Chọn đối tác --</option>';
                    $.each(data, function (i, ward) {
                        items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                    });
                    $('#edit-partner-lichhen').html(items);
                });
            });

            $("#modal-edit-appointment").modal("show");
        }
    });
}

/** xóa lịch sử liên hệ **/
function deleteContactHistory(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ContractOtherTab/DeleteContactHistory',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#lichsulienhe").html(data);
        }
    });
}

/** cập nhật lịch sử liên hệ **/
function updateContactHistory(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ContractOtherTab/EditContactHistory',
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