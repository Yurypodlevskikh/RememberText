"use strict";

const sentenceBox = document.getElementById("sentence_box");
const logoBox = document.getElementById("logo_box");
const logoCharacter = document.getElementById("logo_character");
const logoBaseCharacter = "R";
const logoLine = document.getElementById("logo_line");
let completeAnswer = false;

sentenceBox.onfocusin = inputFocus;

let sentenceInputs = document.getElementsByClassName("sentence-input");
if (sentenceInputs) {
    sentenceInputs[0].focus();
}

function inputFocus(event) {
    if (event.target.nodeName === "INPUT") {
        const currentSentenceInput = event.target;
        const inputIndex = parseInt(event.target.id.split('_')[1]);
        completeAnswer = false;

        // Get html object button
        let answerBox = currentSentenceInput.closest("div").previousElementSibling;
        // How many child nodes have this from begining
        const basicChildNodesLength = answerBox.childNodes.length;
        const correctSentence = answerBox.dataset.content;

        currentSentenceInput.oninput = function () {
            let inputValue = currentSentenceInput.value;
            let inputValueLength = inputValue.length;

            if (inputValueLength > 0) {
                clearingAnswerBox(answerBox, basicChildNodesLength);

                let answerNodes = document.createRange()
                    .createContextualFragment(compileInputValueForAnswerBox(correctSentence, inputValue));
                answerBox.appendChild(answerNodes);
                
                if (completeAnswer) {
                    if (inputIndex < sentenceInputs.length - 1) {
                        sentenceInputs[inputIndex + 1].focus();
                    } else if (inputIndex === sentenceInputs.length - 1) {
                        sentenceInputs[0].focus();
                    }
                    currentSentenceInput.value = "";
                }
            }
            else {
                // Logotype box
                updateLogoBoxClassToRight();
                updateLogoBoxToBaseCharacter();

                clearingAnswerBox(answerBox, basicChildNodesLength);
            }
        };
    }
}

function clearingAnswerBox(answerBox, basicChildNodesLength) {
    // How many child nodes contains here now
    let currentChildNodesLength = answerBox.childNodes.length;
    // Clear answer string
    while (currentChildNodesLength > basicChildNodesLength) {
        answerBox.removeChild(answerBox.childNodes[currentChildNodesLength - 1]);
        currentChildNodesLength--;
    }
}

function compileInputValueForAnswerBox(origVal, inptVal) {
    const origValLength = origVal.length;
    const inptValLength = inptVal.length;
    const spanSuccessStatrt = "<span class='text-success'>";
    const spanDangerStatrt = "<span class='text-danger'>";
    const spanEnd = "</span>";

    let lastCharIsFalse = false;
    let answer = "";

    for (let i = 0; i < inptValLength; i++) {
        if (origVal[i] === inptVal[i]) {
            if (i === 0) {
                answer += spanSuccessStatrt + inptVal[i];
            } else {
                if (lastCharIsFalse) {
                    answer += spanEnd + spanSuccessStatrt + inptVal[i];
                } else {
                    answer += inptVal[i];
                }
            }

            if (logoBox.classList.contains("custom-warning")) {
                updateLogoBoxClassToRight();
                updateLogoBoxToBaseCharacter();
            }
            
            if (i === origValLength - 1) {
                completeAnswer = true;
            }
        } else {
            if (i === 0) {
                answer += spanDangerStatrt + inptVal[i];
                lastCharIsFalse = true;
            } else {
                if (lastCharIsFalse) {
                    answer += inptVal[i];
                } else {
                    answer += spanEnd + spanDangerStatrt + inptVal[i];
                }
            }
            
            // If the characters are not identical
            updateLogoBoxClassToWarning();
            logoCharacter.innerText = origVal[i] === undefined ? "!?" :
                origVal[i] === " " ? '" "' : origVal[i];
        }

        if (i === inptValLength - 1) {
            answer += spanEnd;
        }
    }
    return answer;
}

function updateLogoBoxToBaseCharacter() {
    if (logoCharacter.innerText !== logoBaseCharacter)
        logoCharacter.innerText = logoBaseCharacter;
}

function updateLogoBoxClassToRight() {
    if (logoBox.classList.contains("custom-warning"))
        logoBox.classList.remove("custom-warning");

    if (!logoBox.classList.contains("custom-success"))
        logoBox.classList.add("custom-success");
}

function updateLogoBoxClassToWarning() {
    if (logoBox.classList.contains("custom-success"))
        logoBox.classList.remove("custom-success");

    if (!logoBox.classList.contains("custom-warning"))
        logoBox.classList.add("custom-warning");
}
