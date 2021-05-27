app.directive('enterAsTab', function () {
        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs) {
                $element.bind("keypress", function (event) {
                    if (event.which === 13) {
                        event.preventDefault();
                        $element.next().focus();
                    }
                });
            }
        }
});