


    function validateAndProceed()
    {
            const visibleToShopper = document.querySelector('input[name="visibleToShopper"]:checked');
    const sachetHangerAvailable = document.querySelector('input[name="sachetHangerAvailable"]:checked');
    const sachetHangerLaundryVisible = document.querySelector('input[name="sachetHangerLaundryVisible"]:checked');
    const displayOption = document.querySelector('input[name="displayOption"]:checked');
        const displayPhoto = document.getElementById('displayPhoto');
        const preVisitPhoto = document.getElementById('preVisitPhoto');


    const unileverSeparate = document.querySelector('input[name="unileverSeparate"]:checked');
    const brandVariantsSeparate = document.querySelector('input[name="brandVariantsSeparate"]:checked');
    const nonUnileverNotBetween = document.querySelector('input[name="nonUnileverNotBetween"]:checked');
    const shelfStripAvailable = document.querySelector('input[name="shelfStripAvailable"]:checked');
    const sachetHangerDisplayOption = document.querySelector('input[name="sachetHangerDisplayOption"]:checked');
    const sachetHangerdisplayPhoto = document.getElementById('sachetHangerdisplayPhoto');
    const sachetHangerunileverSeparate = document.querySelector('input[name="unileverPlanogramSeparate"]:checked');
    const sachetHangerBrandVariantsSeparate = document.querySelector('input[name="unileverBrandVariantsSeparate"]:checked');
    const sachetHangernonUnileverPlacement = document.querySelector('input[name="nonUnileverPlacement"]:checked');

    const sachetHangerSavouryVisible = document.querySelector('input[name="sachetHangerSavouryVisible"]:checked');
    const sachetHangerSavouryDisplayOption = document.querySelector('input[name="sachetHangerSavouryDisplayOption"]:checked');
    const sachetHangerSavouryPhoto = document.getElementById('sachetHangerSavouryPhoto');
    const sachetHangerunileverPlanogramSavourySeparate = document.querySelector('input[name="unileverPlanogramSavourySeparate"]:checked');
    const sachetHangerunileverBrandVariantsSavourySeparate = document.querySelector('input[name="unileverBrandVariantsSavourySeparate"]:checked');
    const sachetHangernonUnileverPlacementSavoury = document.querySelector('input[name="nonUnileverPlacementSavoury"]:checked');









    const sachetHangerHFDVisible = document.querySelector('input[name="sachetHangerHFDVisible"]:checked');



    const sachetHangerHFDDisplayOption = document.querySelector('input[name="sachetHangerHFDDisplayOption"]:checked');
    const sachetHangerHFDPhoto = document.getElementById('sachetHangerHFDPhoto');
    const sachetHangerHFDunileverPlanogramSeparate = document.querySelector('input[name="unileverPlanogramHFDSeparate"]:checked');
    const sachetHangerunileverBrandVariantsHFDSeparate = document.querySelector('input[name="unileverBrandVariantsHFDSeparate"]:checked');
    const sachetHangernonUnileverPlacementHFD = document.querySelector('input[name="nonUnileverPlacementHFD"]:checked');





    let isValid = true;
    let errorMessages = [];

    // Reset previous error styles
    resetErrors();

    // Validate "visibleToShopper"
    if (!visibleToShopper) {
        document.getElementById('visibleToShopperLabel').style.color = 'red';
    errorMessages.push("Please answer: Is this placed in a clearly visible place to the shopper?");
    isValid = false;
            }

    // Validate "sachetHangerAvailable"
    if (!sachetHangerAvailable) {
        document.getElementById('sachetHangerLabel').style.color = 'red';
    errorMessages.push("Please answer: Any (Competition or Unilever) Sachet Hanger available in the Outlet?");
    isValid = false;
            }

    // Validate sachetHangerLaundryVisible **only if sachetHangerAvailable is "yes"**
    if (sachetHangerAvailable && sachetHangerAvailable.value === "yes") {
                if (!sachetHangerLaundryVisible) {
        document.getElementById('sachetHangerLaundryLabel').style.color = 'red';
    errorMessages.push("Please answer: Is this placed in a clearly visible place to the shopper?");
    isValid = false;
                }

    if (!sachetHangerSavouryVisible) {
        document.getElementById('sachetHangerSavouryLabel').style.color = 'red';
    errorMessages.push("Please answer: Is the Sachet Hanger (Savoury) placed in a clearly visible place to the shopper?");
    isValid = false;
                }

    if (!sachetHangerHFDVisible) {
        document.getElementById('sachetHangerHFDLabel').style.color = 'red';
    errorMessages.push("Please answer: Is the Sachet Hanger (HFD) placed in a clearly visible place to the shopper?");
    isValid = false;
                }


            }

    // ✅ Validate these fields **only if "visibleToShopper" is "yes"**
    if (visibleToShopper && visibleToShopper.value === "yes")
    {
                if (!displayOption)
    {
        document.getElementById('displayOptionLabel').style.color = 'red';
    errorMessages.push("Please select a Display Option.");
    isValid = false;
                }

    if (!displayPhoto.files.length)
    {
        document.getElementById('displayPhotoLabel').style.color = 'red';
    errorMessages.push("Please upload a Display Photo.");
    isValid = false;
        }
        if (!preVisitPhoto.files.length) {
            document.getElementById('preVisitPhotolabel').style.color = 'red';
            errorMessages.push("Please upload a Display Photo.");
            isValid = false;
        }


    if (!unileverSeparate)
    {
        document.getElementById('unileverSeparateLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever planogram, are Unilever brands merchandised separately?");
    isValid = false;
                }

    if (!brandVariantsSeparate)
    {
        document.getElementById('brandVariantsSeparateLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;
                }

    if (!nonUnileverNotBetween)
    {
        document.getElementById('nonUnileverNotBetweenLabel').style.color = 'red';
    errorMessages.push("Please answer: Are non-Unilever brands placed in between Unilever brands?");
    isValid = false;
                }

    if (!shelfStripAvailable)

    {


        document.getElementById('shelfStripAvailableLabel').style.color = 'red';
    errorMessages.push("Please answer: Is ANY Unilever SHELF STRIP available in this Planogram?");
    isValid = false;
                }
             }


    if (sachetHangerLaundryVisible && sachetHangerLaundryVisible.value === "yes")
    {
               if (!sachetHangerDisplayOption) {

        document.getElementById('sachetHangerLaundrydisplayOptionLabel').style.color = 'red';
    errorMessages.push("Please select a Display Option.");
    isValid = false;
                }


    if(!sachetHangerdisplayPhoto.files.length){

        document.getElementById('sachetHangerLaundrydisplayPhotoLabel').style.color = 'red';
    errorMessages.push("Please upload a Display Photo.");
    isValid = false;
            }


    if(!sachetHangerunileverSeparate)
    {
        document.getElementById('sachetHangerLaundryunileverPlanogramLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever planogram, are Unilever brands merchandised separately?");
    isValid = false;

            }


    if(!sachetHangerBrandVariantsSeparate){
        document.getElementById('sachetHangerLaundryunileverBrandVariantsSLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;

            }



    if(!sachetHangernonUnileverPlacement){
        document.getElementById('sachetHangerLaundrynonUnileverPlacementLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;
            }




          }








    if (sachetHangerSavouryVisible && sachetHangerSavouryVisible.value === "yes")

    {
            if (!sachetHangerSavouryDisplayOption) {

        document.getElementById('sachetHangerSavourydisplayOptionLabel').style.color = 'red';
    errorMessages.push("Please select a Display Option.");
    isValid = false;
            }


    if(!sachetHangerSavouryPhoto.files.length){

        document.getElementById('sachetHangerSavourydisplayPhotoLabel').style.color = 'red';
    errorMessages.push("Please upload a Display Photo.");
    isValid = false;
            }


    if(!sachetHangerunileverPlanogramSavourySeparate)
    {
        document.getElementById('sachetHangerSavouryunileverPlanogramLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever planogram, are Unilever brands merchandised separately?");
    isValid = false;

            }


    if(!sachetHangerunileverBrandVariantsSavourySeparate){
        document.getElementById('sachetHangerSavouryunileverBrandVariantsSLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;

            }



    if(!sachetHangernonUnileverPlacementSavoury){
        document.getElementById('sachetHangerSavourynonUnileverPlacementLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;
            }




          }






    if (sachetHangerHFDVisible && sachetHangerHFDVisible.value === "yes")
    {
            if (!sachetHangerHFDDisplayOption) {

        document.getElementById('sachetHangerHFDdisplayOptionLabel').style.color = 'red';
    errorMessages.push("Please select a Display Option.");
    isValid = false;
            }

    if(!sachetHangerHFDPhoto.files.length){

        document.getElementById('sachetHangerHFDdisplayPhotoLabel').style.color = 'red';
    errorMessages.push("Please upload a Display Photo.");
    isValid = false;
            }




    if(!sachetHangerHFDunileverPlanogramSeparate)
    {
        document.getElementById('sachetHangerHFDunileverPlanogramLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever planogram, are Unilever brands merchandised separately?");
    isValid = false;

            }


    if(!sachetHangerunileverBrandVariantsHFDSeparate){
        document.getElementById('sachetHangerHFDunileverBrandVariantsSLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;

            }



    if(!sachetHangernonUnileverPlacementHFD){
        document.getElementById('sachetHangerHFDnonUnileverPlacementLabel').style.color = 'red';
    errorMessages.push("Please answer: Within the Unilever brands, are relevant brand variants merchandised separately?");
    isValid = false;
            }




         }






    // Show error messages if validation fails
    if (!isValid)
    {
        document.getElementById('errorMessage').innerHTML = errorMessages.join("<br>");
    showModal();
    return;
            }

    sendDataToDatabase();
        }




    // Reset error styling before validation
    function resetErrors() {
            const labels = [
    'visibleToShopperLabel', 'sachetHangerLabel', 'sachetHangerLaundryLabel', 'sachetHangerSavouryLabel','sachetHangerHFDLabel',
                'displayOptionLabel', 'displayPhotoLabel', 'unileverSeparateLabel','preVisitPhotolabel',
    'brandVariantsSeparateLabel', 'nonUnileverNotBetweenLabel', 'shelfStripAvailableLabel',
    'sachetHangerLaundrydisplayOptionLabel','sachetHangerLaundrydisplayPhotoLabel','sachetHangerLaundryunileverPlanogramLabel','sachetHangerLaundryunileverBrandVariantsSLabel'
    ,'sachetHangerLaundrynonUnileverPlacementLabel','sachetHangerSavourydisplayOptionLabel','sachetHangerSavourydisplayPhotoLabel','sachetHangerSavourydisplayPhotoLabel','sachetHangerSavouryunileverBrandVariantsSLabel'
    ,'sachetHangerSavourynonUnileverPlacementLabel','sachetHangerSavouryunileverPlanogramLabel','sachetHangerHFDdisplayOptionLabel','sachetHangerHFDdisplayPhotoLabel','sachetHangerHFDunileverPlanogramLabel','sachetHangerHFDunileverBrandVariantsSLabel'
    ,'sachetHangerHFDnonUnileverPlacementLabel'
    ];

            labels.forEach(id => {
                const element = document.getElementById(id);
    if (element) element.style.color = 'black';
            });
        }

    // Remove highlight on field change
    function removeHighlight(id) {
            const element = document.getElementById(id);
    if (element) element.style.color = 'black';
        }

    // Show error modal
    function showModal() {
        document.getElementById('errorModal').style.display = 'block';
        }

    // Close error modal
    function closeModal() {
        document.getElementById('errorModal').style.display = 'none';
        }




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

    function toggleDisplayOption(isVisible) {
        document.getElementById("displayOptionSection").style.display = isVisible ? "block" : "none";
    document.getElementById("displayPhotoSection").style.display = isVisible ? "block" : "none";
    document.getElementById("additionalQuestionsSection").style.display = isVisible ? "block" : "none";
                }

    // Function to open image modal (Placeholder function)
    function openImageModal() {
        var imageModal = new bootstrap.Modal(document.getElementById('imageModal'));
    imageModal.show();
    }




    function toggleSachetHangerVisibility(containerId)

    {

        let sachetHangerContainer = document.getElementById(containerId);
    let sachetHangerToggleBtn = document.getElementById("sachetHangerMainBtn");
    let sachetHangerToggleArrow = sachetHangerToggleBtn.querySelector(".sachetHangerArrow");

    sachetHangerContainer.classList.toggle("visible");

    sachetHangerToggleArrow.innerHTML = sachetHangerContainer.classList.contains("visible") ? "▲" : "▼";
        }

    function toggleSachetHangerOptions(isDisplayed)
    {
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

    function toggleSachetHangerSubSection(subSectionContainerId)
    {
        let sachetHangerSubSectionContainer = document.getElementById(subSectionContainerId);
    let sachetHangerSubSectionBtn = document.querySelector(`[data-section='${subSectionContainerId}']`);
    let sachetHangerSubSectionArrow = sachetHangerSubSectionBtn.querySelector(".sachetHangerArrow");

    sachetHangerSubSectionContainer.classList.toggle("visible");

    sachetHangerSubSectionArrow.innerHTML = sachetHangerSubSectionContainer.classList.contains("visible") ? "▲" : "▼";
        }


    function toggleDisplayQuestions(isDisplayed, containerId)
    {
        let container = document.getElementById(containerId);
    container.style.display = isDisplayed ? "block" : "none";
            }

    function toggleHangerVisibility(isVisible, sectionId) {
        let section = document.getElementById(sectionId);
    section.style.display = isVisible ? "block" : "none";
            }



    function toggleSachetHangerDetails(isVisible, sectionId)
    {
        let section = document.getElementById(sectionId);
    section.style.display = isVisible ? "block" : "none";
            }




    //function sendDataToDatabase() {


    //                    const formData = new FormData();






    //// Function to calculate percentage of filled fields
    //function calculatePercentage(fieldNames) {
    //    let filledFields = 0;

    //        fieldNames.forEach(name => {
    //            const isRadioGroup = document.querySelectorAll(`input[name="${name}"]`).length > 1;

    //if (isRadioGroup) {
    //                // Handle radio groups as single field
    //                const checked = document.querySelector(`input[name="${name}"]:checked`);
    //if (checked) filledFields++;
    //            } else {
    //                // Handle other fields
    //                const field = document.querySelector(`[name="${name}"]`);
    //if (!field) return;

    //if (field.type === "file") {
    //                    if (field.files.length > 0) filledFields++;
    //                } else if (field.type === "radio") {
    //                    if (field.checked) filledFields++;
    //                } else {
    //                    if (field.value.trim() !== "") filledFields++;
    //                }
    //            }
    //        });

    //        return fieldNames.length > 0 ? Math.round((filledFields / fieldNames.length) * 100) : 0;
    //    }

    //// Define sections with their actual field names
    //const sections1 = {
    //    "ModelDisplay": [
    //"visibleToShopper",
    //"displayOption",
    //"unileverSeparate",
    //"brandVariantsSeparate",
    //"nonUnileverNotBetween",
    //"shelfStripAvailable",
    //"displayPhoto"
    //],
    //"SachetHanger": ["sachetHangerAvailable"],
    //"SachetHanger.Laundry": [
    //"sachetHangerLaundryVisible",
    //"sachetHangerDisplayOption",
    //"unileverPlanogramSeparate",
    //"unileverBrandVariantsSeparate",
    //"nonUnileverPlacement",
    //"sachetHangerdisplayPhoto"
    //],
    //"SachetHanger.Savoury": [
    //"sachetHangerSavouryVisible",
    //"sachetHangerSavouryDisplayOption",
    //"unileverPlanogramSavourySeparate",
    //"unileverBrandVariantsSavourySeparate",
    //"nonUnileverPlacementSavoury",
    //"sachetHangerSavouryPhoto"
    //],
    //"SachetHanger.HFD": [
    //"sachetHangerHFDVisible",
    //"sachetHangerHFDDisplayOption",
    //"unileverPlanogramHFDSeparate",
    //"unileverBrandVariantsHFDSeparate",
    //"nonUnileverPlacementHFD",
    //"sachetHangerHFDPhoto"
    //]
    //    };

    //// Calculate percentages
    //const sectionPercentages = { };
    //    Object.entries(sections1).forEach(([sectionName, fieldNames]) => {
    //    sectionPercentages[sectionName] = calculatePercentage(fieldNames);
    //    });

    //console.log("Field Completion Percentages:", sectionPercentages);




    //sessionStorage.setItem("sectionPercentages", JSON.stringify(sectionPercentages));






    //// Model Display Data
    //formData.append("ModelDisplay.VisibleToShopper", document.querySelector('input[name="visibleToShopper"]:checked')?.value || "");
    //formData.append("ModelDisplay.DisplayOption", document.querySelector('input[name="displayOption"]:checked')?.value || "");
    //formData.append("ModelDisplay.UnileverSeparate", document.querySelector('input[name="unileverSeparate"]:checked')?.value || "");
    //formData.append("ModelDisplay.BrandVariantsSeparate", document.querySelector('input[name="brandVariantsSeparate"]:checked')?.value || "");
    //formData.append("ModelDisplay.NonUnileverNotBetween", document.querySelector('input[name="nonUnileverNotBetween"]:checked')?.value || "");
    //formData.append("ModelDisplay.ShelfStripAvailable", document.querySelector('input[name="shelfStripAvailable"]:checked')?.value || "");

    //// Append Model Display Photo (File)
    //const displayPhoto = document.getElementById('displayPhoto').files[0];
    //if (displayPhoto) {
    //    formData.append("ModelDisplay.DisplayPhoto", displayPhoto);
    //    }

    //// Sachet Hanger Data
    //formData.append("SachetHanger.SachetHangerAvailable", document.querySelector('input[name="sachetHangerAvailable"]:checked')?.value || "");

    //// Laundry Section
    //formData.append("SachetHanger.Laundry.Visible", document.querySelector('input[name="sachetHangerLaundryVisible"]:checked')?.value || "");
    //formData.append("SachetHanger.Laundry.DisplayOption", document.querySelector('input[name="sachetHangerDisplayOption"]:checked')?.value || "");
    //formData.append("SachetHanger.Laundry.PlanogramSeparate", document.querySelector('input[name="unileverPlanogramSeparate"]:checked')?.value || "");
    //formData.append("SachetHanger.Laundry.BrandVariantsSeparate", document.querySelector('input[name="unileverBrandVariantsSeparate"]:checked')?.value || "");
    //formData.append("SachetHanger.Laundry.NonUnileverPlacement", document.querySelector('input[name="nonUnileverPlacement"]:checked')?.value || "");

    //// Append Laundry Photo (File)
    //const laundryPhoto = document.querySelector('input[name="sachetHangerdisplayPhoto"]')?.files[0];
    //if (laundryPhoto) {
    //    formData.append("SachetHanger.Laundry.Photo", laundryPhoto);
    //    }

    //// Repeat for Savoury and HFD Sections
    //const sections = ["Savoury", "HFD"];
    //    sections.forEach(section => {
    //    formData.append(`SachetHanger.${section}.Visible`, document.querySelector(`input[name="sachetHanger${section}Visible"]:checked`)?.value || "");
    //formData.append(`SachetHanger.${section}.DisplayOption`, document.querySelector(`input[name="sachetHanger${section}DisplayOption"]:checked`)?.value || "");
    //formData.append(`SachetHanger.${section}.PlanogramSeparate`, document.querySelector(`input[name="unileverPlanogram${section}Separate"]:checked`)?.value || "");
    //formData.append(`SachetHanger.${section}.BrandVariantsSeparate`, document.querySelector(`input[name="unileverBrandVariants${section}Separate"]:checked`)?.value || "");
    //formData.append(`SachetHanger.${section}.NonUnileverPlacement`, document.querySelector(`input[name="nonUnileverPlacement${section}"]:checked`)?.value || "");

    //// Append Photos for Each Section
    //const photoInput = document.querySelector(`input[name="sachetHanger${section}Photo"]`)?.files[0];
    //if (photoInput) {
    //    formData.append(`SachetHanger.${section}.Photo`, photoInput);
    //        }
    //    });

    //// ✅ Debugging - Log FormData Contents
    //for (let pair of formData.entries()) {
    //    console.log(pair[0], pair[1]);
    //    }


    //console.log(formData);

    //console.log("Field Completion Percentages:", sectionPercentages);



    //fetch("/api/Display/save", {
    //    method: "POST",
    //body: formData,
    //headers: {
    //    "Accept": "application/json"
    //        }
    //    })
    //    .then(response => {
    //        if (!response.ok) {
    //            throw new Error(`HTTP error! Status: ${response.status}`);
    //        }
    //return response.text(); // Read response as text for debugging
    //    })
    //    .then(data => {
    //    console.log("Response:", data);
    //alert("✅ Data saved successfully!"); // Success message
    //        window.location.href = reviewSummaryUrl;
    //    })
    //    .catch(error => {
    //    console.error("Fetch Error:", error);
    //alert(`❌ Failed to save data. Error: ${error.message}`); // Alert with error status
    //    });



    //        }




//function sendDataToDatabase() {
//    const formData = new FormData();
//    let totalFields = 0;
//    let filledFields = 0;

//    const sectionCompletion = {};

//    const sectionDetails = {};




//    const getRadioValue = (name) => {
//        const checked = document.querySelector(`input[name="${name}"]:checked`);
//        return checked ? checked.value : null;
//    };

//    const getFile = (name) => {
//        const fileInput = document.getElementById(name);
//        return fileInput?.files?.length > 0 ? fileInput.files[0] : null;
//    };


//    const trackSection = (sectionName, isFilled, questionData = null) => {
//        if (!sectionCompletion[sectionName]) {
//            sectionCompletion[sectionName] = {
//                yesAnswers: 0,
//                totalYesNoQuestions: 0,
//                questions: []
//            };
//        }

//        if (questionData) {
//            sectionCompletion[sectionName].questions.push(questionData);

//            // Only count Yes/No questions (excluding photos/files)
//            if (questionData.type === 'yesno') {
//                sectionCompletion[sectionName].totalYesNoQuestions++;
//                if (isFilled) {
//                    sectionCompletion[sectionName].yesAnswers++;
//                }
//            }
//        }
//    };





//    const processField = (section, name, value, question) => {
//        const isPhotoUpload = name.toLowerCase().includes('photo');
//        const isFileUpload = value instanceof File;

//        // Add to form data
//        formData.append(name, value || '');

//        // Prepare question data for tracking
//        const questionData = {
//            name,
//            question,
//            answer: value,
//            isAnswered: false
//        };

//        // Determine if field is filled based on type
//        let isFilled;
//        if (isFileUpload) {
//            isFilled = value !== null;
//            questionData.type = 'file';
//        } else if (isPhotoUpload) {
//            isFilled = value !== null;
//            questionData.type = 'photo';
//        } else {
//            isFilled = value !== null && value !== "" && value !== undefined;
//            questionData.type = 'field';
//        }

//        questionData.isAnswered = isFilled;

//        // Track the field
//        trackSection(section, isFilled, questionData);

//        // Log the processing
//        const fieldType = isFileUpload ? 'File' : (isPhotoUpload ? 'Photo' : 'Field');
//        console.log(`[${section}] ${fieldType}: ${name} | Filled: ${isFilled} | Value:`, value, "| Question:", question);
//    };

//    const processFields = (section, fields) => {
//        console.log(`\n=== Processing Section: ${section} ===`);
//        fields.forEach(([name, value, question]) => {
//            processField(section, name, value, question);
//        });
//    };





//    // ===== Process All Sections =====
//    processFields("Model 1", [
//        ["visibleToShopper", getRadioValue("visibleToShopper"), "Is this placed in a clearly visible place to the shopper?"],
//        ["displayOption", getRadioValue("displayOption"), "Display Option"],
//        ["displayPhoto", getFile("displayPhoto"), "Display Photo 1"],
//        ["preVisitPhoto", getFile("preVisitPhoto"), "Display Pre-Visit Photo"],
//        ["unileverSeparate", getRadioValue("unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
//        ["brandVariantsSeparate", getRadioValue("brandVariantsSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//        ["nonUnileverNotBetween", getRadioValue("nonUnileverNotBetween"), "Are non-Unilever brands placed in between Unilever brands?"],
//        ["shelfStripAvailable", getRadioValue("shelfStripAvailable"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



//    ]);

//    processFields("Sachet Hanger", [
//        ["sachetHangerAvailable", getRadioValue("sachetHangerAvailable"), "Any (Competition or Unilever) Sachet Hanger available in the Outlet?"],
//        ["sachetHangerLaundryVisible", getRadioValue("sachetHangerLaundryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//        ["sachetHangerDisplayOption", getRadioValue("sachetHangerDisplayOption"), "Display Option"],
//        ["sachetHangerdisplayPhoto", getFile("sachetHangerdisplayPhoto"), "Display Photo 1"],
//        ["unileverPlanogramSeparate", getRadioValue("unileverPlanogramSeparate"), "Within the Unilever planogram, Unilever brands are merchandised separately?"],
//        ["unileverBrandVariantsSeparate", getRadioValue("unileverBrandVariantsSeparate"), " Within the Unilever brands, relevant brand variants are merchandised separately?"],
//        ["nonUnileverPlacement", getRadioValue("nonUnileverPlacement"), "Non-Unilever brands are not placed in between Unilever brands?"]
//    ]);



//    processFields("sachetHangerSavouryVisible", [
//        ["sachetHangerSavouryVisible", getRadioValue("sachetHangerSavouryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//        ["sachetHangerSavouryDisplayOption", getRadioValue("sachetHangerSavouryDisplayOption"), "Display Option"],
//        ["sachetHangerSavouryPhoto", getRadioValue("sachetHangerSavouryPhoto"), "Display Photo"],
//        ["unileverPlanogramSavourySeparate", getFile("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//        ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//        ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
//    ]);



//    processFields("Sachet SHFD", [
//        ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
//        ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
//        ["sachetHangerHFDPhoto", getRadioValue("sachetHangerHFDPhoto"), " Display Photo 1"],
//        ["unileverPlanogramHFDSeparate", getFile("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//        ["unileverBrandVariantsHFDSeparate", getFile("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//        ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
//    ]);





//    const completionData = {};
//    let totalPercentage = 0;
//    const sectionCount = Object.keys(sectionCompletion).length;

//    Object.entries(sectionCompletion).forEach(([section, data]) => {
//        const percentage = data.total > 0 ? Math.min((data.filled / data.total) * 100, 100) : 0;
//        totalPercentage += percentage;

//        completionData[section] = {
//            answered: data.filled,
//            total: data.total,
//            percentage: percentage.toFixed(2),
//            questions: data.questions || []
//        };
//    });

//    // Calculate overall average
//    const totalAverage = sectionCount > 0 ? totalPercentage / sectionCount : 0;
//    completionData.overall = {
//        average: totalAverage.toFixed(2)
//    };

//    // Store data in sessionStorage
//    sessionStorage.setItem('completionData', JSON.stringify(completionData));
//    sessionStorage.setItem('formData', JSON.stringify(Object.fromEntries(formData)));

//    // Generate completion report
//    console.log('======== Audit Completion Report ========');
//    Object.entries(completionData).forEach(([section, data]) => {
//        if (section !== 'overall') {
//            console.log(`Section: ${section}`);
//            console.log(`  Completion: ${data.percentage}% (${data.answered}/${data.total})`);
//            console.log('  Questions:');
//            data.questions.forEach(q => {
//                const answer = q.type === 'file' ? (q.isAnswered ? 'File uploaded' : 'No file') :
//                    q.type === 'photo' ? (q.isAnswered ? 'Photo uploaded' : 'No photo') :
//                        q.answer || 'Not answered';
//                console.log(`    - ${q.question}: ${answer} (${q.isAnswered ? '✔' : '✖'})`);
//            });
//            console.log('-----------------------------------------');
//        }
//    });
//    console.log(`Overall Completion: ${completionData.overall.average}%`);
//    console.log('=========================================');




//    console.log("\n🚀 Submitting form data to server...");
//    for (let pair of formData.entries()) {
//        console.log(`${pair[0]}:`, pair[1]);
//    }


//    fetch('/ReviewPlane/SaveFswsFormData', {
//        method: 'POST',
//        body: formData
//    })
//        .then(async (response) => {
//            const contentType = response.headers.get("content-type") || "";
//            if (response.ok) {
//                if (contentType.includes("application/json")) {
//                    const result = await response.json();

//                    Swal.fire("✅ Success", `Form ID: ${result.formId}`, "success")
//                        .then(() => {
//                            window.location.href = reviewSummaryUrl;
//                        });
//                } else {
//                    Swal.fire("❌ Error", "Submission failed.", "error");
//                }
//            } else {
//                Swal.fire("❌ Error", "Server responded with an error.", "error");
//            }
//        })
//        .catch((error) => {
//            console.error("🔥 Error:", error);
//            Swal.fire("🚫 Error", "Something went wrong.", "error");
//        });














//}



//function sendDataToDatabase() {
//    const formData = new FormData();
//    const sectionCompletion = {};
//    const sectionDetails = {};

//    const getRadioValue = (name) => {
//        const checked = document.querySelector(`input[name="${name}"]:checked`);
//        return checked ? checked.value : null;
//    };

//    const getFile = (name) => {
//        const fileInput = document.getElementById(name);
//        return fileInput?.files?.length > 0 ? fileInput.files[0] : null;
//    };

//    const trackSection = (sectionName, isYes, questionData = null) => {
//        if (!sectionCompletion[sectionName]) {
//            sectionCompletion[sectionName] = {
//                yesAnswers: 0,
//                totalYesNoQuestions: 0,
//                questions: []
//            };
//        }

//        if (questionData?.type === 'yesno') {
//            sectionCompletion[sectionName].totalYesNoQuestions++;
//            if (isYes) {
//                sectionCompletion[sectionName].yesAnswers++;
//            }
//            sectionCompletion[sectionName].questions.push(questionData);
//        }
//    };

//    const processField = (section, name, value, question) => {
//        const isPhotoUpload = name.toLowerCase().includes('photo');
//        const isFileUpload = value instanceof File;

//        // Always add to form data (including photos/files)
//        formData.append(name, value || '');

//        // Prepare question data
//        const questionData = {
//            name,
//            question,
//            answer: value,
//            type: isFileUpload ? 'file' : (isPhotoUpload ? 'photo' : 'yesno')
//        };

//        if (isPhotoUpload || isFileUpload) {
//            // Just log photos/files, don't count them
//            console.log(`[${section}] ${isPhotoUpload ? 'Photo' : 'File'}: ${name} | Uploaded: ${value !== null}`);
//        } else {
//            // For radio buttons (Yes/No questions)
//            const standardizedValue = String(value).toLowerCase().trim();
//            const isYes = standardizedValue === 'yes' || standardizedValue === 'y';

//            trackSection(section, isYes, questionData);
//            console.log(`[${section}] Question: ${name} | Answer: ${value} | Counted as Yes: ${isYes}`);
//        }
//    };

//    const processFields = (section, fields) => {
//        console.log(`\n=== Processing Section: ${section} ===`);
//        fields.forEach(([name, value, question]) => {
//            processField(section, name, value, question);
//        });
//    };


//        // ===== Process All Sections =====
//        processFields("Model 1", [
//            ["visibleToShopper", getRadioValue("visibleToShopper"), "Is this placed in a clearly visible place to the shopper?"],
//            ["displayOption", getRadioValue("displayOption"), "Display Option"],
//            ["displayPhoto", getFile("displayPhoto"), "Display Photo 1"],
//            ["preVisitPhoto", getFile("preVisitPhoto"), "Display Pre-Visit Photo"],
//            ["unileverSeparate", getRadioValue("unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
//            ["brandVariantsSeparate", getRadioValue("brandVariantsSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//            ["nonUnileverNotBetween", getRadioValue("nonUnileverNotBetween"), "Are non-Unilever brands placed in between Unilever brands?"],
//            ["shelfStripAvailable", getRadioValue("shelfStripAvailable"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



//        ]);

//        processFields("Sachet Hanger", [
//            ["sachetHangerAvailable", getRadioValue("sachetHangerAvailable"), "Any (Competition or Unilever) Sachet Hanger available in the Outlet?"],
//            ["sachetHangerLaundryVisible", getRadioValue("sachetHangerLaundryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//            ["sachetHangerDisplayOption", getRadioValue("sachetHangerDisplayOption"), "Display Option"],
//            ["sachetHangerdisplayPhoto", getFile("sachetHangerdisplayPhoto"), "Display Photo 1"],
//            ["unileverPlanogramSeparate", getRadioValue("unileverPlanogramSeparate"), "Within the Unilever planogram, Unilever brands are merchandised separately?"],
//            ["unileverBrandVariantsSeparate", getRadioValue("unileverBrandVariantsSeparate"), " Within the Unilever brands, relevant brand variants are merchandised separately?"],
//            ["nonUnileverPlacement", getRadioValue("nonUnileverPlacement"), "Non-Unilever brands are not placed in between Unilever brands?"]
//        ]);



//        processFields("sachetHangerSavouryVisible", [
//            ["sachetHangerSavouryVisible", getRadioValue("sachetHangerSavouryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//            ["sachetHangerSavouryDisplayOption", getRadioValue("sachetHangerSavouryDisplayOption"), "Display Option"],
//            ["sachetHangerSavouryPhoto", getRadioValue("sachetHangerSavouryPhoto"), "Display Photo"],
//            ["unileverPlanogramSavourySeparate", getFile("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//            ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//            ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
//        ]);



//        processFields("Sachet SHFD", [
//            ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
//            ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
//            ["sachetHangerHFDPhoto", getRadioValue("sachetHangerHFDPhoto"), " Display Photo 1"],
//            ["unileverPlanogramHFDSeparate", getFile("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//            ["unileverBrandVariantsHFDSeparate", getFile("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//            ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
//        ]);





//    // Calculate completion based only on Yes/No questions
//    const completionData = {};
//    let totalYesAnswers = 0;
//    let totalYesNoQuestions = 0;

//    Object.entries(sectionCompletion).forEach(([section, data]) => {
//        const percentage = data.totalYesNoQuestions > 0
//            ? Math.min((data.yesAnswers / data.totalYesNoQuestions) * 100, 100)
//            : 0;

//        totalYesAnswers += data.yesAnswers;
//        totalYesNoQuestions += data.totalYesNoQuestions;

//        completionData[section] = {
//            yesAnswers: data.yesAnswers,
//            totalYesNoQuestions: data.totalYesNoQuestions,
//            percentage: percentage.toFixed(2),
//            questions: data.questions
//        };
//    });

//    // Calculate overall average
//    const totalAverage = totalYesNoQuestions > 0
//        ? (totalYesAnswers / totalYesNoQuestions) * 100
//        : 0;

//    completionData.overall = {
//        average: totalAverage.toFixed(2),
//        totalYesAnswers,
//        totalYesNoQuestions
//    };

//    // Store data
//    sessionStorage.setItem('completionData', JSON.stringify(completionData));
//    sessionStorage.setItem('formData', JSON.stringify(Object.fromEntries(formData)));

//    // Generate report
//    console.log('======== Yes Answers Report ========');
//    Object.entries(completionData).forEach(([section, data]) => {
//        if (section !== 'overall') {
//            console.log(`Section: ${section}`);
//            console.log(`  Yes Answers: ${data.yesAnswers}/${data.totalYesNoQuestions} (${data.percentage}%)`);
//            data.questions.forEach(q => {
//                console.log(`    - ${q.question}: ${q.answer} (${q.answer?.toLowerCase() === 'yes' ? '✔ Yes' : '✖ No'})`);
//            });
//            console.log('-----------------------------------------');
//        }
//    });
//    console.log(`Overall Yes Answers: ${completionData.overall.totalYesAnswers}/${completionData.overall.totalYesNoQuestions} (${completionData.overall.average}%)`);

//    // Submit to server
//    fetch('/ReviewPlane/SaveFswsFormData', {
//        method: 'POST',
//        body: formData
//    })
//        .then(async (response) => {
//            // ... existing response handling ...
//        })
//        .catch((error) => {
//            // ... existing error handling ...
//        });
//}




//function sendDataToDatabase() {
//    const formData = new FormData();
//    const sectionCompletion = {};
//    const sectionDetails = {};

//    const getRadioValue = (name) => {
//        const checked = document.querySelector(`input[name="${name}"]:checked`);
//        return checked ? checked.value : null;
//    };

//    const getFile = (name) => {
//        const fileInput = document.getElementById(name);
//        return fileInput?.files?.length > 0 ? fileInput.files[0] : null;
//    };

//    const trackSection = (sectionName, isYes, questionData = null) => {
//        if (!sectionCompletion[sectionName]) {
//            sectionCompletion[sectionName] = {
//                yesAnswers: 0,
//                totalYesNoQuestions: 0,
//                questions: []
//            };
//        }

//        if (questionData?.type === 'yesno') {
//            sectionCompletion[sectionName].totalYesNoQuestions++;
//            if (isYes) {
//                sectionCompletion[sectionName].yesAnswers++;
//            }
//            sectionCompletion[sectionName].questions.push(questionData);
//        }
//    };

//    const processField = (section, name, value, question) => {
//        const isPhotoUpload = name.toLowerCase().includes('photo');
//        const isFileUpload = value instanceof File;
//        const isDisplayOption = name.toLowerCase().includes('displayoption');

//        // Always add to form data (including photos/files and display options)
//        formData.append(name, value || '');

//        // Prepare question data
//        const questionData = {
//            name,
//            question,
//            answer: value,
//            type: isFileUpload ? 'file' :
//                isPhotoUpload ? 'photo' :
//                    isDisplayOption ? 'displayoption' : 'yesno'
//        };

//        if (isPhotoUpload || isFileUpload) {
//            // Just log photos/files, don't count them
//            console.log(`[${section}] ${isPhotoUpload ? 'Photo' : 'File'}: ${name} | Uploaded: ${value !== null}`);
//        }
//        else if (isDisplayOption) {
//            // Log display options but don't count them
//            console.log(`[${section}] Display Option: ${name} | Value: ${value}`);
//        }
//        else {
//            // For Yes/No questions (not display options)
//            const hasAnswer = value !== null && value !== undefined; // empty string is considered an answer ("")
//            const isYes = hasAnswer && String(value).toLowerCase().trim() === 'yes';

//            trackSection(section, isYes, questionData);
//            console.log(`[${section}] Question: ${name} | Answer: ${value} | Counted as Yes: ${isYes} | Has Answer: ${hasAnswer}`);
//        }
//    };

//    const processFields = (section, fields) => {
//        console.log(`\n=== Processing Section: ${section} ===`);
//        fields.forEach(([name, value, question]) => {
//            processField(section, name, value, question);
//        });
//    };




//    // ===== Process All Sections =====
//            processFields("Model 1", [
//                ["visibleToShopper", getRadioValue("visibleToShopper"), "Is this placed in a clearly visible place to the shopper?"],
//                ["displayOption", getRadioValue("displayOption"), "Display Option"],
//                ["displayPhoto", getFile("displayPhoto"), "Display Photo 1"],
//                ["preVisitPhoto", getFile("preVisitPhoto"), "Display Pre-Visit Photo"],
//                ["unileverSeparate", getRadioValue("unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                ["brandVariantsSeparate", getRadioValue("brandVariantsSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                ["nonUnileverNotBetween", getRadioValue("nonUnileverNotBetween"), "Are non-Unilever brands placed in between Unilever brands?"],
//                ["shelfStripAvailable", getRadioValue("shelfStripAvailable"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



//            ]);

//            processFields("Sachet Hanger", [
//                ["sachetHangerAvailable", getRadioValue("sachetHangerAvailable"), "Any (Competition or Unilever) Sachet Hanger available in the Outlet?"],
//                ["sachetHangerLaundryVisible", getRadioValue("sachetHangerLaundryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                ["sachetHangerDisplayOption", getRadioValue("sachetHangerDisplayOption"), "Display Option"],
//                ["sachetHangerdisplayPhoto", getFile("sachetHangerdisplayPhoto"), "Display Photo 1"],
//                ["unileverPlanogramSeparate", getRadioValue("unileverPlanogramSeparate"), "Within the Unilever planogram, Unilever brands are merchandised separately?"],
//                ["unileverBrandVariantsSeparate", getRadioValue("unileverBrandVariantsSeparate"), " Within the Unilever brands, relevant brand variants are merchandised separately?"],
//                ["nonUnileverPlacement", getRadioValue("nonUnileverPlacement"), "Non-Unilever brands are not placed in between Unilever brands?"]
//            ]);



//            processFields("sachetHangerSavouryVisible", [
//                ["sachetHangerSavouryVisible", getRadioValue("sachetHangerSavouryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                ["sachetHangerSavouryDisplayOption", getRadioValue("sachetHangerSavouryDisplayOption"), "Display Option"],
//                ["sachetHangerSavouryPhoto", getRadioValue("sachetHangerSavouryPhoto"), "Display Photo"],
//                ["unileverPlanogramSavourySeparate", getFile("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
//            ]);



//            processFields("Sachet SHFD", [
//                ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
//                ["sachetHangerHFDPhoto", getRadioValue("sachetHangerHFDPhoto"), " Display Photo 1"],
//                ["unileverPlanogramHFDSeparate", getFile("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                ["unileverBrandVariantsHFDSeparate", getFile("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
//            ]);



//    // Calculate completion based only on Yes/No questions (excluding display options)
//    const completionData = {};
//    let totalYesAnswers = 0;
//    let totalYesNoQuestions = 0;

//    Object.entries(sectionCompletion).forEach(([section, data]) => {
//        const percentage = data.totalYesNoQuestions > 0
//            ? Math.min((data.yesAnswers / data.totalYesNoQuestions) * 100, 100)
//            : 0;

//        totalYesAnswers += data.yesAnswers;
//        totalYesNoQuestions += data.totalYesNoQuestions;

//        completionData[section] = {
//            yesAnswers: data.yesAnswers,
//            totalYesNoQuestions: data.totalYesNoQuestions,
//            percentage: percentage.toFixed(2),
//            questions: data.questions
//        };
//    });

//    // Calculate overall average
//    const totalAverage = totalYesNoQuestions > 0
//        ? (totalYesAnswers / totalYesNoQuestions) * 100
//        : 0;

//    completionData.overall = {
//        average: totalAverage.toFixed(2),
//        totalYesAnswers,
//        totalYesNoQuestions
//    };

//    // Store data
//    sessionStorage.setItem('completionData', JSON.stringify(completionData));
//    sessionStorage.setItem('formData', JSON.stringify(Object.fromEntries(formData)));

//    // Generate report
//    console.log('======== Yes Answers Report ========');
//    Object.entries(completionData).forEach(([section, data]) => {
//        if (section !== 'overall') {
//            console.log(`Section: ${section}`);
//            console.log(`  Yes Answers: ${data.yesAnswers}/${data.totalYesNoQuestions} (${data.percentage}%)`);
//            data.questions.forEach(q => {
//                const answerStatus = q.answer === null || q.answer === undefined ?
//                    'Not answered' :
//                    q.answer === '' ? 'Empty answer (No)' :
//                        q.answer.toLowerCase() === 'yes' ? '✔ Yes' : '✖ No';
//                console.log(`    - ${q.question}: ${answerStatus}`);
//            });
//            console.log('-----------------------------------------');
//        }
//    });
//    console.log(`Overall Yes Answers: ${completionData.overall.totalYesAnswers}/${completionData.overall.totalYesNoQuestions} (${completionData.overall.average}%)`);

//    // Submit to server
//    fetch('/ReviewPlane/SaveFswsFormData', {
//        method: 'POST',
//        body: formData
//    })
//        .then(async (response) => {
//            // ... existing response handling ...
//        })
//        .catch((error) => {
//            // ... existing error handling ...
//        });
//}




//function sendDataToDatabase() {
//    const formData = new FormData();
//    const sectionCompletion = {};

//    const getRadioValue = (name) => {
//        const checked = document.querySelector(`input[name="${name}"]:checked`);
//        return checked ? checked.value : null;
//    };

//    const getFile = (name) => {
//        const fileInput = document.getElementById(name);
//        return fileInput?.files?.length > 0 ? fileInput.files[0] : null;
//    };

//    //const trackSection = (sectionName, isYes, questionData) => {
//    //    if (!sectionCompletion[sectionName]) {
//    //        sectionCompletion[sectionName] = {
//    //            yesAnswers: 0,
//    //            answeredQuestions: 0,
//    //            totalQuestions: 0,
//    //            questions: []
//    //        };
//    //    }

//    //    sectionCompletion[sectionName].totalQuestions++;

//    //    if (questionData.type === 'yesno') {
//    //        const hasAnswer = value !== null && value !== undefined;
//    //        const isYes = hasAnswer && String(value).toLowerCase().trim() === 'yes';


//    //        if (hasAnswer) {
//    //            sectionCompletion[sectionName].answeredQuestions++;
//    //            if (isYes) {
//    //                sectionCompletion[sectionName].yesAnswers++;
//    //            }
//    //        }
//    //    }
//    //    sectionCompletion[sectionName].questions.push(questionData);
//    //};

//    //const processField = (section, name, value, question) => {
//    //    const isPhotoUpload = name.toLowerCase().includes('photo');
//    //    const isFileUpload = value instanceof File;
//    //    const isDisplayOption = name.toLowerCase().includes('displayoption');

//    //    // Always add to form data
//    //    formData.append(name, value || '');

//    //    const questionData = {
//    //        name,
//    //        question,
//    //        answer: value,
//    //        type: isFileUpload ? 'file' :
//    //            isPhotoUpload ? 'photo' :
//    //                isDisplayOption ? 'displayoption' : 'yesno'
//    //    };

//    //    if (isPhotoUpload || isFileUpload || isDisplayOption) {
//    //        // Don't count these in completion percentage
//    //        console.log(`[${section}] ${isPhotoUpload ? 'Photo' : isFileUpload ? 'File' : 'Display Option'}: ${name} | Value: ${value}`);
//    //        trackSection(section, false, false, questionData);
//    //    } else {
//    //        // For Yes/No questions
//    //        const hasAnswer = value !== null && value !== undefined;
//    //        const isYes = hasAnswer && String(value).toLowerCase().trim() === 'yes';

//    //        trackSection(section, hasAnswer, isYes, questionData);
//    //        console.log(`[${section}] Question: ${name} | Answer: ${value} | Has Answer: ${hasAnswer} | Is Yes: ${isYes}`);
//    //    }
//    //};



//    const trackSection = (sectionName, value, questionData) => {
//        if (!sectionCompletion[sectionName]) {
//            sectionCompletion[sectionName] = {
//                yesAnswers: 0,
//                answeredQuestions: 0,
//                totalQuestions: 0,
//                questions: []
//            };
//        }

//        sectionCompletion[sectionName].totalQuestions++;

//        // For Yes/No questions
//        if (questionData.type === 'yesno') {
//            const hasAnswer = value !== null && value !== undefined;
//            const isYes = hasAnswer && String(value).toLowerCase().trim() === 'yes';

//            if (hasAnswer) {
//                sectionCompletion[sectionName].answeredQuestions++;
//                if (isYes) {
//                    sectionCompletion[sectionName].yesAnswers++;
//                }
//            }
//        }
//        // For other types (photos, display options) - don't count in completion
//        else {
//            questionData.isAnswered = value !== null && value !== undefined;
//        }

//        sectionCompletion[sectionName].questions.push(questionData);
//    };

//    const processField = (section, name, value, question) => {
//        const isPhotoUpload = name.toLowerCase().includes('photo');
//        const isFileUpload = value instanceof File;
//        const isDisplayOption = name.toLowerCase().includes('displayoption');

//        formData.append(name, value || '');

//        const questionData = {
//            name,
//            question,
//            answer: value,
//            type: isFileUpload ? 'file' :
//                isPhotoUpload ? 'photo' :
//                    isDisplayOption ? 'displayoption' : 'yesno'
//        };

//        trackSection(section, value, questionData);

//        // Enhanced logging
//        let status;
//        if (questionData.type === 'yesno') {
//            if (value === null || value === undefined) {
//                status = 'Not answered';
//            } else {
//                status = String(value).toLowerCase().trim() === 'yes' ? '✔ Yes' : '✖ No';
//            }
//        } else {
//            status = value !== null ? `Value: ${value}` : 'Not uploaded';
//        }

//        console.log(`[${section}] ${questionData.type.toUpperCase()}: ${name} | ${status}`);
//    };

//    const processFields = (section, fields) => {
//        console.log(`\n=== Processing Section: ${section} ===`);
//        fields.forEach(([name, value, question]) => {
//            processField(section, name, value, question);
//        });
//    };

//                processFields("Model 1", [
//                    ["visibleToShopper", getRadioValue("visibleToShopper"), "Is this placed in a clearly visible place to the shopper?"],
//                    ["displayOption", getRadioValue("displayOption"), "Display Option"],
//                    ["displayPhoto", getFile("displayPhoto"), "Display Photo 1"],
//                    ["preVisitPhoto", getFile("preVisitPhoto"), "Display Pre-Visit Photo"],
//                    ["unileverSeparate", getRadioValue("unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                    ["brandVariantsSeparate", getRadioValue("brandVariantsSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                    ["nonUnileverNotBetween", getRadioValue("nonUnileverNotBetween"), "Are non-Unilever brands placed in between Unilever brands?"],
//                    ["shelfStripAvailable", getRadioValue("shelfStripAvailable"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



//                ]);

//                processFields("Sachet Hanger", [
//                    ["sachetHangerAvailable", getRadioValue("sachetHangerAvailable"), "Any (Competition or Unilever) Sachet Hanger available in the Outlet?"],
//                    ["sachetHangerLaundryVisible", getRadioValue("sachetHangerLaundryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                    ["sachetHangerDisplayOption", getRadioValue("sachetHangerDisplayOption"), "Display Option"],
//                    ["sachetHangerdisplayPhoto", getFile("sachetHangerdisplayPhoto"), "Display Photo 1"],
//                    ["unileverPlanogramSeparate", getRadioValue("unileverPlanogramSeparate"), "Within the Unilever planogram, Unilever brands are merchandised separately?"],
//                    ["unileverBrandVariantsSeparate", getRadioValue("unileverBrandVariantsSeparate"), " Within the Unilever brands, relevant brand variants are merchandised separately?"],
//                    ["nonUnileverPlacement", getRadioValue("nonUnileverPlacement"), "Non-Unilever brands are not placed in between Unilever brands?"]
//                ]);



//                processFields("sachetHangerSavouryVisible", [
//                    ["sachetHangerSavouryVisible", getRadioValue("sachetHangerSavouryVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                    ["sachetHangerSavouryDisplayOption", getRadioValue("sachetHangerSavouryDisplayOption"), "Display Option"],
//                    ["sachetHangerSavouryPhoto", getRadioValue("sachetHangerSavouryPhoto"), "Display Photo"],
//                    ["unileverPlanogramSavourySeparate", getFile("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                    ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                    ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
//                ]);



//                processFields("Sachet SHFD", [
//                    ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
//                    ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
//                    ["sachetHangerHFDPhoto", getRadioValue("sachetHangerHFDPhoto"), " Display Photo 1"],
//                    ["unileverPlanogramHFDSeparate", getFile("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
//                    ["unileverBrandVariantsHFDSeparate", getFile("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
//                    ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
//                ]);


//    // Calculate completion data
//    const completionData = {};
//    let totalYesAnswers = 0;
//    let totalAnsweredQuestions = 0;
//    let totalQuestions = 0;

//    Object.entries(sectionCompletion).forEach(([section, data]) => {
//        const answerRate = data.totalQuestions > 0
//            ? (data.answeredQuestions / data.totalQuestions) * 100
//            : 0;

//        const yesRate = data.answeredQuestions > 0
//            ? (data.yesAnswers / data.answeredQuestions) * 100
//            : 0;

//        totalYesAnswers += data.yesAnswers;
//        totalAnsweredQuestions += data.answeredQuestions;
//        totalQuestions += data.totalQuestions;

//        completionData[section] = {
//            yesAnswers: data.yesAnswers,
//            answeredQuestions: data.answeredQuestions,
//            totalQuestions: data.totalQuestions,
//            answerRate: answerRate.toFixed(2),
//            yesRate: yesRate.toFixed(2),
//            questions: data.questions
//        };
//    });

//    // Overall calculations
//    const overallAnswerRate = totalQuestions > 0
//        ? (totalAnsweredQuestions / totalQuestions) * 100
//        : 0;

//    const overallYesRate = totalAnsweredQuestions > 0
//        ? (totalYesAnswers / totalAnsweredQuestions) * 100
//        : 0;

//    completionData.overall = {
//        yesAnswers: totalYesAnswers,
//        answeredQuestions: totalAnsweredQuestions,
//        totalQuestions: totalQuestions,
//        answerRate: overallAnswerRate.toFixed(2),
//        yesRate: overallYesRate.toFixed(2)
//    };

//    // Store and report data
//    sessionStorage.setItem('completionData', JSON.stringify(completionData));
//    sessionStorage.setItem('formData', JSON.stringify(Object.fromEntries(formData)));

//    console.log('======== Completion Report ========');
//    Object.entries(completionData).forEach(([section, data]) => {
//        if (section !== 'overall') {
//            console.log(`Section: ${section}`);
//            console.log(`  Answered: ${data.answeredQuestions}/${data.totalQuestions} (${data.answerRate}%)`);
//            console.log(`  Yes Rate: ${data.yesAnswers}/${data.answeredQuestions} (${data.yesRate}%)`);
//            data.questions.forEach(q => {
//                const status = q.answer === null || q.answer === undefined ? 'Not answered' :
//                    q.type === 'yesno' ? (q.answer.toLowerCase() === 'yes' ? '✔ Yes' : '✖ No') :
//                        `Value: ${q.answer}`;
//                console.log(`    - ${q.question}: ${status}`);
//            });
//            console.log('-----------------------------------------');
//        }
//    });
//    console.log(`Overall Answered: ${completionData.overall.answeredQuestions}/${completionData.overall.totalQuestions} (${completionData.overall.answerRate}%)`);
//    console.log(`Overall Yes Rate: ${completionData.overall.yesAnswers}/${completionData.overall.answeredQuestions} (${completionData.overall.yesRate}%)`);

//    // Submit to server
//    fetch('/ReviewPlane/SaveFswsFormData', {
//        method: 'POST',
//        body: formData
//    })
//        .then(async (response) => {
//            // ... existing response handling ...
//        })
//        .catch((error) => {
//            // ... existing error handling ...
//        });
//}




function sendDataToDatabase() {
    const formData = new FormData();
    const sectionCompletion = {};

    const getRadioValue = (name) => {
        const checked = document.querySelector(`input[name="${name}"]:checked`);
        return checked ? checked.value : null;
    };

    const getFile = (name) => {
        const fileInput = document.getElementById(name);
        return fileInput?.files?.length > 0 ? fileInput.files[0] : null;
    };

    const trackSection = (sectionName, value, questionData) => {
        if (!sectionCompletion[sectionName]) {
            sectionCompletion[sectionName] = {
                yesAnswers: 0,
                totalYesNoQuestions: 0,  // Only count Yes/No questions
                questions: []
            };
        }

        // Only process Yes/No questions for metrics
        if (questionData.type === 'yesno') {
            sectionCompletion[sectionName].totalYesNoQuestions++;

            if (value !== null && value !== undefined) {
                const isYes = String(value).toLowerCase().trim() === 'yes';
                if (isYes) {
                    sectionCompletion[sectionName].yesAnswers++;
                }
            }
        }

        sectionCompletion[sectionName].questions.push(questionData);
    };

    const processField = (section, name, value, question) => {
        const isPhotoUpload = name.toLowerCase().includes('photo');
        const isFileUpload = value instanceof File;
        const isDisplayOption = name.toLowerCase().includes('displayoption');

        // Always add to form data (including all field types)
        formData.append(name, value || '');

        const questionData = {
            name,
            question,
            answer: value,
            type: isFileUpload ? 'file' :
                isPhotoUpload ? 'photo' :
                    isDisplayOption ? 'displayoption' : 'yesno'
        };

        trackSection(section, value, questionData);

        // Enhanced logging
        let status;
        if (questionData.type === 'yesno') {
            if (value === null || value === undefined) {
                status = 'Not answered';
            } else {
                status = String(value).toLowerCase().trim() === 'yes' ? '✔ Yes' : '✖ No';
            }
        } else {
            status = value !== null ? `Value: ${value}` : 'Not uploaded';
        }

        console.log(`[${section}] ${questionData.type.toUpperCase()}: ${name} | ${status}`);
    };

    const processFields = (section, fields) => {
        console.log(`\n=== Processing Section: ${section} ===`);
        fields.forEach(([name, value, question]) => {
            processField(section, name, value, question);
        });
    };

    // Process all sections (your existing code)



    processFields("Model 1", [
                        ["visibleToShopper", getRadioValue("visibleToShopper"), "Is this placed in a clearly visible place to the shopper?"],
                        ["displayOption", getRadioValue("displayOption"), "Display Option"],
                        ["displayPhoto", getFile("displayPhoto"), "Display Photo 1"],
                        ["preVisitPhoto", getFile("preVisitPhoto"), "Display Pre-Visit Photo"],
                        ["unileverSeparate", getRadioValue("unileverSeparate"), " Within the Unilever planogram, are Unilever brands merchandised separately?"],
                        ["brandVariantsSeparate", getRadioValue("brandVariantsSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
                        ["nonUnileverNotBetween", getRadioValue("nonUnileverNotBetween"), "Are non-Unilever brands placed in between Unilever brands?"],
                        ["shelfStripAvailable", getRadioValue("shelfStripAvailable"), "Is ANY Unilever SHELF STRIP available in this Planogram?"]



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
                        ["sachetHangerSavouryPhoto", getFile("sachetHangerSavouryPhoto"), "Display Photo"],
                        ["unileverPlanogramSavourySeparate", getRadioValue("unileverPlanogramSavourySeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
                        ["unileverBrandVariantsSavourySeparate", getRadioValue("unileverBrandVariantsSavourySeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
                        ["nonUnileverPlacementSavoury", getRadioValue("nonUnileverPlacementSavoury"), "Non-Unilever brands are not placed in between Unilever brands?"]
                    ]);



                    processFields("Sachet SHFD", [
                        ["sachetHangerHFDVisible", getRadioValue("sachetHangerHFDVisible"), "Is this placed in a clearly visible place to the shopper?"],
                        ["sachetHangerHFDDisplayOption", getRadioValue("sachetHangerHFDDisplayOption"), "Display Option"],
                        ["sachetHangerHFDPhoto", getFile("sachetHangerHFDPhoto"), " Display Photo 1"],
                        ["unileverPlanogramHFDSeparate", getRadioValue("unileverPlanogramHFDSeparate"), "Within the Unilever planogram, are Unilever brands merchandised separately?"],
                        ["unileverBrandVariantsHFDSeparate", getRadioValue("unileverBrandVariantsHFDSeparate"), "Within the Unilever brands, are relevant brand variants merchandised separately?"],
                        ["nonUnileverPlacementHFD", getRadioValue("nonUnileverPlacementHFD"), "Non-Unilever brands are not placed in between Unilever brands?"]
                    ]);



    // Calculate completion data (only for Yes/No questions)
    const completionData = {};
    let totalYesAnswers = 0;
    let totalYesNoQuestions = 0;

    Object.entries(sectionCompletion).forEach(([section, data]) => {
        const yesRate = data.totalYesNoQuestions > 0
            ? (data.yesAnswers / data.totalYesNoQuestions) * 100
            : 0;

        totalYesAnswers += data.yesAnswers;
        totalYesNoQuestions += data.totalYesNoQuestions;

        completionData[section] = {
            yesAnswers: data.yesAnswers,
            totalYesNoQuestions: data.totalYesNoQuestions,
            percentage: yesRate.toFixed(2),
            questions: data.questions.filter(q => q.type === 'yesno') // Only show Yes/No questions
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

    // Submit to server
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