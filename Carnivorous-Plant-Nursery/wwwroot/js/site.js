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

    initAttachmentLightbox();
});

function closeToast(toastElement) {
    if (!toastElement) return;
    toastElement.classList.add('fade-out');
    setTimeout(() => {
        toastElement.remove();
    }, 500);
}

function initAttachmentLightbox() {
    if (document.querySelector('.attachment-lightbox')) {
        return;
    }

    const lightbox = document.createElement('div');
    lightbox.className = 'attachment-lightbox';
    lightbox.innerHTML = `
        <div class="attachment-lightbox-panel" role="dialog" aria-modal="true" aria-label="Image preview">
            <button type="button" class="attachment-lightbox-close" aria-label="Close image preview">&times;</button>
            <button type="button" class="attachment-lightbox-nav attachment-lightbox-prev" aria-label="Previous image">&lsaquo;</button>
            <img class="attachment-lightbox-image" alt="" />
            <button type="button" class="attachment-lightbox-nav attachment-lightbox-next" aria-label="Next image">&rsaquo;</button>
        </div>`;
    document.body.appendChild(lightbox);

    const image = lightbox.querySelector('.attachment-lightbox-image');
    const closeButton = lightbox.querySelector('.attachment-lightbox-close');
    const previousButton = lightbox.querySelector('.attachment-lightbox-prev');
    const nextButton = lightbox.querySelector('.attachment-lightbox-next');
    let currentItems = [];
    let currentIndex = 0;

    const showImage = index => {
        if (currentItems.length === 0) {
            return;
        }

        currentIndex = (index + currentItems.length) % currentItems.length;
        const item = currentItems[currentIndex];
        image.src = item.src;
        image.alt = item.alt;
        previousButton.hidden = currentItems.length < 2;
        nextButton.hidden = currentItems.length < 2;
    };

    const close = () => {
        lightbox.classList.remove('is-visible');
        image.removeAttribute('src');
        image.alt = '';
    };

    document.addEventListener('click', event => {
        const trigger = event.target.closest('[data-gallery-trigger="true"]');
        if (!trigger) {
            return;
        }

        const gallery = trigger.closest('[data-attachment-gallery]');
        if (!gallery) {
            return;
        }

        event.preventDefault();
        currentItems = Array.from(gallery.querySelectorAll('[data-gallery-trigger="true"]'))
            .map(item => ({
                src: item.dataset.gallerySrc,
                alt: item.dataset.galleryAlt || ''
            }))
            .filter(item => item.src);
        showImage(Number.parseInt(trigger.dataset.galleryIndex || '0', 10));
        lightbox.classList.add('is-visible');
    });

    closeButton.addEventListener('click', close);
    previousButton.addEventListener('click', () => showImage(currentIndex - 1));
    nextButton.addEventListener('click', () => showImage(currentIndex + 1));
    lightbox.addEventListener('click', event => {
        if (event.target === lightbox) {
            close();
        }
    });
    document.addEventListener('keydown', event => {
        if (!lightbox.classList.contains('is-visible')) {
            return;
        }

        if (event.key === 'Escape') {
            close();
        } else if (event.key === 'ArrowLeft') {
            showImage(currentIndex - 1);
        } else if (event.key === 'ArrowRight') {
            showImage(currentIndex + 1);
        }
    });
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
        '/inventory', '/inventory/index',
        '/account/login', '/account/register', '/account/manage'
    ];

    function norm(p) {
        return (p || '/').toLowerCase().replace(/\/$/, '') || '/';
    }

    function isNavPage(href) {
        return NAV_PAGES.indexOf(norm(href)) !== -1;
    }

    function routeSection(path) {
        var normalized = norm(path);
        if (normalized === '/' || normalized.indexOf('/home') === 0) {
            return 'home';
        }

        return normalized.split('/')[1] || 'home';
    }

    function shouldAnimateToNavPage(targetHref) {
        if (!isNavPage(targetHref)) {
            return false;
        }

        if (isNavPage(window.location.pathname)) {
            return true;
        }

        return routeSection(window.location.pathname) !== routeSection(targetHref);
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

    function exitJawSubmit(form) {
        var j = buildJaws();
        j.top.classList.add('jaw-slamming');
        j.bot.classList.add('jaw-slamming');
        document.body.appendChild(j.overlay);

        setTimeout(function () {
            sessionStorage.setItem('pendingTransition', 'jaw');
            HTMLFormElement.prototype.submit.call(form);
        }, 350);
    }

    function exitJawBack() {
        var j = buildJaws();
        j.top.classList.add('jaw-slamming');
        j.bot.classList.add('jaw-slamming');
        document.body.appendChild(j.overlay);

        setTimeout(function () {
            sessionStorage.setItem('pendingTransition', 'jaw');
            if (window.history.length > 1) {
                window.history.back();
            } else {
                window.location.href = '/';
            }
        }, 350);
    }

    // ENTER: jaws start closed (covering hidden page), reveal page,
    // then snap open using the existing flytrap animation.
    function enterJaw() {
        document.querySelectorAll('.flytrap-overlay').forEach(function (overlay) {
            overlay.remove();
        });

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

    window.addEventListener('pageshow', function () {
        var pendingBackTransition = sessionStorage.getItem('pendingTransition');
        if (pendingBackTransition) {
            sessionStorage.removeItem('pendingTransition');
            enterJaw();
        } else {
            document.documentElement.style.visibility = '';
        }
    });

    // Intercept nav link clicks
    document.addEventListener('click', function (e) {
        var backLink = e.target.closest('[data-history-back="true"]');
        if (backLink) {
            e.preventDefault();
            if (isNavPage(window.location.pathname)) {
                exitJawBack();
            } else if (window.history.length > 1) {
                window.history.back();
            } else {
                window.location.href = '/';
            }
            return;
        }

        var link = e.target.closest('a[href]');
        if (!link) return;
        var href = link.getAttribute('href');
        if (!href || href.charAt(0) === '#' || href.indexOf('javascript') === 0) return;
        if (link.target === '_blank' || e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
        if (/^(https?:)?\/\//.test(href)) return;
        if (!shouldAnimateToNavPage(href)) return;

        e.preventDefault();
        exitJaw(href);
    });

    document.addEventListener('submit', function (e) {
        var form = e.target.closest('form[data-transition-submit="true"]');
        if (!form) return;

        e.preventDefault();
        exitJawSubmit(form);
    });

}());
