(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        var toggler = document.getElementById("neoNavbarToggler");
        var target = document.querySelector(".navbar-collapse");

        if (!toggler || !target) {
            return;
        }

        toggler.addEventListener("click", function () {
            var isShown = target.classList.toggle("show");
            toggler.setAttribute("aria-expanded", isShown ? "true" : "false");
        });
    });
})();
