(function ($) {
    "use strict";
    $(document).on('ready', function () {
        $('#bookAppointment').on('shown.bs.modal', function () {
            switch (typeBooking) {
                case 1:
                    loadBookingServiceDetail(serviceId);
                    break;
                default:
                    loadBookingServicePanel(0);
                    break;
            }
        });
        $('#bookAppointment').on('hidden.bs.modal', function () {
            $('#loadBookingAppointment').empty();
        });
    });
})(jQuery); // End jQuery

let doctorId = '';
let serviceId = '';
let typeBooking = 0;

function registerService(that) {
    let userId = $(that).data('userid');

    if (userId != null && userId != '') {
        typeBooking = 1;
        $('.register-service-date-of-week').empty();
        $('.register-service-working-time').empty();
        $('.doctor-list').empty();

        serviceId = $(that).data('serviceid');

        $('#bookAppointment').modal();
    }
    else {
        logon();
    }
}

function bookAppointment(that) {
    let userId = $(that).data('userid');
    doctorId = $(that).data('doctorid');
    $('.booking-appointment').empty();

    if (userId != null && userId != '') {
        typeBooking = 0;
        $('#bookAppointment').modal();
    }
    else {
        logon();
    }
}

function loadBookingServiceDetail(serviceId) {
    $.ajax({
        url: '/service/getServicesDetailForBookingAppointment/' + serviceId,
        type: 'GET',
        contentType: 'application/json',
        dataType: 'html',
        success: function (data) {
            if (data) {
                $('#loadBookingAppointment').append(data);
            }

            $('#loadBookingAppointment').find('div.modal-header').slideDown('fast');
            $('#loadBookingAppointment').find('div.modal-body').slideDown('slow');
            $('#loadBookingAppointment').find('div.modal-footer').slideDown('slow');

            loadListDoctors(serviceId);
        },
        failure: function (response) {
            toastNoti('error', '', response.responseText);
        },
        error: function (response) {
            toastNoti('error', '', response.responseText);
        }
    });
}

function loadListDoctors(serviceId) {
    $('.register-service .doctor-list').empty();
    let doctorsHTML = '';

    $.ajax({
        url: '/service/getDoctorByServiceId/' + serviceId,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            if (data) {
                $.each(data, (i, v) => {
                    doctorsHTML += `
                            <div class="register-doctor-item` + (v.doctorId == doctorId ? ' active' : '') + `" onclick="selectedDoctor(this)" data-doctorId="` + v.doctorId + `">
                                <div class="doctor-avatar">
                                    <img src="` + v.doctorAvatarUrl + `" alt="">
                                </div>
                                <span class="doctor-name">` + v.doctorName + `</span>
                            </div>
                        `;
                });

                $('.register-service .doctor-list').append(doctorsHTML);

                if (doctorId != null && doctorId != '') {
                    loadDoctorWorkingHours(doctorId);
                }
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

function loadDoctorWorkingHours(serviceId) {
    $('.register-service-date-of-week').empty();
    $('.register-service-working-time').empty();
    let workingDayHTML = '';

    $.ajax({
        url: '/service/getDoctorWorkingHour/' + serviceId,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            if (data) {
                let curr = new Date; // get current date
                let firstDay = getFirstDayOfWeek(curr)

                for (let i = 2; i <= 8; i++) {
                    const lastDay = new Date(firstDay);
                    lastDay.setDate(lastDay.getDate() + i - 2);

                    let tmp = data.find(x => x.daysOfWeek == i);
                    if (tmp != null) {
                        workingDayHTML += `
                            <div class="day-of-week-item"
                                data-startTime="` + tmp.startTime + `"
                                data-endTime="` + tmp.endTime + `"
                                data-slot="` + (tmp.slot > 0 ? tmp.slot : 30) + `"
                                onclick="selectedWorkingDay(this)">
                                <button class="btn-custom select-date-of-week" tabindex="0" type="button">
                                    <div class="d-flex flex-column"><span>` + (tmp.daysOfWeek == 1 || tmp.daysOfWeek == 8 ? "CN" : "TH " + tmp.daysOfWeek) + `</span><span class="date-time">` + lastDay.getDate() + `</span></div>
                                </button>
                            </div>
                        `;
                    }
                    else {
                        workingDayHTML += `
                            <div class="day-of-week-item"
                                data-startTime=""
                                data-endTime=""
                                data-slot=""
                                onclick="selectedWorkingDay(this)">
                                <button class="btn-custom select-date-of-week" tabindex="0" type="button">
                                    <div class="d-flex flex-column"><span>` + (i == 8 ? "CN" : "TH " + i) + `</span><span class="date-time">` + lastDay.getDate() + `</span></div>
                                </button>
                            </div>
                        `;
                    }
                }

                $('.register-service-date-of-week').append(workingDayHTML);
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

function selectedDoctor(that) {
    $('.register-service .register-doctor-item').removeClass('active');
    $(that).addClass('active');

    loadDoctorWorkingHours($(that).data('doctorid'));
}

function selectedWorkingDay(that) {
    let startTime = new Date($(that).data('starttime'));
    let endTime = new Date($(that).data('endtime'));
    let slot = parseInt($(that).data('slot'));

    $('.register-service-working-time').empty();

    $('.day-of-week-item').removeClass('active');
    $(that).addClass('active');

    let workingTime = '';

    while (startTime.getTime() - endTime.getTime() < 0) {
        workingTime += `
        <div class="working-time" onclick="selectedWorkingHour(this)" data-time="` + startTime + `">
            <button class="btn-custom select-working-time" tabindex="0" type="button">` + startTime.getHours().toString().padStart(2, '0') + ':' + startTime.getMinutes().toString().padStart(2, '0') + `</button>
        </div>`;

        startTime.setTime(startTime.getTime() + (slot * 60 * 1000));
    }

    $('.register-service-working-time').append(workingTime);
}

function selectedWorkingHour(that) {
    $('.working-time').removeClass('active');
    $(that).addClass('active');
}

function registerServiceNow(that) {
    let data = {
        patientId: $(that).data('userid'),
        DoctorId: $('.register-service .register-doctor-item.active').data('doctorid'),
        title: $(that).data('servicetitle'),
        totalAmount: parseFloat($(that).data('amount')),
        serviceIds: [$(that).data('serviceid')],
        paymentMethods: 0,
        paymentStatus: 0,
        status: 0,
        appointmentNotes: $('#registerServiceNote').val(),
    }

    $.ajax({
        url: '/Appointment/CreateAppointment',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (data) {
            if (data) {
                if (data.isSuccess) {
                    toastNoti('success', '', 'Đăng ký gói khám thành công.');
                    $('#bookAppointment').modal('hide');
                }
                else {
                    toastNoti('error', '', data.message);
                }
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
