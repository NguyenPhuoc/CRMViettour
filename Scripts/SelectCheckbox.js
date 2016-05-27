$(document).ready(function () {
    $("#btnSave").attr("disabled", "disabled");
    CheckSelect();
    CheckSelectCus();
    CheckSelectVisa();


});

function SelectAllCb() {
    var isChecked = $('#allcb').is(':checked');

    if (isChecked) {
        $("input:checkbox[name='cb']").prop("checked", true);

        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnCreateMap").removeAttr("disabled", "disabled");
        $("#btnAddUser").removeAttr("disabled", "disabled");
        $("#btnSetupRole").removeAttr("disabled", "disabled");
        $("#btnForm").removeAttr("disabled", "disabled");
        $("#btnRemove").attr("disabled", "disabled");
        $("#btnPassword").attr("disabled", "disabled");
        $("#btnLock").attr("disabled", "disabled");
        $("#btnUnLock").attr("disabled", "disabled");
        $("#btnAssign").removeAttr("disabled", "disabled");
        $("#btnWork").removeAttr("disabled", "disabled");
        $("#btnFinish").removeAttr("disabled", "disabled");
        $("#btnAddCustomer").removeAttr("disabled", "disabled");
        $("#btnUserSupport").removeAttr("disabled", "disabled");
        $("#btnCreateTour").removeAttr("disabled", "disabled");
        $("#btnSendDocument").attr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnUpdateType").removeAttr("disabled", "disabled");
    }
    else {
        $("input:checkbox[name='cb']").prop("checked", false);

        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnCreateMap").removeAttr("disabled", "disabled");
        $("#btnAddUser").removeAttr("disabled", "disabled");
        $("#btnSetupRole").removeAttr("disabled", "disabled");
        $("#btnForm").removeAttr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
        $("#btnAssign").removeAttr("disabled", "disabled");
        $("#btnWork").removeAttr("disabled", "disabled");
        $("#btnFinish").removeAttr("disabled", "disabled");
        $("#btnAddCustomer").removeAttr("disabled", "disabled");
        $("#btnUserSupport").removeAttr("disabled", "disabled");
        $("#btnCreateTour").removeAttr("disabled", "disabled");
        $("#btnSendDocument").removeAttr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnUpdateType").removeAttr("disabled", "disabled");
    }
    CheckSelect();
}

function CheckSelect() {
    var cb = 0;
    $('tbody tr td').find("input:checkbox[name='cb']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnCreateMap").attr("disabled", "disabled");
        $("#btnAdd").removeAttr("disabled", "disabled");
        $("#btnAddUser").attr("disabled", "disabled");
        $("#btnSetupRole").attr("disabled", "disabled");
        $("#btnRemove").attr("disabled", "disabled");
        $("#btnForm").attr("disabled", "disabled");
        $("#btnAddSchedule").attr("disabled", "disabled");
        $("#btnAddTask").attr("disabled", "disabled");
        $("#btnPassword").attr("disabled", "disabled");
        $("#btnLock").attr("disabled", "disabled");
        $("#btnUnLock").attr("disabled", "disabled");
        $("#btnAssign").attr("disabled", "disabled");
        $("#btnWork").attr("disabled", "disabled");
        $("#btnFinish").attr("disabled", "disabled");
        $("#btnAddCustomer").attr("disabled", "disabled");
        $("#btnUserSupport").attr("disabled", "disabled");
        $("#btnCreateTour").attr("disabled", "disabled");
        $("#btnSendDocument").attr("disabled", "disabled");
        $("#btnUpdate").attr("disabled", "disabled");
        $("#btnUpdateType").attr("disabled", "disabled");
    } else if (cb == 1) {
        $("#btnAdd").attr("disabled", "disabled");
        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnCreateMap").removeAttr("disabled", "disabled");
        $("#btnAddUser").removeAttr("disabled", "disabled");
        $("#btnSetupRole").removeAttr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
        $("#btnForm").removeAttr("disabled", "disabled");
        $("#btnSave").removeAttr("disabled", "disabled");
        $("#btnAddSchedule").removeAttr("disabled", "disabled");
        $("#btnAddTask").removeAttr("disabled", "disabled");
        $("#btnAssign").removeAttr("disabled", "disabled");
        $("#btnWork").removeAttr("disabled", "disabled");
        $("#btnFinish").removeAttr("disabled", "disabled");
        $("#btnAddCustomer").removeAttr("disabled", "disabled");
        $("#btnUserSupport").removeAttr("disabled", "disabled");
        $("#btnCreateTour").removeAttr("disabled", "disabled");
        $("#btnSendDocument").removeAttr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnUpdateType").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnCreateMap").attr("disabled", "disabled");
        $("#btnAddUser").attr("disabled", "disabled");
        $("#btnSetupRole").attr("disabled", "disabled");
        $("#btnForm").attr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
        $("#btnSave").attr("disabled", "disabled");
        $("#btnAddSchedule").attr("disabled", "disabled");
        $("#btnAddTask").attr("disabled", "disabled");
        $("#btnAssign").attr("disabled", "disabled");
        $("#btnWork").attr("disabled", "disabled");
        $("#btnFinish").attr("disabled", "disabled");
        $("#btnAddCustomer").attr("disabled", "disabled");
        $("#btnUserSupport").attr("disabled", "disabled");
        $("#btnCreateTour").attr("disabled", "disabled");
        $("#btnSendDocument").removeAttr("disabled", "disabled");
        $("#btnUpdate").attr("disabled", "disabled");
        $("#btnUpdateType").attr("disabled", "disabled");
    }
}

//Tab khách hàng trong quản lý tour

function SelectAllCheckCus() {
    var isChecked = $('#allcheck').is(':checked');

    if (isChecked) {
        $("input:checkbox[name='check']").prop("checked", true);
        $("#btnUpdateCustomer").attr("disabled", "disabled");
        $("#btnRemoveLichHen").attr("disabled", "disabled");
        $("#btnEditLichHen").attr("disabled", "disabled");
    }
    else {
        $("input:checkbox[name='check']").prop("checked", false);
        $("#btnUpdateCustomer").removeAttr("disabled", "disabled");
        $("#btnRemoveLichHen").removeAttr("disabled", "disabled");
        $("#btnEditLichHen").removeAttr("disabled", "disabled");
    }
    CheckSelectCus();
}

function CheckSelectCus() {
    var cb = 0;
    $('tbody tr td').find("input:checkbox[name='check']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnUpdateCustomer").attr("disabled", "disabled");
        $("#btnRemoveLichHen").attr("disabled", "disabled");
        $("#btnEditLichHen").attr("disabled", "disabled");
    } else if (cb == 1) {
        $("#btnUpdateCustomer").removeAttr("disabled", "disabled");
        $("#btnRemoveLichHen").removeAttr("disabled", "disabled");
        $("#btnEditLichHen").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnUpdateCustomer").removeAttr("disabled", "disabled");
        $("#btnRemoveLichHen").removeAttr("disabled", "disabled");
        $("#btnEditLichHen").attr("disabled", "disabled");
    }
}

// tab visa trong quản lý tour
function SelectAllCheckVisa() {
    var isChecked = $('#allcheckvisa').is(':checked');

    if (isChecked) {
        $("input:checkbox[name='checkvisa']").prop("checked", true);
        $("#btnUpdateVisa").attr("disabled", "disabled");
        //$("#btnUpdateVisa").removeAttr("href", "#modal-update-visa");
        $("#btnRemoveDocument").attr("disabled", "disabled");
    }
    else {
        $("input:checkbox[name='checkvisa']").prop("checked", false);
        $("#btnUpdateVisa").removeAttr("disabled", "disabled");
        //$("#btnUpdateVisa").attr("href", "#modal-update-visa");
        $("#btnRemoveDocument").removeAttr("disabled", "disabled");
    }
    CheckSelectVisa();
}

function CheckSelectVisa() {
    var cb = 0;
    $('tbody tr td').find("input:checkbox[name='checkvisa']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnUpdateVisa").attr("disabled", "disabled");
        //$("#btnUpdateVisa").removeAttr("href", "#modal-update-visa");
        $("#btnRemoveDocument").attr("disabled", "disabled");
    } else if (cb == 1) {
        $("#btnUpdateVisa").removeAttr("disabled", "disabled");
        //$("#btnUpdateVisa").attr("href", "#modal-update-visa");
        $("#btnRemoveDocument").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnUpdateVisa").removeAttr("disabled", "disabled");
        //$("#btnUpdateVisa").attr("href", "#modal-update-visa");
        $("#btnRemoveDocument").removeAttr("disabled", "disabled");
    }
}

// thêm form, tính năng trong phân quyền
function SelectAllCbForm() {
    var isChecked = $('#allcbform').is(':checked');

    if (isChecked) {
        $("input:checkbox[name='cbform']").prop("checked", true);
        $("#btnTinhNang").attr("disabled", "disabled");
    }
    else {
        $("input:checkbox[name='cbform']").prop("checked", false);
        $("#btnTinhNang").removeAttr("disabled", "disabled");
    }
    CheckSelectForm();
}

function CheckSelectForm() {
    var cb = 0;
    $('tbody tr td').find("input:checkbox[name='cbform']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnTinhNang").attr("disabled", "disabled");
    } else if (cb == 1) {
        $("#btnTinhNang").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnTinhNang").attr("disabled", "disabled");
    }
}