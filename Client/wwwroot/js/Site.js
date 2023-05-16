function menu_treeview() {
    $('[data-widget="treeview"]').Treeview('init')

    /** add active class and stay opened when selected */
    var url = window.location;

    // for sidebar menu entirely but not cover treeview
    $('ul.nav-sidebar a').filter(function () {
        return this.href == url;
    }).addClass('active');

    // for treeview
    $('ul.nav-treeview a').filter(function () {
        return this.href == url;
    }).parentsUntil(".nav-sidebar > .nav-treeview").addClass('menu-open').prev('a').addClass('active');
}

function maskCurrency() {
    Inputmask.extendAliases({
        'currency': {
            prefix: "",
            alias: 'numeric',
            digits: '*',
            digitsOptional: true,
            radixPoint: '.',
            groupSeparator: ',',
            autoGroup: true,
            placeholder: '0',
            min: '0',
            max: '999999999'
        }
    });

    $("[name='currency-mask']").inputmask({
        alias: "currency"
    });
}

function maskPercent() {
    Inputmask.extendAliases({
        'percent': {
            placeholder: "0",
            digits: 2,
            digitsOptional: false,
            radixPoint: ".",
            groupSeparator: ".",
            autoGroup: true,
            allowPlus: false,
            allowMinus: false,
            clearMaskOnLostFocus: false,
            removeMaskOnSubmit: true,

        }
    });

    $("[name='percent-mask']").inputmask({
        alias: "percent"
    });
}

function maskPeriod() {

    Inputmask.extendAliases({
        'period': {
            alias: "datetime",
            inputFormat: "yyyymm",
            placeholder: "yyyyMM",
        }
    });

    $("[name='mask-period']").inputmask({
        alias: 'period',
    });
}

function maskDate() {

/*    Inputmask().mask("input");*/

    Inputmask.extendAliases({
        'date': {
            alias: 'datetime',
            inputFormat: "dd/mm/yyyy",
            placeholder: "dd/mm/yyyy"
        }
    });

    $("[name='date-mask']").inputmask({
        alias: 'date',
        min: '01/01/1900',
        max: '01/01/2999'
    });
}

function maskTime() {

    Inputmask.extendAliases({
        'time': {
            alias: "datetime",
            inputFormat: "HH:MM",
            placeholder: "HH:mm",
            insertMode: false,
            hourFormat: "24"
        }
    });

    $("[name='time-mask']").inputmask({
        alias: 'time',
    });
}

function maskDateTime() {

    /*    Inputmask().mask("input");*/

    Inputmask.extendAliases({
        'date': {
            alias: 'datetime',
            inputFormat: "dd/mm/yyyy HH:MM",
            placeholder: "dd/mm/yyyy HH:mm"
        }
    });

    $("[name='datetime-mask']").inputmask({
        alias: 'date',
        min: '01/01/1900',
        max: '01/01/2999'
    });
}

function datepicker() {

    //$("#datepickerTerminateDate1").datetimepicker("destroy");

    //jQuery.datetimepicker.setLocale('vi');

    //jQuery('#datepickerTerminateDate1').datetimepicker({

    //    timepicker: false,
    //    format: 'd/m/Y',

    //    closeOnDateSelect: false,
    //    closeOnTimeSelect: true,
    //    closeOnWithoutClick: true,
    //    closeOnInputClick: true,

    //    onSelect: function (date) {

    //        var myElement = $(this)[0];

    //        var event = new Event('change');

    //        myElement.dispatchEvent(event);

    //    },

    //      onChangeDateTime: function (dp, $input) {
    //        alert($input.val())
    //    }

    //});

}

function bootrap_select() {
    $('select').selectpicker();
}

function bootrap_select_refresh() {
    $('.selectpicker').selectpicker('refresh');
}

function focusInputNextID(_inputNextID) {
    if (document.getElementById(_inputNextID) != null) {
        setTimeout("document.getElementById('" + _inputNextID + "').focus()", 100);
        setTimeout("document.getElementById('" + _inputNextID + "').select()", 100);
    }
}

function openInNewTab(_url) {
    if (_url != null) {
        window.open(_url, '_blank');
    }
}

function openWinNewTab(_url) {
    if (_url != null) {
        var strWindowFeatures = "location=yes,width=800,height=800,left=200,top=200,menubar=off,toolbar=off,scrollbars=yes,status=yes";
        window.open(_url, "_blank", strWindowFeatures);
    }
}

function CloseModal(id) {
    $(id).modal('hide');
}

function ShowModal(id) {
    $(id).modal();
}

//Cookie
function Cookie_User(name, value, days) {

    var expires;
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    }
    else {
        expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function updateScrollToBottom() {
    $('#updateScrollToBottom').animate({
        scrollTop: $('#updateScrollToBottom').get(0).scrollHeight
    }, 1000);
}

function keyPressNextInput() {
    $('#keyPressNextInput input').keyup(function (e) {
        if (e.which == 39) { // right arrow
            $(this).closest('td').next().find('input').focus();

        } else if (e.which == 37) { // left arrow
            $(this).closest('td').prev().find('input').focus();

        } else if (e.which == 40) { // down arrow
            $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
            $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').select();

        } else if (e.which == 38) { // up arrow
            $(this).closest('tr').prev().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
            $(this).closest('tr').prev().find('td:eq(' + $(this).closest('td').index() + ')').find('input').select();
        }
    });
}

function tooltip() {
    $('[data-toggle="tooltip"]').tooltip()
}






