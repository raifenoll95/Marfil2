class FormulaSuperficie implements IFormula {
    calculate(cantidad: number, largo: number, ancho: number, grueso: number,metros:number, decimales: number): number {
        var operador = decimales <= 0 ? 1 : Math.pow(10,decimales);
        return (Math.round(cantidad * largo * ancho * operador) / operador);
    }
}

class FormulaVolumen implements IFormula {
    calculate(cantidad: number, largo: number, ancho: number, grueso: number, metros: number, decimales: number): number {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * largo * ancho * grueso * operador) / operador);
    }
}

class FormulaLinear implements IFormula {
    calculate(cantidad: number, largo: number, ancho: number, grueso: number, metros: number, decimales: number): number {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * largo * operador) / operador);
    }
}

class FormulaCantidad implements IFormula {
    calculate(cantidad: number, largo: number, ancho: number, grueso: number, metros: number, decimales: number): number {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * operador) / operador);
    }
}

class FormulaTotal implements IFormula {
    calculate(cantidad: number, largo: number, ancho: number, grueso: number, metros: number, decimales: number): number {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(metros * operador) / operador);
    }
}