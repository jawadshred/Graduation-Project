$("#my-menu-toggle").click( function (e) {
    e.preventDefault();
    $("#my-wrapper").toggleClass("menuDisplayed"); 
});


$(document).ready(function () {
    document.getElementById("my-sidebar-wrapper").style.height = (window.innerHeight - 60) + "px";

});

$(window).resize(function () {
    document.getElementById("my-sidebar-wrapper").style.height = (window.innerHeight- 60) + "px";

});
