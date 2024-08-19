"use strict";
const createProjectArea = document.getElementById("create_project_area");

createProjectArea.onclick = function (event) {
    if (event.target.closest("a")) {

        const url = event.target.href;

        // Execution only for links such as a.create-project
        if (event.target.closest("a").classList.contains("create-project")) {
            event.preventDefault();

            const spinner = document.getElementById("project_spinner");
            spinner.style.display = "block";

            // Remove all childs
            while (createProjectArea.firstChild && createProjectArea.removeChild(createProjectArea.firstChild));

            fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } })
                .then(response => response.text())
                .then(function (html) {
                    createProjectArea.innerHTML = html;
                    spinner.style.display = "none";
                })
                .catch(error => console.log("Create Project: Wrong request!"));
                //.catch(error => console.log("Error: " + error));
        } else {
            window.location = url;
        }
    }
}



