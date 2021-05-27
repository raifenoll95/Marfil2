/* jshint boss:true*/
(function (global) {
    'use strict';
    
    var formatRegExp = new RegExp("{(\\w+)(\\*(?:\"\"|\"((?:.|\\\\\")*?[^\\\\])\")?(?:,(?:\"\"|\"((?:.|\\\\\")*?[^\\\\])\"))?(?:;(?:\"\"|\"((?:.|\\\\\")*?[^\\\\])\"))?)?(,(-?\\d+))?(;(-?\\d+))?(\\:(.+?))?(\\|(\\d+)(,(\\d+))?)?(\\?(\"\"|(?:\"((?:.|\\\\\")*?[^\\\\])\"))?(,(?:\"\"|\"((?:.|\\\\\")*[^\\\\])\"))?(\\:(?:\"\"|\"((?:.|\\\\\")*?[^\\\\])\"))?)?}", "g");

    function isArray(obj) {
        return Object.prototype.toString.call(obj) === '[object Array]';
    }
    
    function isFunction(obj) {
        return Object.prototype.toString.call(obj) === '[object Function]';
    }

    function formatArgument(match, arg, useLocale) {
        var result = null;

        if (match[2] && isArray(arg)) {
            result = '';
            if (arg.length > 0) {
                var connector = match[3] || '';
                var twoPartConnector = match[4] || connector;
                var lastConnector = match[5] || twoPartConnector;
                for (var i = 0, l = arg.length; i < l; i++) {
                    var item = formatToString(arg[i], match[11], useLocale);
                    if (item) {
                        if (i === 0) {
                            result += item;
                        }
                        else if (i > 0 && i < (l - 1)) {
                            result += connector + item;
                        }
                        else if (i === 1 && i === (l - 1)) {
                            result += twoPartConnector + item;
                        } else {
                            result += lastConnector + item;
                        }
                    }
                }
            } 
        }

        if (result === null) {
            result = formatToString(arg, match[11], useLocale);
        }

        if (match[6]) {
            var argMinLength = parseInt(match[7], 10);
            while (Math.abs(argMinLength) > result.length) {
                if (argMinLength > 0) {
                    result = result + ' ';
                }
                else if (argMinLength < 0) {
                    result = ' ' + result;
                }
            }
        }
        if (match[8]) {
            var argMaxLength = parseInt(match[9], 10);
            if (argMaxLength > 0 && argMaxLength <= result.length) {
                result = result.substr(0, argMaxLength);
            }
            else if (argMaxLength < 0 && Math.abs(argMaxLength) <= result.length) {
                result = result.substr(result.length + argMaxLength);
            }
        }
        if (match[12]) {
            var argStartIndex = parseInt(match[13], 10);
            var argLength = -1;
            if (match[14]) {
                argLength = parseInt(match[15], 10);
            }
            if (argStartIndex >= 0 && argStartIndex < result.length) {
                if (argLength >= 0 && argStartIndex + argLength < result.length) {
                    result = result.substr(argStartIndex, argLength);
                }
                else {
                    result = result.substr(argStartIndex);
                }
            }
            else {
                result = '';
            }
        }
        if (match[16]) {
            var prefix = '';
            if (match[17]) {
                prefix = match[18];
            }
            var suffix = '';
            if (match[19]) {
                suffix = match[20];
            }
            var content = '';
            if (match[21]) {
                content = match[22];
            }
            if (result === '') {
                result = content;
            }
            else {
                result = prefix + result + suffix;
            }
        }
        return result;
    }

    function formatToString(value, format, useLocale) {
        var result = null;
        if (format) {
            if (useLocale && isFunction(value.localeFormat)) {
                result = value.localeFormat(format);
            }
            if (result === null && isFunction(value.format)) {
                result = value.format(format);
            }
        }
        if (result === null && value !== null && typeof value !== "undefined") {
            if (useLocale && isFunction(value.toLocaleString)) {
                result = value.toLocaleString();
            } else {
                result = value.toString();
            }
        }
        if (result === null) {
            result = '';
        }
        return result;
    }

    function indexedFormat(args, useLocale) {
        var format = args[0];
        var result = format.replace(formatRegExp, function () {
            var argIndex = parseInt(arguments[1], 10) + 1;
            if (isNaN(argIndex) || argIndex < 1 || argIndex >= (args.length)) {
                throw "Invalid format";
            }
            var arg = args[argIndex];
            return formatArgument(arguments, arg, useLocale);
        });
        return result;
    }

    function namedFormat(format, namedArgs, useLocale) {
        var result = format.replace(formatRegExp, function () {
            var argName = arguments[1];
            if (!(argName in namedArgs)) {
                throw "Invalid format";
            }
            var arg = namedArgs[argName];
            return formatArgument(arguments, arg, useLocale);
        });
        return result;
    }

    global.String.format = function () {
        return indexedFormat(arguments, false);
    };

    global.String.namedFormat = function (format, namedArgs) {
        return namedFormat(format, namedArgs, false);
    };
    
    global.String.localeFormat = function () {
        return indexedFormat(arguments, true);
    };

    global.String.localeNamedFormat = function (format, namedArgs) {
        return namedFormat(format, namedArgs, true);
    };

})(typeof global === 'undefined' ? window : global);