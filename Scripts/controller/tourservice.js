$("#insert-company-event1").select2();
CKEDITOR.replace("insert-note-event1");

function OnFailureTourService() {
    alert('Lỗi!');
    $("#modal-insert-event").modal("hide");
}

function OnSuccessTourService() {
    alert('Đã lưu!');
    $("#modal-insert-event").modal("hide");
}

$(document).ready(function () {
    $('input:radio[name=selectServiceTour]').change(function () {
        var id = $(this).val();
        switch (id) {
            case "1":
                $("#modal-insert-services-tour").modal("hide");
                $("#modal-insert-restaurant").modal("show");
                break;
            case "2":
                $("#modal-insert-services-tour").modal("hide");
                $("#modal-insert-hotel").modal("show");
                break;
            case "3":
                $("#modal-insert-services-tour").modal("hide");
                $("#modal-insert-transport").modal("show");
                break;
            case "4":
                $("#modal-insert-services-tour").modal("hide");
                $("#modal-insert-plane").modal("show");
                break;
            case "5":
                /***** popup insert sự kiện *****/
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
                            $("#modal-insert-services-tour").modal("hide");
                            $("#modal-insert-event").modal("show");
                            /*** upload file event đầu tiên ***/
                            $('#insert-file-event1').change(function () {
                                var data = new FormData();
                                data.append('FileName', $('#insert-file-event1')[0].files[0]);
                                data.append('id', 1);

                                var ajaxRequest = $.ajax({
                                    type: "POST",
                                    url: 'TourService/UploadFileEvent',
                                    contentType: false,
                                    processData: false,
                                    data: data
                                });

                                ajaxRequest.done(function (xhr, textStatus) {
                                    // Onsuccess
                                });
                            });
                            /*** duplicate thêm công nợ đối tác ***/
                            $(function () {
                                $('#btnAddEventService').click(function () {
                                    var num = $('.clonedInputEventService').length, // how many "duplicatable" input fields we currently have
                                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                                        newElem = $('#entryEventService' + num).clone().attr('id', 'entryEventService' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                                    // manipulate the name/id values of the input inside the new element
                                    newElem.find('.eventcongty').attr('id', 'insert-company-event' + newNum).attr('name', 'CompanyId' + newNum);
                                    newElem.find('.eventtailieu').attr('id', 'insert-file-event' + newNum);
                                    newElem.find('.eventcode').attr('id', 'insert-code-event' + newNum).attr('name', 'Code' + newNum).val(new Number(new Number($("#insert-code-event" + num).val()) + 1));
                                    newElem.find('.eventgiatri').attr('name', 'Price' + newNum).val('');
                                    newElem.find('.eventlienhe').attr('name', 'StaffContact' + newNum).val('');
                                    newElem.find('.eventcollapse').attr('data-target', '#demo-event' + newNum).val('');
                                    newElem.find('.eventoption').attr('id', 'demo-event' + newNum).val('');
                                    newElem.find('.eventdienthoai').attr('name', 'Phone' + newNum).val('');
                                    newElem.find('.eventghichu').attr('id', 'insert-note-event' + newNum).attr('name', 'Note' + newNum).val('');
                                    newElem.find('.eventtitle').text('OPTION ' + newNum);

                                    // insert the new element after the last "duplicatable" input field
                                    $('#entryEventService' + num).after(newElem);
                                    $("#insert-company-event" + newNum).select2();
                                    CKEDITOR.replace("insert-note-event" + newNum);
                                    $("#countOptionEventService").val(newNum);

                                    $('#insert-file-event' + newNum).change(function () {
                                        var data = new FormData();
                                        data.append('FileName', $('#insert-file-event' + newNum)[0].files[0]);
                                        data.append('id', newNum);

                                        var ajaxRequest = $.ajax({
                                            type: "POST",
                                            url: 'TourService/UploadFileEvent',
                                            contentType: false,
                                            processData: false,
                                            data: data
                                        });

                                        ajaxRequest.done(function (xhr, textStatus) {
                                            // Onsuccess
                                        });
                                    });

                                    for (var i = 1; i < newNum; i++) {
                                        $("#entryEventService" + newNum + " #select2-insert-company-event" + i + "-container").parent().parent().parent().remove();
                                        $("#entryEventService" + newNum).find("#cke_insert-note-event" + i).remove();
                                    }

                                    // enable the "remove" button
                                    $('#btnDelEventService').attr('disabled', false);

                                });

                                $('#btnDelEventService').click(function () {
                                    // confirmation
                                    var num = $('.clonedInputEventService').length;
                                    // how many "duplicatable" input fields we currently have
                                    $('#entryEventService' + num).slideUp('slow', function () {
                                        $(this).remove();
                                        // if only one element remains, disable the "remove" button
                                        if (num - 1 === 1)
                                            $('#btnDelEventService').attr('disabled', true);
                                        // enable the "add" button
                                        $('#btnAddEventService').attr('disabled', false).prop('value', "add section");
                                    });
                                    return false;

                                    $('#btnAddEventService').attr('disabled', false);
                                });
                                $('#btnDelEventService').attr('disabled', true);
                            });
                        }
                    });
                }

                break;
        }
    });
})

/***** xóa sự kiện *****/
function deleteService(tourid, optionid) {
    var dataPost = { tourid: tourid, optionid: optionid };
    $.ajax({
        type: "POST",
        url: '/TourService/DeleteService',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Xóa dữ liệu thành công!!!");
            $("#dichvu").html(data);
        },
        error: function (data) {
            alert("Không xóa được!!!");
            $("#dichvu").html(data);
        }
    });
}