$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
});

$("#insert-tour").select2();
$("#insert-customer").select2();
$("#insert-service1").select2();

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/ReviewTour/ReviewTourInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-reviewtour").html(data);
            CKEDITOR.replace("edit-note");
            $("#edit-tour").select2();
            $("#edit-serviceE1").select2();
            $("#edit-customer").select2();
            $("#modal-edit-mark").modal("show");
            for (var i = 2; i <= $("#countServiceE").val() ; i++) {
                $("#edit-serviceE" + i).select2();
            }

            $(function () {
                $('#btnAddRE').click(function () {
                    var num = $('.clonedInputE').length, // how many "duplicatable" input fields we currently have
                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                        newElem = $('#entryE' + num).clone().attr('id', 'entryE' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                    // manipulate the name/id values of the input inside the new element

                    newElem.find('.serviceE').attr('id', 'insert-serviceE' + newNum).attr('name', 'DictionaryId' + newNum);
                    newElem.find('.markE').attr('name', 'Mark' + newNum).val('');

                    // insert the new element after the last "duplicatable" input field
                    $('#entryE' + num).after(newElem);
                    $("#countServiceE").val(newNum);
                    $("#insert-serviceE" + newNum).select2();

                    for (var i = 1; i < newNum; i++) {
                        $("#entryE" + newNum + " #select2-edit-serviceE" + i + "-container").parent().parent().parent().remove();
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
                        $('#btnAddRE').attr('disabled', false).prop('value', "add section");
                    });
                    return false;

                    $('#btnAddRE').attr('disabled', false);
                });
                $('#btnDelE').attr('disabled', true);
            });
        }
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
    $("#listItemId").val('');
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
$("table#tableDictionary").delegate("tr", "click", function () {
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    $.ajax({
        type: "POST",
        url: '/ReviewTour/ListReviewDetail',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#tab1").html(data);
            CheckSelect();
        }
    });
});

//** duplicate **/
$(function () {
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
});