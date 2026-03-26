'use strict';

// ══════════════════════════════════════════════════════════════════
//  masterData.js — Maintenance Standard page
//  โหลดผ่าน @section Scripts ใน Standard/Index.cshtml
//  ต้องการ: site.js (callApi, safeSetText)
// ══════════════════════════════════════════════════════════════════

// ── Constants ───────────────────────────────────────────────────────
const PAGE_SIZE = 10;
let currentPage = 1;

// ── API Endpoints ───────────────────────────────────────────────────
const STDMT = {
    standards: 'GetStandards',
    indicators: 'GetIndicators',
    assetTypes: 'GetAssetTypes',
    assetsByType: (typeId) => `GetAssetsByType?assetTypeId=${typeId}`,
    statistics: 'GetStatistics'

};

// ── Module ──────────────────────────────────────────────────────────
const MaintenanceTree = (() => {

    // ── Private state ──────────────────────────────────────────────
    let _standards = [];
    let _indicators = [];

    // ══════════════════════════════════════════════════════════════
    //  Public: initMasterData — entry point
    // ══════════════════════════════════════════════════════════════
    async function initMasterData() {
        await Promise.all([
            _loadStandards(),
            _loadIndicators(),
            _loadAssetTypes(),
            _loadStatistics()
        ]);

        _renderStandardsTable(_standards);
        _bindEvents();
        console.log(' Master data initialized');
    }

    // ══════════════════════════════════════════════════════════════
    //  Private: Load data
    // ══════════════════════════════════════════════════════════════
    async function _loadStatistics() {
        const res = await callApi(STDMT.statistics);
        if (!res?.success) return;

        const stats = res.data;

        // อัปเดตการ์ด
        const cardMap = {
            '#totalStandards': stats.totalStandards,
            '#totalIndicators': stats.totalIndicators,
            '#totalControlItems': stats.totalControlItems,
            '#totalSchedules': stats.totalSchedules,
        };
        Object.entries(cardMap).forEach(([id, val]) => $(id).text(val ?? '-'));

        // สร้างกราฟ
        createControlsByAssetTypeBottomChart(stats.controlsByAssetType);
        createSchedulesByIndicatorBottomChart(stats.schedulesByIndicator);
        createByStandardBottomChart(stats.controlsByStandardChart);

        console.log(' Statistics loaded');
    }

    async function _loadStandards() {
        const res = await callApi(STDMT.standards);
        if (!res?.success) return;
        _standards = res.data;
        _populateSelect('#quickAddStandard', _standards, s => ({
            value: s.standardId,
            text: s.standardDesc,
        }));
        console.log('✅ Standards loaded:', _standards.length);
    }

    async function _loadIndicators() {
        const res = await callApi(STDMT.indicators);
        if (!res?.success) return;
        _indicators = res.data;
        _populateSelect('#quickAddIndicator', _indicators, ind => ({
            value: ind.indicatorId,
            text: `${ind.indicatorCode} - ${ind.indicatorDesc} (${ind.unitDesc})`,
        }));
        console.log('✅ Indicators loaded:', _indicators.length);
    }

    async function _loadAssetTypes() {
        const res = await callApi(STDMT.assetTypes);
        if (!res?.success) return;
        _populateSelect('#assetTypeId, #filterAssetType', res.data, t => ({
            value: t.assetTypeId,
            text: t.typeName,
        }));
    }

    // ══════════════════════════════════════════════════════════════
    //  Public: onAssetTypeChange — เรียกจาก onchange ใน HTML
    // ══════════════════════════════════════════════════════════════
    async function onAssetTypeChange() {
        const typeId = document.querySelector('#assetTypeId')?.value;
        const assetDdl = document.querySelector('#assetId');
        if (!assetDdl) return;

        _resetSelect(assetDdl, '-- Choose Asset --');
        assetDdl.disabled = true;
        if (!typeId) return;

        document.getElementById('assetLoading')?.classList.remove('d-none');
        const res = await callApi(STDMT.assetsByType(typeId));
        document.getElementById('assetLoading')?.classList.add('d-none');

        if (!res?.success || !res.data.length) return;

        _populateSelect('#assetId', res.data, a => ({
            value: a.assetId,
            text: `${a.assetCode} - ${a.assetName}`,
        }));
        assetDdl.disabled = false;
    }

    // ══════════════════════════════════════════════════════════════
    //  Private: Render table
    // ══════════════════════════════════════════════════════════════
    function _renderStandardsTable(data) {
        const tbody = document.getElementById('tableBody');
        if (!tbody) return;

        tbody.innerHTML = data.map(s => `
            <tr data-name="${(s.standardDesc || '').toLowerCase()}">
                <td>${String(s.standardId).padStart(3, '0')}</td>
                <td><span class="task-tag tag-inprogress">${s.standardCode ?? ''}</span></td>
                <td>${s.standardDesc ?? ''}</td>
                <td>${s.standardTypeDesc ?? ''}</td>
                <td style="color:var(--muted)">
                    ${s.createdTime ? new Date(s.createdTime).toLocaleDateString() : ''}
                </td>
                <td>
                    <a href="#" class="action-btn"><i class="bi bi-pencil"></i></a>
                    <a href="#" class="action-btn"><i class="bi bi-trash"></i></a>
                </td>
            </tr>`
        ).join('');

        currentPage = 1;
        renderPagination();
    }

    // ══════════════════════════════════════════════════════════════
    //  Private: Bind events
    // ══════════════════════════════════════════════════════════════
    function _bindEvents() {
        document.querySelector('#assetTypeId')
            ?.addEventListener('change', onAssetTypeChange);
    }

    // ══════════════════════════════════════════════════════════════
    //  Private: Select helpers
    // ══════════════════════════════════════════════════════════════
    function _populateSelect(selector, items, mapper) {
        document.querySelectorAll(selector).forEach(el => {
            const placeholder = el.querySelector('option:first-child');
            el.innerHTML = '';
            if (placeholder) el.appendChild(placeholder);

            items.forEach(item => {
                const { value, text } = mapper(item);
                const opt = document.createElement('option');
                opt.value = value;
                opt.textContent = text;
                el.appendChild(opt);
            });
        });
    }

    function _resetSelect(el, placeholderText) {
        el.innerHTML = `<option value="">${placeholderText}</option>`;
    }

    // ── Expose public API ──────────────────────────────────────────
    return {
        initMasterData,   // ✅ expose ออกมา
        onAssetTypeChange,
    };

})();

// ══════════════════════════════════════════════════════════════════
//  Search
// ══════════════════════════════════════════════════════════════════
function doSearch(query) {
    const q = (query || '').toLowerCase();
    document.querySelectorAll('#tableBody tr').forEach(row => {
        const name = row.getAttribute('data-name') || '';
        row.style.display = name.includes(q) ? '' : 'none';
    });
    currentPage = 1;
    renderPagination();
}

// ══════════════════════════════════════════════════════════════════
//  Pagination
// ══════════════════════════════════════════════════════════════════
function renderPagination() {
    const rows = Array.from(document.querySelectorAll('#tableBody tr'))
        .filter(r => r.style.display !== 'none');

    const totalPages = Math.ceil(rows.length / PAGE_SIZE);
    const box = document.getElementById('paginationBox');
    if (!box) return;

    box.style.display = totalPages > 1 ? 'flex' : 'none';
    if (totalPages <= 1) return;

    rows.forEach(r => r.style.display = 'none');
    const start = (currentPage - 1) * PAGE_SIZE;
    rows.slice(start, start + PAGE_SIZE).forEach(r => r.style.display = '');

    const prev = `<div class="page-btn ${currentPage === 1 ? 'disabled' : ''}"
                       onclick="goPage(${currentPage - 1})">‹</div>`;
    const next = `<div class="page-btn ${currentPage === totalPages ? 'disabled' : ''}"
                       onclick="goPage(${currentPage + 1})">›</div>`;
    const pages = Array.from({ length: totalPages }, (_, i) => i + 1)
        .map(i => `<div class="page-btn ${i === currentPage ? 'active' : ''}"
                        onclick="goPage(${i})">${i}</div>`)
        .join('');

    box.innerHTML = prev + pages + next;
}

function goPage(page) {
    const total = Math.ceil(
        document.querySelectorAll('#tableBody tr').length / PAGE_SIZE
    );
    if (page < 1 || page > total) return;
    currentPage = page;
    renderPagination();
}

// ══════════════════════════════════════════════════════════════════
//  Init — รองรับทั้ง DOMContentLoaded และ tabContentReady
// ══════════════════════════════════════════════════════════════════
async function _initPage() {
    try {
        await MaintenanceTree.initMasterData();
        console.log(' Standard Ready!');
    } catch (error) {
        console.error(' Initialization Error:', error);
    }
}

document.addEventListener('DOMContentLoaded', _initPage);
document.addEventListener('tabContentReady', _initPage);