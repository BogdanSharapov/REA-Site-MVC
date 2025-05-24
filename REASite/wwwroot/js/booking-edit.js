$(document).ready(function () {
    // Обработчик клика по кнопке редактирования бронирования
    $(document).on('click', '.edit-booking-btn', function () {
        var bookingId = $(this).data('booking-id');
        // Используем глобальную переменную, определённую на странице
        $.get(getEditBookingModalUrl, { bookingId: bookingId }, function (data) {
            $('#editBookingModal .modal-content').html(data);
            $('#editBookingModal').modal('show');
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error("Ошибка загрузки модального окна редактирования: " + textStatus);
        });
    });

    // Обработчик отправки формы редактирования бронирования
    $(document).on('submit', '#editBookingForm', function (e) {
        e.preventDefault();

        var startDate = new Date($('#StartDate').val());
        var endDate = new Date($('#EndDate').val());
        if (startDate >= endDate) {
            e.preventDefault();
            $('#errorContainer').html('<p>Дата начала должна быть раньше даты окончания.</p>').show();
            return;
        }

        // Формируем объект для отправки данных
        var bookingData = {
            BookingId: parseInt($('#editBookingForm input[name="BookingId"]').val()),
            StartDate: $('#StartDate').val(),
            EndDate: $('#EndDate').val()
        };
        $.ajax({
            type: 'POST',
            url: $('#editBookingForm').attr('action'),
            data: JSON.stringify(bookingData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (data) {
                if (data.success) {
                    $('#editBookingModal').modal('hide');
                    $.get(indexAccountUrl, function (result) {
                        var bookingsHtml = $(result).find('#bookings').html();
                        $('#bookings').html(bookingsHtml);
                    });
                } else {
                    var errorHtml = '<div class="alert alert-danger">';
                    // Если сервер возвращает массив ошибок, выполняем цикл:
                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            errorHtml += '<p>' + error + '</p>';
                        });
                    } else {
                        errorHtml += '<p>' + data.message + '</p>';
                    }
                    errorHtml += '</div>';
                    $('#editBookingModal .modal-body').prepend(errorHtml);
                }
            },
            error: function () {
                alert('Ошибка при обновлении бронирования.');
            }
        });
    });
});
