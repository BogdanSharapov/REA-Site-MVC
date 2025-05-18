$(document).ready(function () {
    $('.delete-image-btn').click(function () {
        var imageId = $(this).data('image-id');
        var apartmentId = $(this).data('apartment-id');
        var deleteUrl = $('#deleteImageUrl').val();
        $.ajax({
            url: deleteUrl,
            type: 'POST',
            data: { imageId: imageId, apartmentId: apartmentId },
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function () {
                $(`[data-image-id="${imageId}"]`).closest('.image-container').remove();
            },
            error: function () {
                alert('Ошибка при удалении изображения.');
            }
        });
    });
});