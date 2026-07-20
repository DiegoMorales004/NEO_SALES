(function () {
    "use strict";

    var AUTH_STORAGE_KEY = "neoSalesAuth";

    function getToken() {
        var raw = window.localStorage.getItem(AUTH_STORAGE_KEY);
        if (!raw) {
            return null;
        }
        try {
            var data = JSON.parse(raw);
            if (!data.token || !data.expiresAtUtc) {
                return null;
            }
            if (new Date(data.expiresAtUtc).getTime() <= Date.now()) {
                return null;
            }
            return data.token;
        } catch (e) {
            return null;
        }
    }

    function setToken(token, expiresAtUtc) {
        window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify({ token: token, expiresAtUtc: expiresAtUtc }));
    }

    document.addEventListener("DOMContentLoaded", function () {
        var form = document.getElementById("neoLoginForm");
        var alertBox = document.getElementById("neoLoginAlert");
        var submitBtn = document.getElementById("neoLoginSubmit");
        var spinner = submitBtn.querySelector(".neo-login-spinner");

        if (getToken()) {
            window.location.href = form.dataset.returnUrl || "/";
            return;
        }

        function showError(message) {
            alertBox.textContent = message;
            alertBox.classList.remove("d-none");
        }

        function setLoading(isLoading) {
            submitBtn.disabled = isLoading;
            spinner.classList.toggle("d-none", !isLoading);
        }

        form.addEventListener("submit", function (evt) {
            evt.preventDefault();
            alertBox.classList.add("d-none");

            var user = document.getElementById("User").value.trim();
            var password = document.getElementById("Password").value;

            if (!user || !password) {
                showError("Usuario y contraseña son obligatorios");
                return;
            }

            setLoading(true);

            fetch("/bff/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json", "Accept": "application/json" },
                body: JSON.stringify({ user: user, password: password })
            })
                .then(function (response) {
                    return response.json().catch(function () { return null; }).then(function (data) {
                        return { ok: response.ok, data: data };
                    });
                })
                .then(function (result) {
                    setLoading(false);

                    if (!result.ok || !result.data || !result.data.token) {
                        showError((result.data && result.data.message) || "Usuario o contraseña incorrectos");
                        return;
                    }

                    setToken(result.data.token, result.data.expiresAtUtc);
                    window.location.href = form.dataset.returnUrl || "/";
                })
                .catch(function () {
                    setLoading(false);
                    showError("No se pudo conectar con el servidor");
                });
        });
    });
})();
