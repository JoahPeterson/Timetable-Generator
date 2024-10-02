

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

window.preventDefault = function (event) {
    event.preventDefault();
};


function SetSession(name, taskId) {
    localStorage.setItem(name, taskId);
}

function GetSession(name)
{
    return localStorage.getItem(name);
}