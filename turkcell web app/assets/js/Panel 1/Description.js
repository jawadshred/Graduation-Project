var firstTimeComment = true;

//click on add description
$('body').on('click', '.AddDescription', clickedAddDescription);
function clickedAddDescription() {


    var text = document.querySelector(".DescriptionText").value;

    if (!text) {
        document.querySelector(".DescriptionText").classList.add("input-validation-error");
        if (firstTimeComment) {
            document.querySelector("#task-main-controller .DescriptionText").insertAdjacentHTML('afterend', '<span class="error-message err0" style="display:inline;">Please type a description before adding</span>');
            firstTimeComment = false;
        }
        return;
    }
    firstTimeComment = true;
    document.querySelector(".DescriptionText").classList.remove("input-validation-error");
    try {
        document.querySelector("#task-main-controller .form-group").removeChild(document.querySelector(".err0"));
    } catch (e) { ; }
    

    document.querySelector(".DescriptionText").value = '';

    var markup = '<li class="list-group-item _task"><span class="_todo-text"> <input class="panel1-items" type="hidden" value="' + text + '" />' + text + '</span ><div class="btn-group pull-right _task-controls" role="group"><button class="removeButtonDescription btn btn-light _todo-remove" type="button" data-task-control="remove"> <i class="fa fa-trash" style="font-size:24px"></i></button></div><div class="clearfix"></div></li >';

    //DONT FORGET TO ADD A HIDDEN ELEMENT LATER ON FOR EACH COMMENT ADDED

    document.querySelector("#tags-list2").innerHTML += markup;
    
}


//remove description panel 1
$('._to-do-list-group').on('click', '.removeButtonDescription', function () {
    $(this).closest(".list-group-item").remove();
    //DONT FORGET TO REMOVE A HIDDEN ELEMENT LATER ON FOR EACH COMMENT REMOVED

});


//if user presses enter while focused on the description input
document.querySelector('.DescriptionText').addEventListener("keyup", enterPressedInput);
function enterPressedInput(event) {
    console.log('we entered the keyup method');
    if (event.keyCode === 13) {
        console.log('we entered the ENTER button condition');

        clickedAddDescription();
    }
}
