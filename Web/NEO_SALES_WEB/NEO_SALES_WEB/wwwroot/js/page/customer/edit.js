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

    function clearToken() {
        window.localStorage.removeItem(AUTH_STORAGE_KEY);
    }

    function redirectToLogin() {
        clearToken();
        window.location.href = "/Auth/Login?returnUrl=" + encodeURIComponent(window.location.pathname);
    }

    function requireAuth() {
        if (!getToken()) {
            redirectToLogin();
            return false;
        }
        return true;
    }

    function apiFetch(method, url, body) {
        var headers = { "Accept": "application/json" };
        var token = getToken();
        if (token) {
            headers["Authorization"] = "Bearer " + token;
        }

        var options = { method: method, headers: headers };
        if (body !== undefined && body !== null) {
            headers["Content-Type"] = "application/json";
            options.body = JSON.stringify(body);
        }

        return fetch(url, options).then(function (response) {
            if (response.status === 401) {
                redirectToLogin();
                return Promise.reject(new Error("No autenticado"));
            }
            if (response.status === 204) {
                return { ok: response.ok, status: response.status, data: null };
            }
            return response.json().catch(function () { return null; }).then(function (data) {
                return { ok: response.ok, status: response.status, data: data };
            });
        });
    }

    function wireLogout() {
        var btn = document.getElementById("neoLogoutBtn");
        if (btn) {
            btn.addEventListener("click", function (evt) {
                evt.preventDefault();
                clearToken();
                window.location.href = "/Auth/Login";
            });
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        wireLogout();
        if (!requireAuth()) {
            return;
        }

        var form = document.getElementById("neoCustomerForm");
        if (!form) {
            return;
        }

        var id = form.dataset.id;
        var alertBox = document.getElementById("neoCustomerFormAlert");
        var loadingEl = document.getElementById("neoCustomerFormLoading");
        var submitBtn = document.getElementById("neoCustomerFormSubmit");

        var nameInput = document.getElementById("Name");
        var nitInput = document.getElementById("Nit");
        var emailInput = document.getElementById("Email");
        var statusInput = document.getElementById("Status");

        function showError(message) {
            alertBox.textContent = message;
            alertBox.classList.remove("d-none");
        }

        apiFetch("GET", "/bff/customer/" + id).then(function (res) {
            loadingEl.classList.add("d-none");

            if (!res.ok || !res.data) {
                showError("No se pudo cargar el cliente");
                return;
            }

            nameInput.value = res.data.name || "";
            nitInput.value = res.data.nit || "";
            emailInput.value = res.data.email || "";
            statusInput.checked = !!res.data.status;

            form.classList.remove("d-none");
        });

        form.addEventListener("submit", function (evt) {
            evt.preventDefault();
            alertBox.classList.add("d-none");

            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }

            var payload = {
                name: nameInput.value.trim(),
                nit: nitInput.value.trim() || null,
                email: emailInput.value.trim(),
                status: statusInput.checked
            };

            submitBtn.disabled = true;

            apiFetch("PUT", "/bff/customer/" + id, payload).then(function (res) {
                submitBtn.disabled = false;

                if (!res.ok) {
                    showError((res.data && res.data.message) || "No se pudo guardar el cliente");
                    return;
                }

                window.location.href = "/Customer/Index";
            });
        });
    });
})();
