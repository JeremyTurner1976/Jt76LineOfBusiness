//homeAdmin.js

//window.onload = function () {
//}

var homeAdminModule = angular.module('homeAdmin', ['ngRoute', 'ngSanitize']);

//note this uses injection for angular items like dataService, $scope, $http, etc
//object functions must be passed to allow minification to work

homeAdminModule.config([
    "$routeProvider", function ($routeProvider) {
        //when url base (similar to route config)

        $routeProvider.when("/",
        {
            controller: "adminController",
            templateUrl: "/templates/adminView.html"
        });

        $routeProvider.when("/errors",
        {
            controller: "adminController",
            templateUrl: "/templates/adminView.html"
        });

        $routeProvider.when("/logMessages",
        {
            controller: "adminController",
            templateUrl: "/templates/adminView.html"
        });

        //for any route it doesnt find it comes to the home page
        $routeProvider.otherwise({ redirectTo: "/" });
    }
]);

//creates a function angular binds to
homeAdminModule.factory("dataService", [
    "$http", "$q", function ($http, $q) {

        var _reloadErrors = true;
        var _reloadLogMessages = true;
        var _errors = [];
        var _logMessages = [];

        var _getErrors = function () {
            var deferred = $q.defer();

            if (_reloadErrors == true) {

                var query = $http.get("/api/v1/errors");
                query.then(function (result) {
                    //alert('success');
                    angular.copy(result.data, _errors);
                    _reloadErrors = false;
                    deferred.resolve();
                },
                    function () {
                        //alert('error');
                        deferred.reject();
                    });
            } else {
                //set _unchanged to false when first time loaded, or when a data change is registered
                deferred.resolve();
            }

            return deferred.promise;
        };

        var _getLogMessages = function () {
            var deferred = $q.defer();

            if (_reloadLogMessages == true) {

                var query = $http.get("/api/v1/logMessages");
                query.then(function (result) {
                    //alert('success');
                    angular.copy(result.data, _logMessages);
                    _reloadLogMessages = false;
                    deferred.resolve();
                },
                    function () {
                        //alert('error');
                        deferred.reject();
                    });
            } else {
                //set _unchanged to false when first time loaded, or when a data change is registered
                deferred.resolve();
            }

            return deferred.promise;
        };

        var _addError = function (newError) {
            var deferred = $q.defer();

            var query = $http.post("/api/v1/errors", newError);
            query.then(function (result) {
                //success
                var newlyCreatedError = result.data;
                //add one item, dont delete any, insert this item
                _errors.splice(0, 0, newlyCreatedError);
                //pass this back in case the consumer needs this
                deferred.resolve(newlyCreatedError);
            }),
                function () {
                    //error
                    deferred.reject();
                };

            return deferred.promise;
        };

        var _addLogMessage = function (newLogMessage) {
            var deferred = $q.defer();

            var query = $http.post("/api/v1/logMessages", newLogMessage);
            query.then(function (result) {
                //success
                var newlyCreatedLogMessage = result.data;
                //add one item, dont delete any, insert this item
                _logMessages.splice(0, 0, newlyCreatedLogMessage);
                //pass this back in case the consumer needs this
                _reloadLogMessages = true;
                deferred.resolve(newlyCreatedLogMessage);
            }),
                function () {
                    //error
                    deferred.reject();
                };

            return deferred.promise;
        }; //singleton
        return {
            errors: _errors,
            logMessages: _logMessages,
            getErrors: _getErrors,
            addError: _addError,
            getLogMessages: _getLogMessages,
            addLogMessage: _addLogMessage
        };
    }
]);

homeAdminModule.controller("adminController", [
    "$scope", "$http", "dataService", function ($scope, $http, dataService) {

        $scope.data = dataService;
        $scope.errorsAreBusy = true;
        $scope.logMessagesAreBusy = true;

        dataService.getErrors()
            .then(function () {
                //note: the data in the angular html files will need to match the SQL/C# Property name in camelcase
            },
                function () {
                    //error
                    alert("Can not load errors");
                })
            .then(function () {
                $scope.errorsAreBusy = false;
            });

        dataService.getLogMessages()
            .then(function () {
                //note: the data in the angular html files will need to match the SQL/C# Property name in camelcase
            },
                function () {
                    //error
                    alert("Can not load log messages");
                })
            .then(function () {
                $scope.logMessagesAreBusy = false;
            });

        //$scope.saveError = function () {
        //        dataService.addError($scope.newError)
        //            .then(function () {
        //                //success
        //                window.location("/");
        //            },
        //            function () {
        //                //error
        //                alert("Unable to save this topic");
        //            });
        //    };

        $scope.simulateError = function () {

            $scope.data.newError = {
                strMessage: "This is an error",
                strErrorLevel: "Seed",
                strSource: "This is the source",
                strStackTrace: "This is a stack trace",
                strAdditionalInformation: "This should be some UI specific remarks"
            };

            dataService.addError($scope.data.newError)
                .then(function (result) {
                    //success
                    //window.location("/");
                },
                    function (response) {
                        //error
                        if (response == null) {
                            $scope.strFormValidation = '<br />Unable to save this error';
                            jtcommon.handleError("Unknown Error in adminController");
                        }
                        else {
                            $scope.strFormValidation = jtcommon.getServerSideValidation(response);
                        };
                    });
        };


        //$scope.saveLogMessage = function () {
        //        dataService.addLogMessage($scope.newLogMessage)
        //            .then(function () {
        //                //success
        //                //window.location("/");
        //            },
        //            function () {
        //                //error
        //                alert("Unable to save this topic");
        //            });
        //    };

        $scope.simulateLogMessage = function () {

            $scope.data.newLogMessage = {
                strLogMessage: "This is a log message"
            };

            dataService.addLogMessage($scope.data.newLogMessage)
                .then(function (result) {
                    //success
                    //window.location("/");
                },
                    function (response) {
                        //error
                        if (response == null) {
                            $scope.strFormValidation = '<br />Unable to save this log message';
                            jtcommon.handleError("Unknown Error in adminController");
                        }
                        else {
                            $scope.strFormValidation = jtcommon.getServerSideValidation(response);
                        };
                    });
        };


        //$scope.name = "Jeremy";
        //$scope.data = [];

        //$scope.data.errors = [{
        //    strMessage: "This is an error",
        //    strErrorLevel: "Seed",
        //    strSource: "This is the source",
        //    strStackTrace: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec imperdiet sem. Nunc fermentum blandit bibendum. Sed viverra metus purus, id aliquet lorem consectetur eget. Maecenas sit amet diam commodo, rutrum turpis vitae, tristique mi. Donec convallis iaculis dictum. Cras posuere venenatis est nec eleifend. Pellentesque pretium dolor in quam accumsan accumsan. Vivamus facilisis eu massa ut bibendum. Nam porta tortor vel facilisis volutpat. Quisque pretium dolor lorem, nec suscipit ligula molestie sed. Fusce varius, nunc eget pulvinar rhoncus, justo erat tincidunt purus, sed porta mi purus a ante. Sed in dolor id tellus fringilla pharetra. Integer placerat quam ut ante vehicula faucibus. Fusce et justo vitae nibh blandit elementum non et libero. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae Praesent sed turpis at lectus lacinia tempor. Quisque a ullamcorper nibh. Donec et tristique elit. Proin vulputate, sem sed feugiat dictum, magna lorem facilisis risus, vitae tincidunt lectus massa vitae nisl. Duis nisi augue, cursus sit amet sollicitudin eu, dignissim in augue. Proin nisl sapien, consequat ut enim a, dictum fermentum eros. Nunc id sagittis lorem. Suspendisse potenti. Nulla eu rutrum eros, nec maximus quam. Vestibulum tincidunt, magna ac mollis dictum, justo lorem rhoncus augue, ac pretium enim justo vitae magna. Quisque consectetur blandit purus eget feugiat. Vestibulum elementum est a eros facilisis convallis nec eget orci. Nam id dictum turpis. Donec venenatis tellus et convallis porta. Donec cursus nibh id congue sagittis. Etiam porta accumsan est sit amet malesuada. Aenean ullamcorper volutpat mi.",
        //    strAdditionalInformation: "This should be some UI specific remarks",
        //    dtCreated: "12/25/2014"
        //}];

        //$scope.data.logMessages = [{
        //    strLogMessage: "This is a log message",
        //    dtCreated: "12/25/2014"
        //}];

        //$scope.dataCount = 0;
        //$scope.logMessagesAreBusy = false;
        //$scope.errorsAreBusy = false;


    }
]);