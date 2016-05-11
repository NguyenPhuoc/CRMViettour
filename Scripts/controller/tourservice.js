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
                /***** popup insert nhà hàng *****/
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
                            $("#modal-insert-restaurant").modal("show");
                            /*** upload file restaurant đầu tiên ***/
                            $('#RestaurantDocument1').change(function () {
                                var data = new FormData();
                                data.append('RestaurantDocument', $('#RestaurantDocument1')[0].files[0]);
                                data.append('id', 1);

                                var ajaxRequest = $.ajax({
                                    type: "POST",
                                    url: 'TourService/UploadFileRestaurant',
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

                break;
            case "2":
                $("#modal-insert-services-tour").modal("hide");
                /***** popup insert khách sạn *****/
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
                            $("#modal-insert-hotel").modal("show");
                        }
                    });
                }
                break;
            case "3":
                $("#modal-insert-services-tour").modal("hide");
                /***** popup insert vận chuyển *****/
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
                            $("#modal-insert-transport").modal("show");
                            /******** upload file đầu tiên *******/
                            $('#RestaurantDocument1').change(function () {
                                var data = new FormData();
                                data.append('TransportDocument', $('#file-transport1')[0].files[0]);
                                data.append('id', 1);

                                var ajaxRequest = $.ajax({
                                    type: "POST",
                                    url: 'TourService/UploadFileTransport',
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
                break;
            case "4":
                $("#modal-insert-services-tour").modal("hide");
                /***** popup insert vé máy bay *****/
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
                            $("#modal-insert-plane").modal("show");
                            /*** upload file restaurant đầu tiên ***/
                            $('#file-deadline-plane11').change(function () {
                                var data = new FormData();
                                data.append('FileNamePlane', $('#file-deadline-plane11')[0].files[0]);
                                data.append('id', 1);

                                var ajaxRequest = $.ajax({
                                    type: "POST",
                                    url: 'TourService/UploadFilePlane',
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
                                data.append('FileNameEvent', $('#insert-file-event1')[0].files[0]);
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

///Event
$("#insert-currency-event1").select2();
$("#insert-company-event1").select2();
$("#deadline-status-event11").select2();
CKEDITOR.replace("insert-note-event1");
CKEDITOR.replace("deadline-note-event11");
/******** duplicate *******/
function addNewOptionEvent() {
    var num = $('.OptionEvent').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#OptionEvent' + num).clone().attr('id', 'OptionHotel' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element
    newElem.find('.OptionEventTitle').html('OPTION ' + newNum);

    newElem.find('.insert-company-event').attr('id', 'insert-company-event' + newNum).attr('name', 'insert-company-event' + newNum);
    newElem.find('.insert-file-event').attr('id', 'insert-file-event' + newNum).val('');
    newElem.find('.insert-code-event').attr('id', 'insert-code-event' + newNum).attr('name', 'insert-code-event' + newNum).val('');
    newElem.find('.insert-price-event').attr('id', 'insert-price-event' + newNum).attr('name', 'insert-price-event' + newNum).val('');
    newElem.find('.insert-currency-event').attr('id', 'insert-currency-event' + newNum).attr('name', 'insert-currency-event' + newNum);
    newElem.find('.insert-contact-event').attr('id', 'insert-contact-event' + newNum).attr('name', 'insert-contact-event' + newNum).val('');
    newElem.find('.insert-phone-event').attr('id', 'insert-phone-event' + newNum).attr('name', 'insert-phone-event' + newNum).val('');
    newElem.find('.insert-note-event').attr('id', 'insert-note-event' + newNum).attr('name', 'insert-note-event' + newNum).val('');
    newElem.find('.OptionEventA').attr('data-target', '#demo-event' + newNum);
    newElem.find('.OptionEventBody').attr('id', 'demo-event' + newNum);

    //deadline
    newElem.find('.DeadlineEvent').attr('id', 'DeadlineEvent' + newNum + 1);
    newElem.find('.btnNewDealineEvent').attr('onclick', 'addNewDeadlineEvent(' + newNum + ')');
    newElem.find('.btnRemoveDealineEvent').attr('onclick', 'removeDeadlineEvent(' + newNum + ',1)').attr('disabled', true);
    newElem.find('.DeadlineEventTitle').html('Deadline 1');

    newElem.find('.deadline-event-a').attr('data-target', '#deadline-event' + newNum + 1);
    newElem.find('.deadline-event-body').attr('id', 'deadline-event' + newNum + 1);

    newElem.find('.deadline-name-event').attr('id', 'deadline-name-event' + newNum + 1).attr('name', 'deadline-name-event' + newNum + 1);
    newElem.find('.deadline-total-event').attr('id', 'deadline-total-event' + newNum + 1).attr('name', 'deadline-total-event' + newNum + 1);
    newElem.find('.deadline-thoigian-event').attr('id', 'deadline-thoigian-event' + newNum + 1).attr('name', 'deadline-thoigian-event' + newNum + 1)
    newElem.find('.deadline-status-event').attr('id', 'deadline-status-event' + newNum + 1).attr('name', 'deadline-status-event' + newNum + 1);
    newElem.find('.deadline-note-event').attr('id', 'deadline-note-event' + newNum + 1).attr('name', 'deadline-note-event' + newNum + 1)

    var arr = newElem.find('.DeadlineEvent').toArray();
    newElem.find('.DeadlineEvent').each(function (index) {
        if (arr.length - 1 != index)
            this.remove();
    });

    $('#OptionEvent' + num).after(newElem);
    $('#countOptionEvent').val(newNum);
    newElem.find('.btnRemoveOptionEvent').attr('disabled', false);
    $('#OptionEvent' + num).find('.actionsOptionEvent').remove();

    /*** upload file ***/
    $('#insert-file-event' + newNum).change(function () {
        var data = new FormData();
        data.append('FileNameEvent', $('#insert-file-event' + newNum)[0].files[0]);
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

    newElem.find("#select2-insert-company-event" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-insert-currency-event" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-deadline-status-event" + num + arr.length + "-container").parent().parent().parent().remove();
    newElem.find("#cke_insert-note-event" + num).remove();
    newElem.find("#cke_deadline-note-event" + num + arr.length).remove();

    $("#insert-company-event" + newNum).select2();
    $("#insert-currency-event" + newNum).select2();
    CKEDITOR.replace("insert-note-event" + newNum);
    $('#deadline-status-event' + newNum + 1).select2();
    CKEDITOR.replace("deadline-note-event" + newNum + 1);
}
function addNewDeadlineEvent(i) {
    var num = $('#OptionEvent' + i + ' .DeadlineEvent').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#DeadlineEvent' + i + num).clone().attr('id', 'DeadlineEvent' + i + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    newElem.find('.DeadlineEventTitle').html('Deadline ' + newNum);

    $('#DeadlineEvent' + i + num).find('.actions').remove();

    newElem.find('.deadline-name-event').attr('name', 'deadline-name-event' + i + newNum).attr('id', 'deadline-name-event' + i + newNum).val('');
    newElem.find('.deadline-total-event').attr('name', 'deadline-total-event' + i + newNum).attr('id', 'deadline-total-event' + i + newNum).val('');
    newElem.find('.deadline-thoigian-event').attr('id', 'deadline-thoigian-event' + i + newNum).attr('name', 'deadline-thoigian-event' + i + newNum).val('');
    newElem.find('.deadline-status-event').attr('id', 'deadline-status-event' + i + newNum).attr('name', 'deadline-status-event' + i + newNum);
    newElem.find('.deadline-note-event').attr('id', 'deadline-note-event' + i + newNum).attr('name', 'deadline-note-event' + i + newNum);

    newElem.find('.deadline-event-a').attr('data-target', '#deadline-event' + i + newNum);
    newElem.find('.deadline-event-body').attr('id', 'deadline-event' + i + newNum);

    newElem.find('.btnRemoveDealineEvent').attr('onclick', 'removeDeadlineEvent(' + i + "," + newNum + ')');
    newElem.find('.btnRemoveDealineEvent').attr('disabled', false);

    $('#DeadlineEvent' + i + num).after(newElem);

    newElem.find("#select2-deadline-status-event" + i + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_deadline-note-event" + i + num).remove();

    $('#deadline-status-event' + i + newNum).select2();
    CKEDITOR.replace("deadline-note-event" + i + newNum);
}
function removeOptionEvent() {
    var num = $('.OptionEvent').length,
        option = $('#OptionEvent' + (num - 1)),
        optionremove = $('#OptionEvent' + num),
        actions = $('#OptionEvent' + num).find('.actionsOptionEvent');
    if (num == 2)
        actions.find('.btnRemoveOptionEvent').attr('disabled', true);
    option.find('.captionOptionEvent').after(actions);
    optionremove.remove();
    $('#countOptionEvent').val(num - 1);
}
function removeDeadlineEvent(x, y) {

    var actions = $('#DeadlineEvent' + x + y).find('.actions').clone();
    actions.find('.btnRemoveDealineEvent').attr('onclick', 'removeDeadlineEvent(' + x + "," + (y - 1) + ')');
    if (y == 2)
        actions.find('.btnRemoveDealineEvent').attr('disabled', true);
    $('#DeadlineEvent' + x + (y - 1)).find('.caption').after(actions);
    $('#DeadlineEvent' + x + y).remove();
}
//end event

//==============================================================================//

///Nhà hàng
$("#RestaurantName1").select2();
$('#DeadlineStatus11').select2();
CKEDITOR.replace("RestaurantNote1");
CKEDITOR.replace("DeadlineNote11");
/******** duplicate *******/
function addNewOption() {
    var num = $('.OptionRestaurant').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#OptionRestaurant' + num).clone().attr('id', 'OptionRestaurant' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    newElem.find('.RestaurantName').attr('id', 'RestaurantName' + newNum).attr('name', 'RestaurantName' + newNum);
    newElem.find('.RestaurantPrice').attr('id', 'RestaurantPrice' + newNum).attr('name', 'RestaurantPrice' + newNum).val('');
    newElem.find('.RestaurantAddress').attr('id', 'RestaurantAddress' + newNum).attr('name', 'RestaurantAddress' + newNum).val('');
    newElem.find('.RestaurantCode').attr('id', 'RestaurantCode' + newNum).attr('name', 'RestaurantCode' + newNum).val('');
    newElem.find('.RestaurantNote').attr('id', 'RestaurantNote' + newNum).attr('name', 'RestaurantNote' + newNum);
    newElem.find('.RestaurantDocument').attr('id', 'RestaurantDocument' + newNum);
    newElem.find('.NguoiLienHe').attr('id', 'NguoiLienHe' + newNum).attr('name', 'NguoiLienHe' + newNum).val('');
    newElem.find('.DienThoai').attr('id', 'DienThoai' + newNum).attr('name', 'DienThoai' + newNum).val('');
    newElem.find('.RestaurantCurrency').attr('id', 'RestaurantCurrency' + newNum).attr('name', 'RestaurantCurrency' + newNum);
    newElem.find('.OptionRestaurantA').attr('data-target', '#restaurant' + newNum);
    newElem.find('.OptionRestaurantBody').attr('id', 'restaurant' + newNum);

    newElem.find('.OptionTitle').html('OPTION ' + newNum);

    //deadline
    newElem.find('.btnNewDealine').attr('onclick', 'addNewDeadline(' + newNum + ')');
    newElem.find('.btnRemoveDealine').attr('onclick', 'removeDeadline(' + newNum + ',1)').attr('disabled', true);

    newElem.find('.DeadlineRestauran').attr('id', 'DeadlineRestauran' + newNum + 1);
    newElem.find('.DeadlineStatus').attr('id', 'DeadlineStatus' + newNum + 1).attr('name', 'DeadlineStatus' + newNum + 1);
    newElem.find('.DeadlineNote').attr('id', 'DeadlineNote' + newNum + 1).attr('name', 'DeadlineNote' + newNum + 1)
    newElem.find('.DeadlineTen').attr('id', 'DeadlineTen' + newNum + 1).attr('name', 'DeadlineTen' + newNum + 1)
    newElem.find('.DeadlineDeposit').attr('id', 'DeadlineDeposit' + newNum + 1).attr('name', 'DeadlineDeposit' + newNum + 1)
    newElem.find('.DeadlineThoiGian').attr('id', 'DeadlineThoiGian' + newNum + 1).attr('name', 'DeadlineThoiGian' + newNum + 1)
    newElem.find('.DeadlineRestauranA').attr('data-target', '#deadline-restauran' + newNum + 1);
    newElem.find('.DeadlineRestauranBody').attr('id', 'deadline-restauran' + newNum + 1);
    newElem.find('.DeadlineTitle').html('Deadline 1');

    var arr = newElem.find('.DeadlineRestauran').toArray();
    newElem.find('.DeadlineRestauran').each(function (index) {
        if (arr.length - 1 != index)
            this.remove();
    });
    $('#OptionRestaurant' + num).after(newElem);
    $('#countOptionRestaurant').val(newNum);

    $('#RestaurantDocument' + newNum).change(function () {
        var data = new FormData();
        data.append('RestaurantDocument', $('#RestaurantDocument' + newNum)[0].files[0]);
        data.append('id', newNum);

        var ajaxRequest = $.ajax({
            type: "POST",
            url: 'TourService/UploadFileRestaurant',
            contentType: false,
            processData: false,
            data: data
        });

        ajaxRequest.done(function (xhr, textStatus) {
            // Onsuccess
        });
    });

    CKEDITOR.replace("RestaurantNote" + newNum);
    // for (var i = 1; i < newNum; i++) {
    newElem.find("#select2-RestaurantName" + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_RestaurantNote" + num).remove();
    newElem.find("#cke_DeadlineNote" + num + arr.length).remove();
    newElem.find("#select2-DeadlineStatus" + num + arr.length + "-container").parent().parent().parent().remove();
    newElem.find("#select2-RestaurantCurrency" + num + arr.length + "-container").parent().parent().parent().remove();
    //}
    $("#RestaurantCurrency" + newNum).select2();
    $("#RestaurantName" + newNum).select2();
    $("#DeadlineStatus" + newNum + 1).select2();
    CKEDITOR.replace("DeadlineNote" + newNum + 1);
    newElem.find('.btnRemoveOption').attr('disabled', false);
    $('#OptionRestaurant' + num).find('.actionsOption').remove();
}
function addNewDeadline(i) {
    var num = $('#OptionRestaurant' + i + ' .DeadlineRestauran').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#DeadlineRestauran' + i + num).clone().attr('id', 'DeadlineRestauran' + i + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    $('#DeadlineRestauran' + i + num).find('.actions').remove();

    newElem.find('.DeadlineStatus').attr('name', 'DeadlineStatus' + i + newNum).attr('id', 'DeadlineStatus' + i + newNum);
    newElem.find('.DeadlineNote').attr('name', 'DeadlineNote' + i + newNum).attr('id', 'DeadlineNote' + i + newNum);
    newElem.find('.DeadlineTen').attr('id', 'DeadlineTen' + i + newNum).attr('name', 'DeadlineTen' + i + newNum)
    newElem.find('.DeadlineDeposit').attr('id', 'DeadlineDeposit' + i + newNum).attr('name', 'DeadlineDeposit' + i + newNum)
    newElem.find('.DeadlineThoiGian').attr('id', 'DeadlineThoiGian' + i + newNum).attr('name', 'DeadlineThoiGian' + i + newNum)

    newElem.find('.DeadlineRestauranA').attr('data-target', '#deadline-restauran' + i + newNum);
    newElem.find('.DeadlineRestauranBody').attr('id', 'deadline-restauran' + i + newNum);

    newElem.find('.DeadlineTitle').html('Deadline ' + newNum);

    newElem.find('.btnRemoveDealine').attr('onclick', 'removeDeadline(' + i + "," + newNum + ')');
    newElem.find('.btnRemoveDealine').attr('disabled', false);

    $('#DeadlineRestauran' + i + num).after(newElem);


    CKEDITOR.replace("DeadlineNote" + i + newNum);
    newElem.find("#select2-DeadlineStatus" + i + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_DeadlineNote" + i + num).remove();
    $("#DeadlineStatus" + i + newNum).select2();
}
function removeDeadline(x, y) {
    var actions = $('#DeadlineRestauran' + x + y).find('.actions').clone();
    actions.find('.btnRemoveDealine').attr('onclick', 'removeDeadline(' + x + "," + (y - 1) + ')');
    if (y == 2)
        actions.find('.btnRemoveDealine').attr('disabled', true);
    $('#DeadlineRestauran' + x + (y - 1)).find('.caption').after(actions);
    $('#DeadlineRestauran' + x + y).remove();
}
function removeOption() {
    var num = $('.OptionRestaurant').length,
        option = $('#OptionRestaurant' + (num - 1)),
        optionremove = $('#OptionRestaurant' + num),
        actions = $('#OptionRestaurant' + num).find('.actionsOption');
    if (num == 2)
        actions.find('.btnRemoveOption').attr('disabled', true);
    option.find('.captionOption').after(actions);
    optionremove.remove();
    $('#countOptionRestaurant').val(num - 1);
}
//end nhà hàng

//=============================================================================//

//khách sạn
$("#hotel-tour1").select2();
$("#currency-hotel-tour1").select2();
$('#star-hotel1').select2();
$('#deadline-status-hotel11').select2();
CKEDITOR.replace("note-hotel1");
CKEDITOR.replace("deadline-note-hotel11");
/******** duplicate *******/
function addNewOptionHotel() {
    var num = $('.OptionHotel').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#OptionHotel' + num).clone().attr('id', 'OptionHotel' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element
    newElem.find('.OptionTitle').html('OPTION ' + newNum);

    newElem.find('.hotel-tour').attr('id', 'hotel-tour' + newNum).attr('name', 'hotel-tour' + newNum);
    newElem.find('.currency-hotel-tour').attr('id', 'currency-hotel-tour' + newNum).attr('name', 'currency-hotel-tour' + newNum);
    newElem.find('.star-hotel').attr('id', 'star-hotel' + newNum).attr('name', 'star-hotel' + newNum);
    newElem.find('.tungay-hotel').attr('id', 'tungay-hotel' + newNum).attr('name', 'tungay-hotel' + newNum).val('');
    newElem.find('.room-hotel').attr('id', 'room-hotel' + newNum).attr('name', 'room-hotel' + newNum).val('');
    newElem.find('.nguoi-lien-he-hotel').attr('id', 'nguoi-lien-he-hotel' + newNum).attr('name', 'nguoi-lien-he-hotel' + newNum).val('');
    newElem.find('.code-hotel').attr('id', 'code-hotel' + newNum).attr('name', 'code-hotel' + newNum).val('');
    newElem.find('.position-hotel').attr('id', 'position-hotel' + newNum).attr('name', 'position-hotel' + newNum).val('');
    newElem.find('.night-hotel').attr('id', 'night-hotel' + newNum).attr('name', 'night-hotel' + newNum).val('');
    newElem.find('.price-hotel').attr('id', 'price-hotel' + newNum).attr('name', 'price-hotel' + newNum).val('');
    newElem.find('.phone-hotel').attr('id', 'phone-hotel' + newNum).attr('name', 'phone-hotel' + newNum).val('');
    newElem.find('.note-hotel').attr('id', 'note-hotel' + newNum).attr('name', 'note-hotel' + newNum);
    newElem.find('.OptionHotelA').attr('data-target', '#hotel' + newNum);
    newElem.find('.OptionHotelBody').attr('id', 'hotel' + newNum);

    //deadline
    newElem.find('.DeadlineHotel').attr('id', 'DeadlineHotel' + newNum + 1);
    newElem.find('.btnNewDealine').attr('onclick', 'addNewDeadlineHotel(' + newNum + ')');
    newElem.find('.btnRemoveDealine').attr('onclick', 'removeDeadlineHotel(' + newNum + ',1)').attr('disabled', true);
    newElem.find('.DeadlineTitle').html('Deadline 1');

    newElem.find('.deadline-hotel-a').attr('data-target', '#deadline-hotel' + newNum + 1);
    newElem.find('.deadline-hotel-body').attr('id', 'deadline-hotel' + newNum + 1);

    newElem.find('.deadline-name-hotel').attr('id', 'deadline-name-hotel' + newNum + 1).attr('name', 'deadline-name-hotel' + newNum + 1);
    newElem.find('.deadline-total-hotel').attr('id', 'deadline-total-hotel' + newNum + 1).attr('name', 'deadline-total-hotel' + newNum + 1);
    newElem.find('.deadline-thoigian-hotel').attr('id', 'deadline-thoigian-hotel' + newNum + 1).attr('name', 'deadline-thoigian-hotel' + newNum + 1)
    newElem.find('.deadline-status-hotel').attr('id', 'deadline-status-hotel' + newNum + 1).attr('name', 'deadline-status-hotel' + newNum + 1);
    newElem.find('.deadline-note-hotel').attr('id', 'deadline-note-hotel' + newNum + 1).attr('name', 'deadline-note-hotel' + newNum + 1)

    var arr = newElem.find('.DeadlineHotel').toArray();
    newElem.find('.DeadlineHotel').each(function (index) {
        if (arr.length - 1 != index)
            this.remove();
    });
    $('#OptionHotel' + num).after(newElem);
    $('#countOptionHotel').val(newNum);
    newElem.find('.btnRemoveOption').attr('disabled', false);
    $('#OptionHotel' + num).find('.actionsOption').remove();

    newElem.find("#select2-hotel-tour" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-currency-hotel-tour" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-star-hotel" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-deadline-status-hotel" + num + arr.length + "-container").parent().parent().parent().remove();
    newElem.find("#cke_note-hotel" + num).remove();
    newElem.find("#cke_deadline-note-hotel" + num + arr.length).remove();

    $("#hotel-tour" + newNum).select2();
    $("#currency-hotel-tour" + newNum).select2();
    $('#star-hotel' + newNum).select2();
    CKEDITOR.replace("note-hotel" + newNum);
    $('#deadline-status-hotel' + newNum + 1).select2();
    CKEDITOR.replace("deadline-note-hotel" + newNum + 1);
}
function addNewDeadlineHotel(i) {
    var num = $('#OptionHotel' + i + ' .DeadlineHotel').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#DeadlineHotel' + i + num).clone().attr('id', 'DeadlineHotel' + i + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    newElem.find('.DeadlineTitle').html('Deadline ' + newNum);

    $('#DeadlineHotel' + i + num).find('.actions').remove();

    newElem.find('.deadline-name-hotel').attr('name', 'deadline-name-hotel' + i + newNum).attr('id', 'deadline-name-hotel' + i + newNum).val('');
    newElem.find('.deadline-total-hotel').attr('name', 'deadline-total-hotel' + i + newNum).attr('id', 'deadline-total-hotel' + i + newNum).val('');
    newElem.find('.deadline-thoigian-hotel').attr('id', 'deadline-thoigian-hotel' + i + newNum).attr('name', 'deadline-thoigian-hotel' + i + newNum).val('');
    newElem.find('.deadline-status-hotel').attr('id', 'deadline-status-hotel' + i + newNum).attr('name', 'deadline-status-hotel' + i + newNum);
    newElem.find('.deadline-note-hotel').attr('id', 'deadline-note-hotel' + i + newNum).attr('name', 'deadline-note-hotel' + i + newNum);

    newElem.find('.deadline-hotel-a').attr('data-target', '#deadline-hotel' + i + newNum);
    newElem.find('.deadline-hotel-body').attr('id', 'deadline-hotel' + i + newNum);

    newElem.find('.btnRemoveDealine').attr('onclick', 'removeDeadlineHotel(' + i + "," + newNum + ')');
    newElem.find('.btnRemoveDealine').attr('disabled', false);

    $('#DeadlineHotel' + i + num).after(newElem);

    newElem.find("#select2-deadline-status-hotel" + i + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_deadline-note-hotel" + i + num).remove();

    $('#deadline-status-hotel' + i + newNum).select2();
    CKEDITOR.replace("deadline-note-hotel" + i + newNum);
}
function removeOptionHotel() {
    var num = $('.OptionHotel').length,
        option = $('#OptionHotel' + (num - 1)),
        optionremove = $('#OptionHotel' + num),
        actions = $('#OptionHotel' + num).find('.actionsOption');
    if (num == 2)
        actions.find('.btnRemoveOption').attr('disabled', true);
    option.find('.captionOption').after(actions);
    optionremove.remove();
    $('#countOptionHotel').val(num - 1);
}
function removeDeadlineHotel(x, y) {

    var actions = $('#DeadlineHotel' + x + y).find('.actions').clone();
    actions.find('.btnRemoveDealine').attr('onclick', 'removeDeadlineHotel(' + x + "," + (y - 1) + ')');
    if (y == 2)
        actions.find('.btnRemoveDealine').attr('disabled', true);
    $('#DeadlineHotel' + x + (y - 1)).find('.caption').after(actions);
    $('#DeadlineHotel' + x + y).remove();
}
//end khách sạn

//===========================================================================//

//Vận chuyển
$("#name-transport1").select2();
$("#ServiceCurrency1").select2();
CKEDITOR.replace("ServiceNote11");
/******** duplicate *******/
function addNewTranport() {
    var num = $('.OptionTransport').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#OptionTransport' + num).clone().attr('id', 'OptionTransport' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element
    newElem.find('.OptionTitle').html('OPTION ' + newNum);

    newElem.find('.code-transport').attr('id', 'code-transport' + newNum).attr('name', 'code-transport' + newNum).val('');
    newElem.find('.name-transport').attr('id', 'name-transport' + newNum).attr('name', 'name-transport' + newNum);
    newElem.find('.nguoilienhe-transport').attr('id', 'nguoilienhe-transport' + newNum).attr('name', 'nguoilienhe-transport' + newNum).val('');
    newElem.find('.country-transport').attr('id', 'country-transport' + newNum).attr('name', 'country-transport' + newNum).val('');
    newElem.find('.phone-transport').attr('id', 'phone-transport' + newNum).attr('name', 'phone-transport' + newNum).val('');
    newElem.find('.file-transport').attr('id', 'file-transport' + newNum).val('');
    newElem.find('.OptionTransportA').attr('data-target', '#transport' + newNum);
    newElem.find('.OptionTransportBody').attr('id', 'transport' + newNum);

    //service
    newElem.find('.ServiceTranport').attr('id', 'ServiceTranport' + newNum + 1);
    newElem.find('.btnNewServiceTranport').attr('onclick', 'addNewServiceTranport(' + newNum + ')');
    newElem.find('.btnRemoveServiceTranport').attr('onclick', 'removeServiceTranport(' + newNum + ',1)').attr('disabled', true);

    newElem.find('.ServiceCurrency').attr('id', 'ServiceCurrency' + newNum + 1).attr('name', 'ServiceCurrency' + newNum + 1);
    newElem.find('.ServiceName').attr('id', 'ServiceName' + newNum + 1).attr('name', 'ServiceName' + newNum + 1).val('');
    newElem.find('.ServicePrice').attr('id', 'ServicePrice' + newNum + 1).attr('name', 'ServicePrice' + newNum + 1).val('');
    newElem.find('.ServiceNote').attr('id', 'ServiceNote' + newNum + 1).attr('name', 'ServiceNote' + newNum + 1).val('');

    var arr = newElem.find('.ServiceTranport').toArray();
    newElem.find('.ServiceTranport').each(function (index) {
        if (arr.length - 1 != index)
            this.remove();
    });
    $('#OptionTransport' + num).after(newElem);
    $('#countOptionTransport').val(newNum);
    newElem.find('.btnRemoveOption').attr('disabled', false);
    $('#OptionTransport' + num).find('.actionsOption').remove();

    /******** upload file *******/
    $('#file-transport' + newNum).change(function () {
        var data = new FormData();
        data.append('TransportDocument', $('#file-transport' + newNum)[0].files[0]);
        data.append('id', newNum);

        var ajaxRequest = $.ajax({
            type: "POST",
            url: 'TourService/UploadFileTransport',
            contentType: false,
            processData: false,
            data: data
        });

        ajaxRequest.done(function (xhr, textStatus) {
            // Onsuccess
        });
    });

    newElem.find("#select2-name-transport" + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_ServiceNote" + num + arr.length).remove();

    $('#name-transport' + newNum).select2();
    CKEDITOR.replace("ServiceNote" + newNum + 1);
}
function addNewServiceTranport(i) {
    var num = $('#OptionTransport' + i + ' .ServiceTranport').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#ServiceTranport' + i + num).clone().attr('id', 'ServiceTranport' + i + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    $('#ServiceTranport' + i + num).find('.btnRemoveServiceTranport').remove();

    newElem.find('.ServiceCurrency').attr('id', 'ServiceCurrency' + newNum + 1).attr('name', 'ServiceCurrency' + newNum + 1);
    newElem.find('.ServiceName').attr('name', 'ServiceName' + i + newNum).attr('id', 'ServiceName' + i + newNum).val('');
    newElem.find('.ServicePrice').attr('name', 'ServicePrice' + i + newNum).attr('id', 'ServicePrice' + i + newNum).val('');
    newElem.find('.ServiceNote').attr('id', 'ServiceNote' + i + newNum).attr('name', 'ServiceNote' + i + newNum);

    newElem.find('.btnRemoveServiceTranport').attr('onclick', 'removeServiceTranport(' + i + "," + newNum + ')');
    newElem.find('.btnRemoveServiceTranport').attr('disabled', false);
    newElem.find("#select2-ServiceCurrency" + num + "-container").parent().parent().parent().remove();

    $('#ServiceTranport' + i + num).after(newElem);
    $("#ServiceCurrency" + newNum).select2();
    newElem.find("#cke_ServiceNote" + i + num).remove();
    CKEDITOR.replace("ServiceNote" + i + newNum);
}
function removeOptionTransport() {
    var num = $('.OptionTransport').length,
        option = $('#OptionTransport' + (num - 1)),
        optionremove = $('#OptionTransport' + num),
        actions = $('#OptionTransport' + num).find('.actionsOption');
    if (num == 2)
        actions.find('.btnRemoveOption').attr('disabled', true);
    option.find('.captionOption').after(actions);
    $('#countOptionTransport').val(num - 1);
    optionremove.remove();
}
function removeServiceTranport(x, y) {
    var actions = $('#ServiceTranport' + x + y).find('.btnRemoveServiceTranport').clone();
    actions.attr('onclick', 'removeServiceTranport(' + x + "," + (y - 1) + ')');
    if (y == 2)
        actions.attr('disabled', true);
    $('#ServiceTranport' + x + (y - 1)).find('.actionRemoveServiceTranport').html(actions);
    $('#ServiceTranport' + x + y).remove();
}
//end vận chuyển

//============================================================================//

//vé máy bay
$("#hang-plane1").select2();
$("#loaitien-plane1").select2();
$("#flight-plane1").select2();
CKEDITOR.replace("note-plane1");
$("#tinhtrang-deadline-plane11").select2();
CKEDITOR.replace("PlaneNoteDeadline11");
/******** duplicate *******/
function addNewOptionPlane() {
    var num = $('.OptionPlane').length,
        newNum = new Number(num + 1),
        newElem = $('#OptionPlane' + num).clone().attr('id', 'OptionPlane' + newNum).fadeIn('slow');

    newElem.find('.OptionTitle').html('OPTION ' + newNum);

    newElem.find('.contacter-plane').attr('id', 'contacter-plane' + newNum).attr('name', 'contacter-plane' + newNum).val('');
    newElem.find('.contacter-phone-plane').attr('id', 'contacter-phone-plane' + newNum).attr('name', 'contacter-phone-plane' + newNum).val('');
    newElem.find('.hang-plane').attr('id', 'hang-plane' + newNum).attr('name', 'hang-plane' + newNum);
    newElem.find('.quantity-plane1').attr('id', 'quantity-plane1' + newNum).attr('name', 'quantity-plane1' + newNum).val('');
    newElem.find('.code-plane').attr('id', 'code-plane' + newNum).attr('name', 'code-plane' + newNum).val('');
    newElem.find('.price-code').attr('id', 'price-code' + newNum).attr('name', 'price-code' + newNum).val('');
    newElem.find('.loaitien-plane').attr('id', 'loaitien-plane' + newNum).attr('name', 'loaitien-plane' + newNum);
    newElem.find('.flight-plane').attr('id', 'flight-plane' + newNum).attr('name', 'flight-plane' + newNum).val('');
    newElem.find('.note-plane').attr('id', 'note-plane' + newNum).attr('name', 'note-plane' + newNum);

    newElem.find('.OptionPlaneA').attr('data-target', '#plane' + newNum);
    newElem.find('.OptionPlaneBody').attr('id', 'plane' + newNum);

    //deadline
    newElem.find('.DeadlinePlane').attr('id', 'DeadlinePlane' + newNum + 1);
    newElem.find('.DeadlineTitle').html('Deadline 1');

    newElem.find('.DeadlinePlaneA').attr('data-target', '#deadline-plane' + newNum + 1);
    newElem.find('.DeadlinePlaneBody').attr('id', 'deadline-plane' + newNum + 1);

    newElem.find('.btnAddNewDeadlinePlane').attr('onclick', 'addNewDeadlinePlane(' + newNum + ')');
    newElem.find('.btnRemoveDeadlinePlane').attr('onclick', 'removeDeadlinePlane(' + newNum + ',1)').attr('disabled', true);

    newElem.find('.name-deadline-plane').attr('id', 'name-deadline-plane' + newNum + 1).attr('name', 'name-deadline-plane' + newNum + 1).val('');
    newElem.find('.sotien-deadline-plane').attr('id', 'sotien-deadline-plane' + newNum + 1).attr('name', 'sotien-deadline-plane' + newNum + 1).val('');
    newElem.find('.thoigian-deadline-plane').attr('id', 'thoigian-deadline-plane' + newNum + 1).attr('name', 'thoigian-deadline-plane' + newNum + 1).val('');
    newElem.find('.tinhtrang-deadline-plane').attr('id', 'tinhtrang-deadline-plane' + newNum + 1).attr('name', 'tinhtrang-deadline-plane' + newNum + 1);
    newElem.find('.file-deadline-plane').attr('id', 'file-deadline-plane' + newNum + 1).val('');
    newElem.find('.PlaneNoteDeadline').attr('id', 'PlaneNoteDeadline' + newNum + 1).attr('name', 'PlaneNoteDeadline' + newNum + 1);

    var arr = newElem.find('.DeadlinePlane').toArray();
    newElem.find('.DeadlinePlane').each(function (index) {
        if (arr.length - 1 != index)
            this.remove();
    });
    $('#OptionPlane' + num).after(newElem);
    $('#countOptionPlane').val(newNum);
    newElem.find('.btnRemoveOption').attr('disabled', false);
    $('#OptionPlane' + num).find('.actionsOption').remove();

    newElem.find("#select2-hang-plane" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-loaitien-plane" + num + "-container").parent().parent().parent().remove();
    newElem.find("#select2-flight-plane" + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_note-plane" + num).remove();
    newElem.find("#select2-tinhtrang-deadline-plane" + num + arr.length + "-container").parent().parent().parent().remove();
    newElem.find("#cke_PlaneNoteDeadline" + num + arr.length).remove();

    $('#hang-plane' + newNum).select2();
    $('#loaitien-plane' + newNum).select2();
    $('#flight-plane' + newNum).select2();
    CKEDITOR.replace("note-plane" + newNum);
    $('#tinhtrang-deadline-plane' + newNum + 1).select2();
    CKEDITOR.replace("PlaneNoteDeadline" + newNum + 1);
}
function addNewDeadlinePlane(i) {
    var num = $('#OptionPlane' + i + ' .DeadlinePlane').length,
        newNum = new Number(num + 1),
        newElem = $('#DeadlinePlane' + i + num).clone().attr('id', 'DeadlinePlane' + i + newNum).fadeIn('slow');

    $('#DeadlinePlane' + i + num).find('.actions').remove();

    newElem.find('.name-deadline-plane').attr('name', 'name-deadline-plane' + i + newNum).attr('id', 'name-deadline-plane' + i + newNum);
    newElem.find('.sotien-deadline-plane').attr('name', 'sotien-deadline-plane' + i + newNum).attr('id', 'sotien-deadline-plane' + i + newNum);
    newElem.find('.thoigian-deadline-plane').attr('id', 'thoigian-deadline-plane' + i + newNum).attr('name', 'thoigian-deadline-plane' + i + newNum);
    newElem.find('.tinhtrang-deadline-plane').attr('id', 'tinhtrang-deadline-plane' + i + newNum).attr('name', 'tinhtrang-deadline-plane' + i + newNum);
    newElem.find('.file-deadline-plane').attr('id', 'file-deadline-plane' + i + newNum).attr('name', 'file-deadline-plane' + i + newNum);
    newElem.find('.PlaneNoteDeadline').attr('id', 'PlaneNoteDeadline' + i + newNum).attr('name', 'PlaneNoteDeadline' + i + newNum);

    newElem.find('.DeadlinePlaneA').attr('data-target', '#deadline-plane' + i + newNum);
    newElem.find('.DeadlinePlaneBody').attr('id', 'deadline-plane' + i + newNum);

    newElem.find('.DeadlineTitle').html('Deadline ' + newNum);

    newElem.find('.btnRemoveDeadlinePlane').attr('onclick', 'removeDeadlinePlane(' + i + "," + newNum + ')');
    newElem.find('.btnRemoveDeadlinePlane').attr('disabled', false);

    $('#DeadlinePlane' + i + num).after(newElem);

    /****** insert file ******/
    $('#file-deadline-plane' + i + num).change(function () {
        var data = new FormData();
        data.append('FileNamePlane', $('#file-deadline-plane' + i + num)[0].files[0]);
        data.append('id', i + num);

        var ajaxRequest = $.ajax({
            type: "POST",
            url: 'TourService/UploadFilePlane',
            contentType: false,
            processData: false,
            data: data
        });

        ajaxRequest.done(function (xhr, textStatus) {
            // Onsuccess
        });
    });

    newElem.find("#select2-tinhtrang-deadline-plane" + i + num + "-container").parent().parent().parent().remove();
    newElem.find("#cke_PlaneNoteDeadline" + i + num).remove();
    CKEDITOR.replace("PlaneNoteDeadline" + i + newNum);
    $("#tinhtrang-deadline-plane" + i + newNum).select2();
}
function removeOptionPlane() {
    var num = $('.OptionPlane').length,
       option = $('#OptionPlane' + (num - 1)),
       optionremove = $('#OptionPlane' + num),
       actions = $('#OptionPlane' + num).find('.actionsOption');
    if (num == 2)
        actions.find('.btnRemoveOption').attr('disabled', true);
    option.find('.captionOption').after(actions);
    $('#countOptionPlane').val(num - 1);
    optionremove.remove();
}
function removeDeadlinePlane(x, y) {

    var actions = $('#DeadlinePlane' + x + y).find('.actions').clone();
    actions.find('.btnRemoveDeadlinePlane').attr('onclick', 'removeDeadlinePlane(' + x + "," + (y - 1) + ')');
    if (y == 2)
        actions.find('.btnRemoveDeadlinePlane').attr('disabled', true);
    $('#DeadlinePlane' + x + (y - 1)).find('.caption').after(actions);
    $('#DeadlinePlane' + x + y).remove();
}
//end vé máy bay

//===================================================================================//

function OnSuccessTourService() {
    alert("Đã lưu!");
    $("#modal-insert-hotel").modal("hide");
    $("#modal-insert-restaurant").modal("hide");
}
function OnFailureTourService() {
    alert("Lỗi. Vui lòng xem lại!");
    $("#modal-insert-hotel").modal("hide");
    $("#modal-insert-restaurant").modal("hide");
}