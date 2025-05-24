// Функция для получения антифорджери-токена (если он используется)
function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : "";
}

// Основная функция переключения избранного
function toggleFavorite(button, apartmentId) {
    // Получаем иконку из кнопки.
    const icon = button.querySelector("i");
    // Определяем состояние по наличию класса: если есть "bi-heart-fill", значит объект в избранном
    const isFavorite = icon.classList.contains("bi-heart-fill");
    // Выбираем URL в зависимости от текущего состояния:
    // Если объект в избранном, то нужно вызвать метод Remove, иначе Add.
    const url = isFavorite ? removeFavoriteUrl : addFavoriteUrl;
    const token = getAntiForgeryToken();

    // Отправляем AJAX-запрос с использованием Fetch API
    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": token
        },
        body: JSON.stringify({ apartmentId: apartmentId })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Ошибка сети при обновлении избранного.");
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Если объект был в избранном, удаляем класс "bi-heart-fill",
                // иначе добавляем его.
                if (isFavorite) {
                    icon.classList.replace("bi-heart-fill", "bi-heart");
                    // При желании можно обновить атрибут, если он используется где-либо ещё.
                    button.setAttribute("data-is-favorite", "false");
                } else {
                    icon.classList.replace("bi-heart", "bi-heart-fill");
                    button.setAttribute("data-is-favorite", "true");
                }
            } else {
                console.error("Ошибка обновления избранного:", data.message);
            }
        })
        .catch(error => console.error("Ошибка при обновлении избранного:", error));
}