function showSuccessMessage(message = 'Saved Successfully!') {
    Swal.fire({
        icon: "success",
        title: "success",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}
function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    }); 
}
$(document).ready(function () {
    var message = $("#Message").text(); 
    if (message !== '')
    {
        showSuccessMessage(message);
    }
});