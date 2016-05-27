$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                null,
                { type: "text" },
                { type: "text" }]
});

/** cập nhật permission **/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/PermissionManage/Edit',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-permission").html(data);
            //CKEDITOR.replace("edit-note-lichhen");
            $("#modal-edit-grouprole").modal("show");
        }
    });
});

/** thêm user **/
$("#btnAddUser").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/PermissionManage/AddUser',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-adduser").html(data);
            $(".dataTable2").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            $("#tableStaff").on("change", ".checkAddUser", function () {
                var ItemID = $(this).val();
                var currentlistItemID = $("#listItemIdAdd").val();
                var stringBranchID = "";
                if ($(this).prop('checked')) {
                    currentlistItemID += ItemID + ",";
                    $("#listItemIdAdd").val(currentlistItemID);
                } else {
                    $("#listItemIdAdd").val(currentlistItemID.replace(ItemID + ",", ""));
                }
            });
            $("#btnLuuAddUser").on("click", function () {
                if ($("#listItemIdAdd").val() == "") {
                    alert("Vui lòng chọn người dùng !");
                    return false;
                }
                var $form = $("#formAddUser");
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize(),
                };
                $.ajax(options).done(function (data) {
                    if (data.Succeed) {
                        alert(data.Message);
                        if (data.RedirectTo != null && data.RedirectTo != "") {
                            window.location.href = data.RedirectTo;
                        }
                    }
                    else {
                        alert(data.Message);
                    }
                });
            });


            $("#modal-add-user").modal("show");
        }
    });
});

$("#btnSave").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val(),
        name: $("#txtName").val()
    };

    $.ajax({
        type: "POST",
        url: '/PermissionManage/SaveData',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            window.location.href = '/AppointmentType/Index';
        }
    });
})

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
$("table#tableDictionary").delegate("tr", "click", function (e) {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    var tab = $(".tab-content").find('.active').data("id");
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    switch (tab) {
        case 'nguoidung':
            $.ajax({
                type: "POST",
                url: '/PermissionManage/InfoNguoiDung',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nguoidung").html(data);
                }
            });
            break;
            //case 'quyentruycap':
            //    $.ajax({
            //        type: "POST",
            //        url: '/PermissionManage/InfoQuyenTruyCap',
            //        data: JSON.stringify(dataPost),
            //        contentType: "application/json; charset=utf-8",
            //        dataType: "html",
            //        success: function (data) {
            //            $("#quyentruycap").html(data);
            //        }
            //    });
            //    break;
    }
});

/** click chọn từng tab -> hiển thị thông tin **/
$("#tabnguoidung").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhóm!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/PermissionManage/InfoNguoiDung',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nguoidung").html(data);
            }
        });
    }
});

//$("#tabquyentruycap").click(function () {
//    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
//        alert("Vui lòng chọn 1 nhóm!");
//    }
//    else {
//        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
//        $.ajax({
//            type: "POST",
//            url: '/PermissionManage/InfoNguoiDung',
//            data: JSON.stringify(dataPost),
//            contentType: "application/json; charset=utf-8",
//            dataType: "html",
//            success: function (data) {
//                $("#quyentruycap").html(data);
//            }
//        });
//    }
//});

function deleteUser(id) {
    if (!confirm("Bạn thực sự muốn xóa người dùng khỏi nhóm này ?")) {
        return false;
    }
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/PermissionManage/DeleteUser',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#nguoidung").html(data);
        }
    });
}

/** thiết lập **/
$("#btnSetupRole").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/PermissionManage/GetIdPermission',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modal-setup-role").modal("show");
        }
    });
});

$(".dataFunc").click(function () {
    var $t = $(this);
    //css .selectForm bên _Partial_SetupRole
    $('.dataFunc').not(this).removeClass('selectForm');
    $t.toggleClass('selectForm');
    $('#funcData').html('');
    $.getJSON('/PermissionManage/JsonFunction/' + $t.data('id'), function (data) {
        var items = '';
        var lstId = '';
        $.each(data, function (i, ward) {
            if (ward.ckeck) {
                items += "<input type='checkbox' checked value='" + ward.id + "' class='funcData' name='StaffSetupActionId' /> ";
                lstId += ward.id + ","
            }
            else
                items += "<input type='checkbox' value='" + ward.id + "' class='funcData' name='StaffSetupActionId' /> ";
            items += ward.name;
            items += "<br />";
        });
        $('#funcData').html(items);
        $("#listItemIdFunc").val(lstId);
        $(".funcData").change(function () {
            var items = '';
            $(".funcData").each(function () {
                var $this = $(this);
                if ($this.is(':checked')) {
                    items += $this.val() + ',';
                }
            });
            $("#listItemIdFunc").val(items);
        })
        $("input[type=radio][name=StaffSetupRoleId]").change(function () {
            //alert($(this).val())
            //if ($("#listItemIdFunc").val() != '') {
            var dataPost = {
                idDataBy: $(this).val(),
                lst: $("#listItemIdFunc").val()
            };
            $.ajax({
                type: "POST",
                url: '/PermissionManage/SaveSetupRole',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                }
            });
            // }
        })
        $("input[type=checkbox][name=StaffSetupActionId]").change(function () {
            //$("#listItemIdFunc").val()
            //if ($("#listItemIdFunc").val() != '') {
            var dataPost = {
                idDataBy: $("input[type=radio][name=StaffSetupRoleId]:checked").val(),
                lst: $("#listItemIdFunc").val()
            };
            $.ajax({
                type: "POST",
                url: '/PermissionManage/SaveSetupRole',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                }
            });
            ////   }
        })
    });
    $.getJSON('/PermissionManage/JsonShowDataBy/' + $t.data('id'), function (data) {
        if (data.id != 0) {
            $("input[name=StaffSetupRoleId][value='" + data.id + "']").prop("checked", true);
        } else {
            $("input[name=StaffSetupRoleId][value='1']").prop("checked", true);
        }
    })
})
//$("#btnLuuSetupRole").click(function () {
//    var dataPost = {
//        idDataBy: $("input[type=radio][name=StaffSetupRoleId]:checked").val(),
//        lst: $("#listItemIdFunc").val()
//    };
//    $.ajax({
//        type: "POST",
//        url: '/PermissionManage/SaveSetupRole',
//        data: JSON.stringify(dataPost),
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        success: function (data) {
//        }
//    });
//})