(function ($) {
    'use strict';
    $(document).on('ready', function () {
        $(document).on('mouseup', function (e) {
            let container = $('.rightSidenav');
            let logonPanel = $('.logon-modal');

            // if the target of the click isn't the container nor a descendant of the container
            if (!container.is(e.target) && container.has(e.target).length === 0 && !logonPanel.is(e.target) && logonPanel.has(e.target).length === 0) {
                closeNav();
            }
        });

        $(document).on('click', function (e) {
            if (e.target.id != 'searchBox' && !e.target.matches('.search-button') && $('#searchBox').hasClass('active')) {
                $('#searchBox').removeClass('active');
            }
        });

        $('.btn-register').on('click', function (e) {
            e.preventDefault();

            if (!$('.account-register-info').is(':visible')) {
                register();
            }
            else {
                if (!registerValidate())
                    return;

                let data = {
                    userName: $('#usrname').val(),
                    firstName: 'First name',
                    lastName: 'Last name',
                    password: $('#pwd').val(),
                    passwordConfirm: $('#repwd').val(),
                    userCode: $('#usrname').val(),
                    gender: 1,
                    dOB: new Date(),
                    phoneNumber: $('#usrname').val(),
                    isActive: true,
                    provinceId: null,
                    districtId: null,
                    address: null,
                };

                $.ajax({
                    url: '/patient/sign-up',
                    type: 'POST',
                    data: JSON.stringify(data),
                    contentType: 'application/json',
                    success: function (data) {
                        if (data.message != null && data.message != '') {
                            toastNoti('error', '', data.message);
                        } else {
                            toastNoti('success', '', 'Đăng ký thành công');

                            window.location.href = '/patient/thong-tin-ca-nhan';
                        }
                    },
                    failure: function (response) {
                        toastNoti('error', '', response.responseText);
                    },
                    error: function (response) {
                        toastNoti('error', '', response.responseText);
                    }
                });
            }
        });

        $('.btn-login').on('click', function (e) {
            e.preventDefault();

            var navigator_info = window.navigator;
            var screen_info = window.screen;
            var uid = navigator_info.mimeTypes.length;
            uid += navigator_info.userAgent.replace(/\D+/g, '');
            uid += navigator_info.plugins.length;
            uid += screen_info.height || '';
            uid += screen_info.width || '';
            uid += screen_info.pixelDepth || '';
            window.localStorage.setItem('my-deviceid', uid);

            if (!$('.pwd-input').is(':visible')) {
                $('.pwd-input').show()
            }
            else {
                let data = {
                    UserName: $('#usrname').val(),
                    Password: $('#pwd').val(),
                    DeviceId: getUid()
                }

                $.ajax({
                    url: '/auth/sign-in',
                    type: 'POST',
                    data: JSON.stringify(data),
                    contentType: 'application/json',
                    success: function (data) {
                        window.location.reload();
                    },
                    failure: function (response) {
                        toastNoti('error', '', response.responseText);
                    },
                    error: function (response) {
                        toastNoti('error', '', response.responseText);
                    }
                });
            }
        });

        $('.logon-modal').on('show.bs.modal', function () {
            $('.logon-form')[0].reset();
        });
    });

    function registerValidate() {
        if ($('#usrname').val() == '') {
            $('#usrname').focus();
            return false;
        }

        if ($('#pwd').val() == '' || $('#pwd').val().length < 6) {
            $('#pwd').focus();
            return false;
        }

        if ($('#repwd').val() == '' || $('#repwd').val().length < 6) {
            $('#repwd').focus();
            return false;
        }
        return (true);
    }
})(jQuery); // End jQuery

function openSearchBox() {
    console.log('a');
    if ($('#searchBox').hasClass('active')) {
        window.location.href = '/postCategory?keyword=' + $('#searchBox').val();
    }

    if (!$('#searchBox').hasClass('active'))
        $('#searchBox').addClass('active');
}

function search() {
    window.location.href = '/postCategory?keyword=' + $('#sidenav-search-keyword').val();
}

function openNav() {
    $('body').css('overflow-y', 'hidden');
    if (!$('#mobileMenuSidenav').hasClass('opened'))
        $('#mobileMenuSidenav').addClass('opened');
}

function closeNav() {
    $('body').css('overflow-y', 'auto');
    $('#mobileMenuSidenav').removeClass('opened');
}

function logon() {
    $('.logon-title').text('Đăng nhập');
    $('.btn-login').show();
    $('.logon-form').show();
    $('.logon-noti').hide();
    $('.account-register-info').hide();

    $('.logon-modal').modal();
}

function signOut() {

    $.ajax({
        url: '/auth/sign-out',
        type: 'POST',
        contentType: 'application/json',
        success: function (data) {
            window.location.href = '/';
        },
        failure: function (response) {
            toastNoti('error', '', response.responseText);
        },
        error: function (response) {
            toastNoti('error', '', response.responseText);
        }
    });
}

function register() {
    $('.logon-title').text('Đăng ký');
    $('.btn-login').hide();
    $('.account-register-info').show();
    $('.logon-modal').modal();
}

function validPassword(that) {
    let parentElm = $(that).parents('.password-input');
    let errorMess = '';
    let isShowError = false;
    if (!isShowError && $(that).val() == '') {
        errorMess = 'Nhập mật khẩu.';
        isShowError = true;
    }
    if (!isShowError && $(that).val().length < 6) {
        errorMess = 'Mật khẩu phải có ít nhất 6 ký tự.';
        isShowError = true;
    }
    if (!isShowError && !(/^[A-Za-z0-9_@@.&amp;!#$%^*]*$/.test($(that).val()))) {
        errorMess = 'Mật khẩu có chứa ký tự không hợp lệ.';
        isShowError = true;
    }
    if ($('.account-register-info').is(':visible')
        && !isShowError
        && $('#repwd').val() != ''
        && $('#pwd').val() != ''
        && $('#repwd').val() != $('#pwd').val()) {
        errorMess = 'Mật khẩu không khớp.';
        isShowError = true;
    }
    else {
        if ($('#repwd').val() != '')
            $('#repwd').parents('.password-input').removeClass('error');
        if ($('#pwd').val() != '')
            $('#pwd').parents('.password-input').removeClass('error');
    }

    if (isShowError) {
        parentElm.addClass('error');
        parentElm.find('.error-message').text(errorMess);
    }
    else {
        parentElm.removeClass('error');
    }
}


function getUid() {
    var navigator_info = window.navigator;
    var screen_info = window.screen;
    var uid = navigator_info.mimeTypes.length;
    uid += navigator_info.userAgent.replace(/\D+/g, '');
    uid += navigator_info.plugins.length;
    uid += screen_info.height || '';
    uid += screen_info.width || '';
    uid += screen_info.pixelDepth || '';
    window.localStorage.setItem('my-deviceid', uid);
    return uid;
}
