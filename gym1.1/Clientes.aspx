<%@ Page Title="Clientes " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="gym1._1.Clientes" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .client-search-panel {
            border: 1px solid rgba(34, 197, 94, .24);
            background: linear-gradient(135deg, rgba(15, 23, 42, .92), rgba(2, 6, 23, .98));
            border-radius: .75rem;
        }

        .client-search-box {
            position: relative;
        }

        .client-search-box i {
            color: #22c55e;
            left: 1rem;
            pointer-events: none;
            position: absolute;
            top: 50%;
            transform: translateY(-50%);
        }

        .client-search-input {
            background-color: #0b1120;
            border: 1px solid rgba(148, 163, 184, .4);
            color: #e5e7eb;
            max-width: none;
            padding-left: 2.75rem;
        }

        .client-search-input:focus {
            background-color: #0b1120;
            border-color: #22c55e;
            box-shadow: 0 0 0 .2rem rgba(34, 197, 94, .18);
            color: #f8fafc;
        }

        .client-search-input::placeholder {
            color: #94a3b8;
        }

        .client-suggestions {
            display: none;
            gap: .5rem;
            flex-wrap: wrap;
        }

        .client-suggestions.is-visible {
            display: flex;
        }

        .client-suggestion {
            align-items: center;
            background: rgba(15, 23, 42, .95);
            border: 1px solid rgba(34, 197, 94, .35);
            border-radius: 999px;
            color: #d1fae5;
            display: inline-flex;
            gap: .45rem;
            max-width: 100%;
            padding: .4rem .75rem;
            transition: background-color .15s ease, border-color .15s ease, transform .15s ease;
        }

        .client-suggestion:hover {
            background-color: rgba(22, 163, 74, .22);
            border-color: #22c55e;
            color: #fff;
            transform: translateY(-1px);
        }

        .client-table-wrap {
            border: 1px solid rgba(148, 163, 184, .18);
            border-radius: .75rem;
            overflow: hidden;
        }

        .client-table-wrap .table {
            margin-bottom: 0;
        }

        .client-table-wrap mark {
            background-color: rgba(34, 197, 94, .28);
            border-radius: .25rem;
            color: #f8fafc;
            padding: .05rem .15rem;
        }

        .empty-search-state {
            display: none;
            border: 1px dashed rgba(148, 163, 184, .35);
            border-radius: .75rem;
            color: #cbd5e1;
        }

        .empty-search-state.is-visible {
            display: block;
        }
    </style>

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
