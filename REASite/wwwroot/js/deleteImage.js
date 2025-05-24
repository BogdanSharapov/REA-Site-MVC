document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.delete-image-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const imageId = this.dataset.imageId;
            const apartmentId = this.dataset.apartmentId;
            const imageContainer = this.closest('.image-container');

            if (!confirm('Вы уверены, что хотите удалить это изображение?')) return;

            fetch('/Home/DeleteImage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    imageId: imageId,
                    apartmentId: apartmentId
                })
            })
                .then(response => {
                    if (!response.ok) throw new Error('Ошибка удаления');
                    imageContainer.remove();
                })
                .catch(error => {
                    console.error(error);
                    alert(error.message);
                });
        });
    });
});