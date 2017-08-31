document.addEventListener("DOMContentLoaded", function () {
    var logoutForm = (function () {
        return document.getElementById("logout-form");
    }());
    var logOff = (function () {
        return document.getElementById("log-off");
    }());

    if (logOff !== undefined && logOff !== null) {
        logOff.addEventListener("click", function () { logoutForm.submit(); });
    }
});