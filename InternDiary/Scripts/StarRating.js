// Displaying readonly stars
var displayRating = document.getElementById("displayRating");
if (displayRating !== null) {
    displayRating.innerHTML = "\u2605".repeat(displayRating.innerHTML) + "\u2606".repeat(5 - displayRating.innerHTML);
}
// Displaying stars for Create and Edit
var starRatingControl = document.getElementById("starRatingControl");
if (starRatingControl !== null) {
    var star1 = document.createElement("span");
    star1.innerHTML = "☆";
    star1.classList.add("btn", "star");

    var star2 = document.createElement("span");
    star2.innerHTML = "☆";
    star2.classList.add("btn", "star");

    var star3 = document.createElement("span");
    star3.innerHTML = "☆";
    star3.classList.add("btn", "star");

    var star4 = document.createElement("span");
    star4.innerHTML = "☆";
    star4.classList.add("btn", "star");

    var star5 = document.createElement("span");
    star5.innerHTML = "☆";
    star5.classList.add("btn", "star");

    starRatingControl.append(star1, star2, star3, star4, star5);

    // Display stars if rating already has a value
    var ratingValue = document.getElementById("ratingValue");
    if (ratingValue.value > 0) {
        var stars = $(".star");
        for (var i = 0; i < ratingValue.value; i++) {
            stars[i].innerHTML = '★';
        }
    }
}

// When a star is selected
$(".star").click(function () {
    $("#ratingValue").val($(this).index() + 1);

    var selected = $(this).prevAll().addBack();
    for (var i = 0; i < selected.length; i++) {
        selected[i].innerHTML = '★';
    }

    var next = $(this).nextAll();
    for (var i = 0; i < next.length; i++) {
        next[i].innerHTML = '☆';
    }
});