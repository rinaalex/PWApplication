$(document).ready(function () {

    var timer;
    var repeatUpdations = false;

    // Отправка авторизации
    $('#submitLogin').click(function (e) {
        e.preventDefault();
        $("#errors").hide();
        $("#errors").empty();        

        var loginData = {
            grant_type: 'password',
            Email: $('#emailLogin').val(),
            Password: $('#passwordLogin').val()
        };
        
        if (Login(loginData)) {
            // Очистка формы
            $('#loginForm')[0].reset();

            $('#unauthorizedUser').toggle();
            $('#authorizedUser').toggle();

            $('#errors').hide();
            $('#errors').empty();

            // Запуск обновлений данных пользователя
            repeatUpdations = true;
            StartUpdations();

            // Загрузка списка получателей
            GetRecipientList();
        }              
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

        Logout();

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

    // Отправка регистрации
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

        if (Registration(regData)) {
            alert("Registration completed successfully!");

            // Очистка формы
            $('#registrationForm')[0].reset();

            $('#registrationForm').toggle();
            $('#unauthorizedUser').toggle();
        }        
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

    // Авторизация
    function Login(loginData) {
        var result = false;
        $.ajax({
            type: 'POST',
            url: 'api/login',
            data: JSON.stringify(loginData),
            contentType: 'application/json;charset=utf-8',
            async: false
        }).success(function (data) {
            result = true;
        }).fail(function (data) {
            ShowValidationError(data);
            result = false;
        });
        return result;
    }

    function Logout() {
        $.ajax({
            type: 'GET',
            url: 'api/logout',
            beforeSend: function (xhr) {
                var token = get_cookie("token");
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            }
        }).error(function () {
            console.log("error");
        });
    }

    // Регистрация
    function Registration(regData) {
        var result = false;
        $.ajax({
            type: 'POST',
            url: 'api/registration',
            data: JSON.stringify(regData),
            contentType: 'application/json;charset=utf-8',
            async: false
        }).success(function (data) {
            result = true;
        }).fail(function (data) {
            result = false;
            ShowValidationError(data);
        });
        return result;
    }

    // Загрузка информации о пользователе
    function GetUserInfo() {

        var oldBalance = $("#userBalance").text();

        $.ajax({
            url: 'api/account',
            type: 'GET',
            dataType: 'json',
            async: false,
            beforeSend: function (xhr) {
                var token = get_cookie("token");
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
            error: function (data) {
                var errorcode = data.status;
                if (errorcode == "401") {
                    $('#unauthorizedUser').toggle();
                    $('#authorizedUser').toggle();
                    StopUpdations();
                    repeatUpdations = false;
                    ShowError("Session is over, you need to log in again.");
                }
                else
                    ShowValidationError(data);
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
                var token = get_cookie("token");
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
                    var token = get_cookie("token");
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
                success: function (data) {
                    GetUserInfo();
                    $("#tableTransactions tbody").prepend(row(data));
                    
                    $('#recipientAdd').empty();
                    $('#amountAdd').empty();
                },
                error: function (data) {
                    var errorcode = data.status;
                    if (errorcode == "401") {
                        $('#unauthorizedUser').toggle();
                        $('#authorizedUser').toggle();
                        StopUpdations();
                        repeatUpdations = false;
                        ShowError("Session is over, you need to log in again.");
                    }
                    else
                        ShowValidationError(data);
                }
            });
        }
    }

    // Вывод списка транзакций в таблицу
    function WriteResponse(transactions) {      
        var rows = "";
        $.each(transactions, function (index, transaction) {
            rows += row(transaction);
        });
        $("#tableTransactions tbody").append(rows);
    };

    // Загрузка списка получателей
    function GetRecipientList() {      
        $.ajax({
            url: '/api/account/recipients',
            type: 'GET',
            dataType: 'json',
            beforeSend: function (xhr) {
                var token = get_cookie("token");
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
        var token = document.cookie.match(new RegExp(
            "(?:^|; )" + "token".replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));
        if (token == null) {
            $('#unauthorizedUser').toggle();

            $('#errors').hide();
            $('#errors').empty();
        }
        else {
            $('#authorizedUser').toggle();

            // Запуск обновлений данных пользователя
            repeatUpdations = true;
            StartUpdations();

            // Загрузка списка получателей
            GetRecipientList();
        }        
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

    // Создание строки для таблицы
    var row = function (transaction) {
        return "<tr data-rowid='" + transaction.transferId + "'>" +
            "<td>" + transaction.transferId + "</td>" +
            "<td>" + transaction.timestamp + "</td>" +
            "<td>" + transaction.correspondent + "</td > " +
            "<td>" + transaction.amount + "</td>" + 
            "<td>" + transaction.type + "</td>" + 
            "<td>" + transaction.resultingBalance + "</td>" + 
            "<td><a id='copyBtn' data-id='" + transaction.transferId + "'>Copy</a></td></tr>";
    };

    function get_cookie(cookie_name) {
        var results = document.cookie.match('(^|;) ?' + cookie_name + '=([^;]*)(;|$)');
        if (results)
            return (unescape(results[2]));
        else
            return null;
    }

});