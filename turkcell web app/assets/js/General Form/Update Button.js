
//click on update
document.querySelector('#man-update').onclick = function () {
    document.querySelector('#FrdType').value = 0;
    document.querySelector('#FrdId').setAttribute("name", "Panel0.Id");




    var indexPanel1 = 0;
    var panel1Edited = document.querySelectorAll('.edited')

    try {
    var panel1 = document.querySelectorAll('.panel1-items');
    
    for (; indexPanel1 < panel1.length; ++indexPanel1) {
        panel1[indexPanel1].setAttribute('name', 'Panel1.Items[' + indexPanel1 + '].Content');
    }
    } catch (e) {
        console.log('no panel 1 added in this page');
    }


    for (var j = 0; j < panel1Edited.length; j++ , indexPanel1++) {
        panel1Edited[j].previousElementSibling.setAttribute('name', 'Panel1.Items[' + indexPanel1 + '].itemId')
        for (var i = 0; i < panel1Edited[j].querySelectorAll('.new-comment').length; i++) {
            var inputs = panel1Edited[j].querySelectorAll('.new-comment')[i].querySelectorAll('input');

            inputs[0].setAttribute('name', 'Panel1.Items[' + indexPanel1 + '].Comments[' + i + '].Content');
            inputs[1].setAttribute('name', 'Panel1.Items[' + indexPanel1 + '].Comments[' + i + '].Date');
            inputs[2].setAttribute('name', 'Panel1.Items[' + indexPanel1 + '].Comments[' + i + '].Commentor.Name');
        }
    }

    var panel2 = document.querySelectorAll('.panel2checkboxes');
    console.log('panel2 length is is: ' + panel2.length)

    for (var k = 0, x = 0; k < panel2.length; ++k) {
        if (panel2[k].checked === true) {
            panel2[k].setAttribute('name', 'Panel2.Targets[' + x + '].Name');
            panel2[k].previousElementSibling.setAttribute('name', 'Panel2.Targets[' + x + '].Code');
            panel2[k].previousElementSibling.previousElementSibling.setAttribute('name', 'Panel2.Targets[' + x + '].Selected');
            panel2[k].previousElementSibling.previousElementSibling.setAttribute('value', 'True')
            x++
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

    //if (panel1.length==0 && ) {

    //}


}

