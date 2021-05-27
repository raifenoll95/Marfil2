var NifValidator_070 = (function () {
    function NifValidator_070() {
        this.DNI_REGEX = /^(\d{8})([A-Z])$/;
        this.CIF_REGEX = /^([ABCDEFGHJKLMNPQRSUVW])(\d{7})([0-9A-J])$/;
        this.NIE_REGEX = /^[XYZ]\d{7,8}[A-Z]$/;
    }
    NifValidator_070.prototype.SpainIdType = function (str) {
        str = str.toUpperCase().replace(/\s/, '').replace(/\-/, '').replace(/\-/, '');
        if (str.match(this.DNI_REGEX)) {
            return 'dni';
        }
        if (str.match(this.CIF_REGEX)) {
            return 'cif';
        }
        if (str.match(this.NIE_REGEX)) {
            return 'nie';
        }
    };
    ;
    NifValidator_070.prototype.ValidDNI = function (dni) {
        var dni_letters = "TRWAGMYFPDXBNJZSQVHLCKE";
        var letter = dni_letters.charAt(parseInt(dni, 10) % 23);
        return letter == dni.charAt(8);
    };
    ;
    NifValidator_070.prototype.ValidNIE = function (nie) {
        var nie_prefix = nie.charAt(0);
        switch (nie_prefix) {
            case 'X':
                nie_prefix = '0';
                break;
            case 'Y':
                nie_prefix = '1';
                break;
            case 'Z':
                nie_prefix = '2';
                break;
        }
        return this.ValidDNI(nie_prefix + nie.substr(1));
    };
    ;
    NifValidator_070.prototype.ValidCIF = function (cif) {
        var match = cif.match(this.CIF_REGEX);
        var letter = match[1], number = match[2], control = match[3];
        var even_sum = 0;
        var odd_sum = 0;
        var n;
        for (var i = 0; i < number.length; i++) {
            n = parseInt(number[i], 10);
            // Odd positions (Even index equals to odd position. i=0 equals first position)
            if (i % 2 === 0) {
                // Odd positions are multiplied first.
                n *= 2;
                // If the multiplication is bigger than 10 we need to adjust
                odd_sum += n < 10 ? n : n - 9;
            }
            else {
                even_sum += n;
            }
        }
        var control_digit = 0;
        if ((even_sum + odd_sum).toString().substr(-1) + 0 > 0)
            control_digit = (10 - (even_sum + odd_sum).toString().substr(-1));
        var control_letter = 'JABCDEFGHI'.substr(control_digit, 1);
        // Control must be a digit
        if (letter.match(/[ABEH]/)) {
            return control == control_digit;
        }
        else if (letter.match(/[KPQS]/)) {
            return control == control_letter;
        }
        else {
            return control == control_digit || control == control_letter;
        }
    };
    ;
    NifValidator_070.prototype.Validate = function (dni) {
        if (!dni || dni === "")
            return true;
        if (dni.substring(0, 2).match(/[a-zA-Z]{2}/)) {
            dni = dni.substring(2);
        }
        dni = dni.toUpperCase().replace(/\s/, '');
        var valid = false;
        var type = this.SpainIdType(dni);
        switch (type) {
            case 'dni':
                valid = this.ValidDNI(dni);
                break;
            case 'nie':
                valid = this.ValidNIE(dni);
                break;
            case 'cif':
                valid = this.ValidCIF(dni);
                break;
        }
        /*return {
            type: type,
            valid: valid
        };*/
        return valid;
    };
    return NifValidator_070;
}());
//# sourceMappingURL=NifValidator_070.js.map