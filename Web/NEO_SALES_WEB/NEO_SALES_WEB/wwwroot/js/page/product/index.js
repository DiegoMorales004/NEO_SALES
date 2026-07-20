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

    function formatCurrency(value) {
        return "Q" + Number(value).toFixed(2);
    }

    function renderRows(products) {
        if (!products || products.length === 0) {
            wrapperEl.classList.add("d-none");
            emptyEl.classList.remove("d-none");
            return;
        }

        emptyEl.classList.add("d-none");
        wrapperEl.classList.remove("d-none");

        bodyEl.innerHTML = products.map(function (p) {
            var badgeClass = p.status ? "text-bg-success" : "text-bg-secondary";
            var badgeText = p.status ? "Activo" : "Inactivo";
            var toggleLabel = p.status ? "Desactivar" : "Activar";
            var toggleClass = p.status ? "btn-danger" : "btn-success";
            var stockBadge = p.stock <= 5 ? "text-bg-warning" : "text-bg-light";

            return "" +
                "<tr>" +
                "<td data-label=\"Nombre\">" + escapeHtml(p.name) + "</td>" +
                "<td data-label=\"Precio\">" + formatCurrency(p.price) + "</td>" +
                "<td data-label=\"Stock\"><span class=\"badge " + stockBadge + "\">" + p.stock + "</span></td>" +
                "<td data-label=\"Estado\"><span class=\"badge " + badgeClass + "\">" + badgeText + "</span></td>" +
                "<td data-label=\"Acciones\">" +
                "<div class=\"neo-actions\">" +
                "<a class=\"btn btn-sm btn-neo-accent neo-action-btn\" href=\"/Product/Edit/" + p.id + "\">Editar</a>" +
                "<button type=\"button\" class=\"btn btn-sm " + toggleClass + " neo-action-btn neo-toggle-status\" data-id=\"" + p.id + "\">" + toggleLabel + "</button>" +
                "</div>" +
                "</td>" +
                "</tr>";
        }).join("");

        Array.prototype.forEach.call(bodyEl.querySelectorAll(".neo-toggle-status"), function (btn) {
            btn.addEventListener("click", function () {
                toggleStatus(btn.getAttribute("data-id"), products);
            });
        });
    }

    function toggleStatus(id, products) {
        var product = products.filter(function (p) { return p.id === id; })[0];
        if (!product) {
            return;
        }

        apiFetch("PUT", "/bff/product/" + id, {
            name: product.name,
            price: product.price,
            stock: product.stock,
            status: !product.status
        }).then(function (res) {
            if (res.ok) {
                loadProducts();
            }
        });
    }

    function loadProducts(term) {
        loadingEl.classList.remove("d-none");
        wrapperEl.classList.add("d-none");
        emptyEl.classList.add("d-none");

        var url = term ? "/bff/product/search/" + encodeURIComponent(term) : "/bff/product";

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

        loadingEl = document.getElementById("neoProductLoading");
        emptyEl = document.getElementById("neoProductEmpty");
        wrapperEl = document.getElementById("neoProductTableWrapper");
        bodyEl = document.getElementById("neoProductTableBody");
        searchInput = document.getElementById("neoProductSearch");

        if (!bodyEl) {
            return;
        }

        loadProducts();

        searchInput.addEventListener("input", function () {
            var term = searchInput.value.trim();
            window.clearTimeout(searchTimer);
            searchTimer = window.setTimeout(function () {
                loadProducts(term || undefined);
            }, 350);
        });
    });
})();
