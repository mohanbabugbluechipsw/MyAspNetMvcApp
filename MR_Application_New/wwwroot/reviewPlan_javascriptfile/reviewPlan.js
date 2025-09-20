






        // Global variables
    let selectedSrs = { };
    let selectedSr = "";
    let selectedRoute = "";
    let currentOutlet = null;
    const srModal = new bootstrap.Modal('#srModal');
    const routeFilterModal = new bootstrap.Modal('#routeFilterModal');
    const confirmationModal = new bootstrap.Modal('#confirmationModal');
    const imagePreviewModal = new bootstrap.Modal('#imagePreviewModal');

    // Utility Functions
    function showAlert(message, icon = 'warning') {
        let title;
    switch (icon) {
                case 'error': title = 'Error!'; break;
    case 'success': title = 'Success!'; break;
    case 'info': title = 'Information'; break;
    case 'warning':
    default: title = 'Warning!'; break;
            }
    return Swal.fire({icon, title, text: message, confirmButtonColor: '#3085d6' });
        }

    function showLoading(message) {
            return Swal.fire({
        title: message,
    allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });
        }

    function getInitials(name) {
            return name?.split(' ')
                .map(part => part.charAt(0).toUpperCase())
    .join('')
    .substring(0, 2) || 'NA';
        }

    function updateSelectionCount() {
            const count = Object.keys(selectedSrs).length;
    $('#selectedCount').text(`${count} selected`);
    $('#selectSr').prop('disabled', count === 0);
        }

    function getDeviceType() {
            const ua = navigator.userAgent;
    if (/Mobi|Android|iPhone|iPad|iPod|Tablet|IEMobile|Mobile/i.test(ua)) {
                return /Tablet|iPad/i.test(ua) ? "Tablet" : "Mobile";
            }
    return "Desktop";
        }

    // Photo Functions
    function initPhotoHandlers() {
        // Camera functionality
        document.getElementById('openCameraBtn').addEventListener('click', function () {
            document.getElementById('cameraInput').click();
        });

    document.getElementById('cameraInput').addEventListener('change', async function (e) {
                if (e.target.files && e.target.files[0]) {
                    const originalFile = e.target.files[0];

    // Check if the file is an image
    if (!originalFile.type.startsWith('image/')) {
        showAlert('Please select an image file.', 'error');
    return;
                    }

    try {
                        // Resize/compress
                        const compressedBlob = await resizeImage(originalFile, 1024, 0.7);

    // Convert Blob to File (required for proper filename on server/SAS)
    const compressedFile = new File([compressedBlob], originalFile.name.replace(/\s+/g, "_"), {type: 'image/jpeg' });

    // Show preview
    const reader = new FileReader();
                        reader.onload = event => {
                            const img = document.getElementById('capturedImage');
    img.src = event.target.result;
    document.getElementById('photoPreviewContainer').classList.remove('d-none');
    document.getElementById('photoError').classList.add('d-none');
    document.getElementById('photoCaptured').value = 'true';
    checkFormCompletion();
                        };
    reader.readAsDataURL(compressedFile);

    // Upload
    await uploadPhoto(compressedFile);
                    } catch (err) {
        console.error('Image processing error:', err);
    showAlert('Failed to process the photo: ' + err.message, 'error');
                    }
                }
            });

    // Click to view larger image
    $(document).on('click', '#capturedImage', function () {
        $('#modalPreviewImage').attr('src', $(this).attr('src'));
    imagePreviewModal.show();
            });

    // Retake photo functionality
    document.getElementById('retakePhotoBtn').addEventListener('click', function () {
        // Clear the file input
        document.getElementById('cameraInput').value = '';

    // Hide preview
    document.getElementById('photoPreviewContainer').classList.add('d-none');

    // Reset photo captured flag
    document.getElementById('photoCaptured').value = '';

    // Show error if trying to proceed without photo
    checkFormCompletion();
            });
        }

    // Resize & compress function (returns Blob)
    async function resizeImage(file, maxWidth = 1024, quality = 0.7) {
            return new Promise((resolve, reject) => {
                const img = new Image();
    const reader = new FileReader();

                reader.onload = e => {img.src = e.target.result; };

                img.onload = () => {
        let width = img.width;
    let height = img.height;
                    if (width > maxWidth) {
        height = (maxWidth / width) * height;
    width = maxWidth;
                    }

    const canvas = document.createElement('canvas');
    canvas.width = width;
    canvas.height = height;
    canvas.getContext('2d').drawImage(img, 0, 0, width, height);

    canvas.toBlob(
                        blob => resolve(blob),
    'image/jpeg',
    quality
    );
                };

                reader.onerror = err => reject(err);
    reader.readAsDataURL(file);
            });
        }

    // Upload function (accepts File)
    async function uploadPhoto(file) {
            try {
                const loadingSwal = showLoading('Uploading photo...');

    const today = new Date();
    const dateFolder = today.toISOString().slice(0, 10).replace(/-/g, "");
    const folderName = `${dateFolder}_ReviewPhoto`;
    const uniqueId = `${Date.now()}_${Math.floor(Math.random() * 10000)}`;
    const fileName = `${folderName}/review_${uniqueId}.png`;
    const container = "reviewphoto";

    // Request SAS URL
    const sasResponse = await fetch(`/api/Blob/GetUploadSas?fileName=${encodeURIComponent(fileName)}&container=${container}`);
    if (!sasResponse.ok) throw new Error('Failed to get upload URL');

    const sasData = await sasResponse.json();
    if (!sasData.uploadUrl || !sasData.viewUrl) throw new Error('Invalid upload response');

    // Upload via PUT
    const uploadResponse = await fetch(sasData.uploadUrl, {
        method: 'PUT',
    headers: {"x-ms-blob-type": "BlockBlob" },
    body: file
                });

    loadingSwal.close();

    if (uploadResponse.ok) {
        $('#photoCaptured').val(sasData.viewUrl);
    console.log("Photo uploaded successfully: " + sasData.viewUrl);
                } else {
                    throw new Error(await uploadResponse.text());
                }
            } catch (err) {
        console.error("Upload Error:", err);
    showAlert('Failed to upload the photo: ' + err.message, 'error');
    document.getElementById('photoPreviewContainer').classList.add('d-none');
    document.getElementById('photoCaptured').value = '';
            }
        }

    // Route Filter Functions
    async function fetchRouteNames(rscode, srName) {
            if (!rscode) {
        await showAlert('RS Code is required.', 'warning');
    return;
            }

    const loadingSwal = showLoading('Loading routes...');
    try {
                const mrName = $('#mrNameInput').val().trim();
    const response = await $.ajax({
        url: '/ReviewPlane/GetRouteNameOutlet',
    type: 'GET',
    data: {rscode, srname: srName || null, mrCode: mrName || null },
    dataType: 'json'
                });

    const $routeList = $('#routeList .list-group').empty();

                if (response?.length > 0) {
        response.forEach(route => {
            const isSelected = selectedRoute === route.beat;
            $routeList.append(`
                            <div class="route-option list-group-item list-group-item-action ${isSelected ? 'selected' : ''}" 
                                 data-route="${route.beat}">
                                <i class="fas fa-route me-2"></i>${route.beat}
                            </div>
                        `);
        });
    routeFilterModal.show();
                } else {
        $routeList.append('<div class="list-group-item text-muted">No routes available</div>');
                }
            } catch (error) {
        console.error('Error loading routes:', error);
    $('#routeList .list-group').empty().append('<div class="list-group-item text-danger">Error loading routes</div>');
    showAlert('Failed to load routes: ' + (error.responseJSON?.message || error.message), 'error');
            } finally {
        loadingSwal.close();
            }
        }

    function applyRouteFilter() {
            const selectedRouteOption = $('#routeList .route-option.selected');
    if (!selectedRouteOption.length) {
        showAlert('Please select a route', 'warning');
    return;
            }

    selectedRoute = selectedRouteOption.data('route');
    $('#selectedRouteInput').val(selectedRoute);
    $('#selectedRoute').val(selectedRoute);
    routeFilterModal.hide();
    showAlert(`Route filter applied: ${selectedRoute}`, 'success');
        }

    function clearRouteFilter() {
        selectedRoute = "";
    $('#selectedRouteInput').val("");
    $('#selectedRoute').val("");
    showAlert('Route filter cleared', 'success');
        }

    // SR Functions
    async function fetchSrNames(rscode) {
            if (!rscode) {
        await showAlert('RS Code is required to load SR list.', 'warning');
    return;
            }

    const loadingSwal = showLoading('Loading SRs...');
    try {
        $('#srList').html('<tr><td colspan="4" class="text-center py-4"><div class="spinner-border" role="status"></div></td></tr>');
    $('#noResults').addClass('d-none');

    const response = await $.ajax({
        url: '/ReviewPlane/GetSrNames',
    type: 'GET',
    data: {rscode},
    dataType: 'json'
                });

    $('#srList').empty();
    const srData = response.success ? response.data : response;

                if (srData?.length > 0) {
        srData.forEach(sr => {
            const srCode = sr.srCode || sr.Salesperson;
            const srName = sr.srName || sr.SMN_Name;
            const srRscode = sr.rscode || sr.RS_Code;
            const isSelected = selectedSrs[srCode] ? 'checked' : '';

            $('#srList').append(`
                            <tr data-srcode="${srCode}">
                                <td><div class="form-check">
                                    <input class="form-check-input sr-checkbox" type="radio"
                                           name="srSelection" id="sr-${srCode}" value="${srName}"
                                           data-srcode="${srCode}" ${isSelected}>
                                </div></td>
                                <td><div class="d-flex align-items-center">
                                    <div class="sr-avatar me-3">${getInitials(srName)}</div>
                                    <div>${srName}</div>
                                </div></td>
                                <td>${srCode}</td>
                                <td>${srRscode}</td>
                            </tr>
                        `);
        });
    updateSelectionCount();
                } else {
        $('#noResults').removeClass('d-none');
                }
            } catch (error) {
        $('#srList').html('<tr><td colspan="4" class="text-center py-4 text-danger">Error loading data</td></tr>');
    if (error.status === 400) {
        showAlert(error.responseJSON?.message || 'RS Code is required.', 'warning');
                } else {
        console.error('Error loading SR list:', error);
    showAlert(error.responseJSON?.message || 'Failed to load SR data', 'error');
                }
            } finally {
        loadingSwal.close();
            }
        }

    function initSrSearch() {
        $('#srSearchInput').on('keyup', function () {
            const searchText = $(this).val().toLowerCase();
            let hasMatches = false;

            $('#srList tr[data-srcode]').each(function () {
                const matches = $(this).text().toLowerCase().includes(searchText);
                $(this).toggle(matches);
                if (matches) hasMatches = true;
            });

            $('#noResults').toggle(!hasMatches && searchText.length > 0);
        });
        }

    function handleRowSelection() {
        $('#srList').on('click', 'tr[data-srcode]', function (e) {
            if ($(e.target).is('input[type="radio"]')) return;
            const checkbox = $(this).find('.sr-checkbox');
            checkbox.prop('checked', true).trigger('change');
        });

    $('#srList').on('change', '.sr-checkbox', function () {
                const srCode = $(this).data('srcode');
    const srName = $(this).val();

    // Clear previous selections
    selectedSrs = { };

    // Add current selection
    if ($(this).is(':checked')) {
        selectedSrs[srCode] = srName;
                }

    updateSelectionCount();
            });
        }

    async function confirmSrSelection() {
            const selected = Object.entries(selectedSrs);
    if (selected.length === 0) {
        await showAlert("Please select at least one SR.");
    return;
            }

    selectedSr = selected[0][1];
    $("#selectedSrInput").val(selectedSr);
    srModal.hide();
    checkFormCompletion();
        }

    // RSCODE Functions
    async function fetchRscodeByMrName() {
            const mrName = $('#mrNameInput').val().trim();
    if (!mrName) {
        await showAlert('MR Name is not available.');
    return;
            }

    try {
                const loadingSwal = showLoading(`Looking up RSCODE for ${mrName}`);
    const response = await $.ajax({
        url: '/ReviewPlane/GetRSCodes',
    type: 'GET',
    data: {term: mrName },
    dataType: 'json'
                });

    loadingSwal.close();

                if (response?.length > 0) {
                    const firstResult = response[0];
    $('#rscodeInput').val(`${firstResult.code} - ${firstResult.name}`);
    $('#selectedRscode').val(firstResult.code);

    await Swal.fire({
        icon: 'success',
    title: 'Success!',
    text: `Found RSCODE: ${firstResult.code}`,
    timer: 1500,
    showConfirmButton: false
                    });
                } else {
                    throw new Error('No RSCODEs found');
                }
            } catch (error) {
        $('#rscodeInput, #selectedRscode').val('');
    showAlert(error.responseJSON?.message || error.message, 'error');
            }
        }

    // Outlet Functions
    //function displayOutletSuggestions(outlets) {
    //        const $suggestions = $("#outletSuggestions").empty();

    //if (!outlets || outlets.length === 0) {
    //    $suggestions.append(`
    //                <div class="list-group-item text-muted">
    //                    No outlets found
    //                </div>
    //            `);
    //$suggestions.show();
    //return;
    //        }

    //        outlets.forEach(outlet => {
    //            const $item = $(`
    //<div class="list-group-item outlet-suggestion py-3">
    //    <div class="d-flex justify-content-between align-items-start">
    //        <div style="flex: 1;">
    //            <h6 class="mb-1">${outlet.name}</h6>
    //            <div class="d-flex flex-wrap gap-2 mb-2">
    //                <span class="badge bg-primary">${outlet.code}</span>
    //                ${outlet.outletSubType ? `<span class="badge bg-secondary">${outlet.outletSubType}</span>` : ''}
    //                ${outlet.beat ? `<span class="badge bg-info">${outlet.beat}</span>` : ''}
    //            </div>
    //            <div class="outlet-details" style="font-size: 0.85rem;">
    //                <div><strong>Address:</strong> ${outlet.address || 'N/A'}</div>
    //                ${outlet.childParty ? `<div><strong>Child Party:</strong> ${outlet.childParty}</div>` : ''}
    //                ${outlet.servicingPLG ? `<div><strong>Servicing PLG:</strong> ${outlet.servicingPLG}</div>` : ''}
    //            </div>
    //        </div>
    //        <button class="btn btn-sm btn-outline-primary view-details-btn ms-2">
    //            <i class="fas fa-info-circle"></i> Details
    //        </button>
    //    </div>
    //</div>
    //`);

    //$item.data('outlet', outlet);
    //$suggestions.append($item);
    //        });

    //$suggestions.show();
    //    }



function displayOutletSuggestions(outlets) {
    const $suggestions = $("#outletSuggestions").empty();

    if (!outlets || outlets.length === 0) {
        $suggestions.hide(); // Just hide instead of showing "No outlets found"
        return;
    }

    outlets.forEach(outlet => {
        const $item = $(`
            <div class="list-group-item outlet-suggestion py-3">
                <div class="d-flex justify-content-between align-items-start">
                    <div style="flex: 1;">
                        <h6 class="mb-1">${outlet.name}</h6>
                        <div class="d-flex flex-wrap gap-2 mb-2">
                            <span class="badge bg-primary">${outlet.code}</span>
                            ${outlet.outletSubType ? `<span class="badge bg-secondary">${outlet.outletSubType}</span>` : ''}
                            ${outlet.beat ? `<span class="badge bg-info">${outlet.beat}</span>` : ''}
                        </div>
                        <div class="outlet-details" style="font-size: 0.85rem;">
                            <div><strong>Address:</strong> ${outlet.address || 'N/A'}</div>
                            ${outlet.childParty ? `<div><strong>Child Party:</strong> ${outlet.childParty}</div>` : ''}
                            ${outlet.servicingPLG ? `<div><strong>Servicing PLG:</strong> ${outlet.servicingPLG}</div>` : ''}
                        </div>
                    </div>
                    <button class="btn btn-sm btn-outline-primary view-details-btn ms-2">
                        <i class="fas fa-info-circle"></i> Details
                    </button>
                </div>
            </div>
        `);

        $item.data('outlet', outlet);
        $suggestions.append($item);
    });

    $suggestions.show();
}

    function updateOutletDetails(outlet) {
        $("#outlet-name").val(outlet.name || '');
    $("#outlet-code").val(outlet.code || '');
    $("#outlet-sub-type").val(outlet.outletSubType || '');
    $("#outlet-address").val(outlet.address || '');
    $("#outlet-route").val(outlet.beat || selectedRoute || 'N/A');
    $("#outlet-child-party").val(outlet.childParty || '');
    $("#outlet-plg").val(outlet.servicingPLG || '');
        }

    function showOutletDetails(outlet) {
            const detailsHtml = `
    <div class="container-fluid">
        <div class="row g-3" style="max-height: 70vh; overflow-y: auto;">
            <div class="col-12 col-md-6">
                <h5 class="mb-3">Outlet Information</h5>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered">
                        <tr><th>Name</th><td>${outlet.name}</td></tr>
                        <tr><th>Code</th><td>${outlet.code}</td></tr>
                        <tr><th>Type</th><td>${outlet.outletSubType || 'N/A'}</td></tr>
                        <tr><th>Address</th><td>${outlet.address || 'N/A'}</td></tr>
                    </table>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <h5 class="mb-3">SR Assignment</h5>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered">
                        <tr><th>SR Code</th><td>${outlet.salesperson || 'N/A'}</td></tr>
                        <tr><th>SR Name</th><td>${outlet.srName || 'N/A'}</td></tr>
                        <tr><th>Beat/Route</th><td>${outlet.beat || 'N/A'}</td></tr>
                        <tr><th>Child Party</th><td>${outlet.childParty || 'N/A'}</td></tr>
                        <tr><th>Servicing PLG</th><td>${outlet.servicingPLG || 'N/A'}</td></tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    `;

    Swal.fire({
        title: 'Outlet Details',
    html: detailsHtml,
    width: '90%',
    maxWidth: '900px',
    showCloseButton: true,
    showConfirmButton: false,
    customClass: {
        container: 'scrollable-modal'
                }
            });
        }

    function initOutletHandlers() {
        // Outlet search input handler
        $("#outletInput").on("keyup", function () {
            const searchTerm = $(this).val().trim();
            const rscode = $("#selectedRscode").val();
            const routeName = selectedRoute;
            const mrCode = $("#mrNameInput").val();
            const srName = selectedSr;

            console.log(searchTerm);

            if (searchTerm.length >= 1 && rscode) {
                showLoading('Searching outlets...');

                const requestData = {
                    term: searchTerm,
                    rscode: rscode,
                    srname: srName || null,
                    routeName: routeName || null,
                    mrCode: mrCode || null
                };

                $.get('/ReviewPlane/GetOutlets', requestData)
                    .done(function (data) {
                        Swal.close();
                        if (data && data.length > 0) {
                            displayOutletSuggestions(data);
                        } else {
                            //Swal.fire('No Results', 'No matching outlets found.', 'info');
                            $("#outletSuggestions").empty().hide();
                        }
                    })
                    .fail(function (error) {
                        Swal.close();
                        if (error.status === 400) {
                            const message = error.responseText || "Please select SR and Route first.";
                            Swal.fire('Validation Error', message, 'warning');
                        } else {
                            console.error('Outlet search failed:', error);
                            Swal.fire('Error', 'Failed to search outlets: ' + (error.responseJSON?.message || error.statusText), 'error');
                        }
                        $("#outletSuggestions").empty().hide();
                    });
            } else {
                $("#outletSuggestions").empty().hide();
            }
        });

    // Handle outlet selection
    $(document).on("click", ".outlet-suggestion", function () {
                const outletData = $(this).data('outlet');
    $("#outletInput").val(`${outletData.name} (${outletData.code})`);
    $("#selectedOutlet").val(outletData.code);
    $("#outletSuggestions").hide();
    currentOutlet = outletData;
    updateOutletDetails(outletData);
    checkFormCompletion();
            });

    // Handle details button click
    $(document).on("click", ".view-details-btn", function (e) {
        e.stopPropagation();
    const outletData = $(this).closest('.outlet-suggestion').data('outlet');
    showOutletDetails(outletData);
            });
        }

    // Route selection handler
    function initRouteSelectionHandler() {
        $(document).on('click', '.route-option', function () {
            $('.route-option').removeClass('selected');
            $(this).addClass('selected');
        });
        }

    function buildReviewData(lat, lon, accuracy) {
            return {
        mrName: ($('#mrNameInput').val() || '').trim(),
    rscode: $('#selectedRscode').val() || '',
    srName: $('#selectedSrInput').val() || (currentOutlet?.srName || ''),
    srCode: Object.keys(selectedSrs)[0] || (currentOutlet?.salesperson || ''),
    routeName: $('#outlet-route').val() || selectedRoute || '',
    outletCode: $('#selectedOutlet').val() || '',
    outletName: $('#outlet-name').val() || '',
    outletSubType: $('#outlet-sub-type').val() || '',
    outletAddress: $('#outlet-address').val() || '',
    childParty: $('#outlet-child-party').val() || '',
    servicingPLG: $('#outlet-plg').val() || '',
    photoUrl: $('#photoCaptured').val() || null,
    latitude: lat ?? '',
    longitude: lon ?? '',
    accuracy: accuracy ?? '',
    deviceInfo: navigator.userAgent || '',
    deviceType: getDeviceType() || ''
            };
        }

    function validateReviewData(data) {
            if (!data.mrName) {
        showAlert("MR Name is required.", 'warning');
    return false;
            }
    if (!data.rscode) {
        showAlert("RS Code is required.", 'warning');
    return false;
            }
    if (!data.srName) {
        showAlert("SR Name is required.", 'warning');
    return false;
            }
    if (!data.srCode) {
        showAlert("SR Code is required.", 'warning');
    return false;
            }
    if (!data.routeName) {
        showAlert("Route Name is required.", 'warning');
    return false;
            }
    if (!data.outletCode) {
        showAlert("Outlet Code is required.", 'warning');
    return false;
            }
    if (!data.outletName) {
        showAlert("Outlet Name is required.", 'warning');
    return false;
            }
    if (!data.photoUrl) {
        showAlert("Photo is required. Please capture a photo.", 'warning');
    return false;
            }

    return true;
        }

    function sendReviewData(rawReviewData, retryCount = 2, baseDelayMs = 2000) {
        let attempts = 0;
    const reviewData = sanitizeReviewData(rawReviewData);

    console.debug("📦 Sanitized Review Data:", reviewData);

            return new Promise((resolve, reject) => {
        function attempt() {
            attempts++;
            console.debug(`🚀 Attempt ${attempts} to send review...`);

            // Prevent user from leaving mid-save
            window.onbeforeunload = () => "Saving review... Please wait.";

            const formData = new FormData();
            Object.entries(reviewData).forEach(([key, value]) => {
                if (value !== null && value !== undefined) {
                    formData.append(key, value);
                }
            });

            if (rawReviewData.reviewMessage) {
                formData.set("reviewMessage", rawReviewData.reviewMessage);
            }


            // Debug FormData contents
            console.group("📤 FormData to be sent");
            for (let [key, val] of formData.entries()) {
                console.log(`${key}:`, val);
            }
            console.groupEnd();

            $.ajax({
                url: '/ReviewPlane/SaveReview',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                timeout: 3600000,
                success: function (response) {
                    console.debug("✅ Server Response:", response);
                    window.onbeforeunload = null;
                    resolve(response);
                },
                error: function (jqXHR, textStatus) {
                    console.error("❌ AJAX Error:", textStatus, jqXHR);
                    window.onbeforeunload = null;

                    if ((jqXHR.status === 0 || textStatus === 'timeout') && attempts <= retryCount) {
                        const delay = baseDelayMs * Math.pow(2, attempts - 1);
                        console.warn(`⏳ Retrying... Attempt ${attempts} after ${delay}ms`);
                        setTimeout(attempt, delay);
                        return;
                    }

                    let errorMessage = jqXHR.responseJSON?.message ||
                        jqXHR.statusText ||
                        `HTTP ${jqXHR.status}`;

                    Swal.fire({
                        icon: 'error',
                        title: 'Save Failed!',
                        text: errorMessage
                    });

                    reject(new Error(errorMessage));
                }
            });
        }

                attempt();
            });
        }

    function sanitizeReviewData(data) {
        console.debug("🛠 Raw Review Data:", data);
    return {
        mrName: data.mrName || '',
    rscode: data.rscode || '',
    srName: data.srName || '',
    srCode: data.srCode || '',
    routeName: data.routeName || '',
    outletCode: data.outletCode || '',
    outletName: data.outletName || '',
    outletSubType: data.outletSubType || '',
    outletAddress: data.outletAddress || '',
    childParty: data.childParty || '',
    servicingPLG: data.servicingPLG || '',
    latitude: data.latitude ?? '',
    longitude: data.longitude ?? '',
    accuracy: data.accuracy ?? '',
    deviceInfo: data.deviceInfo || '',
    deviceType: data.devType || '',
    reviewId: data.reviewId || '',
    photoUrl: data.photoUrl || null
            };
        }

//    function handleReviewResponse(response, withLocation) {
//            if (!response) {
//        Swal.fire('No Response', 'No response from server. Please try again.', 'error');
//    return;
//            }

//    if (response.success) {
//                // Store ReviewId in localStorage
//                if (response.reviewId) {
//        localStorage.setItem('currentReviewId', response.reviewId);
//    console.log('Stored ReviewId:', response.reviewId);
//                } else {
//        console.warn('Successful response but missing ReviewId');
//                }

//    $('#saveSuccessMsg').removeClass('d-none');
//                setTimeout(() => $('#saveSuccessMsg').addClass('d-none'), 10000);

//    const distanceValue = parseFloat(response.distance);
//    const distanceText = isNaN(distanceValue)
//    ? "Distance not available"
//                    : distanceValue >= 1000
//    ? `Distance: ${(distanceValue / 1000).toFixed(2)} km`
//    : `Distance: ${distanceValue} meters`;

//    let locationMsg = withLocation ? 'Review saved successfully!' : 'Review saved without location.';

//    //$('#nextStep1').prop('disabled', false);

//    Swal.fire({
//        title: locationMsg,
//    html: `Status: ${response.status}<br>${distanceText}`,
//        icon: 'success',
//                    didClose: () => {
//                        // Additional debug output
//                        const storedId = localStorage.getItem('currentReviewId');
//        console.log('Current stored ReviewId:', storedId);
//        console.log('Full response:', response);
//                    }
//                });
//            } else {
//            console.error('SaveReview error:', response.message || 'Unknown error');
//        Swal.fire('Error', response.message || 'Failed to save review', 'error');
//            }
//}


function handleReviewResponse(response, withLocation) {
    if (!response) {
        Swal.fire('No Response', 'No response from server. Please try again.', 'error');
        return;
    }

    if (response.success) {
        // Store ReviewId in localStorage
        if (response.reviewId) {
            localStorage.setItem('currentReviewId', response.reviewId);
            console.log('Stored ReviewId:', response.reviewId);

            // ENABLE THE NEXT BUTTON ONLY AFTER SUCCESSFUL SAVE
            $('#nextStep1').prop('disabled', false);
        } else {
            console.warn('Successful response but missing ReviewId');
        }

        $('#saveSuccessMsg').removeClass('d-none');
        setTimeout(() => $('#saveSuccessMsg').addClass('d-none'), 10000);

        const distanceValue = parseFloat(response.distance);
        const distanceText = isNaN(distanceValue)
            ? "Distance not available"
            : distanceValue >= 1000
                ? `Distance: ${(distanceValue / 1000).toFixed(2)} km`
                : `Distance: ${distanceValue} meters`;

        let locationMsg = withLocation ? 'Review saved successfully!' : 'Review saved without location.';

        Swal.fire({
            title: locationMsg,
            html: `Status: ${response.status}<br>${distanceText}`,
            icon: 'success',
            didClose: () => {
                // Additional debug output
                const storedId = localStorage.getItem('currentReviewId');
                console.log('Current stored ReviewId:', storedId);
                console.log('Full response:', response);
            }
        });
    } else {
        console.error('SaveReview error:', response.message || 'Unknown error');
        Swal.fire('Error', response.message || 'Failed to save review', 'error');

        // Keep Next button disabled if save failed
        $('#nextStep1').prop('disabled', true);
    }
}







async function saveWithoutLocation(reviewMessage) {
    if (!navigator.onLine) {
        Swal.fire('Offline', 'You are currently offline. Please check your internet connection.', 'error');
        return;
    }

    const loadingSwal = showLoading('Saving review without location...');

    const reviewData = buildReviewData(null, null, null);
    reviewData.reviewMessage = reviewMessage || "Saved without location";

    // Validate mandatory fields before sending
    if (!validateReviewData(reviewData)) {
        loadingSwal.close();
        return;
    }

    try {
        const response = await sendReviewData(reviewData);
        loadingSwal.close();
        handleReviewResponse(response, false); // false = location not available

        // ✅ Enable Next button after success
        $('#nextStep1').prop('disabled', false);

        // Refresh the page after successful save
        //setTimeout(() => {
        //    location.reload();
        //}, 2000);
    } catch (error) {
        loadingSwal.close();
        console.error('Save without location error:', error);
        Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');

        $('#nextStep1').prop('disabled', true);
    }
}





        //async function saveWithoutLocation() {
        //    if (!navigator.onLine) {
        //    Swal.fire('Offline', 'You are currently offline. Please check your internet connection.', 'error');
        //return;
        //    }

        //const loadingSwal = showLoading('Saving review without location...');

        //    const reviewData = buildReviewData(null, null, null);


        //    reviewData.reviewMessage = reviewMessage || "Saved without location";


        //// Validate mandatory fields before sending
        //if (!validateReviewData(reviewData)) {
        //    loadingSwal.close();
        //return;
        //    }

        //try {
        //        const response = await sendReviewData(reviewData);
        //loadingSwal.close();
        //handleReviewResponse(response, false); // false = location not available
        //    } catch (error) {
        //    loadingSwal.close();
        //console.error('Save without location error:', error);
        //    Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');

        //    $('#nextStep1').prop('disabled', true);

        //    }
        //}

        function checkFormCompletion() {
            const isComplete = (
        $('#selectedRscode').val() &&
        $('#selectedSrInput').val() &&
        $('#selectedOutlet').val() &&
        $('#photoCaptured').val()
        );

        //$('#nextStep1').prop('disabled', !isComplete);
        return isComplete;
        }

        // Route Filter Handlers
        function initRouteFilterHandlers() {
            $('#openRouteFilterModal').click(async function () {
                const rscode = $('#selectedRscode').val();
                if (!rscode) {
                    await showAlert('Please select RSCODE first', 'warning');
                    return;
                }
                await fetchRouteNames(rscode, selectedSr || null);
            });

        $('#applyRouteFilter').click(applyRouteFilter);
        $('#clearRouteFilter').click(clearRouteFilter);
        }
//function initSaveHandler() {
//    $('#saveReview').click(async function () {
//        // Validate Photo
//        if (!$('#photoCaptured').val()) {
//            $('#photoError').removeClass('d-none');
//            return;
//        } else {
//            $('#photoError').addClass('d-none');
//        }

//        // Validate Outlet
//        if (!$('#selectedOutlet').val()) {
//            Swal.fire('Missing Outlet', 'Please select an outlet first.', 'warning');
//            return;
//        }

//        // Check Internet
//        if (!navigator.onLine) {
//            Swal.fire('Offline', 'You are currently offline. Please check your internet connection.', 'error');
//            return;
//        }

//        // Step 1: Ask user if continuing review
//        const reviewDecision = await Swal.fire({
//            title: 'Continue Review?',
//            text: 'Are you continuing the review?',
//            icon: 'question',
//            showCancelButton: true,
//            confirmButtonText: 'Yes',
//            cancelButtonText: 'No'
//        });

//        let reviewMessage = '';

//        if (reviewDecision.isConfirmed) {
//            // User said YES → default message
//            reviewMessage = "User continuous review";
//        } else if (reviewDecision.dismiss === Swal.DismissReason.cancel) {
//            // User said NO → ask reason (dropdown only)
//            const reasonInput = await Swal.fire({
//                title: 'Reason Required',
//                input: 'select',
//                inputOptions: {
//                    closed: 'Shop Closed',
//                    notInterested: "I don\'t like to continue",
//                    noStock: 'No Stock Available',
//                    ownerNotAvailable: 'Owner Not Available',
//                    holiday: 'Outlet on Holiday'
//                },
//                inputPlaceholder: 'Select a reason...',
//                inputValidator: (value) => {
//                    if (!value) {
//                        return 'You need to select a reason!';
//                    }
//                },
//                showCancelButton: true
//            });

//            if (reasonInput.isConfirmed) {
//                reviewMessage = reasonInput.value;
//            } else {
//                return; // user cancelled dropdown
//            }
//        } else {
//            return; // user closed without selecting
//        }

//        // Step 2: Save Review with Location
//        const loadingSwal = showLoading('Capturing location and saving review...');

//        try {
//            navigator.geolocation.getCurrentPosition(async function (position) {
//                const lat = position.coords.latitude;
//                const lon = position.coords.longitude;
//                const accuracy = position.coords.accuracy;

//                const reviewData = buildReviewData(lat, lon, accuracy);
//                reviewData.reviewMessage = reviewMessage; // add message to payload

//                // Validate data before sending
//                if (!validateReviewData(reviewData)) {
//                    loadingSwal.close();
//                    return;
//                }

//                try {
//                    const response = await sendReviewData(reviewData);
//                    console.log(response.reviewId);
//                    loadingSwal.close();
//                    handleReviewResponse(response, true);

//                    // ✅ Enable Next button after success
//                    $('#nextStep1').prop('disabled', false);

//                    // Refresh the page after successful save
//                    setTimeout(() => {
//                        location.reload();
//                    }, 2000);

//                } catch (error) {
//                    loadingSwal.close();
//                    console.error('Error:', error);
//                    Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');
//                }

//            }, async function (error) {
//                loadingSwal.close();

//                if (error.code === error.PERMISSION_DENIED) {
//                    Swal.fire('Permission Denied', 'Location access was denied. Please enable location services to continue.', 'warning');
//                } else if (error.code === error.POSITION_UNAVAILABLE || error.code === error.TIMEOUT) {
//                    const result = await Swal.fire({
//                        title: 'Location Not Available',
//                        text: 'Could not get your location. Do you want to save without location data?',
//                        icon: 'warning',
//                        showCancelButton: true,
//                        confirmButtonText: 'Yes, save without location',
//                        cancelButtonText: 'Cancel'
//                    });

//                    if (result.isConfirmed) {
//                        await saveWithoutLocation(reviewMessage);

//                        // ✅ Enable Next button after success
//                        $('#nextStep1').prop('disabled', false);

//                        // Refresh the page after successful save
//                        //setTimeout(() => {
//                        //    location.reload();
//                        //}, 2000);
//                    }
//                } else {
//                    Swal.fire('Error', 'Location error: ' + error.message, 'error');
//                }

//            }, {
//                enableHighAccuracy: true,
//                timeout: 3600000,
//                maximumAge: 0
//            });

//        } catch (error) {
//            loadingSwal.close();
//            console.error('Error:', error);
//            Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');
//        }
//    });
//}

function initSaveHandler() {
    $('#saveReview').click(async function () {
        // Validate Photo
        if (!$('#photoCaptured').val()) {
            $('#photoError').removeClass('d-none');
            return;
        } else {
            $('#photoError').addClass('d-none');
        }

        // Validate Outlet
        if (!$('#selectedOutlet').val()) {
            Swal.fire('Missing Outlet', 'Please select an outlet first.', 'warning');
            return;
        }

        // Check Internet
        if (!navigator.onLine) {
            Swal.fire('Offline', 'You are currently offline. Please check your internet connection.', 'error');
            return;
        }

        // Step 1: Ask user if continuing review
        const reviewDecision = await Swal.fire({
            title: 'Continue Review?',
            text: 'Are you continuing the review?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        });

        let reviewMessage = '';

        if (reviewDecision.isConfirmed) {
            // ✅ User said YES → continue review (no refresh)
            reviewMessage = "User continuous review";

            const loadingSwal = showLoading('Capturing location and saving review...');
            try {
                navigator.geolocation.getCurrentPosition(async function (position) {
                    const lat = position.coords.latitude;
                    const lon = position.coords.longitude;
                    const accuracy = position.coords.accuracy;

                    const reviewData = buildReviewData(lat, lon, accuracy);
                    reviewData.reviewMessage = reviewMessage;

                    if (!validateReviewData(reviewData)) {
                        loadingSwal.close();
                        return;
                    }

                    const response = await sendReviewData(reviewData);
                    loadingSwal.close();
                    handleReviewResponse(response, true);

                    // ✅ Enable Next button after success
                    $('#nextStep1').prop('disabled', false);

                    // ❌ No refresh here
                }, async (error) => {
                    loadingSwal.close();

                    if (error.code === error.PERMISSION_DENIED) {
                        Swal.fire('Permission Denied', 'Location access was denied. Please enable location services to continue.', 'warning');
                    } else if (error.code === error.POSITION_UNAVAILABLE || error.code === error.TIMEOUT) {
                        const result = await Swal.fire({
                            title: 'Location Not Available',
                            text: 'Could not get your location. Do you want to save without location data?',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Yes, save without location',
                            cancelButtonText: 'Cancel'
                        });

                        if (result.isConfirmed) {
                            await saveWithoutLocation(reviewMessage);

                            // ✅ Enable Next button
                            $('#nextStep1').prop('disabled', false);

                            // ❌ No refresh here
                        }
                    } else {
                        Swal.fire('Error', 'Location error: ' + error.message, 'error');
                    }
                }, {
                    enableHighAccuracy: true,
                    timeout: 3600000,
                    maximumAge: 0
                });
            } catch (error) {
                loadingSwal.close();
                Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');
            }

        } else if (reviewDecision.dismiss === Swal.DismissReason.cancel) {
            // ❌ User said NO → ask reason
            const reasonInput = await Swal.fire({
                title: 'Reason Required',
                input: 'select',
                inputOptions: {
                    closed: 'Shop Closed',
                    notInterested: "I don't like to continue",
                    noStock: 'No Stock Available',
                    ownerNotAvailable: 'Owner Not Available',
                    holiday: 'Outlet on Holiday'
                },
                inputPlaceholder: 'Select a reason...',
                inputValidator: (value) => {
                    if (!value) {
                        return 'You need to select a reason!';
                    }
                },
                showCancelButton: true
            });

            if (reasonInput.isConfirmed) {
                reviewMessage = reasonInput.value;

                await saveWithoutLocation(reviewMessage);

                // ✅ Enable Next button
                $('#nextStep1').prop('disabled', false);

                // 🔄 Refresh after reason-based save
                setTimeout(() => {
                    location.reload();
                }, 2000);
            }
        }
    });
}


//function initSaveHandler() {
//    $('#saveReview').click(async function () {
//        // Validate Photo
//        if (!$('#photoCaptured').val()) {
//            $('#photoError').removeClass('d-none');
//            return;
//        } else {
//            $('#photoError').addClass('d-none');
//        }

//        // Validate Outlet
//        if (!$('#selectedOutlet').val()) {
//            Swal.fire('Missing Outlet', 'Please select an outlet first.', 'warning');
//            return;
//        }

//        // Check Internet
//        if (!navigator.onLine) {
//            Swal.fire('Offline', 'You are currently offline. Please check your internet connection.', 'error');
//            return;
//        }

//        // Step 1: Ask user if continuing review
//        const reviewDecision = await Swal.fire({
//            title: 'Continue Review?',
//            text: 'Are you continuing the review?',
//            icon: 'question',
//            showCancelButton: true,
//            confirmButtonText: 'Yes',
//            cancelButtonText: 'No'
//        });

//        let reviewMessage = '';

//        if (reviewDecision.isConfirmed) {
//            // User said YES → default message
//            reviewMessage = "User continuous review";
//        } else if (reviewDecision.dismiss === Swal.DismissReason.cancel) {
//            // User said NO → ask reason (dropdown only)
//            const reasonInput = await Swal.fire({
//                title: 'Reason Required',
//                input: 'select',
//                inputOptions: {
//                    closed: 'Shop Closed',
//                    notInterested: "I don\'t like to continue",
//                    noStock: 'No Stock Available',
//                    ownerNotAvailable: 'Owner Not Available',
//                    holiday: 'Outlet on Holiday'
//                },
//                inputPlaceholder: 'Select a reason...',
//                inputValidator: (value) => {
//                    if (!value) {
//                        return 'You need to select a reason!';
//                    }
//                },
//                showCancelButton: true
//            });

//            if (reasonInput.isConfirmed) {
//                reviewMessage = reasonInput.value;
//            } else {
//                return; // user cancelled dropdown
//            }
//        } else {
//            return; // user closed without selecting
//        }

//        // Step 2: Save Review with Location
//        const loadingSwal = showLoading('Capturing location and saving review...');

//        try {
//            navigator.geolocation.getCurrentPosition(async function (position) {
//                const lat = position.coords.latitude;
//                const lon = position.coords.longitude;
//                const accuracy = position.coords.accuracy;

//                const reviewData = buildReviewData(lat, lon, accuracy);
//                reviewData.reviewMessage = reviewMessage; // add message to payload

//                // Validate data before sending
//                if (!validateReviewData(reviewData)) {
//                    loadingSwal.close();
//                    return;
//                }

//                try {
//                    const response = await sendReviewData(reviewData);
//                    console.log(response.reviewId);
//                    loadingSwal.close();
//                    handleReviewResponse(response, true);

//                    // ✅ Enable Next button after success
//                    $('#nextButton').prop('disabled', false);

//                } catch (error) {
//                    loadingSwal.close();
//                    console.error('Error:', error);
//                    Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');
//                }

//            }, async function (error) {
//                loadingSwal.close();

//                if (error.code === error.PERMISSION_DENIED) {
//                    Swal.fire('Permission Denied', 'Location access was denied. Please enable location services to continue.', 'warning');
//                } else if (error.code === error.POSITION_UNAVAILABLE || error.code === error.TIMEOUT) {
//                    const result = await Swal.fire({
//                        title: 'Location Not Available',
//                        text: 'Could not get your location. Do you want to save without location data?',
//                        icon: 'warning',
//                        showCancelButton: true,
//                        confirmButtonText: 'Yes, save without location',
//                        cancelButtonText: 'Cancel'
//                    });

//                    if (result.isConfirmed) {
//                        await saveWithoutLocation(reviewMessage);

//                        // ✅ Enable Next button after success
//                        $('#nextButton').prop('disabled', false);
//                    }
//                } else {
//                    Swal.fire('Error', 'Location error: ' + error.message, 'error');
//                }

//            }, {
//                enableHighAccuracy: true,
//                timeout: 3600000,
//                maximumAge: 0
//            });

//        } catch (error) {
//            loadingSwal.close();
//            console.error('Error:', error);
//            Swal.fire('Error', 'Failed to save review: ' + error.message, 'error');
//        }
//    });
//}




        // Event Handlers
        function initEventHandlers() {
            // RSCODE Refresh
            $('#refreshRscodeBtn').click(fetchRscodeByMrName);

        // SR Modal
        $("#openSrModal").click(async function () {
                const rscodeFull = $("#rscodeInput").val()?.trim() || "";
        const rscode = rscodeFull.split(/-|\s/)[0]?.trim();
        const mrName = $("#mrNameInput").val()?.trim();

        if (!rscode || !mrName) {
            await showAlert("Please select RSCODE and ensure MR Name is available");
        return;
                }

        selectedSrs = { };
        await fetchSrNames(rscode, mrName);
        srModal.show();
            });

        // Next button handler
        $("#nextStep1").click(function (e) {
            e.preventDefault();
        try {
                    const srCode = Object.keys(selectedSrs || { })[0] || '';
        const routeName = $('#outlet-route').val() || selectedRoute || '';

        const isSrValid = !!srCode;
        const isRouteValid = !!routeName;

        const reviewId = localStorage.getItem('currentReviewId');

        if (reviewId === null || reviewId === "null" || reviewId === "undefined" || reviewId.trim() === "") {
            // ❌ Review ID not present
            Swal.fire({
                title: 'Action Required',
                text: 'Please submit the Review form properly before proceeding.',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
        return;
                    }

        // ✅ Review ID exists → show modal to proceed
        Swal.fire({
            title: 'Success',
        text: 'Review is there in local storage. Click OK to proceed to OutletView.',
        icon: 'success',
        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            // Check SR/Route validity before navigating
                            if (!isSrValid || !isRouteValid) {
                                const modalEl = document.getElementById('confirmationModal');
        if (!modalEl) return;

        const confirmationModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);

        $('#confirmProceed').off('click');
        $('#confirmProceed').on('click', function () {
            window.location.href = '/StoreVisit/OutletView';
                                });

        confirmationModal.show();
                            } else {
            window.location.href = '/StoreVisit/OutletView';
                            }
                        }
                    });

                } catch (error) {
            console.error('Error in nextStep1 handler:', error);
        Swal.fire('Error', 'An error occurred. Please check the console for details.', 'error');
                }
            });
        }

        // Document ready
$(document).ready(function () {

    $('#nextStep1').prop('disabled', true);

            // Initial data load
            fetchRscodeByMrName();

        // Initialize event handlers
        initEventHandlers();
        initRouteFilterHandlers();
        initOutletHandlers();
        initPhotoHandlers();
        initSaveHandler();
        initRouteSelectionHandler();

        // Initialize selection handlers
        $('#selectSr').click(confirmSrSelection);
        handleRowSelection();
        initSrSearch();

        // Modal cleanup
        $('#srModal').on('hidden.bs.modal', function () {
            $('#srSearchInput').val('');
            });

        $('#routeFilterModal').on('hidden.bs.modal', function () {
            $('#routenameInput').val('');
            });
        });
