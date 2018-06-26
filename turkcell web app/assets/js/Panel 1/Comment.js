var id = 0;

//panel1 clicked on expand comment changes icon arrows
function expandComments(i) {
    if ($('#icon--' + i + '').attr('class') === 'fa fa-angle-double-down') {

        $('#icon--' + i + '')[0].setAttribute('class', 'fa fa-angle-double-up');
    }
    else {
        $('#icon--' + i + '')[0].setAttribute('class', 'fa fa-angle-double-down');
    }
}



//add comment panel1
function addCommentPressed(index, currentUser) {


    //get value of the input field
    var commentContent = document.querySelector('.input-comment-' + index).value;


    if (!commentContent) {

        document.querySelector(".input-comment-" + index).classList.add("input-validation-error");

        return;
    }

    document.querySelector(".input-comment-" + index).classList.remove("input-validation-error");




    document.querySelector(".input-comment-" + index).value = '';

    // var markup = '<li class="list-group-item _task"><span class="_todo-text"> <input class="panel1checkboxes" type="hidden" value="' + text + '" />' + text + '</span ><div class="btn-group pull-right _task-controls" role="group"><button class="removeButtonDescription btn btn-light _todo-remove" type="button" data-task-control="remove"> <i class="fa fa-trash" style="font-size:24px"></i></button></div><div class="clearfix"></div></li >';

    //DONT FORGET TO ADD A HIDDEN ELEMENT LATER ON FOR EACH COMMENT ADDED

    //document.querySelector("#task-list").innerHTML += markup;


    var currentDate = moment(new Date()).format('DD/M/YYYY h:mm:ss A');


    //part where actual comment is added

    document.querySelector('.list-comments-' + index).classList.add("edited");

    document.querySelector('.list-comments-' + index).innerHTML +=
        `<li id="commentid-` + index + `-` + (id + 1) + `" class="list-group-item new-comment comment" style="margin-bottom:6px;">
        <input type="hidden" value="`+ commentContent + `">
         <input type="hidden" value="`+ currentDate + `">
        <input type="hidden" value="`+ currentUser + `">
        <div class="media">
        <div class="media-body"><div class="media" style="overflow:visible;">
        <div style="margin-right:10px;"><i class="fa fa-user" style="font-size:16px;">
        </i></div><div class="media-body" style="overflow:visible;"><div class="row"><div class="col-md-12"><p><a href="#">` + currentUser + `: 
        </a>` + commentContent + `<br><small class="text-muted">` + currentDate + `</small></p></div></div></div></div></div>
        <button class="btn btn-light _todo-remove" onclick="RemoveComment(` + index + `,` + (id + 1) + `)" type="button" data-task-control="remove"> <i class="fa fa-trash"></i></button></div></li>`;
    id++;
    if (document.querySelectorAll('.list-comments-' + index + ' .comment').length === 1 && document.querySelector('.list-comments-' + index + ' #no-comments') !== null) {
        
        document.querySelector('.list-comments-' + index + ' #no-comments').setAttribute('style', 'margin-bottom:6px;display:none;');
       
    }
}



//if user presses enter while focused on the comment input
function enterPressedComment(event, index, currentUser) {
    console.log('we entered the keyup method');
    if (event.keyCode === 13) {
        console.log('we entered the ENTER button condition');
        event.preventDefault();

        addCommentPressed(index, currentUser);
    }
}



//clicked on remove comment
function RemoveComment(indexItem, id) {
    var comment = document.querySelectorAll('.list-comments-' + indexItem + ' .new-comment')
    if (comment.length === 1 && document.querySelector('.list-comments-' + indexItem + ' #no-comments') !== null) {

        document.querySelector('.list-comments-' + indexItem).classList.remove("edited");
        document.querySelector('#commentid-' + indexItem + '-' + id).remove();
        document.querySelector('.list-comments-' + indexItem + ' #no-comments').setAttribute('style', 'margin-bottom:6px;');

    }
    else {
        document.querySelector('#commentid-' + indexItem + '-' + id).remove();
    }
    //if (document.querySelectorAll('.list-comments-' + indexItem + ' .comment').length === 0 && document.querySelector('.list-comments-' + indexItem + ' #no-comments') !== null) {
    //    document.querySelector('.list-comments-' + indexItem + ' #no-comments').setAttribute('style', 'margin-bottom:6px;');
    //}
}