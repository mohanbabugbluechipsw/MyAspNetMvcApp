

        document.addEventListener('DOMContentLoaded', function() {
                // Initialize Select2 with proper placeholder handling
                const initSelect2 = () => {
                    $('.select2').each(function() {
                        $(this).select2({
                            placeholder: $(this).data('placeholder') || "Select an option",
                            allowClear: true,
                            width: '100%',
                            dropdownParent: $(this).closest('.card-body')
                        });
                    });
                };

                // Initialize Date Pickers using Flatpickr API
                const initDatePickers = () => {
                    flatpickr(".datepicker", {
                        allowInput: true,
                        clickOpens: true,
                        dateFormat: "Y-m-d",
                        altInput: true,
                        altFormat: "F j, Y",
                        minDate: "2000-01-01",
                        maxDate: new Date()
                    });
                };



                // MR Dropdown Change Handler
                $('#mrDropdown').on('change', function() {
                    const mrId = $(this).val();
                    $('#rsCodeInput').val(''); // Clear immediately

                    if (mrId) {
                        $.getJSON('/OSA/GetRsCodes', { mrId: mrId })
                            .done(function(data) {
                                if (data?.length > 0) {
                                    $('#rsCodeInput').val(data[0].value);
                                } else {
                                    Swal.fire({
                                        icon: 'info',
                                        title: 'No RS Code Found',
                                        text: 'No associated RS Code for selected MR',
                                        confirmButtonColor: '#4e73df'
                                    });
                                }
                            })
                            .fail(function() {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error',
                                    text: 'Failed to fetch RS Codes',
                                    confirmButtonColor: '#4e73df'
                                });
                            });
                    }
                });

                // Event Listeners
                // $('#resetButton').on('click', resetForm);

                        window.resetForm = function() {
                // Redirect to OSA/Index
            window.location.href = redirectToIndexUrl;
            };

            // Event listener for reset button
            document.getElementById('resetButton').addEventListener('click', resetForm);


                // Initializations
                initSelect2();
                initDatePickers();
            });

        $(document).ready(function() {


            // Client-side pagination and sorting implementation
            if ($('#osaResultsTable').length) {
                const table = $('#osaResultsTable');
                let allRows = table.find('tbody tr').get();
                let currentSort = {
                    column: null,
                    direction: 'asc'
                };
                let recordsPerPage = 10;
                let currentPage = 1;
                let totalRecords = allRows.length;
                let totalPages = Math.ceil(totalRecords / recordsPerPage);

                // Initialize table
                updatePagination();
                showPage(currentPage);

                // Rows per page dropdown
                $('.dropdown-item').on('click', function(e) {
                    e.preventDefault();
                    recordsPerPage = parseInt($(this).data('value'));
                    $('#currentRowsPerPage').text(recordsPerPage);
                    totalPages = Math.ceil(totalRecords / recordsPerPage);
                    currentPage = 1;
                    updatePagination();
                    showPage(currentPage);
                });

                // Sort function
                function sortTable(column, direction) {
                    allRows.sort(function(a, b) {
                        const aVal = $(a).find(`td[data-${column}]`).data(column);
                        const bVal = $(b).find(`td[data-${column}]`).data(column);

                        // Special handling for dates (stored as ticks)
                        if (column === 'date') {
                            return direction === 'asc' ? aVal - bVal : bVal - aVal;
                        }

                        // Numeric comparison if both values are numbers
                        if (!isNaN(aVal) && !isNaN(bVal)) {
                            return direction === 'asc' ? aVal - bVal : bVal - aVal;
                        }

                        // String comparison
                        const aText = String(aVal || '').toLowerCase();
                        const bText = String(bVal || '').toLowerCase();

                        if (aText < bText) {
                            return direction === 'asc' ? -1 : 1;
                        }
                        if (aText > bText) {
                            return direction === 'asc' ? 1 : -1;
                        }
                        return 0;
                    });

                    // Reattach sorted rows
                    $.each(allRows, function(index, row) {
                        table.find('tbody').append(row);
                    });
                }

                // Sorting event handler
                $('.sortable').on('click', function() {
                    const column = $(this).data('sort');
                    const $icon = $(this).find('.fa-sort');

                    // Reset all sort indicators
                    $('.sortable').removeClass('sort-asc sort-desc');
                    $('.sortable .fa-sort').removeClass('fa-sort-up fa-sort-down');

                    // Determine new sort direction
                    if (currentSort.column === column) {
                        currentSort.direction = currentSort.direction === 'asc' ? 'desc' : 'asc';
                    } else {
                        currentSort.column = column;
                        currentSort.direction = 'asc';
                    }

                    // Update UI indicators
                    $(this).addClass(currentSort.direction === 'asc' ? 'sort-asc' : 'sort-desc');
                    $icon.addClass(currentSort.direction === 'asc' ? 'fa-sort-up' : 'fa-sort-down');

                    // Sort the table
                    sortTable(column, currentSort.direction);

                    // After sorting, show first page of newly ordered rows
                    currentPage = 1;
                    updatePagination();
                    showPage(currentPage);
                });

                // Show page function
                function showPage(page) {
                    table.find('tbody tr').hide();
                    const start = (page - 1) * recordsPerPage;
                    const end = start + recordsPerPage;
                    $(allRows.slice(start, end)).show();

                    // Update pagination controls
                    $('#showingFrom').text(start + 1);
                    $('#showingTo').text(Math.min(end, totalRecords));
                    $('#totalRecords').text(totalRecords);

                    // Update active page button
                    $('.pagination li').removeClass('active');
                    $(`.pagination li:eq(${page})`).addClass('active');

                    // Enable/disable prev/next buttons
                    $('#prevPage').toggleClass('disabled', page === 1);
                    $('#nextPage').toggleClass('disabled', page === totalPages || totalPages === 0);
                }

                // Update pagination buttons
                function updatePagination() {
                    const pagination = $('.pagination');
                    pagination.find('li:not(#prevPage):not(#nextPage)').remove();

                    // Determine range of pages to show
                    let startPage = Math.max(1, currentPage - 2);
                    let endPage = Math.min(totalPages, currentPage + 2);

                    // Adjust if we're at the beginning or end
                    if (currentPage <= 3) {
                        endPage = Math.min(5, totalPages);
                    }
                    if (currentPage >= totalPages - 2) {
                        startPage = Math.max(1, totalPages - 4);
                    }

                    // Add page numbers
                    for (let i = startPage; i <= endPage; i++) {
                        if (i === 1 && startPage > 1) {
                            pagination.find('#nextPage').before(
                                `<li class="page-item ${i === currentPage ? 'active' : ''}"><a class="page-link" href="#">1</a></li>`
                            );
                            if (startPage > 2) {
                                pagination.find('#nextPage').before(
                                    `<li class="page-item disabled"><a class="page-link" href="#">...</a></li>`
                                );
                            }
                            continue;
                        }
                        if (i === totalPages && endPage < totalPages) {
                            if (endPage < totalPages - 1) {
                                pagination.find('#nextPage').before(
                                    `<li class="page-item disabled"><a class="page-link" href="#">...</a></li>`
                                );
                            }
                            pagination.find('#nextPage').before(
                                `<li class="page-item ${i === currentPage ? 'active' : ''}"><a class="page-link" href="#">${totalPages}</a></li>`
                            );
                            continue;
                        }
                        pagination.find('#nextPage').before(
                            `<li class="page-item ${i === currentPage ? 'active' : ''}"><a class="page-link" href="#">${i}</a></li>`
                        );
                    }
                }

                // Page click handler
                $(document).on('click', '.page-item:not(.disabled) a', function(e) {
                    e.preventDefault();
                    const text = $(this).text().toLowerCase();
                    const $item = $(this).closest('.page-item');

                    if ($item.is('#prevPage')) {
                        if (currentPage > 1) {
                            currentPage--;
                            showPage(currentPage);
                            updatePagination();
                        }
                    } else if ($item.is('#nextPage')) {
                        if (currentPage < totalPages) {
                            currentPage++;
                            showPage(currentPage);
                            updatePagination();
                        }
                    } else if (!isNaN(parseInt(text))) {
                        currentPage = parseInt(text);
                        showPage(currentPage);
                        updatePagination();
                    }
                });
            }
        });