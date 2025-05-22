document.addEventListener('DOMContentLoaded', function () {
    const deleteModal = document.getElementById('deleteModal');
    const confirmDeleteBtn = document.getElementById('confirmDelete');

    document.querySelectorAll('.card').forEach(card => {
        card.addEventListener('click', function (event) {
            if (event.target.closest('.carousel') || event.target.closest('.btn')) {
                return;
            }
            const url = this.getAttribute('data-detail-url');
            if (url) {
                window.location.href = url;
            }
        });

        card.addEventListener('contextmenu', function (event) {
            event.preventDefault();
            const deleteUrl = this.getAttribute('data-delete-url');
            if (deleteUrl) {
                deleteModal.setAttribute('data-delete-url', deleteUrl);
                new bootstrap.Modal(deleteModal).show();
            }
        });
    });

    confirmDeleteBtn.addEventListener('click', async function () {
        const deleteUrl = deleteModal.getAttribute('data-delete-url');
        if (deleteUrl) {
            const response = await fetch(deleteUrl, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });
            if (response.ok) {
                const card = document.querySelector(`[data-delete-url="${deleteUrl}"]`);
                if (card) card.remove();
            } else {
                alert('Ошибка при удалении.');
            }
            bootstrap.Modal.getInstance(deleteModal).hide();
        }
    });
})
