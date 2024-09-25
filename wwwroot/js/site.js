const scrolltoTop = function () {
    window.scrollTo(0, 0);
}
const showloading = function () {
    $('.loading').css('display', 'flex');
}
const hideloading = function () {
    $('.loading').css('display', 'none');
};


function LoadDatatable(className, isOrdering, pagelength) {
    $(className).DataTable({
        "responsive": false, "autoWidth": false,
        "ordering": isOrdering, destroy: true,
        "pageLength": pagelength,
        lengthMenu: [
            [10, 15, 25, 50, -1],
            ['10 rows', '15 rows', '25 rows', '50 rows', 'Show all']
        ],
    });
}