document.querySelectorAll('.card').forEach(function (card) {
    card.addEventListener('click', function (e) {
        // Если клик произошёл не внутри элементов карусели, кнопок и т.п.
        if (!e.target.closest('.carousel, .btn, .favorite-button')) {
            const url = card.getAttribute('data-detail-url');
            if (url) {
                window.location.href = url;
            }
        }
    });
});