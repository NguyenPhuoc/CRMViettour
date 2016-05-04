function OnSuccessAppointment() {
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");
    $('#calendar').fullCalendar('removeEvents');
    $('#calendar').fullCalendar('refetchEvents');
}

function OnFailureAppointment() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");
}

CKEDITOR.replace("insert-note-lichhen");

$("#trangthai-lichhen").select2();
$("#loai-lichhen").select2();


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
                { type: "text" }]
});


/*** Them lich hen ***/
//function btnCreateLichHen() {

$("#insert-tour-lichhen").select2();
$("#insert-program-lichhen").select2();
$("#insert-task-lichhen").select2();
$("#insert-status-lichhen").select2();
$("#insert-service-lichhen").select2();
$("#insert-partner-lichhen").select2();
$("#insert-type-lichhen").select2();
$("#insert-partner-lichhen").select2();

$("#insert-service-customer").select2();
$("#insert-service-tour").select2();
$("#insert-service-conteact").select2();

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

$('#insert-service-lichhen').change(function () {
    $.getJSON('/CustomerOtherTab/LoadPartner/' + $('#insert-service-lichhen').val(), function (data) {
        var items = '<option value=' + 0 + '>-- Chọn đối tác --</option>';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#insert-partner-lichhen').html(items);
    });
});


//}


$("#insert-check-repeat").click(function () {
    if (this.checked) {
        $("#insert-laplai-lichhen").removeAttr("disabled", "disabled");
        $("#insert-laplai-lichhen").select2();
    }
    else {
        $("#insert-laplai-lichhen").attr("disabled", "disabled");
    }
});

$('#insert-service-lichhen').change(function () {
    $.getJSON('/CustomerOtherTab/LoadPartner/' + $('#insert-service-lichhen').val(), function (data) {
        var items = '<option value=' + 0 + '>-- Chọn đối tác --</option>';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#insert-partner-lichhen').html(items);
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
/** cập nhật lịch hẹn **/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/AppointmentManage/EditAppointment',
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

            $("#edit-service-customer").select2();
            $("#edit-service-tour").select2();
            $("#edit-service-conteact").select2();

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
                $.getJSON('/CustomerOtherTab/LoadPartner/' + $('#edit-service-lichhen').val(), function (data) {
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

$("#tabdangluoi").click(function () {
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
})

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


$(".FilterAppoi").change(function () {
    var tu = $("#tungay-lichhen").val()
    var den = $("#denngay-lichhen").val()
    var status = $("#trangthai-lichhen").val()
    var type = $("#loai-lichhen").val()

    var dataPost = {
        start: tu,
        end: den,
        statusId: status,
        typeId: type
    };
    $.ajax({
        type: "POST",
        url: '/AppointmentManage/FilterStatusTypeDate',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#dangluoi").html(data);
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
                            { type: "text" }]
            });
        }
    })
})


$(document).ready(function () {
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        lang: 'vi',
        defaultDate: new Date(),
        businessHours: false, // display business hours
        editable: true,
        defaultView: 'month',
        dayClick: function (date, jsEvent, view) {
            $(".fc-highlight").removeClass("fc-highlight")
            $(jsEvent.toElement).addClass("fc-highlight")
        },
        dayRender: function (date, element, view) {
            element.bind('dblclick', function () {
                var d = moment(date).format("YYYY-MM-DDTHH:mm");
                $("#insert-ngayhen-lichhen").val(d);
                $("#modal-insert-appointment").modal("show");
            });
        },
        events: "/AppointmentManage/JsonCalendar",
        eventRender: function (event, element) {
            element.bind('dblclick', function () {
                var dataPost = {
                    id: event.id
                };
                $.ajax({
                    type: "POST",
                    url: '/AppointmentManage/EditAppointment',
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

                        $("#edit-service-customer").select2();
                        $("#edit-service-tour").select2();
                        $("#edit-service-conteact").select2();

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
                            $.getJSON('/CustomerOtherTab/LoadPartner/' + $('#edit-service-lichhen').val(), function (data) {
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
            })
        },
    });

});

