function validateDates() {
    const departureDateInput = document.querySelector('input[name="DepartureDate"]');
    const returnDateInput = document.querySelector('input[name="ReturnDate"]');

    if (departureDateInput && returnDateInput) {
        returnDateInput.addEventListener('change', function () {
            const departureDate = new Date(departureDateInput.value);
            const returnDate = new Date(this.value);

            if (returnDate < departureDate) {
                alert('La fecha de regreso debe ser posterior a la fecha de ida');
                this.value = '';
            }
        });
    }
}

function validateCities() {
    const originSelect = document.querySelector('select[name="OriginCityId"]');
    const destinationSelect = document.querySelector('select[name="DestinationCityId"]');

    if (originSelect && destinationSelect) {
        const validateSelections = function () {
            if (originSelect.value !== '' && destinationSelect.value !== '' &&
                originSelect.value === destinationSelect.value) {
                alert('La ciudad de origen y destino no pueden ser la misma');
                destinationSelect.value = '';
            }
        };

        originSelect.addEventListener('change', validateSelections);
        destinationSelect.addEventListener('change', validateSelections);
    }
}

function addEntryAnimations() {
    document.addEventListener('DOMContentLoaded', function () {
        const elements = document.querySelectorAll('.card');

        elements.forEach(function (element) {
            element.classList.add('fade-in');
        });
    });
}

document.addEventListener('DOMContentLoaded', function () {
    validateDates();
    validateCities();
    addEntryAnimations();

    const searchForm = document.querySelector('form[action*="Search"]');
    if (searchForm) {
        searchForm.addEventListener('submit', function (event) {
            const originSelect = document.querySelector('select[name="OriginCityId"]');
            const destinationSelect = document.querySelector('select[name="DestinationCityId"]');

            if (originSelect.value === destinationSelect.value) {
                event.preventDefault();
                alert('La ciudad de origen y destino no pueden ser la misma');
                return false;
            }
        });
    }

    $('[data-toggle="tooltip"]').tooltip();
});

function formatCurrency(amount) {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0
    }).format(amount);
}

function updateBookingSummary() {
    const summaryElement = document.getElementById('booking-summary');
    if (summaryElement) {

    }
}