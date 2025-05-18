document.addEventListener('DOMContentLoaded', function () {
    // Получаем элементы модального окна и кнопки подтверждения
    var modal = document.getElementById('deleteModal');
    var confirmButton = document.getElementById('confirmDelete');
    // Получаем CSRF-токен из скрытого поля
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Событие при открытии модального окна
    modal.addEventListener('show.bs.modal', function (event) {
        // Получаем кнопку, вызвавшую модальное окно
        var button = event.relatedTarget;
        // Извлекаем URL для удаления и тип объекта
        var deleteUrl = button.getAttribute('data-delete-url');
        var itemType = button.getAttribute('data-item-type');
        // Находим тело модального окна
        var modalBody = modal.querySelector('.modal-body');
        // Устанавливаем динамическое сообщение
        modalBody.textContent = `Вы уверены, что хотите удалить ${itemType === 'apartment' ? 'эту квартиру' : 'этот объект'}?`;
        // Сохраняем URL в кнопке подтверждения
        confirmButton.setAttribute('data-delete-url', deleteUrl);
    });

    // Событие при нажатии на кнопку "Удалить"
    confirmButton.addEventListener('click', function () {
        // Получаем URL для удаления
        var deleteUrl = this.getAttribute('data-delete-url');
        // Отправляем POST-запрос
        fetch(deleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({
                '__RequestVerificationToken': token
            }).toString()
        })
            .then(response => {
                if (response.ok) {
                    // Перезагружаем страницу при успешном удалении
                    location.reload();
                } else {
                    // Показываем сообщение об ошибке
                    alert('Не удалось удалить объект.');
                }
            })
            .catch(error => {
                // Логируем ошибку и показываем сообщение
                console.error('Ошибка:', error);
                alert('Произошла ошибка при удалении объекта.');
            });
    });
});