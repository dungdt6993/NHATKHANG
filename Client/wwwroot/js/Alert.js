function Swal_Confirm(title, message, type) {
    return new Promise((resolve) => {
        Swal.fire({
            icon: type,
            title: title,
            text: message,
            type: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.value) {
                resolve(true);
            } else {
                resolve(false);
            }
        });
    });
}

function Swal_Alert(title, type) {
    Swal.fire({
        position: 'top-end',
        icon: 'success',
        title: title,
        type: type,
        showConfirmButton: false,
        timer: 1000
    })
}

function Toast_Alert(title, type) {
    toastMixin.fire({
        animation: true,
        title: title,
        icon: type
    });
}

var toastMixin = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top',
    showConfirmButton: false,
    timer: 2000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});