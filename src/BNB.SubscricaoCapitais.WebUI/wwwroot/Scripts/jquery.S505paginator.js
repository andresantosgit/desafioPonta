(function ($) {
    $.fn.S505paginator = function (rowsCount, page, rowsPerPage, callback) 
    {
        this.puipaginator(
        {
            paginate: function (e, state)
            {
                $(this).data('quantidade-linhas', state.pageCount)
                if (callback !== null)
                {
                    callback(state.page + 1);
                }
            },
            page: page - 1,
            rows: rowsPerPage,
            totalRecords: rowsCount
        });

        return this;
    }
}(jQuery));