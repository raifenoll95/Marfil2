interface IValidator {
    IsValidObject(data: any,obj:any);
}

class ItemValidator {
    public key: string;
    public validator: IValidator;
    constructor(key: string, validator: IValidator) {
        this.key = key;
        this.validator = validator;

    }
}

class ValidatorAgregator {
    private _vectorValidators: ItemValidator[];

    constructor() {
        this._vectorValidators = [];
    }

    RegisterValidator(key: string, validator: IValidator) {

        this._vectorValidators.push(new ItemValidator(key,validator));
    }

    Validate(key: string,data:any, object: any) {
        var vectorValidator = this._vectorValidators.filter(function(obj) {
            return obj.key === key;
        });
        if (vectorValidator.length > 0) {
            for(var i=0;i<vectorValidator.length;i++)
                if (!vectorValidator[i].validator.IsValidObject(data,object))
                    return false;
        }

        return true;
    }
}

var validatorAgregator = new ValidatorAgregator();