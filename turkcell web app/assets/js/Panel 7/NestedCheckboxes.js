var testing = document.querySelectorAll('.parent-checkbox');



//used for panel 7 checkboxes nesting and clicking
for (var n = 0; n < testing.length; n++) {
    for (var j = 0; j < testing[n].getElementsByTagName('li').length; j++) {
        testing[n].getElementsByTagName('li')[j].querySelector('.subOption').onclick = function () {
            for (var i = 0; i < testing.length; i++) {
                var checkedCount = testing[i].querySelectorAll('input.subOption:checked').length;
                var checkboxes2 = testing[i].querySelectorAll('input.subOption');
                testing[i].querySelectorAll('input')[0].checked = checkedCount > 0;
                testing[i].querySelector('input').indeterminate = checkedCount > 0 && checkedCount < checkboxes2.length;
            }
        }
    }
}



//used for panel 7 checkboxes: if suboption is clicked, click top
for (var n = 0; n < testing.length; n++) {
    for (var j = 0; j < testing[n].getElementsByTagName('li').length; j++) {
        testing[n].getElementsByTagName('li')[j].querySelector('.subOption').onclick = function () {

            for (var i = 0; i < testing.length; i++) {

                var checkedCount = testing[i].querySelectorAll('input.subOption:checked').length;
                var checkboxes2 = testing[i].querySelectorAll('input.subOption');
                testing[i].querySelectorAll('input')[0].checked = checkedCount > 0;
                testing[i].querySelector('input').indeterminate = checkedCount > 0 && checkedCount < checkboxes2.length;

            }
        }
    }
    testing[n].getElementsByTagName('li')[0].querySelector('.subOption').click();
    testing[n].getElementsByTagName('li')[0].querySelector('.subOption').click();

}

//for panel 7 checkboxes: if top is clicked, click suboptions
for (var n = 0; n < testing.length; n++) {

    testing[n].querySelector('.option').onclick = function () {
        for (var k = 0; k < this.closest(".parent-checkbox").querySelectorAll('input.subOption').length; k++) {
            this.closest(".parent-checkbox").querySelectorAll('input.subOption')[k].checked = this.checked;
        }
    }
}


for (var n = 0; n < testing.length; n++) {
    testing[n].querySelector('.option').onclick = function () {

        for (var k = 0; k < this.closest(".parent-checkbox").querySelectorAll('input.subOption').length; k++) {
            this.closest(".parent-checkbox").querySelectorAll('input.subOption')[k].checked = this.checked;
        }
    }
}