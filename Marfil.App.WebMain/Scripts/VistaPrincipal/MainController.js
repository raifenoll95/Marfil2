var LiteEvent = (function () {
    function LiteEvent() {
        this.handlers = [];
    }
    LiteEvent.prototype.on = function (handler) {
        this.handlers.push(handler);
    };
    LiteEvent.prototype.trigger = function (data) {
        this.handlers.slice(0).forEach(function (h) { return h(data); });
    };
    return LiteEvent;
}());
var MainControllerAggregator = (function () {
    function MainControllerAggregator() {
        this.onChange = new LiteEvent();
        this._vector = [];
    }
    Object.defineProperty(MainControllerAggregator.prototype, "OnChange", {
        get: function () { return this.onChange; },
        enumerable: true,
        configurable: true
    });
    MainControllerAggregator.prototype.RegisterElement = function (elementId) {
        this._vector.push(elementId);
        this.onChange.trigger();
    };
    MainControllerAggregator.prototype.DisposeElement = function (elementId) {
        var index = this._vector.indexOf(elementId);
        if (index >= 0)
            this._vector.splice(index, 1);
        this.onChange.trigger();
    };
    MainControllerAggregator.prototype.ExistsElement = function () {
        return this._vector.length > 0;
    };
    return MainControllerAggregator;
}());
var mainControllerAggregator = new MainControllerAggregator();
//# sourceMappingURL=MainController.js.map