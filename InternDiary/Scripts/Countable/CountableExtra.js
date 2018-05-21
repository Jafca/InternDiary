$(function () {
    Countable.on(document.getElementById('Entry_Content'), counter => $("#wordCount").val(counter.words))
});