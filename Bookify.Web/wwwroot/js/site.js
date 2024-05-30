var updatedRow; // Variable to keep track of the row being updated

function showSuccessMessage(message = 'Saved Successfully!') {
    // Function to show a success message using SweetAlert
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
    // Function to show an error message using SweetAlert
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    }); 
}

function onModalSuccess(item) {
    // Function to handle the success of modal operations
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow === undefined) {
        // If no row is being updated, append the new item to the table
        $('tbody').append(item);
    } else {
        // If a row is being updated, replace it with the new item
        $(updatedRow).replaceWith(item);
        updatedRow = undefined; // Reset the updatedRow variable
    }

    KTMenu.init(); // Reinitialize the KTMenu
    KTMenu.initHandlers(); // Reinitialize KTMenu handlers
}

$(document).ready(function () {
    var message = $('#Message').text(); 
    if (message !== '') {
        showSuccessMessage(message); // Show success message if present
    }

    // Handle bootstrap modal
    // Add event listener on clicks that occur on elements with class .js-render-modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title')); // Set the modal title

        if (btn.data('update') !== undefined) {
            // If the button has data-update attribute, set updatedRow to the parent row
            updatedRow = btn.parents('tr');
            console.log(updatedRow); // Log the updated row
        }

        $.get({
            url: btn.data('url'), // Dynamic URL from the button's data-url attribute
            success: function (form) {
                modal.find('.modal-body').html(form); // Load the form into the modal body
                $.validator.unobtrusive.parse(modal); // Parse the form for unobtrusive validation
            },
            error: function () {
                showErrorMessage(); // Show error message on failure
            }
        });
        modal.modal('show'); // Show the modal
    });
});
