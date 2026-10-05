// Inventory Management System - Global Client Scripts

/**
 * Safe print trigger utility.
 * Focuses the window and invokes window.print() asynchronously to decouple
 * from the click event handler, preventing browser UI freezes and avoiding
 * Visual Studio debugger detachment / CDP disconnect issues.
 * 
 * @param {string|Event} [customTitleOrEvent] - Optional title to set before printing (e.g., 'INV-00008') or Event object.
 * @param {Event} [evt] - Optional click event object.
 */
function triggerPrint(customTitleOrEvent, evt) {
    var title = null;
    var eventObj = null;

    if (customTitleOrEvent) {
        if (typeof customTitleOrEvent === 'string') {
            title = customTitleOrEvent;
            eventObj = evt;
        } else if (typeof customTitleOrEvent === 'object' && (customTitleOrEvent instanceof Event || customTitleOrEvent.target)) {
            eventObj = customTitleOrEvent;
        }
    }

    if (eventObj && typeof eventObj.preventDefault === 'function') {
        try {
            eventObj.preventDefault();
            if (typeof eventObj.stopPropagation === 'function') {
                eventObj.stopPropagation();
            }
        } catch (e) {
            // Ignore synthetic event errors
        }
    }

    var originalTitle = document.title;
    if (title && typeof title === 'string' && title.trim() !== '') {
        document.title = title.trim();
    }

    // Restore title after print dialog closes
    var restoreTitleHandler = function () {
        window.removeEventListener('afterprint', restoreTitleHandler);
        if (originalTitle && title && document.title === title.trim()) {
            document.title = originalTitle;
        }
    };
    window.addEventListener('afterprint', restoreTitleHandler);

    // Asynchronously decouple from click event loop so the handler returns immediately
    // and doesn't block the browser UI thread or debugger connection
    if (typeof window.requestAnimationFrame === 'function') {
        window.requestAnimationFrame(function () {
            setTimeout(executePrint, 150);
        });
    } else {
        setTimeout(executePrint, 150);
    }

    function executePrint() {
        try {
            window.focus();
            window.print();
        } catch (err) {
            console.error('Safe print trigger encountered an error:', err);
        }
    }
}

// Expose globally on window
window.triggerPrint = triggerPrint;

// Synchronize document.title for browser print dialogs if a print-ready element defines data-print-title
window.addEventListener('beforeprint', function () {
    try {
        var printEl = document.querySelector('[data-print-title]');
        if (printEl) {
            var customTitle = printEl.getAttribute('data-print-title');
            if (customTitle && customTitle.trim() !== '') {
                document.title = customTitle.trim();
            }
        }
    } catch (e) {
        console.warn('Could not set custom print title:', e);
    }
});
