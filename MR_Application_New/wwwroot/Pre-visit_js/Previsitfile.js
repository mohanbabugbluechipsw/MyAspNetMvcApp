
    // Show loading indicator immediately
    Swal.fire({
        title: 'Loading Pre-Visit Questions',
    text: 'Please wait...',
    allowOutsideClick: false,
        didOpen: () => Swal.showLoading()
    });

    var questions = @Html.Raw(jsonData);
    var container = document.getElementById('questionContainer');
    var currentPreviewIndex = -1;
    var imagesUrls = { };

    // Image processing function (resize + border)
    // async function processImage(file) {
        //     return new Promise((resolve, reject) => {
        //         const reader = new FileReader();
        //         reader.onload = (event) => {
        //             const img = new Image();
        //             img.onload = () => {
        //                 const canvas = document.createElement('canvas');
        //                 const ctx = canvas.getContext('2d');

        //                 // Set canvas dimensions (max 800px width, maintain aspect ratio)
        //                 const maxWidth = 800;
        //                 const scale = Math.min(1, maxWidth / img.width);
        //                 canvas.width = img.width * scale;
        //                 canvas.height = img.height * scale;

        //                 // Draw original image
        //                 ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

        //                 // Add border
        //                 ctx.strokeStyle = '#000';
        //                 ctx.lineWidth = 2;
        //                 ctx.strokeRect(0, 0, canvas.width, canvas.height);

        //                 // Convert to Blob with quality based on file size
        //                 let quality = 0.85;
        //                 if (file.size > 2000000) { // If > 2MB
        //                     quality = 0.75;
        //                 } else if (file.size > 5000000) { // If > 5MB
        //                     quality = 0.65;
        //                 }

        //                 canvas.toBlob(
        //                     blob => resolve(blob),
        //                     'image/jpeg',
        //                     quality
        //                 );
        //             };
        //             img.onerror = reject;
        //             img.src = event.target.result;
        //         };
        //         reader.onerror = reject;
        //         reader.readAsDataURL(file);
        //     });
        // }


        async function processImage(file) {
            return new Promise((resolve, reject) => {
                const img = new Image();
                const url = URL.createObjectURL(file); // 👈 no base64

                img.onload = () => {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');

                    // Set canvas dimensions (max 800px width, maintain aspect ratio)
                    const maxWidth = 800;
                    const scale = Math.min(1, maxWidth / img.width);
                    canvas.width = img.width * scale;
                    canvas.height = img.height * scale;

                    // Draw original image
                    ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

                    // Add border
                    ctx.strokeStyle = '#000';
                    ctx.lineWidth = 2;
                    ctx.strokeRect(0, 0, canvas.width, canvas.height);

                    // Convert to Blob with quality based on file size
                    let quality = 0.85;
                    if (file.size > 5000000) {        // > 5MB
                        quality = 0.65;
                    } else if (file.size > 2000000) { // > 2MB
                        quality = 0.75;
                    }

                    canvas.toBlob(blob => {
                        // ✅ cleanup to release memory
                        ctx.clearRect(0, 0, canvas.width, canvas.height);
                        canvas.width = canvas.height = 0;
                        URL.revokeObjectURL(url);

                        if (blob) {
                            resolve(blob);
                        } else {
                            reject(new Error("Image processing failed"));
                        }
                    }, 'image/jpeg', quality);
                };

                img.onerror = reject;
                img.src = url;
            });
        }


    // Create question cards
    questions.forEach(function (q, index) {
        var html = `
    <div class="col">
        <div class="card card-question p-3 shadow-sm rounded-4 h-100">
            <h5 class="mb-3 text-primary text-center">${q.Text}</h5>

            <!-- Planogram Section -->
            <div class="mb-2 text-center">
                <button type="button" class="btn btn-sm btn-outline-info" onclick="showPlanogram(${q.QuestionId})">
                    <i class="bi bi-image-fill me-1"></i> View Planogram
                </button>
            </div>

            <input type="file" id="file_${index}" accept="image/*" capture="environment" class="d-none" data-index="${index}" onchange="handleCapture(this)" />

            <button type="button" data-index="${index}" onclick="handleCaptureClick(this)"
                class="btn btn-outline-secondary w-100 py-2 mb-2 capture-btn"
                id="captureBtn_${index}">
                <i class="bi bi-camera-fill me-2"></i> Capture Photo
            </button>

            <div class="d-flex flex-column align-items-center">
                <img id="thumb_${index}" src="" class="img-thumbnail mb-2 d-none" alt="Captured image" />
                <button id="previewBtn_${index}" type="button" onclick="showPreview(${index})" class="btn btn-info btn-sm d-none">
                    <i class="bi bi-eye-fill me-1"></i> View Image
                </button>
            </div>

            <div class="form-check form-switch d-flex align-items-center justify-content-center gap-2 mt-3">
                <input class="form-check-input" type="checkbox" role="switch" id="newCheck_${index}" />
                <label class="form-check-label fw-semibold text-dark" for="newCheck_${index}">Mark as New</label>
            </div>
        </div>
    </div>
    `;
    container.insertAdjacentHTML('beforeend', html);

        // Add listener for disabling capture when 'New' is checked
        setTimeout(() => {
            const newCheck = document.getElementById('newCheck_' + index);
    const captureBtn = document.getElementById('captureBtn_' + index);
    newCheck.addEventListener('change', function () {
        captureBtn.disabled = this.checked;
    if (this.checked) {
        // Clear any existing image if marked as new
        document.getElementById('thumb_' + index).classList.add('d-none');
    document.getElementById('previewBtn_' + index).classList.add('d-none');
    delete imagesUrls[index];
                }
            });
        }, 0);
    });

    Swal.close();

    function handleCaptureClick(button) {
        const index = button.getAttribute('data-index');
    const isNewChecked = document.getElementById('newCheck_' + index).checked;
    if (isNewChecked) {
        Swal.fire({
            title: 'Action Blocked',
            text: 'You marked this as "New". Capture is not allowed.',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
    return;
        }

    // Check if device is mobile for better camera experience
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        document.getElementById('file_' + index).click();
        } else {
        Swal.fire({
            title: 'Upload Photo',
            text: 'Please select a photo to upload',
            icon: 'info'
        }).then(() => {
            document.getElementById('file_' + index).click();
        });
        }
    }

    // async function handleCapture(input) {
        //     var index = input.getAttribute('data-index');
        //     var file = input.files[0];

        //     if (!file) return;

        //     // Show processing indicator
        //     const processingSwal = Swal.fire({
        //         title: 'Processing Image',
        //         text: 'Please wait while we process your photo...',
        //         allowOutsideClick: false,
        //         didOpen: () => Swal.showLoading()
        //     });

        //     try {
        //         // Process image (resize + border)
        //         const processedBlob = await processImage(file);
        //         const processedFile = new File([processedBlob], file.name, { type: 'image/jpeg' });

        //         // Upload to storage
        //         const today = new Date();
        //         const dateFolder = today.toISOString().slice(0, 10).replace(/-/g, "");
        //         const folderName = `${dateFolder}_Pre_visit`;
        //         const fileName = `${folderName}/visit_${Date.now()}_${index}.jpg`;
        //         const containerName = "visitphotos";

        //         const sasResponse = await fetch(`/api/Blob/GetUploadSas?fileName=${encodeURIComponent(fileName)}&container=${containerName}`);
        //         if (!sasResponse.ok) throw new Error('Failed to get upload URL');

        //         const sasData = await sasResponse.json();

        //         const uploadResponse = await fetch(sasData.uploadUrl, {
        //             method: 'PUT',
        //             headers: { "x-ms-blob-type": "BlockBlob" },
        //             body: processedFile
        //         });

        //         if (uploadResponse.ok) {
        //             imagesUrls[index] = sasData.viewUrl;
        //             const thumbElement = document.getElementById('thumb_' + index);
        //             thumbElement.src = sasData.viewUrl;
        //             thumbElement.classList.remove('d-none');
        //             document.getElementById('previewBtn_' + index).classList.remove('d-none');

        //             await Swal.fire({
        //                 title: 'Success!',
        //                 text: 'Photo uploaded successfully',
        //                 icon: 'success',
        //                 timer: 1500,
        //                 showConfirmButton: false
        //             });
        //         } else {
        //             const errorText = await uploadResponse.text();
        //             throw new Error(errorText || 'Upload failed');
        //         }
        //     } catch (err) {
        //         console.error('Upload error:', err);
        //         await Swal.fire({
        //             title: 'Upload Failed',
        //             text: err.message || 'Image processing or upload failed',
        //             icon: 'error'
        //         });
        //     } finally {
        //         processingSwal.close();
        //          input.value = "";
        // file = null;
        // processedFile = null;
        //     }
        // }

        async function handleCapture(input) {
            let file = input.files[0];
            if (!file) return;

            // Show processing indicator
            Swal.fire({
                title: 'Processing Image',
                text: 'Please wait while we process your photo...',
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            let processedFile = null;
            try {
                // Process image (resize + border)
                const processedBlob = await processImage(file);
                processedFile = new File([processedBlob], file.name, { type: 'image/jpeg' });

                // Prepare upload
                const today = new Date();
                const dateFolder = today.toISOString().slice(0, 10).replace(/-/g, "");
                const folderName = `${dateFolder}_Pre_visit`;
                const fileName = `${folderName}/visit_${Date.now()}_${input.dataset.index}.jpg`;
                const containerName = "visitphotos";

                // Get SAS token
                const sasResponse = await fetch(`/api/Blob/GetUploadSas?fileName=${encodeURIComponent(fileName)}&container=${containerName}`);
                if (!sasResponse.ok) throw new Error('Failed to get upload URL');
                const sasData = await sasResponse.json();

                // Upload
                const uploadResponse = await fetch(sasData.uploadUrl, {
                    method: 'PUT',
                    headers: { "x-ms-blob-type": "BlockBlob" },
                    body: processedFile
                });

                if (!uploadResponse.ok) {
                    const errorText = await uploadResponse.text();
                    throw new Error(errorText || 'Upload failed');
                }

                // Success UI update
                imagesUrls[input.dataset.index] = sasData.viewUrl;
                const thumbElement = document.getElementById('thumb_' + input.dataset.index);
                thumbElement.src = sasData.viewUrl;
                thumbElement.classList.remove('d-none');
                document.getElementById('previewBtn_' + input.dataset.index).classList.remove('d-none');

                await Swal.fire({
                    title: 'Success!',
                    text: 'Photo uploaded successfully',
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false
                });
            } catch (err) {
                console.error('Upload error:', err);
                await Swal.fire({
                    title: 'Upload Failed',
                    text: err.message || 'Image processing or upload failed',
                    icon: 'error'
                });
            } finally {
                // Close processing modal safely
                Swal.close();

                // Cleanup memory
                input.value = "";
                file = null;
                processedFile = null;

                // Force GC hint (only helps a bit in Chrome)
                if (window.gc) window.gc();
            }
        }


    function showPreview(index) {
        currentPreviewIndex = index;
    document.getElementById('modalImage').src = document.getElementById('thumb_' + index).src;
    const modal = new bootstrap.Modal(document.getElementById('imageModal'));
    modal.show();
    }

    document.getElementById('retakeButton').addEventListener('click', function () {
        if (currentPreviewIndex >= 0) {
            var fileInput = document.getElementById('file_' + currentPreviewIndex);
    fileInput.value = "";
    document.getElementById('thumb_' + currentPreviewIndex).src = "";
    document.getElementById('thumb_' + currentPreviewIndex).classList.add('d-none');
    document.getElementById('previewBtn_' + currentPreviewIndex).classList.add('d-none');
    delete imagesUrls[currentPreviewIndex];

    var modal = bootstrap.Modal.getInstance(document.getElementById('imageModal'));
    modal.hide();

            setTimeout(() => fileInput.click(), 10000);
        }
    });

    const baseUrl = "https://unileverslstorage001.blob.core.windows.net/planograms";

    const planograms = [
    {questionId: 1, format: "OTC", model: "Model 1", images: ["MD1.1.jpg", "MD1.2.jpg"] },
    {questionId: 2, format: "OTC", model: "Model 2", images: ["MD2.1.jpg","MD2.2.jpg"] },
    {questionId: 3, format: "OTC", model: "MD3: Savoury & Spreads", images: ["MD3 Savoury & Spreads.jpg"] },
    {questionId: 4, format: "OTC", model: "MD4 : Malted Food Drinks (MFD)", images: ["MD4 MFD.jpg"] },
    {questionId: 119, format: "OTC", model: "Baby Cabinet", images: ["DC Baby Care.jpg"] },
    {questionId: 5, format: "OTC", model: "Laundry Sachet Hanger", images: ["Sachet Hanger Laundry.jpg"] },
    {questionId: 6, format: "OTC", model: "Savoury sachet Hanger", images: ["Sachet Hanger Savoury.jpg"] },
    {questionId: 7, format: "OTC", model: "MFD sachet Hanger", images: ["Sachet Hanger MFD.jpg"] },
    {questionId: 15, format: "SS", model: "Laundry", images: ["SS Laundry 2 bay.jpg", "SS Laundry 3 bay.jpg"] },
    {questionId: 16, format: "SS", model: "Household Cleaning", images: ["SS HHC 1 bay.jpg"] },
    {questionId: 17, format: "SS", model: "Hair Care", images: ["SS Hair Care 1 Bay.jpg", "SS Hair Care 2 Bay.jpg"] },
    {questionId: 18, format: "SS", model: "Skin Care", images: ["SS Skin Care 1 bay.jpg", "SS Skin Care 2 bay.jpg"] },
    {questionId: 19, format: "SS", model: "Skin Cleansing", images: ["SS Skin Cleansing 1 bay.jpg", "SS Skin Cleansing 2 Bay.jpg"] },
    {questionId: 20, format: "SS", model: "Oral Care", images: ["SS Oral Care 1 Bay.jpg", "SS Oral Care 2 bay.1.jpg", "SS Oral Care 2 Bay.jpg"] },
    {questionId: 21, format: "SS", model: "Baby Care", images: ["SS Baby Care 1 Bay.jpg"] },
    {questionId: 22, format: "SS", model: "Deo & Fragrances", images: ["SS Deo & Fragrance.jpg", "SS Deo & Fragrances 2.jpg"] },
    {questionId: 121, format: "SS", model: "Men's Care", images: ["SS Men's Care.jpg"] },
    {questionId: 24, format: "SS", model: "Spreads", images: ["SS Spreads.jpg"] },
    {questionId: 25, format: "SS", model: "Malted Food Drinks", images: ["SS MFD.jpg"] },
    {questionId: 23, format: "SS", model: "Culinary", images: ["SS Savoury.jpg"] },
    {questionId: 112, format: "DCHB", model: "Supplier Box", images: ["DC Supply Block.jpg"] },
    {questionId: 113, format: "DCHB", model: "Hair Care", images: ["DC Hair Care.jpg"] },
    {questionId: 114, format: "DCHB", model: "Face Care", images: ["DC Skin Care.jpg"] },
    {questionId: 115, format: "DCHB", model: "Body Care", images: ["DC Skin Care.2.jpg"] },
    {questionId: 116, format: "DCHB", model: "Deo & Fragrances", images: ["DC Deo & Fragrances.jpg"] },
    {questionId: 117, format: "DCHB", model: "Men's Care", images: ["DC Mens Care.jpg"] },
    {questionId: 118, format: "DCHB", model: "Baby Care", images: ["DC Baby Care.jpg"] },
    {questionId: 87, format: "Hybrid", model: "Model 1", images: ["MD1.1.jpg", "MD1.2.jpg"] },
    {questionId: 88, format: "Hybrid", model: "Model 2", images: ["MD2.1.jpg","MD2.2.jpg"] },
    {questionId: 89, format: "Hybrid", model: "MD3: Savoury & Spreads", images: ["MD3 Savoury & Spreads.jpg"] },
    {questionId: 90, format: "Hybrid", model: "MD4 : Malted Food Drinks (MFD)", images: ["MD4 MFD.jpg"] },
    {questionId: 123, format: "Hybrid", model: "Baby Cabinet", images: ["DC Baby Care.jpg"] },
    {questionId: 91, format: "Hybrid", model: "Laundry Sachet Hanger", images: ["Sachet Hanger Laundry.jpg"] },
    {questionId: 92, format: "Hybrid", model: "Savoury sachet Hanger", images: ["Sachet Hanger Savoury.jpg"] },
    {questionId: 93, format: "Hybrid", model: "MFD sachet Hanger", images: ["Sachet Hanger MFD.jpg"] },
    {questionId: 94, format: "Hybrid", model: "Laundry", images: ["SS Laundry 2 bay.jpg", "SS Laundry 3 bay.jpg"] },
    {questionId: 95, format: "Hybrid", model: "Household Cleaning", images: ["SS HHC 1 bay.jpg"] },
    {questionId: 96, format: "Hybrid", model: "Hair Care", images: ["SS Hair Care 1 Bay.jpg", "SS Hair Care 2 Bay.jpg"] },
    {questionId: 97, format: "Hybrid", model: "Skin Care", images: ["SS Skin Care 1 bay.jpg", "SS Skin Care 2 bay.jpg"] },
    {questionId: 98, format: "Hybrid", model: "Skin Cleansing", images: ["SS Skin Cleansing 1 bay.jpg", "SS Skin Cleansing 2 Bay.jpg"] },
    {questionId: 99, format: "Hybrid", model: "Oral Care", images: ["SS Oral Care 1 Bay.jpg", "SS Oral Care 2 bay.1.jpg", "SS Oral Care 2 Bay.jpg"] },
    {questionId: 100, format: "Hybrid", model: "Baby Care", images: ["SS Baby Care 1 Bay.jpg"] },
    {questionId: 101, format: "Hybrid", model: "Deo & Fragrances", images: ["SS Deo & Fragrance.jpg", "SS Deo & Fragrances 2.jpg"] },
    {questionId: 124, format: "Hybrid", model: "Men's Care", images: ["SS Men's Care.jpg"] },
    {questionId: 103, format: "Hybrid", model: "Spreads", images: ["SS Spreads.jpg"] },
    {questionId: 104, format: "Hybrid", model: "Malted Food Drinks", images: ["SS MFD.jpg"] },
    {questionId: 102, format: "Hybrid", model: "Culinary", images: ["SS Savoury.jpg"] }
    ];

    function showPlanogram(questionId) {
        var fallbackImg = `${baseUrl}/placeholder.jpg`;
        const planogram = planograms.find(p => p.questionId === questionId);

    if (!planogram) {
        Swal.fire('Planogram Missing', 'No planogram found for this question', 'info');
    return;
        }

    // Update modal title with planogram info
    document.getElementById('planogramModalTitle').textContent = `Planogram: ${planogram.model}`;

        const imgPaths = planogram.images.map(img =>
    `${baseUrl}/${planogram.format}/${encodeURIComponent(img)}`
    );

    var container = document.getElementById('planogramContainer');
    container.innerHTML = '';

    // Show loading in modal
    const modal = new bootstrap.Modal(document.getElementById('planogramModal'));
    modal.show();

    container.innerHTML = '<div class="text-center py-4"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';

    // Load images with better error handling
    let loadedImages = 0;
        imgPaths.forEach((path, index) => {
            const img = document.createElement('img');
    img.classList.add('img-fluid');
    img.style.maxHeight = '70vh';
    img.style.objectFit = 'contain';
    img.alt = `Planogram ${index + 1}`;

    const tempImg = new Image();
            tempImg.onload = () => {
        img.src = path;
    loadedImages++;
    if (loadedImages === 1) { // First image loaded
        container.innerHTML = '';
                }
    container.appendChild(img);

    // Initialize swipe for mobile
    if (loadedImages === imgPaths.length && 'ontouchstart' in window) {
        initTouchSwipe(container);
                }
            };
            tempImg.onerror = () => {
        loadedImages++;
    if (loadedImages === imgPaths.length && container.children.length === 0) {
        container.innerHTML = '<div class="alert alert-warning text-center">Planogram images not available</div>';
                }
            };
    tempImg.src = path;
        });
    }

    // Touch swipe for mobile planogram navigation
    function initTouchSwipe(container) {
        let startX, moveX;
    const threshold = 50; // minimum pixels to consider a swipe

        container.addEventListener('touchstart', (e) => {
        startX = e.touches[0].clientX;
        }, {passive: true });

        container.addEventListener('touchmove', (e) => {
        moveX = e.touches[0].clientX;
        }, {passive: true });

        container.addEventListener('touchend', () => {
            if (startX - moveX > threshold) {
        // Swipe left
        container.scrollBy({ left: 200, behavior: 'smooth' });
            } else if (moveX - startX > threshold) {
        // Swipe right
        container.scrollBy({ left: -200, behavior: 'smooth' });
            }
        }, {passive: true });
    }

    // Network status detection
    function checkNetworkStatus() {
        if (!navigator.onLine) {
        Swal.fire({
            title: 'Offline Mode',
            text: 'You are currently offline. Some features may be limited.',
            icon: 'warning',
            timer: 3000
        });
        }
    }

    // Check network status on load and periodically
    window.addEventListener('load', checkNetworkStatus);
    window.addEventListener('online', checkNetworkStatus);
    window.addEventListener('offline', checkNetworkStatus);
    setInterval(checkNetworkStatus, 30000);



    async function fetchWithRetry(url, options, retries = 3, timeout = 7200000) {
        return new Promise((resolve, reject) => {
            const timer = setTimeout(() => {
                if (retries > 0) {
        console.log(`Timeout - retrying (${retries} left)`);
    resolve(fetchWithRetry(url, options, retries - 1, timeout));
                } else {
        reject(new Error('Request timeout'));
                }
            }, timeout);

    fetch(url, options)
                .then(response => {
        clearTimeout(timer);
    if (response.ok) {
        resolve(response);
                    } else if (retries > 0) {
        console.log(`Request failed - retrying (${retries} left)`);
                        setTimeout(() => {
        resolve(fetchWithRetry(url, options, retries - 1, timeout));
                        }, 2000);
                    } else {
        reject(new Error(`HTTP error: ${response.status}`));
                    }
                })
                .catch(err => {
        clearTimeout(timer);
                    if (retries > 0) {
        console.log(`Network error - retrying (${retries} left)`);
                        setTimeout(() => {
        resolve(fetchWithRetry(url, options, retries - 1, timeout));
                        }, 2000);
                    } else {
        reject(err);
                    }
                });
        });
    }





    document.getElementById('nextButton').addEventListener('click', async function () {
        const reviewId = localStorage.getItem('currentReviewId');
    console.log("currentReviewId value from localStorage:", reviewId);

    let message = null;

    if (reviewId === null) {
        message = 'The key "currentReviewId" does not exist in localStorage.';
        } else if (reviewId === "null") {
        message = 'The key "currentReviewId" has the string value "null".';
        } else if (reviewId === "undefined") {
        message = 'The key "currentReviewId" has the string value "undefined".';
        } else if (reviewId.trim() === "") {
        message = 'The key "currentReviewId" is empty in localStorage.';
        } else if (reviewId.toLowerCase() === "invalid") {
        message = 'The key "currentReviewId" has invalid value.';
        }

    if (message) {
        await Swal.fire({
            title: 'currentReviewId Invalid',
            text: message,
            icon: 'error',
            confirmButtonText: 'OK'
        });
    return;
        }

    // ✅ If reached here → valid reviewId
    console.log("Valid currentReviewId:", reviewId);

    if (!navigator.onLine) {
        await Swal.fire({
            title: 'Offline Detected',
            text: 'You are currently offline. Please connect to the internet to submit.',
            icon: 'error'
        });
    return;
        }

    const submitBtn = this;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';

    try {
            var answers = [];
    var hasAtLeastOneAnswer = false;

    if (!questions || !Array.isArray(questions)) {
        console.error("Questions array is missing or invalid.");
    await Swal.fire({
        title: 'Error',
    text: 'Questions data not loaded. Please refresh the page.',
    icon: 'error'
                });
    return;
            }

    questions.forEach(function (q, index) {
                const isNew = document.getElementById('newCheck_' + index)?.checked;
    const blobUrl = imagesUrls[index];

    if (isNew || blobUrl) {
        hasAtLeastOneAnswer = true;
    answers.push({
        QuestionId: q.QuestionId,
    Text: q.Text,
    BlobUrl: blobUrl || "",
    IsNew: isNew ? 1 : 0
                    });
                }
            });

    if (!hasAtLeastOneAnswer) {
        await Swal.fire({
            title: 'No Input',
            text: 'Please either capture photo or mark as "New" for at least one question.',
            icon: 'warning'
        });
    return;
            }

    // Create model including ReviewId
    const model = {
        VisitId: document.getElementById('VisitId').value,
    VisitType: document.getElementById('VisitType').value,
    ChannelType: document.getElementById('ChannelType').value,
    Answers: answers,
    ReviewId: reviewId
            };

    const confirmResult = await Swal.fire({
        title: 'Submit Pre-Visit?',
    text: 'Are you sure you want to submit your pre-visit data?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonText: 'Yes, Submit',
    cancelButtonText: 'Cancel',
    reverseButtons: true
            });

    if (confirmResult.isConfirmed) {
        Swal.fire({
            title: 'Saving Data',
            text: 'Please wait while we save your pre-visit information...',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

    try {
                    const response = await fetchWithRetry('/StoreVisit/PreVisitJson', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    'X-Requested-With': 'XMLHttpRequest'
                        },
    body: JSON.stringify(model)
                    });

    if (!response.ok) {
                        const errorText = await response.text();
    throw new Error(errorText || 'Failed to save data');
                    }

    const result = await response.json();

    console.log(result);
    Swal.close();

    if (result.success) {
                        // ✅ Save Pre-Visit Guid locally
                        if (result.rowGuid) {
        console.log(result.rowGuid);
    localStorage.setItem("preVisitGuid", result.rowGuid);
                        }

    await Swal.fire({
        title: 'Success!',
    text: result.message || 'Pre-Visit data saved successfully',
    icon: 'success',
    timer: 1500,
    showConfirmButton: false
                        });

    // Redirect to post-visit
    window.location.href = '/StoreVisit/PostVisit?ChannelType=' +
    encodeURIComponent(model.ChannelType) + '&_=' + Date.now();
                    }
                } catch (error) {
        Swal.close();
    await Swal.fire({
        title: 'Submission Failed',
    text: error.message || 'An error occurred while saving your data',
    icon: 'error'
                    });
    console.error('Submission error:', error);
                }
            }
        } catch (error) {
        console.error('Unexpected error:', error);
    await Swal.fire({
        title: 'Error',
    text: 'An unexpected error occurred',
    icon: 'error'
            });
        } finally {
        submitBtn.disabled = false;
    submitBtn.innerHTML = '<span class="d-none d-sm-inline">Next </span>(Post-Visit)';
        }
    });


    // Initialize tooltips
    document.addEventListener('DOMContentLoaded', function() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    });
