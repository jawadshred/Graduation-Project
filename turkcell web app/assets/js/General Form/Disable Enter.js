//disable form enter key
$(document).keypress(
    function (event) {
        console.log('document key pressed fired');

        if (event.which === 13) {
            console.log('document key pressed fired and we are inside 13');
            event.preventDefault();
        }
    });
