var MessageType = {
    Success: 200,
    Failed: 400,
    Unauthorized: 4001,
    IPBlacklisted: 4002,
    NotFound: 4003,
    DataExist: 4004,
    InvalidRequestFormat: 4005,
    RequiredParameterMissing: 4006,
    ExceptionOccured: 4007,
    UnknownError: 4010
};

function add(url) {
    $.get(url,
        function (result) {
            $('#MainModalBody').empty();
            $('#MainModalBody').html(result);
            //var inputtext = $('#MainModalBody').find(':input').filter(':input:visible:first');
            //console.log(inputtext);
            $('#MainModalBody').find('*').filter(':input:visible:first').focus();
            //inputtext.focus();
            $("#MainModal").modal('show');
        });
}
function addwithId(id, url) {
    $.get(url + id,
        function (result) {
            $('#MainModalBody').empty();
            $('#MainModalBody').html(result);
            $('#MainModalBody').find('*').filter(':input:visible:first').focus();
            $("#MainModal").modal('show');
        });
}
function edit(id, url) {
    $.get(url + id,
        function (result) {
            $('#MainModalBody').empty();
            $('#MainModalBody').html(result);
            $('#MainModalBody').find('*').filter(':input:visible:first').focus();
            $("#MainModal").modal('show');
        });
}
function loaddetails(id, url){
    $.get(url + id,
        function (result) {
            $('#MainModalBody').empty();
            $('#MainModalBody').html(result);
            $('#MainModalBody').find('*').filter(':input:visible:first').focus();
            $("#MainModal").modal('show');
        });
}
function deleteItem(id, url) {
    $.get(url + id,
        function (result) {
            ToastNotification(result);
            if (result.MessageType == MessageType.Success) {
                setTimeout(() => {
                    location.reload();
                }, 1000);
            }
        });
}
var results = $("#Results"); 
// jQuery plugin to prevent double submission of forms
jQuery.fn.preventDoubleSubmission = function () {
    $(this).on('submit', function (e) {
        var $form = $(this);

        if ($form.data('submitted') === true) {
            // Previously submitted - don't submit again
            e.preventDefault();
        } else {
            // Mark it so that the next submit can be ignored
            $form.data('submitted', true);
        }
    });

    // Keep chainability
    return this;
};
var onBegin = function () {
    $('form').preventDoubleSubmission();
    results.html("loading");
};

var onComplete = function () {
    results.html("stop");
};

var onSuccess = function (context) {
    ToastNotification(context);
};
var onSuccessRegister = function (context) {
    ToastNotification(context);
};
var onFailed = function (context) {
    ToastNotification(context);
};

function ToastNotification(result) {
    if (result.MessageType == MessageType.Success) {
        //toastr.success(result.StatusMessage);
        $(document).Toasts('create', {
            class: 'bg-success',
            title: 'Message',
            position: 'bottomRight',
            autohide: true,
            fade: true,
            delay: 3000,
            body: result.StatusMessage
        })
    }
    else if (result.MessageType == MessageType.DataExist) {
        /*toastr.info(result.StatusMessage);*/
        $(document).Toasts('create', {
            class: 'bg-info',
            title: 'Data Exist',
            position: 'bottomRight',
            autohide: true,
            fade: true,
            delay: 3000,
            body: result.StatusMessage
        });
        return false;
    }
    else if (result.MessageType == MessageType.NotFound || result.MessageType == MessageType.InvalidRequestFormat || result.MessageType == MessageType.RequiredParameterMissing) {
        //toastr.warning(result.StatusMessage);
        $(document).Toasts('create', {
            class: 'bg-warning',
            title: 'Error',
            position: 'bottomRight',
            autohide: true,
            delay: 3000,
            body: result.StatusMessage
        })
        return false;
    }
    else if (result.MessageType == MessageType.Unauthorized || result.MessageType == MessageType.IPBlacklisted || result.MessageType == MessageType.Failed) {
        //toastr.error(result.StatusMessage);
        $(document).Toasts('create', {
            class: 'bg-danger',
            title: 'Failed',
            position: 'bottomRight',
            autohide: true,
            delay: 3000,
            body: result.StatusMessage
        });
        return false;
    }
    else {
        //alertify.confirm(result.StatusMessage,function () { alertify.success('Ok') }).set({ title: "Error" });
        alertify.alert('Error', result.StatusMessage);
        //alertify.alert('Error', result.StatusMessage, function () { alertify.success('Ok') });
        return false;
    }
    return true;
}
function OnSuccessRequestNotification(result) {
    //Success: 200, Unauthorized: 4001, NotFound: 4002,IPBlacklisted: 4003, ExceptionOccured: 4004,InvalidRequestFormat: 4005, RequiredParameterMissing: 4022
    if (result.MessageType == MessageType.Success) {
        alertify.success(result.StatusMessage);
        //   //   alertify.confirm('Success',result.message, function(){ alertify.success('Success') });
        //     alertify.alert('Congratulation', result.message, function(){ console.log('success'); });
    }
    else if (result.MessageType == MessageType.NotFound || result.MessageType == MessageType.RequiredParameterMissing) {
        alertify.warning(result.StatusMessage);
        return false;
    }
    else if (result.MessageType == MessageType.Error || result.MessageType == MessageType.ExceptionOccured || result.MessageType == MessageType.InvalidRequestFormat) {
        alertify.error(result.StatusMessage);
        return false;
    }
    else {
        alertify.confirm('Something went wrong.For Detail, Please Check Console');
        return false;
    }
    return true;
}
function OnSuccessRequest(result) {
    if (result.messagetype == MessageType.Success) {
        // alertify.success('Success');
        //   alertify.confirm('Success',result.message, function(){ alertify.success('Success') });
        alertify.alert('Congratulation', result.message, function () { console.log('success'); });
    }
    else if (result.messagetype == MessageType.Failed) {
        alertify.error(result.Message);
        return false;
    }
    //   else if (result.messagetype == MessageType.Failed) {
    //         alertify.warning(result.Message);
    //         return false;
    //     }
    else {
        alertify.confirm('Something went wrong.For Detail, Please Check Console');
        return false;
    }
    return true;
}


$(function () {
    // Code here
    $('.trigger-reload').on('click', () => {
        setTimeout(() => {
            location.reload()
        }, 300);
    });
});