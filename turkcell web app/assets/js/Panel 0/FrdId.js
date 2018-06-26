var frdId = '';

//tooltip
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

//panel 0 when frd id is unfocused or enter is pressed, ajax is executed.
document.getElementById('id-input').addEventListener("keyup", enterPressedFrd);
document.getElementById('id-input').onblur = checkFrd;

function enterPressedFrd(event) {
    if (event.keyCode === 13) {
        console.log('we entered the ENTER button condition');

        checkFrd();
    }

}

function checkFrd() {

    //if user clicks on and clicks off and hasnt changed the input, checking wont occur
    if (frdId === document.getElementById('id-input').value) {
        return;
    }
    frdId = document.getElementById('id-input').value;


    var x = document.querySelector('.checkmark');
    x.setAttribute('style', 'visibility:visible;display:inline-block');

    if (!document.getElementById('id-input').value) {
        setTimeout(function () {
            document.querySelector('.checkmark-text').setAttribute('style', 'color:red;display:inline-block;visibility:visible;');
            document.querySelector('.checkmark-text').innerHTML = 'FRD id can not be empty';
            x.setAttribute('style', 'visibility:hidden;display:inline-block');
        }, 1000)
    }
    else {


        setTimeout(function () {
            var xhr = new XMLHttpRequest();

            xhr.open('GET', '../../../Api/FrdId?id=' + document.getElementById('id-input').value);
            xhr.onload = function () {
                if (xhr.responseText === 'true') {
                    $(".check").attr("class", "check check-complete");
                    $(".fill").attr("class", "fill fill-complete");



                    $(".check").attr("class", "check check-complete success");
                    $(".fill").attr("class", "fill fill-complete success");
                    $(".path").attr("class", "path path-complete");
                    document.querySelector('.checkmark-text').setAttribute('style', 'color:green;display:inline-block;visibility:visible;');
                    document.querySelector('.checkmark-text').innerHTML = 'FRD id is not used in the system';
                    setTimeout(function () {
                        $(".check").attr("class", "check");
                        $(".fill").attr("class", "fill");
                        $(".path").attr("class", "path");
                        x.setAttribute('style', 'visibility:hidden;display:inline-block');

                    }, 2000)


                }
                else if (xhr.responseText === 'false') {
                    x.setAttribute('style', 'visibility:hidden;display:inline-block');
                    document.querySelector('.checkmark-text').setAttribute('style', 'color:red;display:inline-block;visibility:visible;');
                    document.querySelector('.checkmark-text').innerHTML = 'FRD id is already used in the system';

                } else {
                    alert('ajax exception');
                }
            };
            xhr.send();
        }, 3000);
    }

}