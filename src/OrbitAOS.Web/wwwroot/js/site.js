// OrbitAOS - Site-wide JavaScript
// Migrated from legacy ASP.NET MVC Scripts/site.js to ASP.NET Core wwwroot/js/site.js
// Uses vanilla JS and Bootstrap 5 (replaces jQuery-dependent patterns from legacy MVC)

// ── Auto-dismiss alerts after 5 seconds ──────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert.alert-success, .alert.alert-info');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 5000);
    });
});

// ── Confirm delete actions ────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const deleteForms = document.querySelectorAll('form[data-confirm]');
    deleteForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            const message = form.getAttribute('data-confirm') || 'Are you sure?';
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
});
