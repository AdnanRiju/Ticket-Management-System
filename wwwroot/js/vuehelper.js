String.prototype.toNumber = function () {
    if (typeof this === undefined || this === null) {
        return 0;
    }
    var _val = parseFloat(this);
    return isNaN(_val) ? 0 : _val;
};

String.prototype.toInt = function () {
    if (typeof this === undefined || this === null) {
        return 0;
    }
    var _val = parseInt(this);
    return isNaN(_val) ? 0 : _val;
};

var helper = function () {
    var dateFormat = 'yyyy-mm-dd';
    var PhoneNoRegex = /^[+]*[(]{0,1}[0-9]{1,3}[)]{0,1}[-\s\./0-9]*$/g;

    var post = function (url, data, callback, async) {
        if (typeof async === "undefined" || async == null) {
            async = true;
        }
        //console.log(data);
        _post('POST', async, url, data, callback);
    };

    var get = function (url, data, callback, async) {
        _post('GET', async, url, data, callback);
    };

    var postFormData = function (url, formData, callback) {
        //console.log(formData);
        $.ajax({
            type: 'post',
            url: url,
            data: formData,
            //dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                return callback(response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                _handleError(jqXHR, textStatus, errorThrown);
            }
        });
    };

    var isNullOrEmpty = function (str) {
        if (typeof str === 'undefined')
            return true;

        if (str === null)
            return true;

        var string = str.toString();
        if (!string)
            return true;
        return string === null || string.match(/^ *$/) !== null;
    };

    var showModal = function (modalTitle, urlToLoad, data, modalSize, callback) {
        modalSize = modalSize || 'md';
        data = data || null;
        if (data != null) {
            urlToLoad = urlToLoad + '?' + $.param(data);
        }

        var d = new Date();
        var modalId = 'PUPModal-' + d.getTime();
        var html = '<div id="' + modalId + '" class="modal fade" aria-hidden="true" data-backdrop="static" data-keyboard="false">' +
            '<div class="modal-dialog">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" onclick="helper.hideModal()" class="close modal-close-button"></button>' +
            '<h4 class="modal-title"></h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        $('body').append(html);

        var title = $('#' + modalId + ' .modal-title');
        var body = $('#' + modalId + ' .modal-body');

        title.text(modalTitle);
        body.load(urlToLoad, function (responseText, textStatus, jqXHR) {
            if (jqXHR.status === 200) {
                $('#' + modalId + ' .modal-dialog').addClass(modalSize);
                $('#' + modalId + '.modal').modal('show');
                if (callback) {
                    callback();
                    uiInit();
                }
            }
            else {
                _handleError(jqXHR, textStatus, responseText);
            }
        });
    };

    var showContentOnModal = function (modalTitle, content, modalSize, callback) {
        modalSize = modalSize || 'md';
        var d = new Date();
        var modalId = 'PUPModal-' + d.getTime();
        var html = '<div id="' + modalId + '" class="modal fade" aria-hidden="true" data-backdrop="static" data-keyboard="false">' +
            '<div class="modal-dialog">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" onclick="helper.hideModal()" class="close modal-close-button"></button>' +
            '<h4 class="modal-title"></h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        $('body').append(html);

        $('#' + modalId + ' .modal-title').text(modalTitle);
        $('#' + modalId + ' .modal-body').append(content);

        $('#' + modalId + ' .modal-dialog').addClass(modalSize);
        $('#' + modalId + '.modal').modal('show');
    };

    var hideModal = function () {
        var idVal = 0;
        var id = $('body div.modal:last').attr('id');

        $(".modal[id^='PUPModal-']").each(function () {
            var modalId = $(this).attr('id');
            modalId = modalId.substring(9);
            if (parseInt(modalId) > idVal) {
                idVal = parseInt(modalId);
            }
        });
        var ctrl = '#PUPModal-' + idVal;
        $(ctrl).nextAll('.modal-backdrop:last').remove();
        $(ctrl).remove();

        //$('body div.modal:last').next('.modal-backdrop').remove();
        //$('body  div.modal:last').remove();
    };

    var load = function (control, url, data, callback) {
        if (data != null) {
            url = url + '?' + $.param(data);
        }
        $(control).load(url, function (responseText, textStatus, jqXHR) {
            if (jqXHR.status === 200) {
                //loadImage();
                if (callback) {
                    uiInit();
                    callback();
                }
            }
            else {
                _handleError(jqXHR, textStatus, responseText);
            }
        });
    };

    var confirmation = function (confrimMessage, callbackFunction) {        
    };

    var showMessage = function (messageObjectOrmessageType, messageString) {
        var messageType = '';
        var message = '';

        if (typeof messageString === 'undefined') {
            messageType = messageObjectOrmessageType['messageType'];
            message = messageObjectOrmessageType['messageString'];
            if (typeof messageType !== 'undefined' && typeof message !== 'undefined') {
                _openMessage(messageType, message);
            }
        }
        else {
            messageType = messageObjectOrmessageType;
            message = messageString;
            _openMessage(messageType, message);
        }
    };

    var _openMessage = function (messageType, message) {
        if (messageType == 1 || messageType == '1' || messageType == 'success') {
            toastr.success(message, "Success !", { closeButton: true });
        }
        else if (messageType == 2 || messageType == '2' || messageType == 'error') {
            toastr.error(message, "Error !", { closeButton: true });
        }
        else if (messageType == 3 || messageType == '3' || messageType == 'warning') {
            toastr.warning(message, "Warning !", { closeButton: true });
        }
    };

    var getFormValue = function (formControl) {
        var form = $(formControl);
        if (typeof form !== "undefined") {
            var obj = {};

            $(formControl + ' input ,' + formControl + ' select ,' + formControl + ' textarea').each(function () {
                if ($(this).is('[data-pi]')) {
                    var name = $(this).attr('name');
                    var property = $(this).data('pi');
                    var value;

                    if ($(this).is(':checkbox')) {
                        value = $(this).is(':checked');
                    }
                    else if ($(this).is(':radio')) {
                        value = $('input[name=' + name + ']:checked').val();
                    }
                    else {
                        value = $(this).val();
                    }

                    if (isNullOrEmpty(property)) {
                        obj[name] = value;
                    }
                    else {
                        obj[property] = value;
                    }
                }
            });

            return obj;
        }

        console.log('Form is undefined: site.js>GetFormValue');

        return null;
    };

    var objectToFormData = function (obj) {
        var formData = new FormData();
        for (var prop in obj) {
            if (!obj.hasOwnProperty(prop)) continue;
            formData.append(prop, obj[prop]);
        }
        return formData;
    };

    var showAlert = function (message, alertType, callBack) {
        swal({
            title: capitalize(alertType) + '!',
            text: message,
            type: alertType,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ok!'
        }).then(function (result) {
            if (result.value && callBack) {
                callBack();
            }
        });
    };

    var bindSelect = function (control, options) {
        var option = '';
        for (var i = 0; i < options.length; i++) {
            option += '<option value="' + options[i].Value + '">' + options[i].Text + '</option>'
        }

        $(control).select2('destroy').empty().append(option);
        $(control).select2();
    };

    var _post = function (type, async, url, data, callback) {        
        $.ajax({
            type: type,
            async: async,
            url: url,
            data: data,
            crossDomain: true,
            headers: { 'Access-Control-Allow-Origin': '*' },
            //contentType: "application/json; charset=utf-8",
            success: function (response) {
                callback(response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                _handleError(jqXHR, textStatus, errorThrown);
            }
        });
    };

    var isSuccess = function (obj) {
        if (typeof obj !== 'undefined') {
            var messageType = obj['messageType'];

            if (typeof messageType !== 'undefined') {
                if (messageType === 1 || messageType === 'true' || messageType === '1' || messageType === 'success') {
                    return true;
                }
            }
        }
        return false;
    };

    
    var toNumber = function (data) {
        if (isNullOrEmpty(data)) {
            return 0;
        }
        else {
            var _val = parseFloat(data);
            return isNaN(_val) ? 0 : _val;
        }
    };

    var uiInit = function () {
        $('[data-toggle="tooltip"]').tooltip();
    };

    var initDataTable = function (table) {
        $(table).dataTable({
            "language": {
                "lengthMenu": "Records per page _MENU_ ",
            },
            "aaSorting": [],
            "pageLength": 15,
            "aLengthMenu": [[10, 15, 25, 35, 50, 100, -1], [10, 15, 25, 35, 50, 100, "All"]]
        });
    };

    var _handleError = function (jqXHR, textStatus, errorThrown) {
        console.log(jqXHR)
        var msg = '';
        if (jqXHR.status === 0) {
            msg = 'Not connect.\n Verify Network.';
        } else if (jqXHR.status == 101) {
            window.location.href = '/Error/SessionExpired';
        } else if (jqXHR.status == 404) {
            //window.location.href = '/Shared/Error';
        } else if (jqXHR.status == 403) {
            window.location.href = '/Error/PermissionDenied';
        } else if (jqXHR.status == 401) {            
            setTimeout(function () {
                window.location.href = '/Login';
            }, 2000);
        } else if (jqXHR.status == 500) {
            msg = 'Internal Server Error, contact with system administrator';
        } else if (textStatus === 'parsererror') {
            msg = 'Requested JSON parse failed.';
        } else if (textStatus === 'timeout') {
            msg = 'Time out error.';
        } else if (textStatus === 'abort') {
            msg = 'Ajax request aborted.';
        } else {
            msg = 'Uncaught Error.\n' + jqXHR.responseText;
        }

        if ($('#xhr-error-dialog').length <= 0) {            
        }
    };

    var _globalEventHandler = function () {
        $(document).off('keypress', '.number-only').on('keypress', '.number-only', function (evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            var isOk = true;

            // Max Value Check
            if ($(this).attr('data-max-value')) {
                var value = $(this).val() + String.fromCharCode(charCode);

                var maxvalue = parseInt($(this).data('max-value'));
                var val = parseInt(value);
                if (val > maxvalue) {
                    isOk = false;
                }
            }

            // Max Length Check
            if ($(this).attr('data-max-length')) {
                var length = parseInt($(this).val().length) + 1;
                var maxlength = parseInt($(this).data('max-length'));
                if (length > maxlength) {
                    isOk = false;
                }
            }

            return isOk;
        });

        $(document).off('keypress', '.money-only').on('keypress', '.money-only', function (evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode

            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46 && charCode != 45)
                return false;

            else if (charCode == 46) {
                var val = $(this).val();
                if (val) {
                    var count = val.replace(/[^.]/g, "").length
                    if (count >= 1) {
                        return false;
                    }
                }
            }
            else if (charCode == 45) {
                if ($(this).hasClass('neg')) {
                    var val = $(this).val();
                    if (val) {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }

            var isOk = true;
            if ($(this).attr('data-max-value')) {
                var value = $(this).val() + String.fromCharCode(charCode);

                var maxvalue = parseFloat($(this).data('max-value'));
                var val = parseFloat(value);
                if (val > maxvalue) {
                    isOk = false;
                }
            }

            // Max Length Check
            if ($(this).attr('data-max-length')) {
                var length = parseInt($(this).val().length) + 1;
                var maxlength = parseInt($(this).data('max-length'));
                if (length > maxlength) {
                    isOk = false;
                }
            }

            return isOk;
        });

        jQuery(document).off('change', '#SetdSubModuleId').on('change', '#SetdSubModuleId', function () {
            var subModulId = $(this).val();
            document.location.href = '/Dashboard/SetSubModule?subModuleId=' + subModulId;
        });

        //Notification Handing
        $(document).off('click', '.nf-btn-showdetails').on('click', '.nf-btn-showdetails', function (e) {
            var $des = $(this).parent().parent().find('.nf-description');
            if ($des.hasClass('hidden')) {
                $des.addClass('show');
                $des.removeClass('hidden');
                var notificationId = $(this).parent().parent().data('id')
                post('/Dashboard/ReadNotification', { notificationId: notificationId }, function () {
                }, true, false, false, false);
            }
            else {
                $des.addClass('hidden');
                $des.removeClass('show');
            }
        });

        //Notification Handing
        $(document).off('click', '.nf-btn-read').on('click', '.nf-btn-read', function (e) {
            var e = this;
            var notificationId = $(this).parent().parent().data('id')
            post('/Dashboard/ReadNotification', { notificationId: notificationId }, function () {
                $(e).parent().parent().parent().remove();

                var nfCount = parseInt($('#notificationNumber').text().trim());
                nfCount = isNaN(nfCount) ? 0 : nfCount;
                if (nfCount > 0) {
                    nfCount = nfCount - 1;
                    $('#notificationNumber').text(nfCount);
                    if (nfCount == 0) {
                        $('#notificationNumber').addClass('display-hide');
                    }
                }
            }, true, false, false, false);
        });
    };

    //Notification Handing
    var viewNotifications = function () {
        get('/Dashboard/GetMyNotifications', {}, function (response) {
            if (response) {
                $('#notificationNumber').text(response.length);
                if (response.length > 0) {
                    $('#notificationNumber').removeClass('display-hide');
                }
                var nf = '';
                response.forEach(function (v) {
                    nf += '<li>' +
                        '<a data-id="' + v.NotificationId + '" href="javascript:;">' +
                        '<span class="time">' +
                        '<button data-toggle="tooltip" title="View description"  type= "button" class="btn green btn-xs nf-btn-showdetails" > <i class="fa fa-list"></i></button >' +
                        '<button data-toggle="tooltip" title="Mask as read" style="margin-left:3px;" type="button" class="btn green-jungle btn-xs nf-btn-read"><i class="fa fa-check"></i></button>' +
                        '</span>' +
                        '<span class="details"> ' + v.Title + '<br /><span class="nf-description hidden">' + v.Description + '</span></span >' +
                        '</a>' +
                        '</li>';
                });
                $('#lst-notifications').empty().append(nf);
            }
            else {
                $('#notificationNumber').text('0');
            }

            $('[data-toggle="tooltip"]').tooltip();
        }, true, false, false, false);
    };

    var money = function (selector) {
    };

    var blockUI = function (ctrl) {
        ctrl = isNullOrEmpty(ctrl) ? 'body' : ctrl;
        $(ctrl).loading({
            stoppable: false,
            zIndex: 10051,
            message: 'Please wait ...'
        });
    };

    var unBlockUI = function (ctrl) {
        ctrl = isNullOrEmpty(ctrl) ? 'body' : ctrl;
        $(ctrl).loading('stop');
    };

    var capitalize = function (str) {
        if (typeof str !== 'string') return '';
        str = str.toLowerCase();
        return str.charAt(0).toUpperCase() + str.slice(1);
    };

    var isMatch = function (str) {
        let isValid = true;
        if (str) {
            var found = str.match(PhoneNoRegex);
            isValid = found == null ? false : true;

            if (str.length < 7 || str.length > 16) {
                isValid = false;
            }
        }
        return isValid;
    }

    var init = function () {
        _globalEventHandler();
        // initializeDatePicker();
    };

    return {
        init: init,
        post: post,
        get: get,
        postFormData: postFormData,
        isNullOrEmpty: isNullOrEmpty,
        showModal: showModal,
        showContentOnModal: showContentOnModal,
        hideModal: hideModal,
        load: load,
        confirmation: confirmation,
        showMessage: showMessage,
        showAlert: showAlert,
        getFormValue: getFormValue,
        objectToFormData: objectToFormData,
        isSuccess: isSuccess,        
        bindSelect: bindSelect,
        uiInit: uiInit,
        initDataTable: initDataTable,
        viewNotifications: viewNotifications,
        toNumber: toNumber,
        _handleError: _handleError,
        blockUI: blockUI,
        unBlockUI: unBlockUI,
        dateFormat: dateFormat,
        isMatch: isMatch,
        PhoneNoRegex: PhoneNoRegex
    };
}();

var modalSize = function () {
    var lg = 'lg';
    var xl = 'xl';
    var md = 'md';
    var report = 'report';
    var xs = 'xs';
    var sm = 'sm';
    var none = '';

    return {
        lg: lg,
        xl: xl,
        md: md,
        report: report,
        sm: sm,
        xs: xs,
        none: none
    };
}();

$(document).ready(function () {
    helper.init();

    function readURL(input) {
        var id = $(input).data('preview');
        if (typeof id !== 'undefined') {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('#' + id).attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }
    }

    $(document).on('change', '.ci-image', function () {
        readURL(this);
    });

    $('.no-paste').bind('paste', function (e) {
        e.preventDefault(); //disable cut,copy,paste
    });
});

$.fn.isValid = function () {
    var form = $(this);
    form.validate();
    return form.valid();
};
if($.validator !== undefined){
    $.validator.setDefaults({
        errorPlacement: function (error, element) {
            if (element.hasClass('select2-hidden-accessible')) {
                error.insertAfter(element.next());
            }
            else if (element.hasClass('error-next')) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });
}


var AlertTypes = function () {
    var success = 'success';
    var error = 'error';
    var warning = 'warning';
    var info = 'info';
    var question = 'question';

    return {
        success: success,
        error: error,
        warning: warning,
        info: info,
        question: question
    };
}();