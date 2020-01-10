//#region date

// get the time for a GMT offset
function getTime(offset) {

    d = new Date();
    // multiply by 60000 to get milliseconds
    utc = d.getTime() + (d.getTimezoneOffset() * 60000);

    // multiply by 360000 to convert hour to milliseconds
    return new Date(utc + (3600000 * offset));

}

function generateSuffix(dayOfMonth) {

    // special case for 11, 12, 13
    while (dayOfMonth >= 14) {
        dayOfMonth %= 10;
    }

    if (dayOfMonth <= 10) {
        if (dayOfMonth == 1) return "st";
        if (dayOfMonth == 2) return "nd";
        if (dayOfMonth == 3) return "rd";
        else return "th";
    }

    return "th";
    
}

// China Standard Time UTC+8
var date = getTime(8);

var days = [ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

var months = ["January", "Feburary", "March", "April", "May", "June", "July","August", "September", "October", "November", "December"];

var dayOfWeek = days[date.getDay()];
var dayOfMonth = date.getDate() + generateSuffix(date.getDate());
var month = months[date.getMonth()];

document.getElementById("date-container").innerHTML = dayOfWeek + " " + dayOfMonth + " " + month + ", " + date.getFullYear();

//#endregion

//#region topbar

document.body.onscroll = (e) => {

    //console.log("test");

    var scroll = window.scrollY;

    var navScrollPos = document.getElementById("navigation-header").offsetTop;

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


    if (sideBarToggled) {
        document.body.classList.add("bodyshifted");
    }
    else {
        document.body.classList.remove("bodyshifted");
    }
}

//#endregion
