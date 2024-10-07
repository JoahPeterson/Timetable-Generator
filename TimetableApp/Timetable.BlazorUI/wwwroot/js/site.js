

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

window.preventDefault = function (event) {
    event.preventDefault();
};


window.initializeModal = () => {
    MicroModal.init();

    const closeModalButton = document.getElementById('closeModalButton');

    const btnName = document.getElementById('closeModalButton').getAttribute('name');
    

    window.modalInterop = {
        // Show the modal by its ID
        showModal: function (modalId) {
            MicroModal.show(modalId); // Show the modal
        },

        // Hide the modal by its ID
        hideModal: function (modalId) {
            MicroModal.close(modalId); // Hide the modal
        }
    };

    if (closeModalButton) {
        closeModalButton.addEventListener('click', function () {
            window.modalInterop.hideModal(btnName);
        });
    } else {
        console.warn('Modal button not found! Make sure the Blazor component has rendered.');
    }
};

function SetSession(name, taskId) {
    localStorage.setItem(name, taskId);
}

function GetSession(name)
{
    return localStorage.getItem(name);
}



