"use strict";

var practiceLinks = document.getElementsByClassName("practice-link");

for (let link of practiceLinks) {
    let linkArr = link.getAttribute("href").split("/");
    linkArr.splice(2, 1, "Practice");
    let newUrl = linkArr.join("/");
    link.setAttribute("href", newUrl);
}



