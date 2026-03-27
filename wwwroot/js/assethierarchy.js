

// ══════════════════════════════════════════════════════════════════
//  assetHierarchy.js  —  Asset Hierarchy page
//  โหลดเฉพาะหน้า AssetHierarchy/Index.cshtml ผ่าน @section Scripts
//  ต้องการ: site.js (callApi, safeGetElement, safeSetHTML, safeSetText)
// ══════════════════════════════════════════════════════════════════

const Tree = (() => {
    // ── State ──────────────────────────────────────────────
    let _allNodesMap = new Map();
    let _rootNodes = [];
    let _currentNode = null;
    let _originalTreeData = [];

    // ── State management ──────────────────────────────────
    function _resetState() {
        _allNodesMap.clear();
        _rootNodes = [];
        _currentNode = null;
        _originalTreeData = [];
    }

    // ── Templates ─────────────────────────────────────────
    const Templates = {
        loading: () => `
            <div class="text-center text-muted py-5">
                <i class="bi bi-diagram-3 fs-3"></i>
                <p>Loading...</p>
            </div>`,
        error: () => `
            <div class="text-center text-danger py-5">
                <i class="bi bi-exclamation-triangle fs-3"></i>
                <p>Failed to load data.</p>
                <button class="btn btn-outline-danger btn-sm mt-2"
                        onclick="Tree.reloadTree()">
                    <i class="bi bi-arrow-clockwise"></i> Retry
                </button>
            </div>`,
        empty: () => `
            <div class="text-center text-muted py-5">
                <i class="bi bi-inbox fs-3"></i>
                <p>No assets found.</p>
                <button class="btn btn-primary btn-sm mt-2"
                        onclick="Tree.openAddRoot()">
                    <i class="bi bi-plus-circle"></i> Add First Line
                </button>
            </div>`
    };

    // ── Build node map ────────────────────────────────────
    function _buildNodeMap(nodes) {
        if (!Array.isArray(nodes)) return;
        nodes.forEach(n => {
            if (!n?.id) return;
            _allNodesMap.set(n.id, n);
            if (n.children?.length) _buildNodeMap(n.children);
        });
    }

    // ── Render nodes ──────────────────────────────────────
    function _renderNodes(container, nodes) {
        nodes.forEach(n => {
            if (!n?.id) return;
            if (n.hasChildren && !n.children) n.children = [];
            n.childrenLoaded = false;
            container.appendChild(_createNodeWrapper(n));
        });
    }

    function _createNodeWrapper(node) {
        const div = document.createElement('div');
        div.dataset.id = node.id;
        div.textContent = node.name ?? node.id;
        return div;
    }

    // ── Stats update ──────────────────────────────────────
    function _updateStats(stats) {
        if (!stats) return;
        safeSetText('stat-line', stats.totalLines ?? 0);
        safeSetText('stat-machine', stats.totalMachines ?? 0);
        safeSetText('stat-unit', stats.totalUnits ?? 0);
        safeSetText('stat-part', stats.totalParts ?? 0);
    }

    // ── Response parser ───────────────────────────────────
    function _parseResponse(response) {
        const payload = response?.data ?? response; // รองรับ ApiResponse wrapper
        return {
            nodes: payload?.nodes ?? (Array.isArray(payload) ? payload : []),
            stats: payload?.stats ?? null,
        };
    }

    // ══════════════════════════════════════════════════════
    //  Public API
    // ══════════════════════════════════════════════════════

    async function reloadTree() {
        const container = safeGetElement('tree-container');
        if (!container) return;

        safeSetHTML(container, Templates.loading());
        _resetState();

        const response = await callApi('GetAssetData');
        if (!response) {
            safeSetHTML(container, Templates.error());
            return;
        }

        const { nodes, stats } = _parseResponse(response);
        if (!nodes.length) {
            safeSetHTML(container, Templates.empty());
            return;
        }

        _originalTreeData = nodes;
        _rootNodes = nodes;
        _buildNodeMap(_rootNodes);

        safeSetHTML(container, '');
        _renderNodes(container, _rootNodes);

        _updateStats(stats);
    }

    function openAddRoot() {
        console.log('[Tree] openAddRoot');
    }

    function getCurrentNode() { return _currentNode; }
    function setCurrentNode(node) { _currentNode = node; }
    function getNodeById(id) { return _allNodesMap.get(id) ?? null; }
    function getOriginalData() { return _originalTreeData; }

    return {
        reloadTree,
        openAddRoot,
        getCurrentNode,
        setCurrentNode,
        getNodeById,
        getOriginalData,
    };
})();

// ── Init ────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => Tree.reloadTree());
document.addEventListener('tabContentReady', () => Tree.reloadTree());
