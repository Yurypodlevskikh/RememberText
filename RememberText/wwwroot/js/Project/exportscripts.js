function alertBox(alerttext, alertclass) {
    let timeout
    clearTimeout(timeout)

    let alertBox = document.getElementById("alert_box")
    alertBox.innerText = alerttext
    let classToAlert = alertclass
    alertBox.classList.add(classToAlert)
    alertBox.parentElement.classList.add("rt-show-hide-alert")

    timeout = setTimeout(function () {
        alertBox.parentElement.classList.remove("rt-show-hide-alert")
        alertBox.innerText = ""
        alertBox.classList.remove(classToAlert)
    }, 4000)
}

export { alertBox };