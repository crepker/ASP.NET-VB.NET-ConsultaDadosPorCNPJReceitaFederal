$(document).ready(function () {
        
    $(".cnpj").mask("99.999.999/9999-99", {placeholder: " ", autoclear:false});
    $(".cnpj").isValidCNPJ();
    $("input[id*='ibCNPJ']").prop('disabled', !$.ValidCNPJ);
    $(document).on("isCNPJValid", function(){$("input[id*='ibCNPJ']").prop('disabled', !$.ValidCNPJ)});

});