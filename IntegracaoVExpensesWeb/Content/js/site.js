// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
    $.fn.showLoadingMessage = function (text) {
        return this.each(function () {
            $(this).html(`<div class="loading"><i class="fa fa-spinner fa-pulse"></i> <span>${text ?? 'Carregando...' }</span> </div>`);
        });
    };

    $(document).on('click', '.parentContent', function () {
        var $icon = $(this).find('.parentIcon svg');
        if ($icon.hasClass('fa-angle-right'))
            $icon.removeClass('fa-angle-right').addClass('fa-angle-down')
        else
            $icon.removeClass('fa-angle-down').addClass('fa-angle-right')
        $(this).next('.childContent').toggle();
    })

    $('body').on('click', '.monthBox', (e) => {
        var $icon = $(e.target).find('svg');
        if ($icon.hasClass('fa-angle-right'))
            $icon.removeClass('fa-angle-right').addClass('fa-angle-down')
        else
            $icon.removeClass('fa-angle-down').addClass('fa-angle-right')
        $(e.target).next('.monthContent').toggle();
    })


    $('body').on('click', '.boxTitle', (e) => {
        $(e.target).next('.boxContent').toggle('slow');
    })


    toastr.options = {
        "closeButton": true, // Mostra o botão de fechar
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000", // Tempo padrão de exibição (5 segundos)
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}(jQuery));

const Alert = (text, status) => {
    if (status) {
        toastr.options.timeOut = 5000; 
        toastr.options.extendedTimeOut = 1000;
        toastr.success(text)
    }
    else {
        toastr.options.timeOut = 0; // Manter indefinidamente
        toastr.options.extendedTimeOut = 0;
        toastr.error(text)
    }
}
