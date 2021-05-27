interface ILiteEvent<T> {
    on(handler: { (data?: T): void }): void;
}

class LiteEvent<T> implements ILiteEvent<T> {
    private handlers: { (data?: T): void; }[] = [];

    public on(handler: { (data?: T): void }) {
        this.handlers.push(handler);
    }

    public trigger(data?: T) {
        this.handlers.slice(0).forEach(h => h(data));
    }
}

class MainControllerAggregator {
    private _vector: string[];

    private onChange = new LiteEvent<void>();

    constructor() {
        this._vector = [];
    }

    public get OnChange(): ILiteEvent<void> { return this.onChange; }

    RegisterElement(elementId:string) {
        this._vector.push(elementId);
        this.onChange.trigger();
    }

    DisposeElement(elementId: string) {
        var index = this._vector.indexOf(elementId);
        if(index>=0)
            this._vector.splice(index, 1);

        this.onChange.trigger();
    }

    ExistsElement():boolean {
        return this._vector.length>0;
    }
}

var mainControllerAggregator = new MainControllerAggregator();