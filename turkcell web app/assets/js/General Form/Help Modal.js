// Get the modal
var modal2 = document.getElementById('helpModal');


// Get the button that opens the modal
var btn2 = document.querySelector("#modal-help");


// Get the <span> element that closes the modal
var span2 = modal2.getElementsByClassName("close")[0];


// When the user clicks on the button, open the modal 
btn2.onclick = function () {
    modal2.style.display = "block";
}


// When the user clicks on <span> (x), close the modal

span2.onclick = function () {
    modal2.style.display = "none";
}


// When the user clicks anywhere outside of the modal, close it
    window.addEventListener('click', function (event) {
        if (event.target == modal2) {
            modal2.style.display = "none";
        }

    });