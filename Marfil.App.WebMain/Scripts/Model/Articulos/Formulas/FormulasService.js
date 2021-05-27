var FFormulasService = (function () {
    function FFormulasService() {
    }
    FFormulasService.CreateFormula = function (tipo) {
        var formula;
        if (tipo === 0) {
            formula = new FormulaSuperficie();
        }
        else if (tipo === 1) {
            formula = new FormulaVolumen();
        }
        else if (tipo === 2) {
            formula = new FormulaLinear();
        }
        else if (tipo === 3) {
            formula = new FormulaCantidad();
        }
        else if (tipo === 4) {
            formula = new FormulaTotal();
        }
        return formula;
    };
    return FFormulasService;
}());
//# sourceMappingURL=FormulasService.js.map