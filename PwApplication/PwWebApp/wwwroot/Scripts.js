$(document).ready(function () {

    var timerId;
    var tokenKey = "accessToken";

    // Авторизация
    $('#submitLogin').click(function (e) {
        e.preventDefault();
        var loginData = {
            grant_type: 'password',
            Email: $('#emailLogin').val(),
            Password: $('#passwordLogin').val()
        };
        $.ajax({
            type: 'POST',
            url: '/token',
            data: JSON.stringify(loginData),
            contentType: "application/json;charset=utf-8"
        }).success(function (data) {
            $('#unauthorizedUser').toggle();
            $('#authorizedUser').toggle();
            $('#errorText').empty();
            $('#errors').toggle();

            // Сохранение токена
            sessionStorage.setItem(tokenKey, data.access_token);

            // Запуск обновлений данных пользователя
            StartUpdations();

            // Загрузка списка получателей
            GetRecipientList();

        }).fail(function (data) {
            $('#errorText').text(data.responseText);
            $('#errors').toggle();//.css('display', 'block');
        });
    });

    // Выход из учетной записи
    $('#logOutBtn').click(function (e) {
        e.preventDefault();
        $('#unauthorizedUser').toggle();
        $('#authorizedUser').toggle();

        $('#recipientAdd').empty();
        $('#amountAdd').empty();

        $('#errors').toggle();
        $('#errorText').empty();

        // Удаление токена
        sessionStorage.removeItem(tokenKey);
        // Остановка таймера обновления информации о пользователе
        clearInterval(timerId);
    });

    // Кнопка Регистрация
    $('#registrationBtn').click(function (e) {
        $('#unauthorizedUser').toggle();
        $('#registrationForm').toggle();

        $('#errorText').empty();
        $('#errors').toggle();
    });

    // Регистрация
    $('#submitReg').click(function (e) {
        e.preventDefault();
        var regData =
        {
            UserName: $('#userNameReg').val(),
            Email: $('#emailReg').val(),
            Password: $('#passwordReg').val(),
            PasswordConfirm: $('#passwordConfirmReg').val()
        };
        $.ajax({
            type: 'POST',
            url: '/registration',
            data: JSON.stringify(regData),
            contentType: "application/json;charset=utf-8"
        }).success(function (data) {
            $('#registrationForm').toggle();
            $('#unauthorizedUser').toggle();

            $('#errorText').empty();
            $('#errors').toggle();
        }).fail(function (data) {
            console.log(data);
            $('#errorText').text(data.responseText);
            $('#errors').toggle();//.css('display', 'block');
        });
    });

    // Выход из регистрации
    $('#canselReg').click(function (e) {
        $('#registrationForm').toggle();
        $('#unauthorizedUser').toggle();

        $('#errorText').empty();
        $('#errors').toggle();
    });

    // Кнопка Добавление транзакции
    $("#addTransactionBtn").click(function (event) {
        event.preventDefault();
        AddTransaction();
    });

    // Переключение таймера при изменении фокуса окна
    window.onfocus = StartUpdations;
    window.onblur = StopUpdations;

    function StartUpdations() {
        // Обновление информации о балансе 1 раз в секунду
        timerId = setInterval(function () {
            GetUserInfo();
        }, 1000);
    };

    function StopUpdations() {
        window.clearInterval(timerId);
        alert("stopped");
    };


    // Загрузка информации о пользователе
    function GetUserInfo() {

        var oldBalance = $("#userBalance").text();

        $.ajax({
            url: '/userInfo',
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                var token = sessionStorage.getItem(tokenKey);
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (data) {
                $("#userName").text(data.userName);
                $("#userBalance").text(data.balance);
                // Если баланс изменился, обновляем список транзакций
                if (oldBalance != data.balance) {
                    GetTransactions();
                };
            },
            fail: function (data) {
                console.log(data);
            }
        });
    };

    // Загрузка списка транзакций пользователя
    function GetTransactions() {
        $('#viewTransactions').css('display', 'block');
        $.ajax({
            url: 'api/transactions',
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                var token = sessionStorage.getItem(tokenKey);
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (data) {
                WriteResponse(data);
            },
            fail: function (data) {
                console.log(data);
            }
        });
    };

    // Создание новой транзакции
    function AddTransaction() {
        // Получение id получателя, выбранного в списке
        var recipientName = $('#recipientAdd').val();
        var recipientId = $('#recipientList option').filter(function () {
            return this.value == recipientName;
        }).data('value');
        // Если введено несуществующее имя
        if (recipientId == null) {
            $('#errorText').text("Recipient not found!");
            $('#errors').toggle();//.css('display', 'block');
        }
        else {
            var transaction = {
                RecipientId: recipientId,
                Amount: $('#amountAdd').val()
            };
            $.ajax({
                url: '/api/transactions/',
                type: 'POST',
                data: JSON.stringify(transaction),
                contentType: "application/json;charset=utf-8",
                beforeSend: function (xhr) {
                    var token = sessionStorage.getItem(tokenKey);
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
                success: function () {
                    GetUserInfo();
                    GetTransactions();
                    $('#errorText').empty();
                    $('#errors').toggle();//.css('display', 'none');
                    $('#recipientAdd').empty();
                    $('#amountAdd').empty();
                },
                error: function (data) {
                    console.log(data);
                    $('#errorText').text(data.responseText);
                    $('#errors').toggle();//.css('display', 'block');
                }
            });
        }
    }

    // Вывод списка транзакций в таблицу
    function WriteResponse(transactions) {
        var strResult = "<table border='1'><th>Id</th><th>Date/Time</th><th>Correspondent</th><th>Amount</th><th>Type</th><th>Resulting balance</th>";
        $.each(transactions, function (index, transaction) {
            strResult += "<tr><td align='center'>" + transaction.transferId + "</td>";
            strResult += "<td align='left'>" + transaction.timestamp + "</td>";
            strResult += "<td align='center'>" + transaction.correspondent + "</td>";
            strResult += "<td align='center'>" + transaction.amount + "</td>";
            strResult += "<td align='center'>" + transaction.type + "</td>";
            strResult += "<td align='center'>" + transaction.resultingBalance + "</td></tr>";
        });
        strResult += "</table>";
        $("#viewTransactions").html(strResult);
    };

    // Загрузка списка получателей
    function GetRecipientList() {
        $.ajax({
            url: '/getRecipientList',
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                var token = sessionStorage.getItem(tokenKey);
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (data) {
                WriteRecipientList(data);
            },
            error: function () {
                alert("fail");
            }
        });
    };

    // Вывод списка получателей
    function WriteRecipientList(recipients) {
        var strResult;
        $.each(recipients, function (index, recipient) {
            strResult += "<option data-value=" + recipient.recipientId + " value=" + recipient.recipientName + "></option>";
        });
        $("#recipientList").html(strResult);
    };
});