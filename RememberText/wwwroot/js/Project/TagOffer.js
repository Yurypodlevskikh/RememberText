import { alertBox } from "./exportscripts.js";

function tagOffer(x) {
    let timeoutFindTag;
    const form = x.target.closest("form");
    let saveTagUrl = form.action;

    // Create Url to find offer tags
    let urlArr = saveTagUrl.split('/');
    urlArr[urlArr.length - 1] = "FindTag";
    const findTagUrl = urlArr.join('/');
    
    let tagIdInpt = form.elements.TagId;
    const offerTagsBox = x.target.nextElementSibling;
    
    x.target.oninput = function () {
        clearTimeout(timeoutFindTag);
        if (x.target.value.length > 0) {
            timeoutFindTag = setTimeout(function () {
                let formData = new FormData(form);

                fetch(findTagUrl, {
                    method: 'POST',
                    body: formData
                })
                    .then(res => res.text())
                    .then((data) => {
                        if (data !== "No tags") {
                            offerTagsBox.innerHTML = data;
                            offerTagsBox.style.display = "block";
                        } else {
                            if (offerTagsBox.style.display === "block") {
                                offerTagsBox.style.display = "none";
                            }
                        }
                    })
                    .catch(error => console.error("Error: ", error));
            }, 700);
        } else {
            offerTagsBox.innerHTML = "";
            offerTagsBox.style.display = "none";
        };
        
    }

    x.target.onblur = function () {
        // Capture a mouse click
        document.onclick = function (e) {
            if (e.target.closest("li")) {
                if (e.target.classList.contains("tag-offer")) {
                    tagIdInpt.value = e.target.dataset.tagid;
                    x.target.value = e.target.textContent;
                    offerTagsBox.innerHTML = "";
                    offerTagsBox.style.display = "none";
                }
            } else {
                offerTagsBox.innerHTML = "";
                offerTagsBox.style.display = "none";
            }
        }
    }
}

function addTagFormSubmit (event) {
    event.preventDefault();
    const tagForm = event.currentTarget;
    const url = tagForm.action;
    
    let formData = new FormData(tagForm);
    const fetchOptions = {
        method: "POST",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        body: formData
    };
    fetch(url, fetchOptions)
        .then(res => res.json())
        .then((res) => {
            if (res.success && res.message !== undefined) {
                tagForm.previousElementSibling.innerHTML = res.message;
                tagForm.elements.TagName.value = "";
                tagForm.elements.TagId.value = "";
                tagForm.elements.TagName.focus();
                loadTagDeleteForms();
            } else {
                if (res.message !== undefined) {
                    alertBox(res.message, "alert-danger");
                }
            }
            
        })
        .catch(error => alertBox("Error: "/* + error*/, "alert-danger"));
};

function tagDelFormSubmit (event) {
    event.preventDefault();
    const tagDelForm = event.currentTarget;
    const url = tagDelForm.action;
    let formData = new FormData(tagDelForm);
    const fetchOptions = {
        method: "POST",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        body: formData
    };
    fetch(url, fetchOptions)
        .then(res => res.json())
        .then((res) => {
            if (res.success && res.message !== undefined) {
                tagDelForm.closest("ul").innerHTML = res.message;
                loadTagDeleteForms();
            } else {
                if (res.message !== undefined) {
                    alertBox(res.message, "alert-danger");
                }
            }
        })
        .catch(error => alertBox("Error: "/* + error*/, "alert-danger"));
};

function loadTagDeleteForms() {
    let tagDelForms = document.getElementsByClassName("tag-delete-form");
    Array.from(tagDelForms).forEach((item) => {
        item.onsubmit = tagDelFormSubmit;
    });
}

window.onload = () => {
    loadTagDeleteForms();
};

const tagNameInpts = document.getElementsByName("TagName");
Array.from(tagNameInpts).forEach((tagInput) => {
    tagInput.onfocus = tagOffer;
});

const tagForms = document.getElementsByClassName("tag-form");
Array.from(tagForms).forEach((tagForm) => {
    tagForm.onsubmit = addTagFormSubmit;
});

