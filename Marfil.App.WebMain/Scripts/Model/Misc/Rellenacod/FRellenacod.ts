interface IRellenacodService {
    Formatea(codigo: string) : string;
}
enum TipoRellenacod {
    Generico,
    Letras,
    LetrasSimple
}
class FRellenacod {
    public CreateRellenacod(longitud: number, tipo: TipoRellenacod): IRellenacodService {
        
        if (tipo == TipoRellenacod.Generico) {
            return new RellenacodGeneric(longitud);
        }
        else if (tipo == TipoRellenacod.Letras) {
            return new RellenacodLetters(longitud);
        }
        else if (tipo == TipoRellenacod.LetrasSimple) 
        {
            return new RellenacodLettersSimple(longitud);
        }

        return new RellenacodGeneric(longitud);
    }
}