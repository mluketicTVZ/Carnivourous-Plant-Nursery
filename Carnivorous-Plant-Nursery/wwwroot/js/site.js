// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let actionFormIdToSubmit = null;

function openDeleteModal(formId, entityName) {
    actionFormIdToSubmit = formId;
    const modal = document.getElementById('global-delete-modal');
    if (modal) {
        const msg = modal.querySelector('.modal-body span');
        if (msg) {
            msg.textContent = entityName;
        }
        modal.classList.add('visible');
    }
}

function closeDeleteModal() {
    actionFormIdToSubmit = null;
    const modal = document.getElementById('global-delete-modal');
    if (modal) {
        modal.classList.remove('visible');
    }
}

function confirmDelete() {
    if (actionFormIdToSubmit) {
        const form = document.getElementById(actionFormIdToSubmit);
        if (form) {
            form.submit();
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const toast = document.querySelector('.toast-notification[data-autodismiss="true"]');
    if (toast) {
        // Auto-dismiss after 6 seconds
        setTimeout(() => {
            closeToast(toast);
        }, 6000);
    }
    
    // Close modal if clicked outside modal box
    const modalOverlay = document.getElementById('global-delete-modal');
    if (modalOverlay) {
        modalOverlay.addEventListener("click", function(e) {
            if (e.target === modalOverlay) {
                closeDeleteModal();
            }
        });
    }
});

function closeToast(toastElement) {
    if (!toastElement) return;
    toastElement.classList.add('fade-out');
    setTimeout(() => {
        toastElement.remove();
    }, 500);
}

// =============================================================
// Venus Flytrap page-load snap animation (once per session)
// =============================================================
(function initFlytrapAnimation() {
    if (sessionStorage.getItem('ftSnapped')) return;
    sessionStorage.setItem('ftSnapped', '1');

    // Build the overlay with two jaws
    var overlay = document.createElement('div');
    overlay.className = 'flytrap-overlay';

    var topJaw = document.createElement('div');
    topJaw.className = 'flytrap-jaw-top';

    var bottomJaw = document.createElement('div');
    bottomJaw.className = 'flytrap-jaw-bottom';

    overlay.appendChild(topJaw);
    overlay.appendChild(bottomJaw);
    document.body.appendChild(overlay);

    // Jaws are in place — reveal the page. The jaws cover it so there is no flash.
    document.documentElement.style.visibility = '';

    // Hold the closed jaws long enough for the page to finish loading, then snap open
    setTimeout(function () {
        topJaw.classList.add('flytrap-snapping');
        bottomJaw.classList.add('flytrap-snapping');

        // Remove overlay after animation finishes
        setTimeout(function () {
            overlay.remove();
        }, 900);
    }, 1900);
}());

// =============================================================
// Page Transition — Jaw Slam
// Navigating to any main index page: jaws slam shut (exit),
// then snap back open on the new page (enter).
// =============================================================
(function initPageTransitions() {
    var NAV_PAGES = [
        '/', '/home', '/home/index',
        '/plants', '/plants/index',
        '/seeds', '/seeds/index',
        '/taxonomy', '/taxonomy/index',
        '/care', '/care/index',
        '/inventory', '/inventory/index'
    ];

    function norm(p) {
        return (p || '/').toLowerCase().replace(/\/$/, '') || '/';
    }

    function isNavPage(href) {
        return NAV_PAGES.indexOf(norm(href)) !== -1;
    }

    function buildJaws() {
        var overlay = document.createElement('div');
        overlay.className = 'flytrap-overlay';
        overlay.style.zIndex = '9998';
        var top = document.createElement('div');
        top.className = 'flytrap-jaw-top';
        var bot = document.createElement('div');
        bot.className = 'flytrap-jaw-bottom';
        overlay.appendChild(top);
        overlay.appendChild(bot);
        return { overlay: overlay, top: top, bot: bot };
    }

    // EXIT: add slam animation BEFORE appending so the browser's first
    // paint already has the jaws at their off-screen 'from' position.
    function exitJaw(href) {
        var j = buildJaws();
        j.top.classList.add('jaw-slamming');
        j.bot.classList.add('jaw-slamming');
        document.body.appendChild(j.overlay);

        // Navigate once jaws are fully closed (animation is 0.32s)
        setTimeout(function () {
            sessionStorage.setItem('pendingTransition', 'jaw');
            window.location.href = href;
        }, 350);
    }

    // ENTER: jaws start closed (covering hidden page), reveal page,
    // then snap open using the existing flytrap animation.
    function enterJaw() {
        var j = buildJaws();
        // No transform needed — flytrap-jaw-top/bottom sit at position 0 by default
        document.body.appendChild(j.overlay);
        document.documentElement.style.visibility = '';

        // Short pause so the user sees the closed jaws before the snap
        setTimeout(function () {
            j.top.classList.add('flytrap-snapping');
            j.bot.classList.add('flytrap-snapping');
            setTimeout(function () { j.overlay.remove(); }, 900);
        }, 150);
    }

    // On page load, check if a transition is pending
    var pending = sessionStorage.getItem('pendingTransition');
    if (pending) {
        sessionStorage.removeItem('pendingTransition');
        enterJaw();
    } else {
        // Fallback: ensure page is visible if visibility was hidden for any reason
        document.documentElement.style.visibility = '';
    }

    // Intercept nav link clicks
    document.addEventListener('click', function (e) {
        var link = e.target.closest('a[href]');
        if (!link) return;
        var href = link.getAttribute('href');
        if (!href || href.charAt(0) === '#' || href.indexOf('javascript') === 0) return;
        if (link.target === '_blank' || e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
        if (/^(https?:)?\/\//.test(href)) return;
        if (!isNavPage(href)) return;

        e.preventDefault();
        exitJaw(href);
    });

}());
