/*
* TextFilter - jQuery plugin for key filtering input boxes
* 
* it's either works with regular expression or character list
* you pass filter expression as parameter or add an attribute named "textfilter" in your input type=text
* for prohibiting matched characters start with minus(-) or allowing matched ones start with plus(+)
* Example 1: jQuery("input[type=text]").textfilter('-[A-Za-z]') -- which prohibits letters
* Example 2: jQuery("input[type=text]").textfilter('+0123456789') -- which allows only numeric characters
* Example 3: <input type="text" id="textToBeFiltered" textfilter="+[0-9]">
*            jQuery("#textToBeFiltered").textfilter();
*
* PS : if you want to put double quote in input attribute user two single quote like this  textfilter="-''!'^+%&/()=?"
* 
* $Version: 2010.11.26 
* Copyright (c) 2010 Osman KO�
* osman.nuri.koc@gmail.com
*/
(function ($) {
    $.fn.textfilter = function (tfilter) {
        var isCtrl = false;
        var isPrevent = false;
        var isRegex = true;
        var regex;

        if (tfilter == undefined)
            tfilter = this.attr("textfilter");


        tfilter = tfilter.replace("''", "\"");
        if (tfilter.substr(0, 1) == '+') {
            isRegex = false;
            isPrevent = true;
            regex = tfilter.substr(1);
        } else if (tfilter.substr(0, 1) == '-') {
            isRegex = false;
            regex = tfilter.substr(1);
        } else {
            regex = tfilter;
        }


        function chekCharIsLegal(chr) {
            if (isRegex) {
                if (isPrevent) {
                    if (!chr.match(regex))
                        return false;
                } else {
                    if (chr.match(regex))
                        return false;
                }
            } else {
                if (isPrevent) {
                    if (regex.indexOf(chr) < 0)
                        return false;
                } else {
                    if (regex.indexOf(chr) >= 0)
                        return false;
                }
            }
            return true;
        }

        this.keypress(function (e) {
            var kCode = e.which;
            switch (kCode) {
                case 37: //left
                case 39: //right
                case 18: //tab
                case 8:  //backspace
                case 9:  //tab
                case 13: //return
                case 46: //del
                    return;
            }
            var chr = String.fromCharCode(kCode);
            if (!chekCharIsLegal(chr))
                e.preventDefault();
        })
                .keyup(function (e) {
                    var tValue = this.value;
                    var newVal = '';
                    for (i = 0; i < tValue.length; i++) {
                        var chr = tValue.substr(i, 1);
                        if (chekCharIsLegal(chr)) {
                            newVal += chr;
                        }
                    }
                    if (newVal != tValue)
                        this.value = newVal;
                });

    };
})(jQuery);