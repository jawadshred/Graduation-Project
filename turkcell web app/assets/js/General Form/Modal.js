// Get the modal
var modal = document.getElementById('myModal');
//var modal2 = document.getElementById('helpModal');


// Get the button that opens the modal
var btn = document.querySelector(".modal-button");
//var btn2 = document.querySelector("#modal-help");


// Get the <span> element that closes the modal
var span = modal.getElementsByClassName("close")[0];
//var span2 = modal2.getElementsByClassName("close")[0];


// When the user clicks on the button, open the modal 
btn.onclick = function () {
    modal.style.display = "block";
}

//btn2.onclick = function () {
//    modal2.style.display = "block";
//}


// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}
//span2.onclick = function () {
//    modal2.style.display = "none";
//}


// When the user clicks anywhere outside of the modal, close it
window.addEventListener('click', function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }

});
//window.onclick = function (event) {
//    if (event.target == modal) {
//        modal2.style.display = "none";
//    }
//}