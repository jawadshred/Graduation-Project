////submit clicked
//$('body').on('click', '#submit-button', SubmitFrd);
//function SubmitFrd(event) {
//    document.getElementById("FrdType").value = 0;
//    return true;
//}

//when submit is clicked
function submitFun(event) {
    console.log('submit has been clicked');
    var typeOfButton = event.currentTarget.getAttribute('id');
    if (!confirm('Are you sure you want to ' + (typeOfButton === 'submit-button' ? 'submit' : 'save to draft') + ' this form?')) {
        event.preventDefault();
        return;
    }

    document.getElementById("FrdType").value = 0;


    var panel1 = document.querySelectorAll('.panel1-items');

    for (var i = 0; i < panel1.length; ++i) {
        panel1[i].setAttribute('name', 'Panel1.Items[' + i + '].Content');
    }









    var panel2 = document.querySelectorAll('.panel2checkboxes');
    console.log('panel2 length is is: ' + panel2.length)

    for (var k = 0, j = 0; k < panel2.length; ++k) {
        if (panel2[k].checked === true) {
            panel2[k].setAttribute('name', 'Panel2.Targets[' + j + '].Name');
            panel2[k].previousElementSibling.setAttribute('name', 'Panel2.Targets[' + j + '].Code');
            panel2[k].previousElementSibling.previousElementSibling.setAttribute('name', 'Panel2.Targets[' + j + '].Selected');
            panel2[k].previousElementSibling.previousElementSibling.setAttribute('value', 'True')
            j++
        }
    }



    var panel3 = document.querySelectorAll('.panel3checkboxes');

    for (var z = 0, v = 0; z < panel3.length; ++z) {
        if (panel3[z].checked === true) {
            panel3[z].setAttribute('name', 'Panel3.Channels[' + v + '].Name');
            panel3[z].previousElementSibling.setAttribute('name', 'Panel3.Channels[' + v + '].Code');
            panel3[z].previousElementSibling.previousElementSibling.setAttribute('name', 'Panel3.Channels[' + v + '].Selected');
            panel3[z].previousElementSibling.previousElementSibling.setAttribute('value', 'True')

            v++
        }
    }


    var panel6Name = document.querySelectorAll('.panel6-name');
    var panel6Code = document.querySelectorAll('.panel6-code');


    for (var u = 0; u < panel6Name.length; ++u) {
        panel6Name[u].setAttribute('name', 'Panel6.DiscountItems[' + u + '].DiscountName');
        panel6Code[u].setAttribute('name', 'Panel6.DiscountItems[' + u + '].DiscountCode');
    }



    var panel7Parents = document.querySelectorAll('.panel7Parentcheckboxes');
    var panel7ParentsHidden = document.querySelectorAll('.panel7ParentcheckboxesHidden');




    for (var n = 0, b = 0; n < panel7Parents.length; ++n) {
        if (panel7Parents[n].checked === true) {
            panel7Parents[n].setAttribute('name', 'Panel7.DistributionGroups[' + b + '].Id');
            panel7ParentsHidden[n].setAttribute('name', 'Panel7.DistributionGroups[' + b + '].Name');

            var panel7 = panel7Parents[n].parentElement.parentElement.querySelectorAll('.panel7checkboxes');
            for (var nn = 0, bb = 0; nn < panel7.length; ++nn) {
                if (panel7[nn].checked === true) {
                    panel7[nn].setAttribute('name', 'Panel7.DistributionGroups[' + b + '].Employess[' + bb + '].Id');
                    panel7[nn].nextElementSibling.setAttribute('name', 'Panel7.DistributionGroups[' + b + '].Employess[' + bb + '].Selected');
                    panel7[nn].nextElementSibling.setAttribute('value', 'true');
                    ++bb
                }

            }
            ++b
        }

    }



    //checking 

    if (typeOfButton === 'submit-button') {

        if (document.querySelector('#id-input').value.length === 0 || document.querySelector('.checkmark-text').getAttribute('style') === 'color:red;display:inline-block;visibility:visible;') {
            document.querySelector('#er3').setAttribute('style', 'color:red;display:block;')
        }
        else {
            document.querySelector('#er3').setAttribute('style', 'color:red;display:none;')
        }

        if (document.querySelector('#name-input').value.length === 0) {
            document.querySelector('#er4').setAttribute('style', 'color:red;display:block;')
        }
        else {
            document.querySelector('#er4').setAttribute('style', 'color:red;display:none;')
        }


        if (panel1.length === 0) {
            document.querySelector('#er1').setAttribute('style', 'color:red;display:block;')
        }
        else {
            document.querySelector('#er1').setAttribute('style', 'color:red;display:none;')
        }

        var panel7subs = document.querySelectorAll('.panel7checkboxes');
        for (var p = 0; p < panel7subs.length; p++) {
            if (panel7subs[p].checked === true) {
                break;
            }
            if (p === panel7subs.length - 1) {
                document.querySelector('#er2').setAttribute('style', 'color:red;display:block;')
                event.preventDefault();
                return;
            }
        }


        document.querySelector('#er2').setAttribute('style', 'color:red;display:none;')
        if (panel1.length === 0) {
            event.preventDefault();
            return;
        }

        if (document.querySelector('.checkmark-text').getAttribute('style') === 'color:red;display:inline-block;visibility:visible;') {
            event.preventDefault();
            return;
        }
    }




}




