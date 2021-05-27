var EventItem = (function () {
    function EventItem(controlId, callBack) {
        this.controlId = controlId;
        this.callBack = callBack;
    }
    return EventItem;
}());
var EventAgregator = (function () {
    function EventAgregator() {
        this._vectorEvents = [];
    }
    EventAgregator.prototype.RegisterEvent = function (controlId, callBack) {
        this._vectorEvents.push(new EventItem(controlId, callBack));
    };
    EventAgregator.prototype.Publish = function (controlId, args) {
        var result = this._vectorEvents.filter(function (obj) { return (obj.controlId === controlId); });
        if (result.length > 0) {
            result[0].callBack(args);
        }
    };
    return EventAgregator;
}());
var eventAggregator = new EventAgregator();
//# sourceMappingURL=EventAgregator.js.map