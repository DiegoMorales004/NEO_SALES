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

    var loadingEl, emptyEl, wrapperEl, bodyEl, searchInput;
    var searchTimer = null;

    function escapeHtml(value) {
        var div = document.createElement("div");
        div.textContent = value == null ? "" : String(value);
        return div.innerHTML;
    }

    function renderRows(customers) {
        if (!customers || customers.length === 0) {
            wrapperEl.classList.add("d-none");
            emptyEl.classList.remove("d-none");
            return;
        }

        emptyEl.classList.add("d-none");
        wrapperEl.classList.remove("d-none");

        bodyEl.innerHTML = customers.map(function (c) {
            var badgeClass = c.status ? "text-bg-success" : "text-bg-secondary";
            var badgeText = c.status ? "Activo" : "Inactivo";
            var toggleLabel = c.status ? "Desactivar" : "Activar";
            var toggleClass = c.status ? "btn-danger" : "btn-success";

            return "" +
                "<tr>" +
                "<td data-label=\"Nombre\">" + escapeHtml(c.name) + "</td>" +
                "<td data-label=\"NIT\">" + escapeHtml(c.nit || "-") + "</td>" +
                "<td data-label=\"Correo\">" + escapeHtml(c.email) + "</td>" +
                "<td data-label=\"Estado\"><span class=\"badge " + badgeClass + "\">" + badgeText + "</span></td>" +
                "<td data-label=\"Acciones\">" +
                "<div class=\"neo-actions\">" +
                "<a class=\"btn btn-sm btn-neo-accent neo-action-btn\" href=\"/Customer/Edit/" + c.id + "\">Editar</a>" +
                "<button type=\"button\" class=\"btn btn-sm " + toggleClass + " neo-action-btn neo-toggle-status\" data-id=\"" + c.id + "\">" + toggleLabel + "</button>" +
                "</div>" +
                "</td>" +
                "</tr>";
        }).join("");

        Array.prototype.forEach.call(bodyEl.querySelectorAll(".neo-toggle-status"), function (btn) {
            btn.addEventListener("click", function () {
                toggleStatus(btn.getAttribute("data-id"), customers);
            });
        });
    }

    function toggleStatus(id, customers) {
        var customer = customers.filter(function (c) { return c.id === id; })[0];
        if (!customer) {
            return;
        }

        apiFetch("PUT", "/bff/customer/" + id, {
            name: customer.name,
            nit: customer.nit,
            email: customer.email,
            status: !customer.status
        }).then(function (res) {
            if (res.ok) {
                loadCustomers();
            }
        });
    }

    function loadCustomers(term) {
        loadingEl.classList.remove("d-none");
        wrapperEl.classList.add("d-none");
        emptyEl.classList.add("d-none");

        var url = term ? "/bff/customer/search/" + encodeURIComponent(term) : "/bff/customer";

        apiFetch("GET", url).then(function (res) {
            loadingEl.classList.add("d-none");
            renderRows(res.ok && Array.isArray(res.data) ? res.data : []);
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        wireLogout();
        if (!requireAuth()) {
            return;
        }

        loadingEl = document.getElementById("neoCustomerLoading");
        emptyEl = document.getElementById("neoCustomerEmpty");
        wrapperEl = document.getElementById("neoCustomerTableWrapper");
        bodyEl = document.getElementById("neoCustomerTableBody");
        searchInput = document.getElementById("neoCustomerSearch");

        if (!bodyEl) {
            return;
        }

        loadCustomers();

        searchInput.addEventListener("input", function () {
            var term = searchInput.value.trim();
            window.clearTimeout(searchTimer);
            searchTimer = window.setTimeout(function () {
                loadCustomers(term || undefined);
            }, 350);
        });
    });
})();
