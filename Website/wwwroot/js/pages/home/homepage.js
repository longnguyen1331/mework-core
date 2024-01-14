(function ($) {
    "use strict";
    $(document).on('ready', function () {
        var provinceId = 36;
        var districtId = 0;
        var wardId = 0;

        $('.banner-items-carousel').owlCarousel({
            loop: true,
            nav: false,
            dots: false,
            autoplay: true,
            items: 1,
            navText: [
                "<i class='fa fa-angle-left'></i>",
                "<i class='fa fa-angle-right'></i>"
            ],
            animateIn: 'fadeIn',
            animateOut: 'fadeOut'
        });

        loadPostCategories();
        loadServices();
        loadServiceTypes();

        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });

        LoadProvinces();
        
        $('#province').select2().on('change', function (e) {
            provinceId = $('#province').val();
            districtId = 0;
            LoadDistricts(provinceId);
            LoadWards(districtId);
        });
        $('#district').select2().on('change', function (e) {
            districtId = $('#district').val();
            LoadWards(districtId);
        });
        $('#ward').select2().on('change', function (e) {
            wardId = $('#ward').val();
        });

        $('#btnLoc').on('click', function (e) {
            loadStatis(provinceId, districtId, wardId, $('#from-date').val(), $('#to-date').val());

        });

        loadStatis(provinceId, districtId, wardId, $('#from-date').val(), $('#to-date').val());

    });

})(jQuery); // End jQuery
function LoadProvinces() {
    $.ajax({
        type: "GET",
        url: "Home/GetProvinces",
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let data = JSON.parse(jqXHR.responseText);
            $.each(data, function (index, value) {
                if (data[index].selected) {

                    LoadDistricts(data[index].id);

                    $('#province').append(
                        '<option value="' + data[index].id + '" selected="selected" >' + data[index].ten + '</option>'
                    );
                } else {
                    $('#province').append(
                        '<option value="' + data[index].id + '"  disabled="' + data[index].disabled +'" >' + data[index].ten + '</option>'
                    );
                }
            });
        }
    });
}
function LoadDistricts(provinceId) {
    $('#district').empty();
    $('#ward').empty();

    $.ajax({
        type: "GET",
        url: "Home/GetDistricts?provinceId=" + provinceId,
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let data = JSON.parse(jqXHR.responseText);
            $.each(data, function (index, value) {
                if (data[index].selected) {
                    LoadWards(data[index].id);

                    $('#district').append(
                        '<option value="' + data[index].id + '" selected="selected" >' + data[index].ten + '</option>'
                    );
                } else {
                    $('#district').append(
                        '<option value="' + data[index].id + '" >' + data[index].ten + '</option>'
                    );
                }
            });
        }
    });
}
function LoadWards(districtId) {
    $('#ward').empty();

    $.ajax({
        type: "GET",
        url: "Home/GetWards?districtId=" + districtId,
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let data = JSON.parse(jqXHR.responseText);
            $.each(data, function (index, value) {
                if (data[index].selected) {
                    $('#ward').append(
                        '<option value="' + data[index].id + '" selected="selected" >' + data[index].ten + '</option>'
                    );
                } else {
                    $('#ward').append(
                        '<option value="' + data[index].id + '" >' + data[index].ten + '</option>'
                    );
                }
            });
        }
    });
}
function loadStatis(provinceId, districtId, wardId, fromDate, toDate) {
    $('.bieuDo').hide();
    $('.spinner-border').show();
    var paramsGetQuery = "";
    var fromDateValue ='';
    var toDateValue = '';
    if (fromDate != null && fromDate != '') {
        var listValue = fromDate.split('/');
        fromDateValue = listValue[2] + '-' + listValue[1] + '-' + listValue[0];
    }

    if (toDate != null && toDate != '') {
        var listValue = toDate.split('/');
        toDateValue = listValue[2] + '-' + listValue[1] + '-' + listValue[0];
    }

    if (provinceId != null && provinceId > 0)
    {
        paramsGetQuery += "idTinhThanh=" + provinceId;
    }
    if (districtId != null && districtId > 0) paramsGetQuery += (paramsGetQuery != '' ? "&" : "") + "idQuanHuyen=" + districtId;
    if (wardId != null && wardId > 0) paramsGetQuery += (paramsGetQuery != '' ? "&" : "") + "idXaPhuong=" + wardId;
    if (fromDateValue != null && fromDateValue != '')
        paramsGetQuery += (paramsGetQuery != '' ? "&" : "") + "fromDate=" + fromDateValue;
    if (toDateValue != null && toDateValue != '')
        paramsGetQuery += (paramsGetQuery != '' ? "&" : "") + "toDate=" + toDateValue;
        
    $.ajax({
        type: "GET",
        url: "Home/GetStatics?" + paramsGetQuery,
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let objectData = JSON.parse(jqXHR.responseText);
            console.log(objectData);
            if (objectData != null) {
                $('#coSoChanNuoiTrau').text("Cơ sở nuôi Trâu: " + objectData.objectThongKe.coSoChanNuoiTrau);
                $('#coSoChanNuoiVit').text("Cơ sở nuôi Vịt: " + objectData.objectThongKe.coSoChanNuoiVit);
                $('#coSoChanNuoiLon').text("Cơ sở nuôi Lợn: " + objectData.objectThongKe.coSoChanNuoiLon);
                $('#coSoChanNuoiGa').text("Cơ sở nuôi Gà: " + objectData.objectThongKe.coSoChanNuoiGa);
                $('#coSoChanNuoiBo').text("Cơ sở nuôi Bò: " + objectData.objectThongKe.coSoChanNuoiBo);

                loadTongDanGiaSuc(objectData.dataTongDanGiaSuc);
                loadTongDanGiaCam(objectData.dataTongDanGiaCam);
                loadSanLuongThitGiaSuc(objectData.dataSanLuongThitGiaSuc);
                loadSanLuongThitGiaCam(objectData.dataSanLuongThitGiaCam);
                loadSanLuongTrung(objectData.dataSanLuongTrung);
                loadSanLuongSua(objectData.dataSanLuongSua);
                loadSanLuongSanXuatThucAn(objectData.dataSanLuongSanXuatThucAn);
                loadSanLuongTieuThuThucAn(objectData.dataSanLuongTieuThuThucAn);
                loadDichBenh(objectData.dataDichBenh);
                loadTiemPhong(objectData.dataTiemPhong);
                loadThongKe(objectData.dataThongKe);
                $('.bieuDo').show();
                //loadThongKeChanNuoi(objectData.dataThongKeChanNuoi);
            } else {
                $('#coSoChanNuoiTrau').text("Cơ sở nuôi Trâu: 0");
                $('#coSoChanNuoiVit').text("Cơ sở nuôi Vịt: 0");
                $('#coSoChanNuoiLon').text("Cơ sở nuôi Lợn: 0");
                $('#coSoChanNuoiGa').text("Cơ sở nuôi Gà: 0");
                $('#coSoChanNuoiBo').text("Cơ sở nuôi Bò: 0");
            }
            
            $('.spinner-border').hide();
        }
    });
}
function loadDichBenh(arrays) {
    const xValues = arrays.labels;
    const yValues = arrays.data;
    const coloR = [];
    for (var i in xValues) {
        coloR.push(dynamicColors());
    }

    new Chart("dichBenhChart", {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: coloR,
                data: yValues
            }]
        },
        options: {
            title: {
                display: true,
                text: "Biểu đồ dịch bệnh"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadTiemPhong(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;
    const coloR = [];
    for (var i in xValues) {
        coloR.push(dynamicColors());
    }

    new Chart("tiemPhongChart", {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: coloR,
                data: yValues
            }]
        },
        options: {
         
            title: {
                display: true,
                text: "Biểu đồ tiêm phòng"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongSanXuatThucAn(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("sanLuongSanXuatThucAnChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                fill: false,
                lineTension: 0,
                backgroundColor: "rgba(0,0,255,1.0)",
                borderColor: "rgba(0,0,255,0.1)",
                data: yValues
            }]
        },
        options: {
            legend: { display: false },
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0, max: Math.max(...yValues) } }],
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongTieuThuThucAn(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("sanLuongTieuThuThucAnChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                fill: false,
                lineTension: 0,
                backgroundColor: "rgba(0,0,255,1.0)",
                borderColor: "rgba(0,0,255,0.1)",
                data: yValues
            }]
        },
        options: {
            legend: { display: false },
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return  value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return  value;
                            }
                        },
                        min: 0, max: Math.max(...yValues)
                    }
                }],
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongTrung(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("sanLuongTrungChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: "blue",
                data: yValues
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0 } }],
            },
            legend: { display: false },
            title: {
                display: true,
                text: "Biểu đồ sản lượng trứng"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongSua(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("sanLuongSuaChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: "blue",
                data: yValues
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0 } }],
            },
            legend: { display: false },
            title: {
                display: true,
                text: "Biểu đồ sản lượng sữa"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongThitGiaSuc(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;
    const coloR = [];
    for (var i in xValues) {
        coloR.push(dynamicColors());
    }

    new Chart("sanLuongThitGiaSucChart", {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: coloR,
                data: yValues
            }]
        },
        options: {
            title: {
                display: true,
                text: "Biểu đồ sản lượng thịt gia súc"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadSanLuongThitGiaCam(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;
    const coloR = [];
    for (var i in xValues) {
        coloR.push(dynamicColors());
    }

    new Chart("sanLuongThitGiaCamChart", {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: coloR,
                data: yValues
            }]
        },
        options: {
            title: {
                display: true,
                text: "Biểu đồ sản lượng thịt gia cầm"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadTongDanGiaCam(arrays) {
    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("tongDanGiaCamChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: "blue",
                data: yValues
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0 } }],
            },
            legend: { display: false },
            title: {
                display: true,
                text: "Biểu đồ tổng đàn gia cầm"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function loadTongDanGiaSuc(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;


    new Chart("tongDanGiaSucChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: "blue",
                data: yValues
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0 } }],
            },
            legend: { display: false },
            title: {
                display: true,
                text: "Biểu đồ tổng đàn gia súc"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}

function loadThongKe(arrays) {

    const xValues = arrays.labels;
    const yValues = arrays.data;

    new Chart("thongKeChart", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: "blue",
                data: yValues
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            } else {
                                return value;
                            }
                        },
                        min: 0 } }],
            },
            legend: { display: false },
            title: {
                display: true,
                text: "Biểu đồ thống kê"
            },
            tooltips: {
                callbacks: {
                    // this callback is used to create the tooltip label
                    label: function (tooltipItem, data) {
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                        return value;
                    }
                }
            }
        }
    });
}
function dynamicColors() {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgb(" + r + "," + g + "," + b + ")";
};

function loadServices() {
    let data = {
        isHighlight: true,
        take: 8
    };

    $('#highlightservicesPanel').empty()

    $.ajax({
        type: "POST",
        url: "Home/GetServices",
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let arrays = JSON.parse(jqXHR.responseText);
            $.map(arrays, (v, i) => {
                let charges = new Intl.NumberFormat(`en-US`).format(v.charges);
                let numberOfConsult = new Intl.NumberFormat(`en-US`).format(v.numberOfConsult);
                let numberOfTests = new Intl.NumberFormat(`en-US`).format(v.numberOfTests);

                $('#highlightservicesPanel').append(`
                    <div class="item">
                        <div class="service-container">
                            <div class="service-box-1">
                                <a href="/servicedetail/` + (v.slug != null && v.slug != '' ? v.slug : v.code) + `_pdt_` + v.id + `">
                                    <div class="service-thumb-img">
                                        <img alt="service" sizes="100vw" src="` + v.imageUrl + `" decoding="async" data-nimg="fill" class="imageBox" loading="lazy" style="position: absolute; height: 100%; width: 100%; inset: 0px; color: transparent;">
                                    </div>
                                </a>
                                <div class="service-box-1-body">
                                    <a href="/servicedetail/` + (v.slug != null && v.slug != '' ? v.slug : v.code) + `_pdt_` + v.id + `">
                                        <span class="service-box-1-content-title text-ellipses">` + v.name + `</span>
                                    </a>
                                    <a href="/servicedetail/` + (v.slug != null && v.slug != '' ? v.slug : v.code) + `_pdt_` + v.id + `">
                                        <span class="service-box-1-content-desc text-ellipses text-ellipses-3-line">` + v.description + `</span>
                                    </a>
                                    <p class="retail-price">
                                        Giá: <span class="price"> ` + charges + ` đ</span>
                                    </p>
                                    <p class="service-box-1-content-inline">
                                        <span>
                                            <img alt="user" src="/img/icon/icon_service_calendar.webp" width="20" height="20" decoding="async" data-nimg="1" loading="lazy" style="color: transparent;">` + numberOfConsult + ` lượt đăng ký
                                        </span>
                                        <span>
                                            <img alt="clock" src="/img/icon/icon_service_user.webp" width="20" height="20" decoding="async" data-nimg="1" loading="lazy" style="color: transparent;">` + v.minimunAge + (v.maxAge > 0 ? ' - ' + v.maxAge : '') + `
                                        </span>
                                    </p>
                                    <p class="service-box-1-content-inline">
                                        <span>
                                            <img alt="user" src="/img/icon/icon_service_cardiologist.webp" width="20" height="20" decoding="async" data-nimg="1" loading="lazy" style="color: transparent;"> xét nghiệm
                                        </span>
                                        <span>
                                            <img alt="clock" src="/img/icon/icon_service_document.webp" width="20" height="20" decoding="async" data-nimg="1" loading="lazy" style="color: transparent;">` + numberOfTests + ` hạng mục tư vấn
                                        </span>
                                    </p>
                                    <p class="retail-price-2 mt-3">
                                        Giá: <span class="price"> ` + charges + ` đ</span>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                `);
            });

            $('.highlight-services-items-carousel').owlCarousel({
                loop: true,
                nav: false,
                dots: true,
                autoplay: true,
                margin: 20,
                responsive: {
                    0: {
                        items: 1
                    },
                    600: {
                        items: 2
                    },
                    1000: {
                        items: 3
                    }
                },
                navText: [
                    "<i class='fa fa-angle-left'></i>",
                    "<i class='fa fa-angle-right'></i>"
                ],
            });

            $('.custom_prevService').on('click', function (e) {
                e.preventDefault();

                $('.highlight-services-items-carousel').trigger('prev.owl.carousel');
            });

            $('.custom_nextService').on('click', function (e) {
                e.preventDefault();

                $('.highlight-services-items-carousel').trigger('next.owl.carousel');
            });
        }
    });
}

function loadServiceTypes() {
    let data = {
        isHighlight: true,
        take: 10
    };

    $('#serviceTypesPanel').empty()

    $.ajax({
        type: "POST",
        url: "Home/GetServiceTypes",
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let arrays = JSON.parse(jqXHR.responseText);
            $.map(arrays, (v, i) => {
                $('#serviceTypesPanel').append(`
                    <div class="col-xs-6 col-sm-3 khung_dv">
                        <a href="/service/` + v.slug + `_p_` + v.id + `">
                            <div class="box_sl_dv">
                                <img src="` + v.imageUrl + `" alt="` + v.name + `">
                                <p>` + v.name + `</p>
                            </div>
                        </a>
                    </div>
                `);
            });
        }
    });
}



function loadPostCategories() {
    let data = {
        Ids: newsCategoryIds,
        take: 10
    };

    $('#newCategoriesPanel').empty()

    $.ajax({
        type: "POST",
        url: "Home/GetPostCatefories",
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let arrays = JSON.parse(jqXHR.responseText);
            $.map(arrays, (v, i) => {
                $('#newCategoriesPanel').append(`
                    <li class="news-tabs ` + (i == 0 ? 'active' : '') + `"><a class="p-0" data-toggle="tab" href="#tabNewsContent" onclick="loadPosts('` + v.id + `')" data-postCategoryId="` + v.id + `">` + v.name + `</a></li>
                `);
            });

            $('li.news-tabs.active>a').trigger('click');
        }
    });
}



function loadPosts(id) {
    $('#newsHighlight').empty();
    $('#newsOther').empty();

    let data = {
        postCategoryId: id,
        take: 4
    };

    $.ajax({
        type: "POST",
        url: "Home/GetPosts",
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let arrays = JSON.parse(jqXHR.responseText);
            $.map(arrays, (v, i) => {
                if (i == 0) {
                    $('#newsHighlight').append(`
                        <div class="news-highlight-thumbnail">
                            <img class="imageBox" src="` + v.pictureUrl + `" alt="news">
                            <div class="news-category-box">
                                <span class="news-category-content">` + v.postCategoryName + `</span>
                            </div>
                        </div>
                        <div class="news-item-box">
                            <div class="d-flex flex-column gap-3">
                                <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                    <span class="news-item-title text-ellipses text-ellipses-2-line">` + v.title + ` </span>
                                </a>
                                <div>
                                    <span class="news-item-content text-ellipses text-ellipses-2-line">` + v.sortDescription + ` </span>
                                </div>
                                <p class="d-flex gap-5 news-item-author">
                                    <span class="d-flex justify-content-center align-items-center">
                                        <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="Person2OutlinedIcon">
                                            <path d="M18.39 14.56C16.71 13.7 14.53 13 12 13s-4.71.7-6.39 1.56C4.61 15.07 4 16.1 4 17.22V20h16v-2.78c0-1.12-.61-2.15-1.61-2.66zM18 18H6v-.78c0-.38.2-.72.52-.88C7.71 15.73 9.63 15 12 15c2.37 0 4.29.73 5.48 1.34.32.16.52.5.52.88V18zm-8.22-6h4.44c1.21 0 2.14-1.06 1.98-2.26l-.32-2.45C15.57 5.39 13.92 4 12 4S8.43 5.39 8.12 7.29L7.8 9.74c-.16 1.2.77 2.26 1.98 2.26zm.32-4.41C10.26 6.67 11.06 6 12 6s1.74.67 1.9 1.59l.32 2.41H9.78l.32-2.41z"></path>
                                        </svg>` + v.posterFullName + `
                                    </span>
                                    <span class="d-flex justify-content-center align-items-center">
                                        <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="WatchLaterOutlinedIcon">
                                            <path d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm.5-13H11v6l5.2 3.2.8-1.3-4.5-2.7V7z"></path>
                                        </svg>` + v.postedDate + `
                                    </span>
                                </p>
                            </div>
                            <div class="d-flex justify-content-end mt-2">
                                <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                    <div class="btn-custom-outline news-btn-detail">
                                        <p class="d-fex gap-3 news-btn-detail-content">
                                            Chi tiết
                                            <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="AddIcon">
                                                <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"></path>
                                            </svg>
                                        </p>
                                    </div>
                                </a>
                            </div>
                        </div>
                    `);
                }
                else {
                    $('#newsOther').append(`
                    <div class="news-item gap-4">
                        <div class="news-item-thumbnail">
                            <img src="` + v.pictureUrl + `" alt="` + v.title + `" class="news-thumbnail-img">
                        </div>
                        <div class="d-flex flex-column gap-2">
                            <div>
                                <span class="news-item-category">` + v.postCategoryName + `</span>
                            </div>
                            <div class="d-flex flex-column gap-2">
                                <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                    <span class="news-item-title text-ellipses text-ellipses-2-line">` + v.title + `</span>
                                </a>
                                <p class="news-item-author">
                                    <span class="d-flex justify-content-start align-items-center">
                                        <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="WatchLaterOutlinedIcon">
                                            <path d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm.5-13H11v6l5.2 3.2.8-1.3-4.5-2.7V7z"></path>
                                        </svg>` + v.postedDate + `
                                    </span>
                                </p>
                            </div>
                        </div>
                    </div>
                    `);
                }
            })
        }
    });
}

function loadHealthNews() {
    $('#healthNewsHighlightPanel').empty();
    $('#healthNewsPanel').empty();

    let data = {
        postCategoryId: healthInfomationIds,
        take: 4
    };

    $.ajax({
        type: "POST",
        url: "Home/GetPosts",
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        complete: function (jqXHR, status) {
            let arrays = JSON.parse(jqXHR.responseText);
            $.map(arrays, (v, i) => {
                if (i == 0) {
                    $('#healthNewsHighlightPanel').append(`
                        <div class="horizontal-news-card-full-thumb">
                            <div class="news-card-full-thumbnail horizontal-news-card-full-thumb-box">
                                <img class="imageBox" src="` + v.pictureUrl + `" alt="news">
                                <div class="news-category-box">
                                    <span class="news-category-content">` + v.postCategoryName + `</span>
                                </div>
                            </div>
                        </div>
                        <div class="horizontal-news-item-box">
                            <div class="news-item-box p-0">
                                <div class="d-flex flex-column gap-4">
                                    <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                        <span class="news-item-title text-ellipses text-ellipses-2-line">` + v.title + ` </span>
                                    </a>
                                    <div>
                                        <span class="news-item-content text-ellipses text-ellipses-2-line">` + v.sortDescription + ` </span>
                                    </div>
                                    <p class="d-flex gap-5 news-item-author">
                                        <span class="d-flex justify-content-center align-items-center">
                                            <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="Person2OutlinedIcon">
                                                <path d="M18.39 14.56C16.71 13.7 14.53 13 12 13s-4.71.7-6.39 1.56C4.61 15.07 4 16.1 4 17.22V20h16v-2.78c0-1.12-.61-2.15-1.61-2.66zM18 18H6v-.78c0-.38.2-.72.52-.88C7.71 15.73 9.63 15 12 15c2.37 0 4.29.73 5.48 1.34.32.16.52.5.52.88V18zm-8.22-6h4.44c1.21 0 2.14-1.06 1.98-2.26l-.32-2.45C15.57 5.39 13.92 4 12 4S8.43 5.39 8.12 7.29L7.8 9.74c-.16 1.2.77 2.26 1.98 2.26zm.32-4.41C10.26 6.67 11.06 6 12 6s1.74.67 1.9 1.59l.32 2.41H9.78l.32-2.41z"></path>
                                            </svg>` + v.posterFullName + `
                                        </span>
                                        <span class="d-flex justify-content-center align-items-center">
                                            <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="WatchLaterOutlinedIcon">
                                                <path d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm.5-13H11v6l5.2 3.2.8-1.3-4.5-2.7V7z"></path>
                                            </svg>` + v.postedDate + `
                                        </span>
                                    </p>
                                </div>
                                <div class="d-flex justify-content-end mt-2">
                                    <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                        <div class="btn-custom-outline news-btn-detail">
                                            <p class="d-fex gap-3 news-btn-detail-content">
                                                Chi tiết
                                                <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="AddIcon">
                                                    <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"></path>
                                                </svg>
                                            </p>
                                        </div>
                                    </a>
                                </div>
                            </div>
                        </div>
                    `);
                }
                else {
                    $('#healthNewsPanel').append(`
                        <div class="item">
                            <div class="news-item flex-column">
                                <div class="news-item-thumbnail">
                                    <img src="` + v.pictureUrl + `" alt="` + v.title + `" class="news-thumbnail-img">
                                </div>
                                <div class="horizontal-news-body d-flex flex-column gap-2">
                                    <div>
                                        <span class="news-item-category">` + v.postCategoryName + `</span>
                                    </div>
                                    <div class="d-flex flex-column gap-4">
                                        <a href="/post/` + v.slug + `_dt_` + v.id + `">
                                            <span class="news-item-title text-ellipses text-ellipses-2-line">` + v.title + `</span>
                                        </a>
                                        <p class="news-item-author">
                                            <span class="d-flex justify-content-start align-items-center">
                                                <svg class="custom-svg-icon" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="WatchLaterOutlinedIcon">
                                                    <path d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm.5-13H11v6l5.2 3.2.8-1.3-4.5-2.7V7z"></path>
                                                </svg>` + v.postedDate + `
                                            </span>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `);
                }
            });

            $('.health-news-items-carousel').owlCarousel({
                loop: true,
                nav: false,
                dots: true,
                autoplay: true,
                margin: 20,
                responsive: {
                    0: {
                        items: 1
                    },
                    500: {
                        items: 2
                    },
                    900: {
                        items: 3
                    }
                },
                navText: [
                    "<i class='fa fa-angle-left'></i>",
                    "<i class='fa fa-angle-right'></i>"
                ],
            });
        }
    });
}


