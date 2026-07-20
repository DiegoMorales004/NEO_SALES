(function () {
    "use strict";

    var AUTH_STORAGE_KEY = "neoSalesAuth";
    var PENDING_STATUS_ID = 1;

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

    function escapeHtml(value) {
        var div = document.createElement("div");
        div.textContent = value == null ? "" : String(value);
        return div.innerHTML;
    }

    function statusBadgeClass(statusId) {
        if (statusId === 1) return "neo-badge-pending";
        if (statusId === 2) return "neo-badge-confirmed";
        return "neo-badge-cancelled";
    }

    function detailButtonClass(statusId) {
        return statusId === 1 || statusId === 2 ? "btn-outline-success" : "btn-outline-danger";
    }

    function formatDate(value) {
        var date = new Date(value);
        if (isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    }

    function titleCase(value) {
        var text = String(value || "").toLowerCase();
        return text.charAt(0).toUpperCase() + text.slice(1);
    }

    // Minimal modal controller: avoids Bootstrap's Modal JS, which toggles
    // inline styles (display/overflow/padding) that a strict CSP (no
    // style-src 'unsafe-inline') blocks. Reuses Bootstrap's CSS classes only.
    function openModal(modalEl) {
        var backdrop = document.createElement("div");
        backdrop.className = "modal-backdrop fade";
        document.body.appendChild(backdrop);
        document.body.classList.add("neo-modal-open");

        modalEl.classList.add("d-block");
        modalEl.removeAttribute("aria-hidden");
        modalEl.setAttribute("aria-modal", "true");
        modalEl.setAttribute("role", "dialog");
        modalEl._neoBackdrop = backdrop;

        // Force layout so the classes below trigger a transition instead of jumping.
        void modalEl.offsetHeight;
        modalEl.classList.add("show");
        backdrop.classList.add("show");

        modalEl._neoKeydownHandler = function (evt) {
            if (evt.key === "Escape") {
                closeModal(modalEl);
            }
        };
        document.addEventListener("keydown", modalEl._neoKeydownHandler);
    }

    function closeModal(modalEl) {
        modalEl.classList.remove("show");
        var backdrop = modalEl._neoBackdrop;
        if (backdrop) {
            backdrop.classList.remove("show");
        }

        if (modalEl._neoKeydownHandler) {
            document.removeEventListener("keydown", modalEl._neoKeydownHandler);
            modalEl._neoKeydownHandler = null;
        }

        window.setTimeout(function () {
            modalEl.classList.remove("d-block");
            modalEl.setAttribute("aria-hidden", "true");
            modalEl.removeAttribute("aria-modal");
            document.body.classList.remove("neo-modal-open");
            if (backdrop && backdrop.parentNode) {
                backdrop.parentNode.removeChild(backdrop);
            }
            modalEl._neoBackdrop = null;
            modalEl.dispatchEvent(new Event("neo:hidden"));
        }, 300);
    }

    function initSaleList() {
        var loadingEl = document.getElementById("neoSaleLoading");
        var emptyEl = document.getElementById("neoSaleEmpty");
        var wrapperEl = document.getElementById("neoSaleTableWrapper");
        var bodyEl = document.getElementById("neoSaleTableBody");
        var filtersEl = document.getElementById("neoSaleStatusFilters");

        if (!bodyEl) {
            return;
        }

        var allSales = [];
        var activeStatusId = PENDING_STATUS_ID;

        function renderList() {
            loadingEl.classList.add("d-none");

            var sales = activeStatusId === null
                ? allSales
                : allSales.filter(function (s) { return s.statusId === activeStatusId; });

            if (sales.length === 0) {
                wrapperEl.classList.add("d-none");
                emptyEl.classList.remove("d-none");
                return;
            }

            emptyEl.classList.add("d-none");
            wrapperEl.classList.remove("d-none");
            bodyEl.innerHTML = sales.map(function (s) {
                return "" +
                    "<tr>" +
                    "<td data-label=\"Cliente\">" + escapeHtml(s.customerName) + "</td>" +
                    "<td data-label=\"Fecha\">" + formatDate(s.date) + "</td>" +
                    "<td data-label=\"Estado\"><span class=\"badge " + statusBadgeClass(s.statusId) + "\">" + escapeHtml(s.statusName) + "</span></td>" +
                    "<td data-label=\"Acciones\" class=\"text-md-end\">" +
                    "<a class=\"btn btn-sm " + detailButtonClass(s.statusId) + " neo-action-btn\" href=\"/Sale/Details/" + s.id + "\">Ver detalle</a>" +
                    "</td>" +
                    "</tr>";
            }).join("");
        }

        function setActiveFilter(statusId) {
            activeStatusId = statusId;
            Array.prototype.forEach.call(filtersEl.querySelectorAll(".neo-status-filter"), function (btn) {
                var btnStatusId = btn.getAttribute("data-status-id");
                var isActive = (statusId === null && btnStatusId === "") || (statusId !== null && btnStatusId === String(statusId));
                btn.classList.toggle("active", isActive);
            });
            renderList();
        }

        function renderFilters(statuses) {
            var buttons = statuses.map(function (s) {
                return "<button type=\"button\" class=\"btn btn-sm neo-status-filter\" data-status-id=\"" + s.id + "\">" + escapeHtml(titleCase(s.name)) + "</button>";
            });
            buttons.push("<button type=\"button\" class=\"btn btn-sm neo-status-filter\" data-status-id=\"\">Todas</button>");

            filtersEl.innerHTML = buttons.join("");

            Array.prototype.forEach.call(filtersEl.querySelectorAll(".neo-status-filter"), function (btn) {
                btn.addEventListener("click", function () {
                    var raw = btn.getAttribute("data-status-id");
                    setActiveFilter(raw === "" ? null : parseInt(raw, 10));
                });
            });

            var hasPending = statuses.some(function (s) { return s.id === PENDING_STATUS_ID; });
            setActiveFilter(hasPending ? PENDING_STATUS_ID : null);
        }

        Promise.all([
            apiFetch("GET", "/bff/sale-status"),
            apiFetch("GET", "/bff/sale")
        ]).then(function (results) {
            allSales = results[1].ok && Array.isArray(results[1].data) ? results[1].data : [];
            allSales.sort(function (a, b) { return new Date(b.date) - new Date(a.date); });

            var statuses = results[0].ok && Array.isArray(results[0].data) ? results[0].data : [];
            renderFilters(statuses);
        });
    }

    function initNewSaleModal() {
        var triggerBtn = document.getElementById("neoNewSaleTrigger");
        var searchInput = document.getElementById("neoNewSaleCustomerSearch");
        var resultsEl = document.getElementById("neoNewSaleCustomerResults");
        var loadingEl = document.getElementById("neoNewSaleLoading");
        var alertBox = document.getElementById("neoNewSaleAlert");
        var modalEl = document.getElementById("neoNewSaleModal");

        if (!searchInput) {
            return;
        }

        var searchTimer = null;
        var busy = false;

        function resetModal() {
            searchInput.value = "";
            resultsEl.innerHTML = "";
            loadingEl.classList.add("d-none");
            alertBox.classList.add("d-none");
            busy = false;
        }

        modalEl.addEventListener("neo:hidden", resetModal);

        if (triggerBtn) {
            triggerBtn.addEventListener("click", function () {
                openModal(modalEl);
            });
        }

        Array.prototype.forEach.call(modalEl.querySelectorAll("[data-bs-dismiss=\"modal\"]"), function (btn) {
            btn.addEventListener("click", function () {
                closeModal(modalEl);
            });
        });

        modalEl.addEventListener("click", function (evt) {
            if (evt.target === modalEl) {
                closeModal(modalEl);
            }
        });

        searchInput.addEventListener("input", function () {
            var term = searchInput.value.trim();

            window.clearTimeout(searchTimer);
            if (term.length < 2) {
                resultsEl.innerHTML = "";
                return;
            }

            searchTimer = window.setTimeout(function () {
                apiFetch("GET", "/bff/customer/search/" + encodeURIComponent(term)).then(function (res) {
                    var customers = res.ok && Array.isArray(res.data) ? res.data.filter(function (c) { return c.status; }) : [];
                    if (customers.length === 0) {
                        resultsEl.innerHTML = "<div class=\"list-group-item text-muted\">Sin resultados</div>";
                        return;
                    }

                    resultsEl.innerHTML = customers.map(function (c) {
                        return "<button type=\"button\" class=\"list-group-item list-group-item-action neo-customer-option\" data-id=\"" + c.id + "\" data-name=\"" + escapeHtml(c.name) + "\">" +
                            escapeHtml(c.name) + " <span class=\"text-muted small\">" + escapeHtml(c.email) + "</span></button>";
                    }).join("");

                    Array.prototype.forEach.call(resultsEl.querySelectorAll(".neo-customer-option"), function (btn) {
                        btn.addEventListener("click", function () {
                            startOrResumeSale(btn.getAttribute("data-id"));
                        });
                    });
                });
            }, 300);
        });

        function startOrResumeSale(customerId) {
            if (busy) {
                return;
            }
            busy = true;
            alertBox.classList.add("d-none");
            resultsEl.innerHTML = "";
            loadingEl.classList.remove("d-none");

            apiFetch("GET", "/bff/sale/customer/" + customerId).then(function (res) {
                var sales = res.ok && Array.isArray(res.data) ? res.data : [];
                var pending = sales.filter(function (s) { return s.statusId === PENDING_STATUS_ID; })[0];

                if (pending) {
                    window.location.href = "/Sale/Details/" + pending.id;
                    return;
                }

                return apiFetch("POST", "/bff/sale", { idCustomer: customerId }).then(function (createRes) {
                    if (!createRes.ok || !createRes.data || !createRes.data.id) {
                        busy = false;
                        loadingEl.classList.add("d-none");
                        alertBox.textContent = (createRes.data && createRes.data.message) || "No se pudo iniciar la venta";
                        alertBox.classList.remove("d-none");
                        return;
                    }

                    window.location.href = "/Sale/Details/" + createRes.data.id;
                });
            });
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        wireLogout();
        if (!requireAuth()) {
            return;
        }
        initSaleList();
        initNewSaleModal();
    });
})();
