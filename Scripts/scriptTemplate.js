$(document).ready(function () {
    $("[data-toggle='collapse']").click(function (e) {
        var $this = $(this);
        var $icon = $this.find("i");

        if ($icon.hasClass('fa-minus')) {
            $icon.removeClass('fa-minus').addClass('fa-plus');
        } else {
            $icon.removeClass('fa-plus').addClass('fa-minus');
        }
    });
});

/********* insert customer, staff, tour on home page *********/
//$("#home-manager-tour").select2();
//$("#home-start-place").select2();
//$("#home-destination-place").select2();
//$("#home-type-tour").select2();
//$("#home-permission-tour").select2();
//$("#home-guide-tour").select2();
//$("#home-startplace-tourguide").select2();
//$("#home-customer-company").select2();
//$("#home-address-company").select2();
//$("#home-nhomkh-company").select2();
//$("#home-nguonden-company").select2();
//$("#home-customer-company").select2();
//$("#home-quanly-company").select2();
//$("#home-certificate").select2();
//$("#home-marriage").select2();
//$("#home-religion").select2();
//$("#home-contact").select2();
//$("#home-nation").select2();
//$("#home-countryvisa1").select2();
//$("#staff-countryvisa1").select2();
//$("#home-placepassport").select2();
//$("#home-placeidentity").select2();
//$("#home-department").select2();
//$("#home-headquarter").select2();
//$("#home-position").select2();
//$("#home-staffgroup").select2();
//$("#home-address").select2();
//$("#home-birthplace").select2();
//$("#home-nametype").select2();
//$("#home-quanly-personal").select2();
//$("#home-nguonden-personal").select2();
//$("#home-nhomkh-personal").select2();
//$("#home-nghenghiep-personal").select2();
//$("#home-address-personal").select2();
//$("#home-quydanh").select2();
//$("#home-address-contact").select2();
//$("#home-quydanh-contact").select2();
//$("#country-home-passport").select2();
//$("#country-home-cmnd").select2();

////CKEDITOR.replace("home-request-tour");
////CKEDITOR.replace("home-note-company");
////CKEDITOR.replace("home-note-personal");
////CKEDITOR.replace("home-note-contact");

//function radCompanyHomeClick() {
//    $('#home-company').show(); $('#home-personal').hide();
//}

//function radPersonalHomeClick() {
//    $('#home-company').hide(); $('#home-personal').show();
//}

/////*** duplicate form visa customer ***/
//$(function () {
//    $('#btnAddHome').click(function () {
//        var num = $('.clonedInputHome').length, // how many "duplicatable" input fields we currently have
//            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
//            newElem = $('#entryHome' + num).clone().attr('id', 'entryHome' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
//        // manipulate the name/id values of the input inside the new element

//        newElem.find('.home-visacard').attr('name', 'VisaNumber' + newNum).val('');
//        newElem.find('.home-ngaycapvisa').attr('id', 'home-ngaycapvisa' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
//        newElem.find('.home-ngayhethanvisa').attr('id', 'home-ngayhethanvisa' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
//        newElem.find('.home-countryvisa').attr('id', 'home-countryvisa' + newNum).attr('name', 'TagsId' + newNum).val('');

//        // insert the new element after the last "duplicatable" input field
//        $('#entryHome' + num).after(newElem);
//        //$("#ngaycapvisa" + newNum).datepicker();
//        //$("#ngayhethanvisa" + newNum).datepicker();
//        $("#home-countryvisa" + newNum).select2();

//        for (var i = 1; i < newNum; i++) {
//            $("#entryHome" + newNum + " #select2-home-countryvisa" + i + "-container").parent().parent().parent().remove();
//        }

//        // enable the "remove" button
//        $('#btnDelHome').attr('disabled', false);

//    });

//    $('#btnDelHome').click(function () {
//        // confirmation
//        var num = $('.clonedInputHome').length;
//        // how many "duplicatable" input fields we currently have
//        $('#entryHome' + num).slideUp('slow', function () {
//            $(this).remove();
//            // if only one element remains, disable the "remove" button
//            if (num - 1 === 1)
//                $('#btnDelHome').attr('disabled', true);
//            // enable the "add" button
//            $('#btnAddHome').attr('disabled', false).prop('value', "add section");
//        });
//        return false;

//        $('#btnAddHome').attr('disabled', false);
//    });
//    $('#btnDelHome').attr('disabled', true);
//});


/////*** duplicate form visa staff ***/
//$(function () {
//    $('#btnAddStaff').click(function () {
//        var num = $('.clonedInputStaff').length, // how many "duplicatable" input fields we currently have
//            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
//            newElem = $('#entryStaff' + num).clone().attr('id', 'entryStaff' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
//        // manipulate the name/id values of the input inside the new element

//        newElem.find('.staff-visacard').attr('name', 'VisaNumber' + newNum).val('');
//        newElem.find('.staff-ngaycapvisa').attr('id', 'staff-ngaycapvisa' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
//        newElem.find('.staff-ngayhethanvisa').attr('id', 'staff-ngayhethanvisa' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
//        newElem.find('.staff-countryvisa').attr('id', 'staff-countryvisa' + newNum).attr('name', 'TagsId' + newNum).val('');

//        // insert the new element after the last "duplicatable" input field
//        $('#entryStaff' + num).after(newElem);
//        //$("#ngaycapvisa" + newNum).datepicker();
//        //$("#ngayhethanvisa" + newNum).datepicker();
//        $("#staff-countryvisa" + newNum).select2();

//        for (var i = 1; i < newNum; i++) {
//            $("#entryStaff" + newNum + " #select2-staff-countryvisa" + i + "-container").parent().parent().parent().remove();
//        }

//        // enable the "remove" button
//        $('#btnDelStaff').attr('disabled', false);

//    });

//    $('#btnDelStaff').click(function () {
//        // confirmation
//        var num = $('.clonedInputStaff').length;
//        // how many "duplicatable" input fields we currently have
//        $('#entryStaff' + num).slideUp('slow', function () {
//            $(this).remove();
//            // if only one element remains, disable the "remove" button
//            if (num - 1 === 1)
//                $('#btnDelStaff').attr('disabled', true);
//            // enable the "add" button
//            $('#btnAddStaff').attr('disabled', false).prop('value', "add section");
//        });
//        return false;

//        $('#btnAddStaff').attr('disabled', false);
//    });
//    $('#btnDelStaff').attr('disabled', true);
//});