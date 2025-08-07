import { alertBox } from "./exportscripts.js";

const topicsTable = document.getElementById("project_topics");

if (topicsTable) {
    topicsTable.onclick = function (event) {
        const topicLink = event.target.closest("a");

        if (!topicLink) return;
        const clList = topicLink.classList;
        if (clList.contains("show-project-text") || clList.contains("publish-project")) {
            event.preventDefault();
            const url = topicLink.href;
            
            if (clList.contains("show-project-text")) {
                const dialogContent = document.getElementById("dialog_content");
                
                $("#modal_for_text").modal("show");
                fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } })
                    .then(response => response.text())
                    .then(function (html) {
                        dialogContent.innerHTML = html;
                        //spinner.style.display = "none";

                        const dialogHeader = document.getElementById("dialog_header");

                        if (dialogHeader) {
                            dialogHeader.onclick = function (event) {

                                let displayLink = event.target.closest("a");

                                if (!displayLink) return;
                                if (!displayLink.classList.contains("how-to-display")) return;

                                event.preventDefault();
                                const howToDisplayUrl = displayLink.href;
                                const dialogBody = document.getElementById("dialog_body");

                                fetch(howToDisplayUrl, { headers: { "X-Requested-With": "XMLHttpRequest" } })
                                    .then(response => response.text())
                                    .then(function (selectedHtml) {
                                        dialogBody.innerHTML = selectedHtml;
                                    });
                            }
                        }
                    })
                    .catch(error => console.error("Create Project: Wrong request!"));
            }

            if (clList.contains("publish-project")) {
                fetch(url, { method: "GET", headers: { "X-Requested-With": "XMLHttpRequest" } })
                    .then(response => response.json())
                    .then(data => {
                        clList.contains("text-success") ? clList.remove("text-success") : clList.add("text-success")
                        alertBox(data.responseText, data.alertClasses)
                    })
                    .catch(err => console.error(err))
            }
        }
    }
}