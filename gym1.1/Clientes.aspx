<%@ Page Title="Clientes " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="gym1._1.Clientes" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="my-3 d-flex align-items-center gap-2">
        <i class="bi bi-people text-success"></i>
        <span class="text-success">Listado de Clientes</span>
    </h2>

    <div class="main-card p-4">

        <div class="client-search-panel p-3 mb-4">
            <div class="row g-3 align-items-center">
                <div class="col-lg-8">
                    <label for="clientSearch" class="form-label text-success fw-semibold mb-2">Buscar cliente</label>
                    <div class="client-search-box">
                        <i class="bi bi-search"></i>
                        <input id="clientSearch"
                            type="search"
                            class="form-control form-control-lg client-search-input"
                            autocomplete="off"
                            placeholder="Nombre, apellido, DNI, teléfono, email o plan" />
                    </div>
                </div>
                <div class="col-lg-4">
                    <div class="d-flex flex-lg-column gap-2 justify-content-between justify-content-lg-center text-lg-end">
                        <span class="text-secondary small">Resultados visibles</span>
                        <strong id="clientResultCount" class="fs-4 text-success">0</strong>
                    </div>
                </div>
            </div>
            <div id="clientSuggestions" class="client-suggestions mt-3" aria-label="Sugerencias de clientes"></div>
        </div>

        <div class="client-table-wrap">
            <asp:GridView ID="gvClientes" runat="server"
                ClientIDMode="Static"
                CssClass="table table-bordered table-striped align-middle"
                AutoGenerateColumns="False"
                EmptyDataText="No hay clientes registrados">

                <Columns>
                    <asp:BoundField DataField="DNI" HeaderText="DNI" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:BoundField DataField="Email" HeaderText="Email" /> 
                    <asp:BoundField DataField="PlanPago" HeaderText="Plan" />
                </Columns>

            </asp:GridView>
        </div>

        <div id="emptySearchState" class="empty-search-state p-4 mt-3 text-center">
            <i class="bi bi-search d-block fs-2 text-success mb-2"></i>
            No se encontraron clientes con esa búsqueda.
        </div>
    </div>

    <script>
        (function () {
            const searchInput = document.getElementById('clientSearch');
            const table = document.getElementById('gvClientes');
            const suggestions = document.getElementById('clientSuggestions');
            const resultCount = document.getElementById('clientResultCount');
            const emptyState = document.getElementById('emptySearchState');

            if (!searchInput || !table || !suggestions || !resultCount || !emptyState) {
                return;
            }

            const rows = Array.from(table.querySelectorAll('tr')).filter(row => row.querySelectorAll('td').length);
            const clients = rows.map(row => {
                const cells = Array.from(row.cells);
                const values = cells.map(cell => cell.textContent.trim());
                const searchable = values.join(' ').toLowerCase();

                return {
                    row,
                    cells,
                    dni: values[0] || '',
                    nombre: values[1] || '',
                    apellido: values[2] || '',
                    telefono: values[3] || '',
                    email: values[4] || '',
                    plan: values[5] || '',
                    searchable
                };
            });

            const escapeHtml = value => value.replace(/[&<>"']/g, char => ({
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#39;'
            }[char]));

            const updateCount = count => {
                resultCount.textContent = count.toString();
            };

            const clearHighlights = client => {
                client.cells.forEach(cell => {
                    cell.textContent = cell.textContent;
                });
            };

            const highlightCell = (cell, query) => {
                const text = cell.textContent;
                const lowerText = text.toLowerCase();
                const index = lowerText.indexOf(query);

                if (index === -1 || !query) {
                    cell.textContent = text;
                    return;
                }

                const before = escapeHtml(text.slice(0, index));
                const match = escapeHtml(text.slice(index, index + query.length));
                const after = escapeHtml(text.slice(index + query.length));
                cell.innerHTML = before + '<mark>' + match + '</mark>' + after;
            };

            const renderSuggestions = matches => {
                suggestions.innerHTML = '';

                if (!matches.length || !searchInput.value.trim()) {
                    suggestions.classList.remove('is-visible');
                    return;
                }

                matches.slice(0, 6).forEach(client => {
                    const button = document.createElement('button');
                    button.type = 'button';
                    button.className = 'btn client-suggestion';
                    button.innerHTML = '<i class="bi bi-person"></i><span>' +
                        escapeHtml(client.nombre + ' ' + client.apellido) +
                        '</span><small class="text-secondary">' + escapeHtml(client.dni) + '</small>';
                    button.addEventListener('click', () => {
                        searchInput.value = (client.nombre + ' ' + client.apellido).trim();
                        applySearch();
                        searchInput.focus();
                    });
                    suggestions.appendChild(button);
                });

                suggestions.classList.add('is-visible');
            };

            function applySearch() {
                const query = searchInput.value.trim().toLowerCase();
                let visibleCount = 0;
                const matches = [];

                clients.forEach(client => {
                    const isMatch = !query || client.searchable.includes(query);
                    client.row.style.display = isMatch ? '' : 'none';
                    clearHighlights(client);

                    if (isMatch) {
                        visibleCount += 1;
                        matches.push(client);

                        if (query) {
                            client.cells.forEach(cell => highlightCell(cell, query));
                        }
                    }
                });

                updateCount(visibleCount);
                emptyState.classList.toggle('is-visible', visibleCount === 0);
                renderSuggestions(matches);
            }

            searchInput.addEventListener('input', applySearch);
            searchInput.addEventListener('keydown', event => {
                if (event.key === 'Escape') {
                    searchInput.value = '';
                    applySearch();
                }
            });

            applySearch();
        })();
    </script>

</asp:Content>
