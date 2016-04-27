function OnSuccessScheduleTour() {
    $("#modal-insert-tourschedule").modal("hide");
    $("#modal-edit-tourschedule").modal("hide");

    var tour = $("#list-tour").val();
    var dataPost = {
        id: tour
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
        }
    })
}

function OnFailureScheduleTour() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-tourschedule").modal("hide");
    $("#modal-edit-tourschedule").modal("hide");
}

CKEDITOR.replace("insert-schedule-tour");

$("#list-tour").select2();
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




$(".FilterAppoi").change(function () {
    var tour = $("#list-tour").val();

    var dataPost = {
        id: tour
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
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
            var tour = $("#list-tour").val();
            if (tour != -1) {
                var d = moment(date).format("YYYY-MM-DD");
                $("#date-insert-tourschedule").val(d);
                $("#modal-insert-tourschedule").modal("show");
            } else alert('Vui lòng chọn tour!');
        },
        events: [],
        eventClick: function (event) {
            //edit
            var dataPost = {
                id: event.id
            };
            $.ajax({
                type: "POST",
                url: '/TourSchedule/EditScheduleTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#info-data-tourschedule").html(data);
                    $("#edit-service-tour").select2();
                    $("#edit-partner-tour").select2();
                    $("#edit-servicepartner-tour").select2();

                    $('#edit-service-tour').change(function () {
                        $.getJSON('/TourOtherTab/LoadPartner/' + $('#edit-service-tour').val(), function (data) {
                            var items = '<option>-- Chọn đối tác --</option>';
                            $.each(data, function (i, ward) {
                                items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                            });
                            $('#edit-partner-tour').html(items);
                        });
                    });

                    $('#edit-partner-tour').change(function () {
                        $.getJSON('/TourOtherTab/LoadServicePartner/' + $('#edit-partner-tour').val(), function (data) {
                            var items = '<option>-- Chọn dịch vụ của đối tác --</option>';
                            $.each(data, function (i, ward) {
                                items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                            });
                            $('#edit-servicepartner-tour').html(items);
                        });
                    });

                    CKEDITOR.replace("edit-schedule-tour");
                    $("#modal-edit-tourschedule").modal("show");
                }
            });//end edit
        }
    });




});

//$("#btnTourSchedule").on("click", function () {
//    var $this = $(this);
//    var $form = $("#frmTourSchedule");
//    var $parent = $form.parent();
//    var options = {
//        url: $form.attr("action"),
//        type: $form.attr("method"),
//        data: $form.serialize()
//    };

//    $.ajax(options).done(function (data) {
//        $('#calendar').fullCalendar('removeEvents');
//        $('#calendar').fullCalendar('addEventSource', data);
//        $("#modal-insert-tourschedule").modal("hide");

//    });
//    return false;
//});

