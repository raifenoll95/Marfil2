var ItemValidator = (function () {
    function ItemValidator(key, validator) {
        this.key = key;
        this.validator = validator;
    }
    return ItemValidator;
}());
var ValidatorAgregator = (function () {
    function ValidatorAgregator() {
        this._vectorValidators = [];
    }
    ValidatorAgregator.prototype.RegisterValidator = function (key, validator) {
        this._vectorValidators.push(new ItemValidator(key, validator));
    };
    ValidatorAgregator.prototype.Validate = function (key, data, object) {
        var vectorValidator = this._vectorValidators.filter(function (obj) {
            return obj.key === key;
        });
        if (vectorValidator.length > 0) {
            for (var i = 0; i < vectorValidator.length; i++)
                if (!vectorValidator[i].validator.IsValidObject(data, object))
                    return false;
        }
        return true;
    };
    return ValidatorAgregator;
}());
var validatorAgregator = new ValidatorAgregator();
//# sourceMappingURL=ValidatorAgregator.js.map