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

    function money(value) {
        return "Q" + Number(value).toFixed(2);
    }

    function statusBadgeClass(statusId) {
        if (statusId === 1) return "neo-badge-pending";
        if (statusId === 2) return "neo-badge-confirmed";
        return "neo-badge-cancelled";
    }

    document.addEventListener("DOMContentLoaded", function () {
        wireLogout();
        if (!requireAuth()) {
            return;
        }

        var root = document.getElementById("neoSaleRoot");
        var saleId = root.getAttribute("data-sale-id");

        var loadingEl = document.getElementById("neoSaleLoading");
        var errorEl = document.getElementById("neoSaleError");
        var contentEl = document.getElementById("neoSaleContent");
        var actionErrorEl = document.getElementById("neoActionError");
        var confirmWarningEl = document.getElementById("neoConfirmWarning");
        var confirmBtn = document.getElementById("neoConfirmBtn");
        var cartHintEl = document.getElementById("neoCartHint");
        var editableSectionEl = document.getElementById("neoEditableSection");
        var readOnlySectionEl = document.getElementById("neoReadOnlySection");
        var catalogFilterEl = document.getElementById("neoCatalogFilter");
        var catalogListEl = document.getElementById("neoCatalogList");
        var cartLinesEl = document.getElementById("neoCartLines");
        var cartTotalEl = document.getElementById("neoCartTotal");
        var readOnlyBodyEl = document.getElementById("neoReadOnlyBody");
        var readOnlyTotalEl = document.getElementById("neoReadOnlyTotal");

        var sale = null;
        var catalog = [];
        var cartItems = [];
        var addingProductId = null;

        function showActionError(message) {
            actionErrorEl.textContent = message;
            actionErrorEl.classList.remove("d-none");
        }

        function clearActionError() {
            actionErrorEl.classList.add("d-none");
        }

        actionErrorEl.addEventListener("click", function (evt) {
            if (evt.target && evt.target.classList.contains("btn-close")) {
                clearActionError();
            }
        });

        function renderHeader() {
            document.getElementById("neoSaleCustomerName").textContent = sale.customerName;
            var badge = document.getElementById("neoSaleStatusBadge");
            badge.textContent = sale.statusName;
            badge.className = "badge " + statusBadgeClass(sale.statusId);
            document.getElementById("neoSaleDate").textContent = new Date(sale.date).toLocaleString();
            confirmBtn.classList.toggle("d-none", sale.statusId !== PENDING_STATUS_ID);
            cartHintEl.classList.toggle("d-none", sale.statusId !== PENDING_STATUS_ID);
        }

        function renderCatalog() {
            var filter = (catalogFilterEl.value || "").trim().toLowerCase();
            var visible = catalog.filter(function (p) {
                return !filter || p.name.toLowerCase().indexOf(filter) !== -1;
            });

            if (visible.length === 0) {
                catalogListEl.innerHTML = "<div class=\"neo-empty-state\">No hay productos que coincidan.</div>";
                return;
            }

            catalogListEl.innerHTML = visible.map(function (p) {
                var busy = addingProductId === p.id;
                return "" +
                    "<div class=\"d-flex justify-content-between align-items-center neo-catalog-item\">" +
                    "<div>" +
                    "<div class=\"fw-semibold\">" + escapeHtml(p.name) + "</div>" +
                    "<div class=\"text-muted small\">" + money(p.price) + " — stock: " + p.stock + "</div>" +
                    "</div>" +
                    "<button type=\"button\" class=\"btn btn-sm btn-neo-primary neo-add-product\" data-id=\"" + p.id + "\" " + (busy ? "disabled" : "") + ">" +
                    (busy ? "Agregando..." : "Agregar") +
                    "</button>" +
                    "</div>";
            }).join("");

            Array.prototype.forEach.call(catalogListEl.querySelectorAll(".neo-add-product"), function (btn) {
                btn.addEventListener("click", function () {
                    var product = catalog.filter(function (p) { return p.id === btn.getAttribute("data-id"); })[0];
                    if (product) {
                        addOrIncrement(product);
                    }
                });
            });
        }

        function renderCart() {
            if (cartItems.length === 0) {
                cartLinesEl.innerHTML = "<div class=\"neo-empty-state\">El carrito esta vacio.</div>";
            } else {
                cartLinesEl.innerHTML = cartItems.map(function (item) {
                    return "" +
                        "<div class=\"neo-cart-line\" data-detail-id=\"" + item.idSaleDetail + "\">" +
                        "<div class=\"d-flex justify-content-between\">" +
                        "<span class=\"fw-semibold\">" + escapeHtml(item.productName) + "</span>" +
                        "<button type=\"button\" class=\"btn btn-sm btn-outline-danger neo-remove-line\">Quitar</button>" +
                        "</div>" +
                        "<div class=\"d-flex justify-content-between align-items-center mt-1\">" +
                        "<div class=\"d-flex align-items-center gap-2\">" +
                        "<label class=\"small text-muted mb-0\">Cantidad</label>" +
                        "<input type=\"number\" min=\"1\" class=\"form-control form-control-sm neo-qty-input neo-line-qty\" value=\"" + item.quantity + "\" />" +
                        "</div>" +
                        "<span>" + money(item.subtotal) + "</span>" +
                        "</div>" +
                        "</div>";
                }).join("");
            }

            var total = cartItems.reduce(function (sum, item) { return sum + item.subtotal; }, 0);
            cartTotalEl.textContent = money(total);

            Array.prototype.forEach.call(cartLinesEl.querySelectorAll(".neo-cart-line"), function (lineEl) {
                var detailId = lineEl.getAttribute("data-detail-id");
                var item = cartItems.filter(function (i) { return i.idSaleDetail === detailId; })[0];

                lineEl.querySelector(".neo-remove-line").addEventListener("click", function () {
                    removeLine(item);
                });

                lineEl.querySelector(".neo-line-qty").addEventListener("change", function (evt) {
                    var quantity = parseInt(evt.target.value, 10);
                    if (!quantity || quantity <= 0) {
                        return;
                    }
                    updateQuantity(item, quantity);
                });
            });
        }

        function loadCart() {
            return apiFetch("GET", "/bff/sale/cart/" + sale.idCustomer).then(function (res) {
                if (res.ok && res.data) {
                    cartItems = res.data.items || [];
                } else {
                    cartItems = [];
                }
                renderCart();
            });
        }

        function addOrIncrement(product) {
            if (addingProductId) {
                return;
            }
            addingProductId = product.id;
            clearActionError();
            renderCatalog();

            var existing = cartItems.filter(function (i) { return i.idProduct === product.id; })[0];
            var request = existing
                ? apiFetch("PUT", "/bff/sale-detail/" + existing.idSaleDetail, { idProduct: product.id, quantity: existing.quantity + 1 })
                : apiFetch("POST", "/bff/sale-detail", { idSale: saleId, idProduct: product.id, quantity: 1 });

            request.then(function (res) {
                if (!res.ok) {
                    showActionError((res.data && res.data.message) || "No se pudo agregar el producto");
                    return;
                }
                return loadCart();
            }).finally(function () {
                addingProductId = null;
                renderCatalog();
            });
        }

        function updateQuantity(item, quantity) {
            clearActionError();
            apiFetch("PUT", "/bff/sale-detail/" + item.idSaleDetail, { idProduct: item.idProduct, quantity: quantity }).then(function (res) {
                if (!res.ok) {
                    showActionError((res.data && res.data.message) || "No se pudo actualizar la cantidad");
                    renderCart();
                    return;
                }
                return loadCart();
            });
        }

        function removeLine(item) {
            clearActionError();
            apiFetch("DELETE", "/bff/sale-detail/" + item.idSaleDetail).then(function (res) {
                if (!res.ok) {
                    showActionError((res.data && res.data.message) || "No se pudo quitar el producto");
                    return;
                }
                return loadCart();
            });
        }

        function confirmSale() {
            clearActionError();
            confirmWarningEl.classList.add("d-none");
            confirmBtn.disabled = true;
            confirmBtn.textContent = "Confirmando...";

            apiFetch("POST", "/bff/sale/confirm/" + saleId).then(function (res) {
                if (res.ok && res.data && res.data.success) {
                    window.location.reload();
                    return;
                }

                var result = res.data || { message: "No se pudo confirmar la venta", lines: [] };
                var missing = (result.lines || []).filter(function (l) { return (l.quantityMissing || 0) > 0; });

                var html = "<p class=\"mb-2\">" + escapeHtml(result.message) + "</p>";
                if (missing.length > 0) {
                    html += "<ul class=\"mb-0\">" + missing.map(function (l) {
                        return "<li>" + escapeHtml(l.name) + ": solicitado " + l.quantityRequested + ", disponible " + l.stockAvailable + " (faltan " + l.quantityMissing + ")</li>";
                    }).join("") + "</ul>";
                }

                confirmWarningEl.innerHTML = html;
                confirmWarningEl.classList.remove("d-none");
            }).finally(function () {
                confirmBtn.disabled = false;
                confirmBtn.textContent = "Terminar la compra";
            });
        }

        function renderReadOnly(details, products) {
            var names = {};
            products.forEach(function (p) { names[p.id] = p.name; });

            var total = 0;
            readOnlyBodyEl.innerHTML = details.map(function (d) {
                var subtotal = d.quantity * d.priceUnit;
                total += subtotal;
                return "" +
                    "<tr>" +
                    "<td>" + escapeHtml(names[d.idProduct] || "Producto") + "</td>" +
                    "<td>" + d.quantity + "</td>" +
                    "<td>" + money(d.priceUnit) + "</td>" +
                    "<td>" + money(subtotal) + "</td>" +
                    "</tr>";
            }).join("");

            readOnlyTotalEl.textContent = money(total);
        }

        loadingEl.classList.remove("d-none");

        apiFetch("GET", "/bff/sale/" + saleId).then(function (res) {
            if (!res.ok || !res.data) {
                throw new Error((res.data && res.data.message) || "Venta no encontrada");
            }

            sale = res.data;
            renderHeader();

            if (sale.statusId === PENDING_STATUS_ID) {
                editableSectionEl.classList.remove("d-none");

                confirmBtn.addEventListener("click", confirmSale);
                catalogFilterEl.addEventListener("input", renderCatalog);

                return Promise.all([
                    apiFetch("GET", "/bff/product").then(function (r) {
                        catalog = (r.ok && Array.isArray(r.data) ? r.data : []).filter(function (p) { return p.status; });
                        renderCatalog();
                    }),
                    loadCart()
                ]);
            }

            readOnlySectionEl.classList.remove("d-none");

            return Promise.all([
                apiFetch("GET", "/bff/sale-detail/sale/" + saleId),
                apiFetch("GET", "/bff/product")
            ]).then(function (results) {
                var details = results[0].ok && Array.isArray(results[0].data) ? results[0].data : [];
                var products = results[1].ok && Array.isArray(results[1].data) ? results[1].data : [];
                renderReadOnly(details, products);
            });
        }).then(function () {
            loadingEl.classList.add("d-none");
            contentEl.classList.remove("d-none");
        }).catch(function (err) {
            loadingEl.classList.add("d-none");
            errorEl.textContent = err.message || "No se pudo cargar la venta";
            errorEl.classList.remove("d-none");
        });
    });
})();
