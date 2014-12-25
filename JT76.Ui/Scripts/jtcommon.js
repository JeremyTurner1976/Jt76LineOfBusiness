//jtcommon.js

(function (jtcommon) {

    jtcommon.isDebug = true;

    jtcommon.log = function (msg) {
        if (jtcommon.isDebug == true)
            console.log(msg);
    };

    jtcommon.handleError = function (msg) {
        if (jtcommon.isDebug == true)
            console.log(msg);
    };

    jtcommon.getServerSideValidation = function (response)
    {
        var strServerSideValidation = '<br />';
        strServerSideValidation += response.data.message;
        $.each(response.data.modelState, function () {
            strServerSideValidation += '<br/>' + this[0];
        });
        return strServerSideValidation;
    }


})(window.jtcommon = window.jtcommon || {});


//app.config(function ($provide) {
//    $provide.decorator("$exceptionHandler",
//    ["$delegate",
//        function ($delegate) {
//            return function (exception, cause) {
//                exception.message = "|JS Exception|" + exception.message;
//                $delegate(exception, cause);
//                alert(exception.message);
//            }
//        }
//    ]);
//});

//throw true;
//throw 5;
//throw "error message";
//throw null;
//throw undefined;
//throw {};
//throw new SyntaxError("useful error message");

window.onerror = function (exception) {
    var test = exception;
}



