var timeOut;

$(function () {
    addEventoToMessageClose($('.message .close'));
    addEventToMessageAutoClose();
});

function addEventoToMessageClose(element) {
    element.on('click', function () {
        clearTimeout(timeOut);
        $(this)
          .closest('.message')
          .transition('fade');

        $('#messageOperacao').hide();
    });
}

function addEventToMessageAutoClose() {
    clearTimeout(timeOut);
    timeOut = setTimeout(function () {
        $('.message.visible').not('.requiredFields').not('.other-messages')
            .closest('.message')
            .transition('fade');

        $('#messageOperacao').hide();
    }, 10000);
}