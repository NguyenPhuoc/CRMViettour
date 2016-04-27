$("#insert-company-event1").select2();
CKEDITOR.replace("insert-note-event1");

/***** popup insert sự kiện *****/
function btnCreateEventService() {
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
                    $('#btnAddEventService').click(function () {
                        var num = $('.clonedInputEventService').length, // how many "duplicatable" input fields we currently have
                            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                            newElem = $('#entryEventService' + num).clone().attr('id', 'entryEventService' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                        // manipulate the name/id values of the input inside the new element
                        newElem.find('.eventcongty').attr('id', 'insert-company-event' + newNum).attr('name', 'CompanyId' + newNum);
                        newElem.find('.eventtailieu').attr('name', 'FileName' + newNum);
                        newElem.find('.eventcode').attr('name', 'Code' + newNum);
                        newElem.find('.eventgiatri').attr('name', 'Price' + newNum);
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
}