interface IEventItem {
    controlId: string;
    callBack:Function;
}
class EventItem implements IEventItem {
    controlId: string;
    callBack: Function;

    constructor(controlId: string, callBack: Function) {
        this.controlId = controlId;
        this.callBack = callBack;
    }

    
}

class EventAgregator {
   
    private _vectorEvents: IEventItem[];

    constructor() {
        this._vectorEvents = [];
    }

    RegisterEvent(controlId: string, callBack: Function) {

        this._vectorEvents.push(new EventItem(controlId, callBack));
    }

    Publish(controlId: string, args: any) {

        var result = this._vectorEvents.filter(obj => (obj.controlId === controlId));
        if (result.length > 0) {
            result[0].callBack(args);
        }

    }
}

var eventAggregator = new EventAgregator();