(function() {
    window.blazorLocalStorage = {
        get: key => key in localStorage ? JSON.parse(localStorage[key]) : null,
        set: (key, value) => { localStorage[key] = JSON.stringify(value); },
        delete: key => { delete localStorage[key]; }
    };


    window.Ambacht = {
        saveAsFile: function(filename, bytesBase64, mimeType) {
            var link = document.createElement('a');
            link.download = filename;
            link.href = "data:" + mimeType + ";base64," + bytesBase64;
            document.body.appendChild(link); // Needed for Firefox
            link.click();
            document.body.removeChild(link);
        },

        setWindowLocation: function(address) {
            window.location.assign(address);
        },

        scrollToTop: function () {
            window.scrollTo(0, 120); // Leave space for top bar
        },

        getBounds: function (el) {
            return el.getBoundingClientRect();
        }
    };

})();

