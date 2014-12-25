//homeMessageBoard.js

//window.onload = function () {
//}

var homeMessageBoardModule = angular.module('homeMessageBoard', ['ngRoute', 'ngSanitize']);

//note this uses injection for angular items like dataService, $scope, $http, etc
//object functions must be passed to allow minification to work

homeMessageBoardModule.config([
    "$routeProvider", function ($routeProvider) {
        //when url base (similar to route config)
        $routeProvider.when("/",
        {
            controller: "topicsController",
            templateUrl: "/templates/topicsView.html"
        });

        $routeProvider.when("/newMessage",
        {
            controller: "newTopicController",
            templateUrl: "/templates/newTopicView.html"
        });

        //parameterized route
        $routeProvider.when("/message/:id",
        {
            controller: "singleTopicController",
            templateUrl: "/templates/singleTopicView.html"
        });

        //for any route it doesnt find it comes to the home page
        $routeProvider.otherwise({ redirectTo: "/" });
    }
]);

//creates a function angular binds to
homeMessageBoardModule.factory("dataService", [
    "$http", "$q", function ($http, $q) {

        //always true - want this non static
        var _reload = true;
        var _topics = [];

        var _getTopics = function () {
            var deferred = $q.defer();

            if (_reload == true) {
                var query = $http.get("/api/v1/topics?includeReplies=true");
                query.then(function (result) {
                    //success
                    angular.copy(result.data, _topics);
                    _reload = true;
                    deferred.resolve();
                },
                    function () {
                        //error
                        deferred.reject();
                    });
            } else {
                //set _unchanged to false when first time loaded, or when a data change is registered
                deferred.resolve();
            }

            return deferred.promise;
        };

        var _addTopic = function (newTopic) {
            var deferred = $q.defer();

            var query = $http.post("/api/v1/topics", newTopic);
            query.then(function (result) {
                //success
                var newlyCreatedTopic = result.data;
                //add one item, dont delete any, insert this item
                _topics.splice(0, 0, newlyCreatedTopic);
                //pass this back in case the consumer needs this
                deferred.resolve(newlyCreatedTopic);
            }, function (response) {
                //error
                deferred.reject(response);
            });

            return deferred.promise;
        };

        var _addReply = function (topic, newReply) {
            var deferred = $q.defer();

            var query = $http.post("/api/v1/topics/" + topic.id + "/replies", newReply);
            query.then(function (result) {
                //success
                //ensure not a null object, create if so
                if (topic.replies == null) topic.replies = [];
                topic.replies.push(result.data);
                deferred.resolve();
            }, function () {
                //error
                deferred.reject();
            });

            return deferred.promise;
        };

        function _findTopic(id) {
            //already know the data is loaded
            var found = null;

            $.each(_topics, function (i, item) {
                if (item.id == id) {
                    found = item;
                    return false;
                }
            });

            return found;
        }

        var _getTopicById = function (id) {
            var deferred = $q.defer();
            var topic = null;

            if (_reload === false) {
                topic = _findTopic(id);
                if (topic) {
                    deferred.resolve(topic);
                } else {
                    deferred.reject();
                }
            } else {
                _getTopics()
                    .then(function () {
                        //success
                        topic = _findTopic(id);
                        if (topic) {
                            deferred.resolve(topic);
                        } else {
                            deferred.reject();
                        }
                    },
                        function (result) {
                            deferred.reject();
                        });
            }

            return deferred.promise;
        }; //singleton
        return {
            topics: _topics,
            getTopics: _getTopics,
            getTopicById: _getTopicById,
            addTopic: _addTopic,
            addReply: _addReply,
        };
    }
]);

homeMessageBoardModule.controller("topicsController", [
    "$scope", "$http", "dataService", function ($scope, $http, dataService) {

        $scope.dataCount = 0;
        $scope.data = dataService;
        $scope.isBusy = true;

        dataService.getTopics()
            .then(function () {
                //success - no need to copy the scope is linked directly to dataService.topics at this point
                //copy($scope.data.topics, dataService.topics);

                //note: the data in the angular html files will need to match the SQL/C# Property name in camelcase
            },
                function () {
                    //error
                    alert("Can not load topics");
                })
            .then(function () {
                $scope.isBusy = false;
            });

        //$scope.name = "Jeremy";
        //$scope.data = [];
        //$scope.data.topics = [{
        //    strTitle: "This is a message",
        //    strBody: "I am right about this",
        //    dtCreated: "12/25/2014",
        //    replies: [{
        //        strTitle: "This is a reply",
        //        strBody: "my reply body"
        //    }]
        //},
        //{
        //    strTitle: "This is a message 2",
        //    strBody: "I am right about this",
        //    dtCreated: "12/25/2014",
        //    replies: [{
        //        strTitle: "This is a reply",
        //        strBody: "my reply body"
        //    }]
        //},
        //{
        //    strTitle: "This is a message 3",
        //    strBody: "I am right about this",
        //    dtCreated: "12/25/2014",
        //    replies: [{
        //        strTitle: "This is a reply",
        //        strBody: "my reply body"
        //    }]
        //}];

        //$scope.dataCount = 0;
        //$scope.isBusy = true;

    }
]);

homeMessageBoardModule.controller("newTopicController", [
    "$scope", "$http", "$window", "dataService", function ($scope, $http, $window, dataService) {
        $scope.newTopic = {};
        $scope.newTopicValidation = "";

        $scope.save = function () {
            dataService.addTopic($scope.newTopic)
                .then(function () {
                    //success
                    window.location = "/Home/MessageBoard";
                },
                    function (response) {
                        //error
                        if (response == null) {
                            $scope.strFormValidation = '<br />Unable to save this topic';
                            jtcommon.handleError("Unknown Error in newTopicController");
                        }
                        else {
                            $scope.strFormValidation = jtcommon.getServerSideValidation(response);
                        };
                    });
        };

        //get the initial modelstate values
        //$scope.save($scope.newTopic);
    }
]);

homeMessageBoardModule.controller("singleTopicController", [
    "$scope", "dataService", "$window", "$routeParams", function ($scope, dataService, $window, $routeParams) {
        $scope.topic = null;
        $scope.newReply = {};

        //route param was marked by :id
        dataService.getTopicById($routeParams.id)
            .then(function (topic) {
                //succes
                $scope.topic = topic;
            },
                function () {
                    //error
                    //alert("Unable to get this topic");
                    window.location = "/Home/MessageBoard";
                });

        $scope.addReply = function () {
            dataService.addReply($scope.topic, $scope.newReply)
                .then(function () {
                    //success
                    //clear the reply form
                    $scope.newReply.strBody = "";
                },
                    function (response) {
                        //error
                        if (response == null) {
                            $scope.strFormValidation = '<br />Unable to save this reply';
                            jtcommon.handleError("Unknown Error in singleTopicController");
                        }
                        else {
                            $scope.strFormValidation = jtcommon.getServerSideValidation(response);
                        };
                    });
        };
    }
]);