var html = document.getElementById("html");

//#region topbar

document.body.onscroll = (e) => {

    //console.log("test");

    // desktop/tablet - if it scrolls past the nav bar
    var scroll = window.scrollY;

    var navScrollPos = document.getElementById("navigation").offsetTop;

    if (scroll > navScrollPos) {
        var navTop = document.getElementById("topbar");
        navTop.classList.add("top-desktop-visible");
        navTop.classList.remove("top-desktop-hidden");
    }
    else {
        var navTop = document.getElementById("topbar");
        navTop.classList.add("top-desktop-hidden");
        navTop.classList.remove("top-desktop-visible");
    }
}

var sideBarToggled = false;

document.getElementById("hamburger-container").onclick = (e) => {

    sideBarToggled = !sideBarToggled;

    var sidebar = document.getElementById("sidebar");
    var bodyArea = document.getElementById("bodyarea");

    if (sideBarToggled) {
        sidebar.classList.remove("pushy-closed");
        sidebar.classList.add("pushy-open");

        bodyArea.classList.add("bodyshifted");
        html.style.overflow = "hidden";
    }
    else {
        sidebar.classList.add("pushy-closed");
        sidebar.classList.remove("pushy-open");

        bodyArea.classList.remove("bodyshifted");

        setTimeout(() => {
            html.style.overflow = "visible";
        }, 200);

    }
}
//#endregion

var searchButtons = document.getElementsByClassName("navigation-search-button");

Array.from(searchButtons).forEach((button) => {
    button.addEventListener("click", () => showSearch(button.getAttribute("data-target")));
});

function showSearch(target) {
    document.getElementById(target).focus();
}
