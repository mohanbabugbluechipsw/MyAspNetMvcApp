



// Accordion toggle with arrow change
document.querySelectorAll(".accordion").forEach(button => {
    button.addEventListener("click", function () {
        this.classList.toggle("active");
        let panel = this.nextElementSibling;
        let arrow = this.querySelector(".arrow");

        if (panel.style.display === "block") {
            panel.style.display = "none";
            arrow.innerHTML = "▼";
        } else {
            panel.style.display = "block";
            arrow.innerHTML = "▲";
        }
    });
});



document.addEventListener('DOMContentLoaded', function () {
    // Setup all file inputs for preview functionality
    setupImagePreviews();
});

function setupImagePreviews() {
    // Model Display Photos
    const modelPhotoInputs = [
        'fsws_model1_preVisitPhoto', 'fsws_model1_displayPhoto',
        'fsws_model2_preVisitPhoto', 'fsws_model2_displayPhoto',
        'fsws_model3_preVisitPhoto', 'fsws_model3_displayPhoto',
        'fsws_model4_preVisitPhoto', 'fsws_model4_displayPhoto'
    ];

    modelPhotoInputs.forEach(id => {
        const input = document.getElementById(id);
        if (input) {
            input.addEventListener('change', function (e) {
                handleImageUpload(e, `${id}PreviewContainer`);
            });
        }
    });

    // Sachet Hanger Photos
    const sachetHangerPhotos = [
        'sachetHangerdisplayPhoto', 'sachetHangerSavouryPhoto', 'sachetHangerHFDPhoto'
    ];

    sachetHangerPhotos.forEach(id => {
        const input = document.getElementById(id);
        if (input) {
            input.addEventListener('change', function (e) {
                handleImageUpload(e, `${id}PreviewContainer`);
            });
        }
    });
}

function handleImageUpload(event, containerId) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const previewContainer = document.getElementById(containerId);
            previewContainer.innerHTML = ''; // Clear previous content

            const previewDiv = document.createElement('div');
            previewDiv.style.display = 'inline-block';
            previewDiv.style.marginTop = '10px';
            previewDiv.style.position = 'relative';

            const img = document.createElement('img');
            img.src = e.target.result;
            img.style.width = '120px';
            img.style.height = '120px';
            img.style.cursor = 'pointer';
            img.style.borderRadius = '10px';
            img.style.border = '2px solid #007bff';
            img.style.transition = 'transform 0.3s ease';

            img.onclick = function () {
                showFullImagePreview(e.target.result);
            };

            // Add delete button
            const deleteBtn = document.createElement('button');
            deleteBtn.innerHTML = '×';
            deleteBtn.style.position = 'absolute';
            deleteBtn.style.top = '-10px';
            deleteBtn.style.right = '-10px';
            deleteBtn.style.background = 'red';
            deleteBtn.style.color = 'white';
            deleteBtn.style.border = 'none';
            deleteBtn.style.borderRadius = '50%';
            deleteBtn.style.width = '25px';
            deleteBtn.style.height = '25px';
            deleteBtn.style.cursor = 'pointer';
            deleteBtn.onclick = function (e) {
                e.stopPropagation();
                previewDiv.remove();
                event.target.value = ''; // Clear the file input
            };

            previewDiv.appendChild(img);
            previewDiv.appendChild(deleteBtn);
            previewContainer.appendChild(previewDiv);
        };
        reader.readAsDataURL(file);
    }
}

function showFullImagePreview(imageSrc) {
    const modal = document.getElementById('imageModal');
    const modalImage = document.getElementById('modalImage');
    modalImage.src = imageSrc;
    const bootstrapModal = new bootstrap.Modal(modal);
    bootstrapModal.show();
}

function capturePhoto(inputId) {
    const input = document.getElementById(inputId);
    if (!input) return;

    // Check if mobile device
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        // Mobile - use camera capture
        input.setAttribute('capture', 'environment');
        input.click();
    } else {
        // Desktop - regular file upload
        input.removeAttribute('capture');
        input.click();
    }
}




//function openImageModal(model, option) {
//    let imagePath = '';

//    if (model === 'model1') {
//        switch (option) {
//            case 'option1':
//                imagePath = '@Url.Content("~/Images/Uploads/Model1ETUP.png")';
//                break;
//            case 'option2':
//                imagePath = '@Url.Content("~/Images/R (1).png")';
//                break;

//            case 'option3':
//                imagePath = '@Url.Content("~/Images/Uploads/Model1ETUP.png")';
//                break;
//        }
//    } else if (model === 'model2') {
//        switch (option) {
//            case 'option1':
//                imagePath = '@Url.Content("~/img/unilever.jpg")';
//                break;
//            case 'option2':
//                imagePath = '@Url.Content("~/Images/placeholder.png")';
//                break;
//        }
//    }

//    if (imagePath) {
//        document.getElementById('modalImage').src = imagePath;
//        document.getElementById('imageModalLabel').textContent = option.toUpperCase() + ' Preview';

//        var imageModal = new bootstrap.Modal(document.getElementById('imageModal'));
//        imageModal.show();


//    }
//}



function openImageModal(model, option) {
    let imagePath = '';

    if (imagePaths[model] && imagePaths[model][option]) { 
        imagePath = imagePaths[model][option];
    }

    if (imagePath) {
        const modalImage = document.getElementById('modalImage');
        const imageModal = new bootstrap.Modal(document.getElementById('imageModal'));
        modalImage.src = imagePath;
        imageModal.show();
    }
}




function togglePanel(event) {
    const button = event.target.closest('button');
    const panel = button.nextElementSibling;
    const arrow = button.querySelector('.arrow');

    const isVisible = !panel.classList.contains('hidden');
    panel.classList.toggle('hidden', isVisible);
    arrow.textContent = isVisible ? '▼' : '▲';
}

function toggleSections(show, model) {
    const sectionIds = [
        `fsws_${model}_sectionsContainer`,
        `fsws_${model}_photoSection`,
        `fsws_${model}_additionalQuestions`
    ];

    sectionIds.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.classList.toggle('hidden', !show);
        }
    });
}

// Hide all panels on initial load
window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.panel').forEach(panel => {
        panel.classList.add('hidden');
    });
});








function validateAndProceed() {
    let isValid = true;
    let errorMessages = [];

    // Reset previous error styles
    resetErrors();

    // Helper function for radio button validation
    function validateRadio(name, labelId, errorMessage) {
        const answer = document.querySelector(`input[name="${name}"]:checked`);
        if (!answer) {
            setError(labelId, errorMessage);
            isValid = false;
        }
        return answer ? answer.value : null;
    }

    // Helper function for file input validation
    function validateFileInput(inputId, labelId, errorMessage) {
        const fileInput = document.getElementById(inputId);
        if (fileInput && !fileInput.files.length) {
            setError(labelId, errorMessage);
            isValid = false;
        }
    }

    // Set error message and highlight labels
    function setError(labelId, message) {
        const label = document.getElementById(labelId);
        if (label) label.style.color = 'red';
        errorMessages.push(message);
    }

    // Validate Model 1
    const fsws_model1visibility = validateRadio("fsws_model1_visible", "fsws_model1_visiblelabel", "Please answer: Is this placed in a clearly visible place to the shopper?");
    const sachetHangerAvailable = validateRadio("sachetHangerAvailable", "sachetHangerLabel", "Please answer: Any Sachet Hanger available in the Outlet?");

    if (fsws_model1visibility === "yes") {
        validateRadio("fsws_model1_displayOption", "fsws_model1_displayOptionlabel", "Please select a Display Option.");
        validateFileInput("fsws_model1_displayPhoto", "fsws_model1_displayPhotolabel", "Please upload a Display Photo.");
        validateFileInput("fsws_model1_preVisitPhoto", "fsws_model1_preVisitPhotolabel", "Please upload a Pre-Visit Photo.");
        validateRadio("fsws_model1_unileverSeparate", "fsws_model1_unileverSeparatelabel", "Are Unilever brands merchandised separately?");
        validateRadio("fsws_model1_brandVariants", "fsws_model1_brandVariantslabel", "Are relevant brand variants merchandised separately?");
        validateRadio("fsws_model1_nonUnilever", "fsws_model1_nonUnileverlabel", "Are non-Unilever brands placed in between Unilever brands?");
        validateRadio("fsws_model1_shelfStrip", "fsws_model1_shelfStriplabel", "Is ANY Unilever SHELF STRIP available in this Planogram?");
    }




    if (sachetHangerAvailable === "yes") {
        const sachetHangerLaundryVisible = validateRadio("sachetHangerLaundryVisible", "sachetHangerLaundryLabel", "Is this placed in a clearly visible place to the shopper?");
        if (sachetHangerLaundryVisible === "yes") {
            validateRadio("sachetHangerDisplayOption", "sachetHangerLaundrydisplayOptionLabel", "Please select a Display Option.");
            validateFileInput("sachetHangerdisplayPhoto", "sachetHangerLaundrydisplayPhotoLabel", "Please upload a Display Photo.");
            validateRadio("unileverPlanogramSeparate", "sachetHangerLaundryunileverPlanogramLabel", "Within the Unilever planogram, Unilever brands are merchandised separately?");
            validateRadio("unileverBrandVariantsSeparate", "sachetHangerLaundryunileverBrandVariantsSLabel", "Are relevant brand variants merchandised separately?");
            validateRadio("nonUnileverPlacement", "sachetHangerLaundrynonUnileverPlacementLabel", "Are non-Unilever brands placed in between Unilever brands?");
        }

        const sachetHangerSavouryVisible = validateRadio("sachetHangerSavouryVisible", "sachetHangerSavouryLabel", "Is the Sachet Hanger (Savoury) visible?");
        if (sachetHangerSavouryVisible === "yes") {
            validateRadio("sachetHangerSavouryDisplayOption", "sachetHangerSavourydisplayOptionLabel", "Please select a Display Option.");
            validateFileInput("sachetHangerSavouryPhoto", "sachetHangerSavourydisplayPhotoLabel", "Please upload a Display Photo.");
            validateRadio("unileverPlanogramSavourySeparate", "sachetHangerSavouryunileverPlanogramLabel", "Within the Unilever planogram, Unilever brands are merchandised separately?");
            validateRadio("unileverBrandVariantsSavourySeparate", "sachetHangerSavouryunileverBrandVariantsSLabel", "Are relevant brand variants merchandised separately?");
            validateRadio("nonUnileverPlacementSavoury", "sachetHangerSavourynonUnileverPlacementLabel", "Are non-Unilever brands placed in between Unilever brands?");
        }

        const sachetHangerHFDVisible = validateRadio("sachetHangerHFDVisible", "sachetHangerHFDLabel", "Is the Sachet Hanger (HFD) visible?");
        if (sachetHangerHFDVisible === "yes") {
            validateRadio("sachetHangerHFDDisplayOption", "sachetHangerHFDdisplayOptionLabel", "Please select a Display Option.");
            validateFileInput("sachetHangerHFDPhoto", "sachetHangerHFDdisplayPhotoLabel", "Please upload a Display Photo.");
            validateRadio("unileverPlanogramHFDSeparate", "sachetHangerHFDunileverPlanogramLabel", "Within the Unilever planogram, Unilever brands are merchandised separately?");
            validateRadio("unileverBrandVariantsHFDSeparate", "sachetHangerHFDunileverBrandVariantsSLabel", "Are relevant brand variants merchandised separately?");
            validateRadio("nonUnileverPlacementHFD", "sachetHangerHFDnonUnileverPlacementLabel", "Are non-Unilever brands placed in between Unilever brands?");
        }
    }




    // Validate Model 2
    const fsws_model2visibility = validateRadio("fsws_model2_visible", "fsws_model2_visiblelabel", "Please answer: Is this placed in a clearly visible place to the shopper?");

    if (fsws_model2visibility === "yes") {
        validateRadio("fsws_model2_displayOption", "fsws_model2_displayOptionlabel", "Please select a Display Option.");
        validateFileInput("fsws_model2_displayPhoto", "fsws_model2_displayPhotolabel", "Please upload a Display Photo.");
        validateFileInput("fsws_model2_preVisitPhoto", "fsws_model2_preVisitPhotolabel", "Please upload a Pre-Visit Photo.");
        validateRadio("fsws_model2_Shelf", "fsws_model2_Shelflabel", "Is this stored on a Shelf (not in a cabinet)?");
        validateRadio("fsws_model2_competitors", "fsws_model2_competitorslabel", "Are Unilever brands merchandised separately?");
        validateRadio("fsws_model2_signage", "fsws_model2_signagelabel", "Are relevant brand variants merchandised separately?");
        validateRadio("fsws_model2_safety", "fsws_model2_safetylabel", "Are non-Unilever brands placed in between Unilever brands?");
        validateRadio("fsws_model2_promoMaterial", "fsws_model2_promoMateriallabel", "Is ANY Unilever SHELF STRIP available in this Planogram?");
    }


    const fsws_model3visibility = validateRadio("fsws_model3_visible", "fsws_model3_visiblelabel",
        "Please answer: Is this placed in a clearly visible place to the shopper?");



    if (fsws_model3visibility === "yes") {
        validateRadio("fsws_model3_displayOption", "fsws_model3_displayOptionlabel",
            "Please select a Display Option.");
        validateFileInput("fsws_model3_displayPhoto", "fsws_model3_displayPhotolabel",
            "Please upload a Display Photo.");
        validateFileInput("fsws_model3_preVisitPhoto", "fsws_model3_preVisitPhotolabel",
            "Please upload a Pre-Visit Photo.");
        validateRadio("fsws_model3_seasonal", "fsws_model3_seasonallabel",
            "Is this stored on a Shelf (not in a cabinet)?");
        validateRadio("fsws_model3_lighting", "fsws_model3_lightinglabel",
            "Are Unilever brands merchandised separately?");
        validateRadio("fsws_model3_pricing", "fsws_model3_pricinglabel",
            "Are relevant brand variants merchandised separately?");
        validateRadio("fsws_model3_maintenance", "fsws_model3_maintenancelabel",
            "Are non-Unilever brands placed in between Unilever brands?");
        validateRadio("fsws_model3_SHELFSTRIP", "fsws_model3_SHELFSTRIPlabel",
            "Is ANY Unilever SHELF STRIP available in this Planogram?");
    }

    const fsws_model4visibility = validateRadio("fsws_model4_visible", "fsws_model4_visiblelabel",
        "Please answer: Is this placed in a clearly visible place to the shopper?");

    if (fsws_model4visibility === "yes") {
        validateRadio("fsws_model4_displayOption", "fsws_model4_displayOptionlabel",
            "Please select a Display Option.");
        validateFileInput("fsws_model4_displayPhoto", "fsws_model4_displayPhotolabel",
            "Please upload a Display Photo.");
        validateFileInput("fsws_model4_preVisitPhoto", "fsws_model4_preVisitPhotolabel",
            "Please upload a Pre-Visit Photo.");
        validateRadio("fsws_model4_planogram", "fsws_model4_planogramlabel",
            "Are Unilever brands merchandised separately?");
        validateRadio("fsws_model4_brands", "fsws_model4_brandslabel",
            "Are relevant brand variants merchandised separately?");
        validateRadio("fsws_model4_Non-Unileverbrands", "fsws_model4_Non-Unileverbrandslabel",
            "Are non-Unilever brands placed in between Unilever brands?");
        validateRadio("fsws_model4_SHELFSTRIP", "fsws_model4_SHELFSTRIPlabel",
            "Is ANY Unilever SHELF STRIP available in this Planogram?");
    }







    // Show errors if validation fails
    if (!isValid) {
        document.getElementById('errorMessage').innerHTML = errorMessages.join("<br>");
        showModal();
        return;
    }

    // If everything is valid, proceed with submission
    sendDataToDatabase();
}

// Reset all error highlights
function resetErrors() {
    const labelIds = [


        "fsws_model1_visiblelabel", "fsws_model1_displayOptionlabel",
        "fsws_model1_displayPhotolabel", "fsws_model1_preVisitPhotolabel", "fsws_model1_unileverSeparatelabel",
        "fsws_model1_brandVariantslabel", "fsws_model1_nonUnileverlabel", "fsws_model1_shelfStriplabel",



        "fsws_model2_visiblelabel", "fsws_model2_displayOptionlabel", "fsws_model2_displayPhotolabel",
        "fsws_model2_preVisitPhotolabel", "fsws_model2_Shelflabel", "fsws_model2_competitorslabel",
        "fsws_model2_signagelabel", "fsws_model2_safetylabel", "fsws_model2_promoMateriallabel",


        "fsws_model3_visiblelabel", "fsws_model3_displayOptionlabel", "fsws_model3_displayPhotolabel",
        "fsws_model3_preVisitPhotolabel", "fsws_model3_seasonallabel", "fsws_model3_lightinglabel",
        "fsws_model3_pricinglabel", "fsws_model3_maintenancelabel", "fsws_model3_SHELFSTRIPlabel",

        "fsws_model4_visiblelabel", "fsws_model4_displayOptionlabel", "fsws_model4_displayPhotolabel",
        "fsws_model4_preVisitPhotolabel", "fsws_model4_planogramlabel", "fsws_model4_brandslabel",
        "fsws_model4_Non-Unileverbrandslabel", "fsws_model4_SHELFSTRIPlabel",




        "sachetHangerLabel", "sachetHangerLaundryLabel", "sachetHangerLaundrydisplayOptionLabel", "sachetHangerLaundrydisplayPhotoLabel",
        "sachetHangerLaundryunileverPlanogramLabel", "sachetHangerLaundryunileverBrandVariantsSLabel", "sachetHangerLaundrynonUnileverPlacementLabel",
        "sachetHangerSavouryLabel", "sachetHangerSavourydisplayOptionLabel", "sachetHangerSavourydisplayPhotoLabel", "sachetHangerSavouryunileverPlanogramLabel",

        "sachetHangerSavouryunileverBrandVariantsSLabel", "sachetHangerSavourynonUnileverPlacementLabel", "sachetHangerHFDLabel", "sachetHangerHFDdisplayOptionLabel",
        "sachetHangerHFDdisplayPhotoLabel", "sachetHangerHFDunileverPlanogramLabel", "sachetHangerHFDunileverBrandVariantsSLabel", "sachetHangerHFDnonUnileverPlacementLabel"


    ];
    labelIds.forEach(id => {
        const label = document.getElementById(id);
        if (label) label.style.color = 'black';
    });
}


// Show error modal
function showModal() {
    document.getElementById('errorModal').style.display = 'block';
}

// Close error modal
function closeModal() {
    document.getElementById('errorModal').style.display = 'none';
}







function toggleSachetHangerVisibility(containerId) {

    let sachetHangerContainer = document.getElementById(containerId);
    let sachetHangerToggleBtn = document.getElementById("sachetHangerMainBtn");
    let sachetHangerToggleArrow = sachetHangerToggleBtn.querySelector(".sachetHangerArrow");

    sachetHangerContainer.classList.toggle("visible");

    sachetHangerToggleArrow.innerHTML = sachetHangerContainer.classList.contains("visible") ? "▲" : "▼";
}

function toggleSachetHangerOptions(isDisplayed) {
    let sachetHangerOptionsContainer = document.getElementById("sachetHangerOptions");

    if (isDisplayed) {
        sachetHangerOptionsContainer.classList.add("visible");
    } else {
        sachetHangerOptionsContainer.classList.remove("visible");

        document.getElementById("sachetHangerLaundry").classList.remove("visible");
        document.getElementById("sachetHangerSavoury").classList.remove("visible");
        document.getElementById("sachetHangerHFD").classList.remove("visible");

        document.querySelectorAll(".sachetHangerArrow").forEach(arrowIcon => {
            arrowIcon.innerHTML = "▼";
        });
    }
}

function toggleSachetHangerSubSection(subSectionContainerId) {
    let sachetHangerSubSectionContainer = document.getElementById(subSectionContainerId);
    let sachetHangerSubSectionBtn = document.querySelector(`[data-section='${subSectionContainerId}']`);
    let sachetHangerSubSectionArrow = sachetHangerSubSectionBtn.querySelector(".sachetHangerArrow");

    sachetHangerSubSectionContainer.classList.toggle("visible");

    sachetHangerSubSectionArrow.innerHTML = sachetHangerSubSectionContainer.classList.contains("visible") ? "▲" : "▼";
}


function toggleDisplayQuestions(isDisplayed, containerId) {
    let container = document.getElementById(containerId);
    container.style.display = isDisplayed ? "block" : "none";
}

function toggleHangerVisibility(isVisible, sectionId) {
    let section = document.getElementById(sectionId);
    section.style.display = isVisible ? "block" : "none";
}



function toggleSachetHangerDetails(isVisible, sectionId) {
    let section = document.getElementById(sectionId);
    section.style.display = isVisible ? "block" : "none";
}












function sendDataToDatabase() {
    const formData = new FormData();
    let totalFields = 0;
    let filledFields = 0;

    const sectionCompletion = {};

    function getRadioValue(name) {
        totalFields++;
        const checked = document.querySelector(`input[name="${name}"]:checked`);
        if (checked) filledFields++;
        console.log(`Radio [${name}]:`, checked ? checked.value : "Not selected");
        return checked ? checked.value : null;
    }

    function getFile(name) {
        totalFields++;
        const fileInput = document.getElementById(name);
        const hasFile = fileInput && fileInput.files.length > 0;
        if (hasFile) filledFields++;
        console.log(`File [${name}]:`, hasFile ? fileInput.files[0].name : "No file selected");
        return hasFile ? fileInput.files[0] : null;
    }

    //function trackSection(sectionName, isFilled) {
    //    if (!sectionCompletion[sectionName]) {
    //        sectionCompletion[sectionName] = { filled: 0, total: 0 };
    //    }
    //    sectionCompletion[sectionName].total++;
    //    if (isFilled) sectionCompletion[sectionName].filled++;
    //}


    function trackSection(sectionName, isFilled, questionData = null) {
        if (!sectionCompletion[sectionName]) {
            sectionCompletion[sectionName] = {
                filled: 0,
                total: 0,
                yesAnswers: 0,
                totalYesNoQuestions: 0,
                questions: []
            };
        }
        sectionCompletion[sectionName].total++;
        if (isFilled) sectionCompletion[sectionName].filled++;
        if (questionData) {
            sectionCompletion[sectionName].questions.push(questionData);
            if (questionData.type === 'yesno') {
                sectionCompletion[sectionName].totalYesNoQuestions++;
                if (questionData.answer?.toLowerCase() === 'yes') {
                    sectionCompletion[sectionName].yesAnswers++;
                }
            }
        }
    }

    //function appendField(section, name, value) {
    //    const isFilled = value !== null && value !== "";
    //    formData.append(name, value);
    //    trackSection(section, isFilled);
    //    console.log(`[${section}] Field: ${name} | Filled: ${isFilled} | Value:`, value);
    //}


    function appendField(section, name, value, questionText) {
        const isFilled = value !== null && value !== "";
        formData.append(name, value);
        const questionData = {
            question: questionText,
            answer: value,
            type: name.includes('Photo') ? 'file' : 'yesno'
        };
        trackSection(section, isFilled, questionData);
        console.log(`[${section}] Field: ${name} | Filled: ${isFilled} | Value:`, value);
    }
    //function processFields(section, fields) {
    //    console.log(`\n=== Processing Section: ${section} ===`);
    //    fields.forEach(([name, value]) => appendField(section, name, value));
    //}


    function processFields(section, fields) {
        console.log(`\n=== Processing Section: ${section} ===`);
        fields.forEach(([name, value, questionText]) => appendField(section, name, value, questionText));
    }

    processFields("Model 1", [
        ["fsws_model1_visible", getRadioValue("fsws_model1_visible"), "Is this placed in a clearly visible place to the shopper?"],
        ["fsws_model1_displayOption", getRadioValue("fsws_model1_displayOption"), "Display Option"],
        ["fsws_model1_displayPhoto", getFile("fsws_model1_displayPhoto"), "Display Photo 1"],
        ["fsws_model1_preVisitPhoto", getFile("fsws_model1_preVisitPhoto"), "Display Pre-Visit Photo"],
        ["fsws_model1_unileverSeparate", getRadioValue("fsws_model1_unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
        ["fsws_model1_brandVariants", getRadioValue("fsws_model1_brandVariants"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
        ["fsws_model1_nonUnilever", getRadioValue("fsws_model1_nonUnilever"), "Are non-Unilever brands placed in between Unilever brands?"],
        ["fsws_model1_shelfStrip", getRadioValue("fsws_model1_shelfStrip"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



    ]);

    processFields("Sachet Hanger", [
        ["sachetHangerAvailable", getRadioValue("sachetHangerAvailable"), "Any (Competition or Unilever) Sachet Hanger available in the Outlet?"],
        ["sachetHangerLaundryVisible", getRadioValue("sachetHangerLaundryVisible"), "Is this placed in a clearly visible place to the shopper?"],
        ["sachetHangerDisplayOption", getRadioValue("sachetHangerDisplayOption"), "Display Option"],
        ["sachetHangerdisplayPhoto", getFile("sachetHangerdisplayPhoto"), "Display Photo 1"],
        ["unileverPlanogramSeparate", getRadioValue("unileverPlanogramSeparate"), "Within the Unilever planogram, Unilever brands are merchandised separately?"],
        ["unileverBrandVariantsSeparate", getRadioValue("unileverBrandVariantsSeparate"), " Within the Unilever brands, relevant brand variants are merchandised separately?"],
        ["nonUnileverPlacement", getRadioValue("nonUnileverPlacement"), "Non-Unilever brands are not placed in between Unilever brands?"]
    ]);



    processFields("sachetHangerSavouryVisible", [
        ["sachetHangerSavouryVisible", getRadioValue("sachetHangerSavouryVisible"), "Is this placed in a clearly visible place to the shopper?"],
        ["sachetHangerSavouryDisplayOption", getRadioValue("sachetHangerSavouryDisplayOption"), "Display Option"],
        ["sachetHangerSavouryPhoto", getRadioValue("sachetHangerSavouryPhoto"), "Display Photo"],
        ["unileverPlanogramSavourySeparate", getFile("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
        ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
        ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
    ]);



    processFields("Sachet SHFD", [
        ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
        ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
        ["sachetHangerHFDPhoto", getRadioValue("sachetHangerHFDPhoto"), " Display Photo 1"],
        ["unileverPlanogramHFDSeparate", getFile("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
        ["unileverBrandVariantsHFDSeparate", getFile("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
        ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
    ]);

    processFields("Model 2", [
        ["fsws_model2_visible", getRadioValue("fsws_model2_visible"), "Is this placed in a clearly visible place to the shopper?"],
        ["fsws_model2_displayOption", getRadioValue("fsws_model2_displayOption"), "Display Option"],
        ["fsws_model2_displayPhoto", getFile("fsws_model2_displayPhoto"), "Display Photo 1"],
        ["fsws_model2_preVisitPhoto", getFile("fsws_model2_preVisitPhoto"), "Display Pre-Visit Photo"],
        ["fsws_model2_Shelf", getRadioValue("fsws_model2_Shelf"), "Is this stored on a Shelf (not in a cabinet)"],
        ["fsws_model2_competitors", getRadioValue("fsws_model2_competitors"), "Within the Unilever planogram,  Unilever brands are merchandised separately?"],
        ["fsws_model2_signage", getRadioValue("fsws_model2_signage"), "Within the Unilever brands,  relevant brand are variants merchandised separately?"],
        ["fsws_model2_safety", getRadioValue("fsws_model2_safety"), "Non-Unilever brands are not placed in between Unilever brands?"],
        ["fsws_model2_promoMaterial", getRadioValue("fsws_model2_promoMaterial"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]
    ]);

    processFields("Model 3", [
        ["fsws_model3_visible", getRadioValue("fsws_model3_visible"), "Is this placed in a clearly visible place to the shopper?"],
        ["fsws_model3_displayOption", getRadioValue("fsws_model3_displayOption"), "Display Option"],
        ["fsws_model3_displayPhoto", getFile("fsws_model3_displayPhoto"), "Display Photo 1"],
        ["fsws_model3_preVisitPhoto", getFile("fsws_model3_preVisitPhoto"), "Display Pre-Visit Photo"],





        ["fsws_model3_seasonal", getRadioValue("fsws_model3_seasonal"), "Is this stored on a Shelf (not in a cabinet)"],
        ["fsws_model3_lighting", getRadioValue("fsws_model3_lighting"), "Within the Unilever planogram,  Unilever brands are merchandised separately?"],
        ["fsws_model3_pricing", getRadioValue("fsws_model3_pricing"), "Within the Unilever brands,  relevant brand are variants merchandised separately?"],
        ["fsws_model3_maintenance", getRadioValue("fsws_model3_maintenance"), "Non-Unilever brands are not placed in between Unilever brands?"],
        ["fsws_model3_SHELFSTRIP", getRadioValue("fsws_model3_SHELFSTRIP"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]

    ]);

    processFields("Model 4", [
        ["fsws_model4_visible", getRadioValue("fsws_model4_visible"), "Is this placed in a clearly visible place to the shopper?"],
        ["fsws_model4_displayOption", getRadioValue("fsws_model4_displayOption"), "Display Option"],
        ["fsws_model4_displayPhoto", getFile("fsws_model4_displayPhoto"), "Display Photo 1"],
        ["fsws_model4_preVisitPhoto", getFile("fsws_model4_preVisitPhoto"), "Display Pre-Visit Photo"],
        ["fsws_model4_planogram", getRadioValue("fsws_model4_planogram"), "Within the Unilever planogram,  Unilever brands are merchandised separately?"],
        ["fsws_model4_brands", getRadioValue("fsws_model4_brands"), "Within the Unilever brands,  relevant brand are variants merchandised separately?"],
        ["fsws_model4_Non-Unileverbrands", getRadioValue("fsws_model4_Non-Unileverbrands"), "Non-Unilever brands are not placed in between Unilever brands?"],
        ["fsws_model4_SHELFSTRIP", getRadioValue("fsws_model4_SHELFSTRIP"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]
    ]);

  






    //const completionData = {};
    //let totalYesAnswers = 0;
    //let totalYesNoQuestions = 0;

    //Object.entries(sectionCompletion).forEach(([section, data]) => {
    //    const yesRate = data.totalYesNoQuestions > 0
    //        ? (data.yesAnswers / data.totalYesNoQuestions) * 100
    //        : 0;

    //    totalYesAnswers += data.yesAnswers;
    //    totalYesNoQuestions += data.totalYesNoQuestions;

    //    completionData[section] = {
    //        yesAnswers: data.yesAnswers,
    //        totalYesNoQuestions: data.totalYesNoQuestions,
    //        percentage: yesRate.toFixed(2),
    //        questions: data.questions.filter(q => q.type === 'yesno') // Only show Yes/No questions
    //    };
    //});





    const completionData = {};
    let totalYesAnswers = 0;
    let totalYesNoQuestions = 0;

    console.log('sectionCompletion:', JSON.stringify(sectionCompletion, null, 2)); // Debug output

    Object.entries(sectionCompletion).forEach(([section, data]) => {
        // Ensure data has all required properties
        data.questions = data.questions || [];
        data.yesAnswers = data.yesAnswers || 0;
        data.totalYesNoQuestions = data.totalYesNoQuestions || 0;

        const yesRate = data.totalYesNoQuestions > 0
            ? (data.yesAnswers / data.totalYesNoQuestions) * 100
            : 0;

        totalYesAnswers += data.yesAnswers;
        totalYesNoQuestions += data.totalYesNoQuestions;

        completionData[section] = {
            yesAnswers: data.yesAnswers,
            totalYesNoQuestions: data.totalYesNoQuestions,
            percentage: yesRate.toFixed(2),
            questions: data.questions.filter(q => q.type === 'yesno') // Safe now
        };
    });

    // Overall calculations
    const overallYesRate = totalYesNoQuestions > 0
        ? (totalYesAnswers / totalYesNoQuestions) * 100
        : 0;

    completionData.overall = {
        totalYesAnswers: totalYesAnswers,
        totalYesNoQuestions: totalYesNoQuestions,
        percentage: overallYesRate.toFixed(2)
    };

    // Store and report data
    sessionStorage.setItem('completionData', JSON.stringify(completionData));
    sessionStorage.setItem('formData', JSON.stringify(Object.fromEntries(formData)));

    console.log('======== Yes/No Questions Report ========');
    Object.entries(completionData).forEach(([section, data]) => {
        if (section !== 'overall') {
            console.log(`Section: ${section}`);
            console.log(`  Yes Answers: ${data.yesAnswers}/${data.totalYesNoQuestions} (${data.percentage}%)`);
            data.questions.forEach(q => {
                const status = q.answer === null || q.answer === undefined
                    ? 'Not answered'
                    : q.answer.toLowerCase() === 'yes' ? '✔ Yes' : '✖ No';
                console.log(`   - ${q.question}: ${status}`);
            });
            console.log('-----------------------------------------');
        }
    });
    console.log(`Overall Yes Answers: ${completionData.overall.totalYesAnswers}/${completionData.overall.totalYesNoQuestions} (${completionData.overall.percentage}%)`);

    console.log("\n🚀 Submitting form data to server...");
    for (let pair of formData.entries()) {
        console.log(`${pair[0]}:`, pair[1]);
    }









    fetch('/ReviewPlane/SaveFswsFormData', {
        method: 'POST',
        body: formData
    })
        .then(async (response) => {
            const contentType = response.headers.get("content-type") || "";
            if (response.ok) {
                if (contentType.includes("application/json")) {
                    const result = await response.json();

                    Swal.fire("✅ Success", `Form ID: ${result.formId}`, "success")
                        .then(() => {
                            window.location.href = reviewSummaryUrl;
                        });
                } else {
                    Swal.fire("❌ Error", "Submission failed.", "error");
                }
            } else {
                Swal.fire("❌ Error", "Server responded with an error.", "error");
            }
        })
        .catch((error) => {
            console.error("🔥 Error:", error);
            Swal.fire("🚫 Error", "Something went wrong.", "error");
        });












}














