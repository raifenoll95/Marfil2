/// <references path="Validators/NifValidator_070.ts" />
/// <references path="Validators/NifValidator_Null.ts" />
interface INifValidationService {
    Validate(dni: string);
}

class FNifValidatorService
{
    private _vectorValidators: { [id: string]: INifValidationService; } = {};

    constructor() {
        this._vectorValidators = {};
        this._vectorValidators["070"] = new NifValidator_070();
    }

    public CreateNifValidator(paises:string) {
        var result = this._vectorValidators[paises];
        if (result != null) {
            return result;
        } else {
            return new NifValidator_Null();
        }
    } 
}