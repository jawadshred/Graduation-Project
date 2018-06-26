//save frd button clicked
$('body').on('click', '#save-button', savefrd);
function savefrd(event) {
    document.getElementById("FrdType").value = 1;

    return true;
}
