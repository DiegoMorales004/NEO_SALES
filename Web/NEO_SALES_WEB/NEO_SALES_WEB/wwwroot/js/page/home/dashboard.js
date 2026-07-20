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

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) {
            el.textContent = value;
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        wireLogout();
        if (!requireAuth()) {
            return;
        }

        apiFetch("GET", "/bff/customer").then(function (res) {
            if (res.ok && Array.isArray(res.data)) {
                var active = res.data.filter(function (c) { return c.status; }).length;
                setText("neoCardCustomers", active);
            } else {
                setText("neoCardCustomers", "-");
            }
        });

        apiFetch("GET", "/bff/product").then(function (res) {
            if (res.ok && Array.isArray(res.data)) {
                var active = res.data.filter(function (p) { return p.status; }).length;
                setText("neoCardProducts", active);
            } else {
                setText("neoCardProducts", "-");
            }
        });

        apiFetch("GET", "/bff/sale").then(function (res) {
            if (res.ok && Array.isArray(res.data)) {
                var pending = res.data.filter(function (s) { return s.statusId === 1; }).length;
                setText("neoCardPendingSales", pending);
                setText("neoCardTotalSales", res.data.length);
            } else {
                setText("neoCardPendingSales", "-");
                setText("neoCardTotalSales", "-");
            }
        });
    });
})();
