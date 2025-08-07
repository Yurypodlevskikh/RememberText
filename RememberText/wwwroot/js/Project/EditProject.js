import { alertBox } from "./exportscripts.js"
"use strict";

let currentItem;
// Error messages
const memoryLimitOver = document.getElementById("error-memory").innerText
const inptRestrictError = document.getElementById("error-input-restriction").innerText
let errorInptOrMemory = memoryLimitOver

function projectFormSubmit(event) {
    const form = event.target;
    const url = form.action;
    let activeElement = document.activeElement;
    // There is so because pressed the Enter button
    if (activeElement.tagName === "INPUT") {
        // Activate the first submitbutton: edit
        activeElement = form.elements.namedItem("ActionBtn")[0];
    }

    // Activate ajax request only for the edit button, until we do ajax processing of the delete button
    if (activeElement.nodeName === "BUTTON" && activeElement.value === "edit") {
        event.preventDefault();

        let formData = new FormData(form);
        formData.append(activeElement.name, activeElement.value);

        let xhr = new XMLHttpRequest();
        xhr.open("POST", url);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("Accept", "application/json");
        xhr.send(formData);

        xhr.onload = responseToAlertBox;
    }
};

function responseToAlertBox() {
    let timeout;
    if (this.readyState === 4 && this.status === 200) {
        
        // Jump to next input item
        if (currentItem !== undefined) {
            let currentItemIdArray = currentItem.split('_');
            if (currentItemIdArray[1] < inputsCount) {
                currentItemIdArray[1] = parseInt(currentItemIdArray[1]) + 1;
                const newItemId = currentItemIdArray.join('_');
                const newInput = document.getElementById(newItemId);
                newInput.focus();
                newInput.select();
            }
        }

        alertBox(response.responseText, response.alertClasses);
    }
};

function centenceEditing(e) {
    if (e.target.nodeName === "INPUT") {
        currentItem = e.target.id
        let spaceAmountBadge = document.getElementById("spaceAmount")
        let spaceAmont = parseInt(spaceAmountBadge.innerText)
        let spaceLimitElement = document.getElementById("spaceLimitation")
        let spaceLimitValue = parseInt(spaceLimitElement.innerText)
        let spaceLeftBadge = document.getElementById("spaceLeft")
        let spaceLeft = parseInt(spaceLeftBadge.innerText)
        let editableValue = e.target.value
        let editableValueLength = editableValue.length
        let commonProgress = document.getElementById("commonProgress")
        let commonPercentElement = document.getElementById("commonPercent")
        let commonPercentage = parseInt(commonPercentElement.innerText)
        let successProgress = document.getElementById("percentSuccess")
        let successMax = successProgress.ariaValueMax
        let warningProgress = document.getElementById("percentWarning")
        let warningMax = warningProgress.ariaValueMax
        let dangerProgress = document.getElementById("percentDanger")
        let dangerMax = dangerProgress.ariaValueMax
        let percentage
        
        // Input restriction
        let extractRestriction = spaceLeft > 0 ? spaceLeft + (editableValueLength - 1) : spaceLeft + editableValueLength
        let limitation = extractRestriction < 250 ? extractRestriction : 250
        errorInptOrMemory = extractRestriction < 250 ? memoryLimitOver : inptRestrictError
        
        e.target.oninput = function () {
            let newValueLength = this.value.length

            // Input restriction
            this.value = this.value.substring(0, limitation)
            
            // Badge editing
            if (newValueLength > editableValueLength) {

                let valIfMore = newValueLength - editableValueLength;
                let increacedValue = spaceAmont + valIfMore
                
                if (increacedValue > spaceLimitValue) {
                    
                    alertBox(errorInptOrMemory, "alert-danger");

                } else {

                    if (increacedValue === spaceLimitValue) {
                        alertBox(errorInptOrMemory, "alert-danger");
                    }

                    spaceAmountBadge.innerText = increacedValue
                    spaceLeftBadge.innerText = spaceLeft - valIfMore
                    percentage = Math.round((increacedValue / spaceLimitValue) * 100)
                }
            } else if (newValueLength < editableValueLength) {

                let valIfLess = editableValueLength - newValueLength
                let decreacedValue = spaceAmont - valIfLess
                spaceAmountBadge.innerText = decreacedValue
                spaceLeftBadge.innerText = spaceLeft + valIfLess
                percentage = Math.round((decreacedValue / spaceLimitValue) * 100)

            } else if (newValueLength === editableValueLength) {

                spaceAmountBadge.innerText = spaceAmont
                spaceLeftBadge.innerText = spaceLeft
            }
            
            // Progressbars editing
            if (percentage !== undefined && commonPercentage !== percentage && percentage <= 100) {
                commonPercentage = percentage
                commonPercent.innerText = percentage
                commonProgress.style.width = percentage + "%"
                console.dir(commonProgress)

                if (percentage <= successMax) {

                    successProgress.ariaValueNow = percentage
                    successProgress.style.width = percentage + "%"

                } else if (percentage > successMax) {

                    let warningPercent = percentage - successMax
                    if (warningPercent <= warningMax) {

                        warningProgress.ariaValueNow = warningPercent
                        warningProgress.style.width = warningPercent + "%"

                    } else if (warningPercent > warningMax) {

                        let dangerPercent = warningPercent - warningMax

                        if (dangerPercent <= dangerMax) {

                            dangerProgress.ariaValueNow = dangerPercent
                            dangerProgress.style.width = dangerPercent

                        }
                    }
                }
            }
        }
    }
}

const sentenceForms = document.getElementsByClassName("edit-project-form");
const inputsCount = sentenceForms.length;

Array.from(sentenceForms).forEach((item, index) => {
    item.onfocusin = centenceEditing;
    item.onsubmit = projectFormSubmit;
})