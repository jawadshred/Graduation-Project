//click on approve
document.querySelector('#man-approve').onclick = function () {
    document.querySelector('#FrdType').value = 3;
    document.querySelector('#FrdId').setAttribute("name", "Panel0.Id");
}