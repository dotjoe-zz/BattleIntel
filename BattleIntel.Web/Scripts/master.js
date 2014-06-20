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

(function ($) {

    /* http://stackoverflow.com/a/3833699/40822 */
    $.fn.localTimeFromUTC = function (format) {

        return this.each(function () {

            // get time offset from browser
            var currentDate = new Date();
            var offset = -(currentDate.getTimezoneOffset() / 60);

            // get provided date
            var tagText = $(this).html();
            var givenDate = new Date(tagText);

            // apply offset
            var hours = givenDate.getHours();
            hours += offset;
            givenDate.setHours(hours);

            // format the date
            var localDateString = $.format.date(givenDate, format);
            $(this).html(localDateString);
        });
    };
})(jQuery);

$(function () {
    $("time.timeago").timeago();
    $("time.utcdate").localTimeFromUTC("MM/dd/yyyy hh:mm a");
});