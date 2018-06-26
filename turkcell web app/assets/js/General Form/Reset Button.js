//reset frd button clicked
$('body').on('click', '#man-reset', ResetFrd);
function ResetFrd(event) {
    document.getElementById("FrdType").value = 2;
    return true;
}
