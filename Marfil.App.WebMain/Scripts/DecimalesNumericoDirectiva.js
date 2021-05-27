app.directive('decimalesnumerico', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            scope.$watch(attrs.ngModel, function (newVal) {
                var char1 = Globalize.culture(Globalize.cultureSelector).numberFormat[","];
                var char2 = Globalize.culture(Globalize.cultureSelector).numberFormat["."];
                if (newVal !== undefined && newVal !== null) {
                    ngModelCtrl.$setViewValue(String(newVal).replace(char1, char2));
                    element.val(String(newVal).replace(char1, char2));
                }
            })

        }
    }
});
