var FNifValidatorService = (function () {
    function FNifValidatorService() {
        this._vectorValidators = {};
        this._vectorValidators = {};
        this._vectorValidators["070"] = new NifValidator_070();
    }
    FNifValidatorService.prototype.CreateNifValidator = function (paises) {
        var result = this._vectorValidators[paises];
        if (result != null) {
            return result;
        }
        else {
            return new NifValidator_Null();
        }
    };
    return FNifValidatorService;
}());
//# sourceMappingURL=NifValidationService.js.map