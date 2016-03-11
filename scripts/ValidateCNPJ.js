(function ($) {
    var input, insText, base_Text, _invalidMessage;
    $.ValidCNPJ = false;
    $.fn.isValidCNPJ = function () {

        return this.each(function (invalidMessage) {

            input = $(this);
            _invalidMessage = invalidMessage ? invalidMessage : input.attr("invalidMessage") ? input.attr("invalidMessage") : "*";

            input.focusout(
                function () {
                    insText = input.val();
                    base_Text = insText.replace(/[^\d]+/g, '');
                    if (base_Text.length > 0) {
                        if (!validateCNPJ()) {
                            invalidCNPJ();
                        } else {
                            $.ValidCNPJ = true;
                            $(document).trigger("isCNPJValid");
                            input.next("span.invalidCNPJ").remove();
                        }
                    } else {
                        input.next("span.invalidCNPJ").remove();
                    }

                });
        });

        function validateCNPJ() {
            if (base_Text.split("").reduce(function (prev, curr) { return prev === curr ? prev : NaN }))
                return false;

            var tamanho = base_Text.length - 2;
            var numeros = base_Text.substring(0, tamanho)
            var dv = base_Text.substring(tamanho)
            var soma = 0;
            var pos = 9;
            var resultado = 0;

            for (i = tamanho; i >= 1; i--) {
                var a = numeros.charAt(i - 1);
                soma += numeros.charAt(i - 1) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11;
            if (resultado != dv.charAt(0))
                return false;

            tamanho = tamanho + 1;
            numeros = base_Text.substring(0, tamanho);
            soma = 0;
            pos = 9;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(i - 1) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11;
            if (resultado != dv.charAt(1))
                return false;


            return true;

        }

        function invalidCNPJ() {
            var _spanInvalid = input.next(".invalidCNPJ");
            if (_spanInvalid.attr("class") !== "invalidCNPJ") {
                input.after("<span title='" + _invalidMessage + "' class='invalidCNPJ'>*</span>");
            } else {
                _spanInvalid.fadeOut(300).fadeIn(300);
            }
            $.ValidCNPJ = false;
            $(document).trigger("isCNPJValid")

        }
    }
}(jQuery));