import { alertBox } from "./exportscripts.js";

const createSearchCopyrightLink = link => {
    let urlArr = link.split('/');
    let copyrightUrl = ""

    if (urlArr !== null && urlArr.length > 0) {
        for (let i = 0; i < urlArr.length; i++) {
            let part = urlArr[i]
            copyrightUrl = copyrightUrl + part + "/"

            if (part === "Projects") {
                copyrightUrl = copyrightUrl + "SearchCopyright"
                break
            }
        }
    }

    return copyrightUrl
}

function submitUpdates(e) {
    e.preventDefault();
    let formData = new FormData(e.currentTarget);
    const fetchOptions = {
        method: "POST",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        body: formData
    };
    fetch(e.target.action, fetchOptions)
        .then(res => res.json())
        .then((res) => {
            if (res.success && res.respobj !== undefined) {
                // Response from the Copyright form
                if (copyrightId !== undefined) {
                    copyrightId.value = res.respobj.copyrightId;
                    alertBox(res.respobj.message, "alert-success");
                }
            } else {
                // Response from the Title form
                if (res.message !== undefined) {
                    if (res.success) {
                        alertBox(res.message, "alert-success");
                    } else {
                        alertBox(res.message, "alert-danger");
                    }
                }
            }
        })
        .catch((error) => {
            alertBox("Error!", "alert-danger");
            //console.error("Error: ", error);
        });
}

// Variables to use in the oninput method and submitUpdates function
let copyrightForm, copyrightId;

const fTitleCopyright = document.getElementsByClassName("title-copyright");
Array.from(fTitleCopyright).forEach((form) => {
    if (form.id === "copyright-form") {
        copyrightForm = form;
        // Get copyright input in the form
        copyrightId = copyrightForm.querySelector("#CopyrightId");
    }

    form.onsubmit = submitUpdates;
});

if (copyrightForm !== undefined) {
    const copyrightUrl = copyrightForm.action;
    const copyrightSearchUrl = createSearchCopyrightLink(copyrightUrl);

    // Get elements in the form
    let copyrightName = copyrightForm.querySelector("#CopyrightName");
    const copyrightSuggestBox = copyrightForm.querySelector(".search-suggestions-box");
    let timeoutCopyright;

    copyrightName.oninput = function (e) {
        clearTimeout(timeoutCopyright);
        if (e.target.value.length > 0) {
            timeoutCopyright = setTimeout(function () {
                if (copyrightId.value !== "")
                    copyrightId.value = "";
                let copyright = e.target.value;
                let formData = new FormData();
                formData.append("copyright", copyright);
                fetch(copyrightSearchUrl, {
                    method: "POST",
                    body: formData
                })
                    .then(res => res.text())
                    .then((data) => {
                        if (data !== "No matches") {
                            copyrightSuggestBox.innerHTML = data;
                            copyrightSuggestBox.style.display = "block";
                        } else {
                            if (copyrightSuggestBox.style.display === "block") {
                                copyrightSuggestBox.style.display = "none";
                            }
                        }
                    })
            }, 700);
        }
    }

    copyrightName.onblur = function (e) {
        document.onclick = function (e) {
            if (e.target.closest("li")) {
                if (e.target.classList.contains("copyright-suggests")) {
                    copyrightId.value = e.target.dataset.copyrightid;
                    copyrightName.value = e.target.textContent;
                    copyrightSuggestBox.style.display = "none";
                }
            } else {
                copyrightSuggestBox.style.display = "none";
            }
        }
    }
}
