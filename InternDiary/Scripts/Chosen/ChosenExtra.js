$(document).ready(function () {
    $(".chosen").chosen({
        placeholder_text_multiple: "Select Some Skills",
        no_results_text: "Skill not found! Press tab to create"
    });

    $(".chosen-search-input").on("keydown", function (evt) {
        var stroke = (_ref = evt.which) !== null ? _ref : evt.keyCode;
        if (stroke === 9) { // 9 = tab key
            if (checkOption($(this).val()) && $(".chosen-results .no-results") !== undefined) {
                $(".chosen").append('<option value="' + $(this).val() + '" selected="selected">' + $(this).val() + '</option>');
                $(".chosen").trigger('chosen:updated');
            }
        }
    });

    function checkOption(newOption) {
        isNewOption = true;
        $('.chosen option').each(function () {
            if (newOption.toUpperCase() === this.value.toUpperCase())
                isNewOption = false;
        });
        return isNewOption;
    }
});