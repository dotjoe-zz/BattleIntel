(function (window, undefined) {
    window.master = {
        /*
        Add Comma separators to a number.
        Credit to http://www.mredkj.com/javascript/numberFormat.html
        */
        formatNumber: function (nStr) {
            nStr += '';
            x = nStr.split('.');
            x1 = x[0];
            x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }
    };
})(window);

$(function () {
    $("time.timeago").timeago();
});