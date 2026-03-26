// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ── Loading ───────────────────────────────────────────────────────


function showLoading() {
    document.getElementById('loadingOverlay')?.classList.add('show');
}

function hideLoading() {
    document.getElementById('loadingOverlay')?.classList.remove('show');
}

// ── Toast ─────────────────────────────────────────────────────────
//let _toastTimer;

function showToast(msg, type = 'success') {
    const el = document.getElementById('toast');
    if (!el) return;
    el.textContent = msg;
    el.className = `toast show toast-${type}`;
    //clearTimeout(_toastTimer);
    //_toastTimer = setTimeout(() => el.classList.remove('show'), 2500);
}

// ── DOM Helpers ───────────────────────────────────────────────────
function safeGetElement(id) {
    const el = document.getElementById(id);
    if (!el) console.warn(`[safeGetElement] ไม่พบ element: #${id}`);
    return el;
}

function safeSetHTML(el, html) {
    if (el) el.innerHTML = html;
}

function safeSetText(id, text) {
    const el = document.getElementById(id);
    if (el) el.textContent = text;
}

async function callApi(endpoint, method = 'GET', data = null) { // AbortController สำหรับ timeout + cancel 
    
    showLoading(); try {
        const options = {
            method, headers: { 'Content-Type': 'application/json' },
           
        };
        if (data) options.body = JSON.stringify(data);
        const response = await fetch(`${endpoint}`, options); // ไม่ใช้ global var
        if (!response.ok) { //  พยายาม parse JSON error ก่อน fallback เป็น text 
            const contentType = response.headers.get('content-type') ?? ''; const body = contentType.includes('application/json')
                ? (await response.json()).message ?? 'Unknown error'
                : await response.text(); throw new Error(`${response.status}: ${body}`);
        } return await response.json();
    } catch (error) { //  แยก abort error ออกจาก error ทั่วไป 
        if (error.name === 'AbortError') {
            showToast('Request timeout — กรุณาลองใหม่', 'danger');
        } else {
            showToast(`เกิดข้อผิดพลาด: ${error.message}`, 'danger');
        }
        console.error('[callApi]', endpoint, error); return null;
    } finally {
       
        hideLoading(); // ทำงานเสมอ ไม่ว่า success หรือ error 
    }
}