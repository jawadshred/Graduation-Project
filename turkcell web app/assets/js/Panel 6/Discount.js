//clicking on add discount
document.querySelector('.add-discount').onclick = clickedAddDiscount;
function clickedAddDiscount() {
    var discountName = document.querySelector('.discount-name').value
    var discountCode = document.querySelector('.discount-code').value

    //visibility
    if (!discountName) {
        document.querySelector(".discount-name").classList.add("input-validation-error");
        //first time discount keeps track of first time entering this function
        if ($('.err1').attr('style') === 'display:inline;visibility:hidden;') {
            document.querySelector(".err1").setAttribute('style', 'display:inline;visibility:visible;');
        }

    }
    if (!discountCode) {
        document.querySelector(".discount-code").classList.add("input-validation-error");
        //first time discount keeps track of first time entering this function
        if ($('.err2').attr('style') === 'display:inline;visibility:hidden;') {
            document.querySelector(".err2").setAttribute('style', 'display:inline;visibility:visible;');
        }
    }
    if (!discountName && !discountCode) {
        return;
    }
    if (!discountName) {
        document.querySelector(".discount-code").classList.remove("input-validation-error");
        try {
            document.querySelector(".err2").setAttribute('style', 'display:inline;visibility:hidden;');


        } catch (e) { ; }
        return;
    }

    if (!discountCode) {
        document.querySelector(".discount-name").classList.remove("input-validation-error");
        try {
            document.querySelector(".err1").setAttribute('style', 'display:inline;visibility:hidden;');

        } catch (e) { ; }
        return;
    }


    document.querySelector(".discount-name").classList.remove("input-validation-error");
    document.querySelector(".discount-code").classList.remove("input-validation-error");

    try {
        document.querySelector(".err1").setAttribute('style', 'display:inline;visibility:hidden;');

    } catch (e) { ; }

    try {
        document.querySelector(".err2").setAttribute('style', 'display:inline;visibility:hidden;');

    } catch (e) { ; }



    document.querySelector(".discount-name").value = '';
    document.querySelector(".discount-code").value = '';



    var markup = '<li class="list-group-item _task"><input class="panel6-name" type="hidden" value="' + discountName + '" /><input class="panel6-code" type="hidden" value="' + discountCode + '" /><div class="row"><div class="col align-self-center"><span class="_todo-text">Discount Name: ' + discountName + '</span></div> <div class="col align-self-center"><span class="_todo-text">Discount Code: ' + discountCode + ' </span></div> <div class="col"> <div class="btn-group pull-right _task-controls" role="group"><button class="removeButtonDiscount btn btn-light _todo-remove" type="button" data-task-control="remove"> <i class="fa fa-trash" style="font-size:24px;"></i></button></div> </div> </div> <div class="clearfix"></div> </li>';


    document.querySelector("#tags-list").innerHTML += markup;


}

//remove discount item trash clicked
$('.add-discount-place').on('click', '.removeButtonDiscount', function () {
    $(this).closest(".list-group-item").remove();

});


//if user presses enter while focused on the discounts inputs
function enterPressedDiscount(event) {
    console.log('we entered the press method');
    //if user presses enter while focused on the discounts inputs
    if (event.keyCode === 13) {
        console.log('we entered the ENTER button condition');

        clickedAddDiscount();
    }
}