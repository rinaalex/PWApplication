$(document).ready(function () {

    var timer;
    var repeatUpdations = false;
    var tokenKey = "accessToken";

    // Авторизация
    $('#submitLogin').click(function (e) {
        e.preventDefault();
        $("#errors").hide();
        $("#errors").empty();        
        var loginData = {
            grant_type: 'password',
            Email: $('#emailLogin').val(),
            Password: $('#passwordLogin').val()
        };
        $.ajax({
            type: 'POST',
            url: 'api/login',
            data: JSON.stringify(loginData),
            contentType: 'application/json;charset=utf-8'
        }).success(function (data) {
            // Очистка формы
            $('#loginForm')[0].reset();

            $('#unauthorizedUser').toggle();
            $('#authorizedUser').toggle();    

            $('#errors').hide();
            $('#errors').empty();            

            // Сохранение токена
            sessionStorage.setItem(tokenKey, data.access_token);

            // Запуск обновлений данных пользователя
            repeatUpdations = true;
            StartUpdations();

            // Загрузка списка получателей
            GetRecipientList();          

        }).fail(function (data) {            
            ShowValidationError(data);
        });
    });

    // Выход из учетной записи
    $('#logOutBtn').click(function (e) {
        e.preventDefault();
        $('#unauthorizedUser').toggle();
        $('#authorizedUser').toggle();

        $('#recipientAdd').empty();
        $('#amountAdd').empty();

        $('#errors').hide();
        $('#errors').empty();

        // Удаление токена
        sessionStorage.removeItem(tokenKey);

        // Остановка таймера обновления информации о пользователе
        StopUpdations();
        repeatUpdations = false;
    });

    // Кнопка Регистрация
    $('#registrationBtn').click(function (e) {
        $('#unauthorizedUser').toggle();
        $('#registrationForm').toggle();

        $('#errors').hide();
        $('#errors').empty();        
    });

    // Регистрация
    $('#submitReg').click(function (e) {
        e.preventDefault();
        $('#errors').hide();
        $('#errors').empty();
        
        var regData =
        {
            UserName: $('#userNameReg').val(),
            Email: $('#emailReg').val(),
            Password: $('#passwordReg').val(),
            PasswordConfirm: $('#passwordConfirmReg').val()
        };
        $.ajax({
            type: 'POST',
            url: 'api/registration',
            data: JSON.stringify(regData),
            contentType: 'application/json;charset=utf-8'
        }).success(function (data) {
            alert("Registration completed successfully!");

            // Очистка формы
            $('#registrationForm')[0].reset();

            $('#registrationForm').toggle();
            $('#unauthorizedUser').toggle();            
        }).fail(function (data) {
            //console.log(data);
            ShowValidationError(data);
        });
    });

    // Выход из регистрации
    $('#canselReg').click(function (e) {
        $('#registrationForm').toggle();
        $('#unauthorizedUser').toggle();

        $('#errors').hide();
        $('#errors').empty();        
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
        if (repeatUpdations) {
            // Обновление информации о балансе 1 раз в секунду
            timer = setInterval(function () {
                GetUserInfo();
            }, 1000);
            console.log("started");
        } 
    };

    function StopUpdations() {
        if (repeatUpdations) {
            window.clearInterval(timer);
            console.log("stopped");
        }
    };

    // Загрузка информации о пользователе
    function GetUserInfo() {

        var oldBalance = $("#userBalance").text();

        $.ajax({
            url: 'api/account',
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
                //console.log(data);
                ShowError("Error loading user's information.");
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
                //console.log(data);
                ShowError("Error loading transaction list.")
            }
        });
    };

    // Создание новой транзакции
    function AddTransaction() {
        $('#errors').hide();
        $('#errors').empty();  

        // Получение id получателя, выбранного в списке
        var recipientName = $('#recipientAdd').val();
        var recipientId = $('#recipientList option').filter(function () {
            return this.value == recipientName;
        }).data('value');
        // Если введено несуществующее имя
        if (recipientId == null) {
            ShowError("Recipient not found!");
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
                    
                    $('#recipientAdd').empty();
                    $('#amountAdd').empty();
                },
                error: function (data) {
                    //console.log(data);
                    ShowValidationError(data);
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
                ShowError("Error loading recipient list.")
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

    window.onload = check;
    function check() {
        
    };

    // Вывод сообщения об ошибках
    function ShowError(errorText) {
        $("#errors").empty();
        $("#errors").append("<p>" + errorText + "</p>")
        $("#errors").show();
    };

    // Вывод сообщения об ошибках валидации
    function ShowValidationError(jxqr) {
        $("#errors").empty();
        if (jxqr.responseText === "") {
            $('#errors').append("<p>" + jxqr.statusText + "</p>");
        }
        var response = JSON.parse(jxqr.responseText);
        $.each(response, function (index, item) {
            $('#errors').append("<p>" + item + "</p>");
        });
        $("#errors").show();
    };
});