interface IFormula {
    calculate(cantidad:number, largo:number, ancho: number,grueso:number,metros:number,decimales:number): number;
}

class  FFormulasService {
    public static CreateFormula(tipo: any): IFormula {

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
        else if(tipo===3)  {
            formula = new FormulaCantidad();
        }
        else if (tipo === 4) {
            formula = new FormulaTotal();
        }


        return formula;
    }
    
}

