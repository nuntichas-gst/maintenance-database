

// ══════════════════════════════════════════════════════════════════
//  assetHierarchy.js  —  Asset Hierarchy page
//  โหลดเฉพาะหน้า AssetHierarchy/Index.cshtml ผ่าน @section Scripts
//  ต้องการ: site.js (callApi, safeGetElement, safeSetHTML, safeSetText)
// ══════════════════════════════════════════════════════════════════

const Tree = (() => {

    // ── Private state ──────────────────────────────────────────────
    // เก็บไว้ใน module scope — ไม่ leak ออกเป็น global variable
    let _allNodesMap = new Map();
    let _rootNodes = [];
    let _currentNode = null;
    let _originalTreeData = [];

    // ── Private: State management ──────────────────────────────────

    function _resetState() {
        _allNodesMap.clear();
        _rootNodes = [];
        _currentNode = null;
        _originalTreeData = [];
    }

    // ── Private: DOM templates ─────────────────────────────────────
    // แยก HTML string ออกมาเป็น function — อ่านง่าย แก้ที่เดียว

    function _loadingTemplate() {
        return `
            <div class="text-center text-muted py-5">
                <i class="bi bi-diagram-3 fs-3"></i>
                <p>Loading...</p>
            </div>`;
    }

    function _errorTemplate() {
        return `
            <div class="text-center text-danger py-5">
                <i class="bi bi-exclamation-triangle fs-3"></i>
                <p>Failed to load data.</p>
                <button class="btn btn-outline-danger btn-sm mt-2"
                        onclick="Tree.reloadTree()">
                    <i class="bi bi-arrow-clockwise"></i> Retry
                </button>
            </div>`;
    }

    function _emptyTemplate() {
        return `
            <div class="text-center text-muted py-5">
                <i class="bi bi-inbox fs-3"></i>
                <p>No assets found.</p>
                <button class="btn btn-primary btn-sm mt-2"
                        onclick="Tree.openAddRoot()">
                    <i class="bi bi-plus-circle"></i> Add First Line
                </button>
            </div>`;
    }

    // ── Private: Build node map ────────────────────────────────────

    function _buildNodeMap(nodes) {
        if (!Array.isArray(nodes)) return;
        nodes.forEach(n => {
            if (!n?.id) return;
            _allNodesMap.set(n.id, n);
            if (n.children?.length) _buildNodeMap(n.children);
        });
    }

    // ── Private: Render tree nodes ─────────────────────────────────

    function _renderNodes(container, nodes) {
        nodes.forEach(n => {
            if (!n?.id) return;
            if (n.hasChildren && !n.children) n.children = [];
            n.childrenLoaded = false;
            container.appendChild(_createNodeWrapper(n, true));
        });
    }

    // NOTE: _createNodeWrapper implement แยกหรือ inline ได้ตามโครงสร้างจริง
    function _createNodeWrapper(node, isRoot) {
        // placeholder — implement ตาม UI จริง
        const div = document.createElement('div');
        div.dataset.id = node.id;
        div.textContent = node.name ?? node.id;
        return div;
    }

    // ── Private: Update stats section ─────────────────────────────

    function _updateStats(stats) {
        if (!stats) return;
        safeSetText('stat-line', stats.totalLines ?? 0);
        safeSetText('stat-machine', stats.totalMachines ?? 0);
        safeSetText('stat-unit', stats.totalUnits ?? 0);
        safeSetText('stat-part', stats.totalParts ?? 0);
    }

    // ── Private: Parse response ────────────────────────────────────
    // รองรับทั้ง { nodes: [...], stats: {...} } และ [...] โดยตรง

    function _parseResponse(response) {
        return {
            nodes: response?.nodes ?? (Array.isArray(response) ? response : []),
            stats: response?.stats ?? null,
        };
    }

    // ══════════════════════════════════════════════════════════════
    //  Public: reloadTree
    //  ทำงาน 5 ขั้นตอนชัดเจน แต่ละขั้นแยก function ย่อย
    // ══════════════════════════════════════════════════════════════

    async function reloadTree() {
        // 1. ตรวจสอบ container
        const container = safeGetElement('tree-container');
        if (!container) return;

        // 2. Reset UI และ state ก่อน fetch
        safeSetHTML(container, _loadingTemplate());
        _resetState();
        

        // 3. เรียก API — callApi มาจาก site.js จัดการ loading/error ให้แล้ว
        const response = await callApi('GetAssetData');
        if (!response) {
            safeSetHTML(container, _errorTemplate());
            return;
        }

        // 4. Parse และตรวจสอบข้อมูล
        const { nodes, stats } = _parseResponse(response);
        if (!nodes.length) {
            safeSetHTML(container, _emptyTemplate());
            return;
        }

        // 5. Build tree DOM
        _originalTreeData = nodes;
        _rootNodes = nodes;
        _buildNodeMap(_rootNodes);
        safeSetHTML(container, '');
        _renderNodes(container, _rootNodes);

        // 6. Update stats (ถ้ามี)
        _updateStats(stats);
    }

    // ── Public: openAddRoot ────────────────────────────────────────

    function openAddRoot() {
        // implement ตาม modal/form จริง
        console.log('[Tree] openAddRoot');
    }

    // ── Public: getCurrentNode / setCurrentNode ────────────────────

    function getCurrentNode() { return _currentNode; }
    function setCurrentNode(node) { _currentNode = node; }
    function getNodeById(id) { return _allNodesMap.get(id) ?? null; }
    function getOriginalData() { return _originalTreeData; }

    // ── Expose public API ──────────────────────────────────────────
    // เฉพาะ function ที่ View หรือ event handler ภายนอกต้องเรียก

    return {
        reloadTree,
        openAddRoot,
        getCurrentNode,
        setCurrentNode,
        getNodeById,
        getOriginalData,
    };

})();

// ── Init ───────────────────────────────────────────────────────────
// เรียก reloadTree เมื่อ DOM พร้อม — ไม่ใช้ window.onload
document.addEventListener('DOMContentLoaded', () => Tree.reloadTree());
document.addEventListener('tabContentReady', () => Tree.reloadTree());