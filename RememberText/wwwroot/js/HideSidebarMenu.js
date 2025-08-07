const sidebarChckbx = document.getElementById("rt-nav-toggle")
const lblChckbx = document.querySelector("label.rt-nav-toggle")

window.onclick = function (event) {
    if (event.target !== sidebarChckbx && event.target !== lblChckbx && event.target.closest("#rt-sidebar-menu") === null) {
        if (sidebarChckbx.checked) {
            sidebarChckbx.checked = false;
        }
    }
}