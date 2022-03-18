toggleFilterSelectors(); // better solution?

$("#list-type-selector").change(function () {
    toggleFilterSelectors();
});


/**
 * Limits values in the year selection field.
 */
$("#year-selector").on("change", function (evt) {
    const value = parseInt($(this).val());
    const max = parseInt($(this).attr("max"));
    const min = parseInt($(this).attr("min"));

    if (value > max) $(this).val(max);
    if (value < min) $(this).val(min);
});


$("#list-type-selector, #month-selector, #year-selector, #page-size-selector").ready(function () {
    $(this).data('old-value', $(this).val()); // TODO: add cookies?
});

$("#list-type-selector, #month-selector, #year-selector, #page-size-selector").change(function () {
    let oldValue = $(this).data('old-value');
    let curValue = $(this).val();

    if (oldValue != curValue)
        $("#data-selector-form").submit();

    $(this).data('old-value', curValue);
});

/**
 * Manages visibility for filtration controls.
 */
function toggleFilterSelectors() {
    let chosenValue = parseInt($("#list-type-selector").val());
    let monthElem = $("#month-selector");
    let yearElem = $("#year-selector");

    switch (chosenValue) {
        case 0:
            monthElem.hide();
            yearElem.hide();
            break;
        case 1:
            monthElem.show();
            yearElem.hide();
            break;
        case 2:
            monthElem.hide();
            yearElem.show();
            break;
    }
}