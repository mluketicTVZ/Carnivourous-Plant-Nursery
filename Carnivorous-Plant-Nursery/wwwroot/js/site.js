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
