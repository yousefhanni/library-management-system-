var table;
var datatable;
var updatedRow; // Variable to keep track of the row being updated
var exportedCols = []; // Array to store indices of columns to be exported

function showSuccessMessage(message = 'Saved Successfully!') {
    // Function to show a success message using SweetAlert
    Swal.fire({
        icon: "success",
        title: "Success",
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
function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalSuccess(row) {
    // Function to handle the success of modal operations
    showSuccessMessage(); // Show success message
    $('#Modal').modal('hide'); // Hide the modal

    // If updating an existing row
    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw(); // Remove the old row from the datatable
        updatedRow = undefined; // Reset the updatedRow variable
    }
    // Add or update the row
    var newRow = $(row);
    datatable.row.add(newRow).draw(); // Add the new row to the datatable

    KTMenu.init(); // Reinitialize KTMenu
    KTMenu.initHandlers(); // Reinitialize KTMenu handlers
}
function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}


// Initialize the array of column indices to be exported
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});

// Class definition for DataTables
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Initialize the datatable
        datatable = $(table).DataTable({
            "info": false, // Disable info display
            'pageLength': 10, // Set default page length
        });
    }

    // Hook up export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-datatables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols // Export only specified columns
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook export menu buttons to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get the clicked export button value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger the click event on the hidden datatable export button
                target.click();
            });
        });
    }

    // Handle search in the datatable
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw(); // Perform search and redraw the table
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable(); // Initialize the datatable
            exportButtons(); // Set up export buttons
            handleSearchDatatable(); // Set up search functionality
        }
    };
}();

$(document).ready(function () {
    // Display success message if present
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    // Initialize DataTables when the DOM is fully loaded
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    // Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title')); // Set the modal title

        if (btn.data('update') !== undefined) {
            // If updating, set updatedRow to the parent row of the button
            updatedRow = btn.parents('tr');
        }

        // Load the form into the modal
        $.get({
            url: btn.data('url'), // Dynamic URL for the form
            success: function (form) {
                modal.find('.modal-body').html(form); // Load form into modal body
                $.validator.unobtrusive.parse(modal); // Parse for unobtrusive validation
            },
            error: function () {
                showErrorMessage(); // Show error message on failure
            }
        });
        modal.modal('show'); // Show the modal
    });
    // Handle Toggle Status button click
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this); // The clicked button
        // Use bootbox.js for confirmation dialog
        bootbox.confirm({
            message: 'Are you sure that need to toggle this item status?', // Fixed the missing quote
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                // If user confirms
                if (result) {
                    // Make request to the server to toggle the status without reloading the page.
                    // The AJAX request is handled by a server-side controller or endpoint.
                    $.ajax({
                        url: btn.data('url'), // URL of ToggleStatus action
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        method: 'POST', // depending on your server-side implementation

                        // Update the UI based on the server response, providing visual feedback to confirm the status toggle.
                        success: function (lastUpdatedOn) {
                            // Handle the success response here
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');
                            // Call showSuccessMessage from site.js to display a "Saved Successfully!" alert dynamically.
                            showSuccessMessage();
                        },
                        error: function () {
                            // Call showErrorMessage from site.js to display a "Something went wrong!" alert dynamically.
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
});
