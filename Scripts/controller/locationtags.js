$("#selectTags").select2();
$("#selectTagsType").select2();

function updateRow2Colum() {
    $('ul li').find("input:checkbox").each(function () {
        if (this.checked) {
            var tr = $(this).next("label");
            var text = tr.text();
            var tr1 = $(this).next("iso");
            var text1 = tr1.text();
            tr.text("");
            tr1.text("");
            tr.append("<input type='text' id='txtName' value='" + text + "' class='form-control' />");
            tr.append("<input type='text' id='txtIsoCode' value='" + text1 + "' class='form-control' />");
        }
    });
    $("#btnAdd").attr("disabled", "disabled");
    $("#btnEdit").attr("disabled", "disabled");
    $("#btnSave").removeAttr("disabled", "disabled");
}

function CheckSelectTag() {
    var cb = 0;
    $('ul li').find("input:checkbox[name='cb']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnAdd").removeAttr("disabled", "disabled");
        $("#btnRemove").attr("disabled", "disabled");
    } else if (cb == 1) {
        $("#btnAdd").attr("disabled", "disabled");
        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnSave").attr("disabled", "disabled");
    }
}

$("#btnSave").click(function () {
    if ($("#txtName").val() == "") {
        alert("Dữ liệu không được để trổng!");
    }
    else {
        var dataPost = {
            id: $("input[type='checkbox']:checked").val(),
            name: $("#txtName").val(),
            isocode: $("#txtIsoCode").val()
        };

        $.ajax({
            type: "POST",
            url: '/LocationTagsManage/Update',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                window.location.href = '/LocationTagsManage/Index';
            }
        });
    }
})

$("#btnRemove").click(function () {
    var checkArray = new Array();
    $('ul li').find("input:checkbox[name='cb']").each(function () {
        if (this.checked) {
            checkArray.push($(this).val());
        }
    });
    var dataPost = { checkArray: checkArray };
    $.ajax({
        url: '/LocationTagsManage/Delete',
        type: 'POST',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert(data);
            window.location.href = '/LocationTagsManage/Index';
        }
    })
})

$('#selectTagsType').change(function () {
    $.getJSON('/LocationTagsManage/LoadTagByType/' + $('#selectTagsType').val(), function (data) {
        var items = '<option>-- Chọn --</option>"';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#selectTags').html(items);
    });
});