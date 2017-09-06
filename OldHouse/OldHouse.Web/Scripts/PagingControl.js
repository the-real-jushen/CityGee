function wireupPaging() {
    if ($('.paging-ajax').length) {
        //alert('wired');
        $('.pageNumber').click(function (e) {
            //replace the page content
            //alert("hh");
            e.preventDefault();
            var self = $(this);
            if (self.parent().hasClass("disabled") == false) {
                $.get(self.attr("href"), function(data) {
                    $('#' + self.parent().parent().attr("target")).replaceWith(data);
                });
            }
            //$('#' + $(this).parent().parent().attr("target")).load(
            //    $(this).attr("href")
            //);
        });
    }
}