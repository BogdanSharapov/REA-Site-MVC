document.addEventListener('DOMContentLoaded', function () {
    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : "";
    }

    let currentApartmentId = null;

    const deleteModalEl = document.getElementById('deleteModal');
    const confirmDeleteBtn = document.getElementById('confirmDelete');

    deleteModalEl.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget;
        if (button) {
            currentApartmentId = button.getAttribute('data-apartment-id');
        }
    });

    confirmDeleteBtn.addEventListener('click', function () {
        if (!currentApartmentId) return;

        const token = getAntiForgeryToken();

        fetch(deleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({ ApartmentId: parseInt(currentApartmentId) })
        })
            .then(response => {
                if (!response.ok) throw new Error("Ошибка сети");
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // Перезагрузка страницы при успешном удалении
                    window.location.reload();
                }
            })
            .catch(error => {
                console.error("Ошибка:", error);
                alert("Не удалось удалить объект");
            })
            .finally(() => {
                const modal = bootstrap.Modal.getInstance(deleteModalEl);
                if (modal) modal.hide();
            });
    });
});