// ══════════════════════════════════════════════════════════════════
//  tabLoader.js — Tab system + content loader
//  ใส่ใน _Layout.cshtml โหลดทุกหน้า ไม่ใส่ใน Partial View
// ══════════════════════════════════════════════════════════════════


// ── Tab references — init หลัง DOM พร้อม ──────────────────────────
var tabBar;
var tabContent;

document.addEventListener('DOMContentLoaded', () => {
    tabBar = document.getElementById('tabBar');
    tabContent = document.getElementById('tabContent');
});

// ── Open Tab ───────────────────────────────────────────────────────
function openTab(url, title, icon, key) {
    const tabId = 'tab-' + key;

    // ถ้า tab มีอยู่แล้ว แค่ activate
    if (document.getElementById(tabId)) {
        activateTab(tabId);
        highlightMenu(key);
        return;
    }

    // สร้าง tab button
    const tab = document.createElement('div');
    tab.className = 'tab-item';
    tab.id = tabId;
    tab.innerHTML = `
        <i class="bi ${icon}"></i>
        <span>${title}</span>
        <span class="tab-close" id="close-${tabId}">
            <i class="bi bi-x"></i>
        </span>`;
    tabBar.appendChild(tab);

    // สร้าง panel
    const panel = document.createElement('div');
    panel.className = 'tab-panel';
    panel.id = tabId + '-content';
    panel.innerHTML = `
        <div style="padding:40px; display:flex; align-items:center; gap:10px; color:var(--muted)">
            <div class="spinner-border spinner-border-sm"></div> Loading...
        </div>`;
    tabContent.appendChild(panel);

    // โหลด content
    loadTabContent(url, panel);

    // คลิก tab
    tab.addEventListener('click', function (e) {
        if (e.target.closest('.tab-close')) return;
        activateTab(tabId);
        highlightMenu(key);
    });

    // ปิด tab
    document.getElementById('close-' + tabId).addEventListener('click', function (e) {
        e.stopPropagation();
        tab.remove();
        panel.remove();
        const last = tabBar.querySelector('.tab-item:last-child');
        if (last) {
            activateTab(last.id);
        } else {
            document.getElementById('welcomeTab')?.classList.add('active');
        }
    });

    activateTab(tabId);
    highlightMenu(key);
}

// ── Load Tab Content ───────────────────────────────────────────────
function loadTabContent(url, panel) {
    fetch(url)
        .then(res => {
            if (!res.ok) throw new Error('HTTP ' + res.status);
            return res.text();
        })
        .then(html => {
            const doc = new DOMParser().parseFromString(html, 'text/html');
            const content = doc.querySelector('.page-content') || doc.body;

            // 1) เก็บ scripts ออกมาก่อน inject HTML
            

                // 1) ใส่ HTML เข้า panel (ตัวหนังสือจะมา แต่ script ยังนิ่ง)
                panel.innerHTML = content.innerHTML;

                // 2) จัดการเรื่อง Modal (ย้ายไป body ตามเดิมของคุณ)
                panel.querySelectorAll('.modal').forEach(modal => {
                    if (!document.getElementById(modal.id)) {
                        document.body.appendChild(modal);
                    }
                });

                // 3) *** จุดสำคัญ: ปลุก Script ให้ทำงาน ***
                // ดึงเฉพาะ Script ที่อยู่ในหน้า View ที่เรา Fetch มา
                const scripts = Array.from(doc.querySelectorAll('script'));

                scripts.forEach(oldScript => {
                    const newScript = document.createElement('script');

                    // ก๊อปปี้ Attributes ทั้งหมด (เช่น src, type)
                    Array.from(oldScript.attributes).forEach(attr => {
                        newScript.setAttribute(attr.name, attr.value);
                    });

                    // ถ้าเป็น Inline Script (มีโค้ดข้างใน)
                    if (oldScript.textContent) {
                        newScript.textContent = oldScript.textContent;
                    }

                    // ฉีดเข้าสู่ DOM จริงเพื่อให้ Browser รันสคริปต์
                    document.body.appendChild(newScript);

                    // ลบแท็กทิ้งหลังจากรันเสร็จ (เพื่อไม่ให้ DOM รก แต่ตัวแปร/ฟังก์ชันจะยังอยู่ใน Memory)
                    if (!newScript.src) {
                        newScript.remove();
                    }
                });

                
            // 5) Re-execute scripts
            //    - skip external ที่โหลดแล้ว (ป้องกัน const ซ้ำ)
            //    - skip inline ที่มี tabBar/tabContent (อยู่ใน tabLoader.js แล้ว)
            // tabLoader.js — แก้ step 5 + 6

            const loadedSrcs = new Set(
                [...document.querySelectorAll('script[src]')].map(s => s.src)
            );
            const skipKeywords = ['tabBar', 'tabContent', 'openTab', 'loadTabContent'];

            // นับว่ามี external script ที่ต้องรอกี่ตัว
            let pendingScripts = 0;

            scripts.forEach(oldScript => {
                const newScript = document.createElement('script');

                if (oldScript.src) {
                    if (loadedSrcs.has(oldScript.src)) return;

                    pendingScripts++; // รอ script นี้โหลดก่อน
                    newScript.src = oldScript.src;
                    newScript.async = false;

                    // ✅ dispatch หลัง script โหลดเสร็จ
                    newScript.onload = () => {
                        pendingScripts--;
                        if (pendingScripts === 0) {
                            panel.dispatchEvent(new CustomEvent('tabContentReady', { bubbles: true }));
                        }
                    };
                    newScript.onerror = () => {
                        pendingScripts--;
                        if (pendingScripts === 0) {
                            panel.dispatchEvent(new CustomEvent('tabContentReady', { bubbles: true }));
                        }
                    };

                    document.body.appendChild(newScript);

                } else {
                    const code = oldScript.textContent || '';
                    if (skipKeywords.some(kw => code.includes(kw))) return;
                    newScript.textContent = code;
                    document.body.appendChild(newScript);
                    newScript.remove();
                }
            });
            // 6) แจ้ง page script ว่า content พร้อมแล้ว
            //    page script รับ event นี้แทน DOMContentLoaded

            // ถ้าไม่มี external script เลย — dispatch ทันที
            if (pendingScripts === 0) {
                panel.dispatchEvent(new CustomEvent('tabContentReady', { bubbles: true }));
            }

        
            //panel.dispatchEvent(new CustomEvent('tabContentReady', { bubbles: true }));
        })
        .catch(err => {
            panel.innerHTML = `
                <div style="padding:40px; color:#ef4444">
                    <i class="bi bi-exclamation-triangle me-2"></i>
                    Failed to load: ${err.message}
                </div>`;
        });
}

// ── Activate Tab ───────────────────────────────────────────────────
function activateTab(tabId) {
    document.querySelectorAll('.tab-item').forEach(t => t.classList.remove('active'));
    document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
    document.getElementById(tabId)?.classList.add('active');
    document.getElementById(tabId + '-content')?.classList.add('active');
}

// ── Highlight Sidebar Menu ─────────────────────────────────────────
function highlightMenu(key) {
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    Array.from(document.querySelectorAll('.nav-item'))
        .find(i => i.getAttribute('onclick')?.includes(`'${key}'`))
        ?.classList.add('active');
}

// ── Cleanup modal backdrop ─────────────────────────────────────────
document.addEventListener('hidden.bs.modal', function () {
    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
    document.body.classList.remove('modal-open');
    document.body.style = '';
});