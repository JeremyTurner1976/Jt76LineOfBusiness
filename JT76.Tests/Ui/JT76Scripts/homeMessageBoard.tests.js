/// <reference path="~/Scripts/jasmine.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-mocks.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-route.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-sanitize.js" />
/// <reference path="~/../JT76.Ui/JT76Scripts/homeMessageBoard.js" />

describe("homeMessageBoard tests ", function () {

	beforeEach(function () {
		module("homeMessageBoard");
	});

	var $httpBackend;

	beforeEach(inject(function ($injector) {
		$httpBackend = $injector.get("$httpBackend");
		$httpBackend.when("GET", "/api/v1/topics?includeReplies=true")
            .respond([{
                    strTitle: "This is a message",
                    strBody: "I am right about this",
                    dtCreated: "12/25/2014",
                    replies: [{
                        strTitle: "This is a reply",
                        strBody: "my reply body"
                    }]
                },
                {
                    strTitle: "This is a message 2",
                    strBody: "I am right about this",
                    dtCreated: "12/25/2014",
                    replies: [{
                        strTitle: "This is a reply",
                        strBody: "my reply body"
                    }]
                },
                {
                    strTitle: "This is a message 3",
                    strBody: "I am right about this",
                    dtCreated: "12/25/2014",
                    replies: [{
                        strTitle: "This is a reply",
                        strBody: "my reply body"
                    }]
                }]);
	}));

	afterEach(function () {
		$httpBackend.verifyNoOutstandingExpectation();
		$httpBackend.verifyNoOutstandingRequest();
	});

	describe("dataService tests ", function () {

		it("can load topics", inject(function (dataService) {
			expect(dataService.topics).toEqual([]);

			//expecting a call to be made
			$httpBackend.expect("GET", "/api/v1/topics?includeReplies=true");
			dataService.getTopics();
			//run all calls
			$httpBackend.flush();
			expect(dataService.topics.length).toBeGreaterThan(0);
			expect(dataService.topics.length).toEqual(3);
		}));

	});

	describe("topicsController ", function () {

		it("load data", inject(function ($controller, $http, dataService) {
			//let injection create this
			var theScope = {};

			//expecting a call to be made
			$httpBackend.expect("GET", "/api/v1/topics?includeReplies=true");

			var controller = $controller('topicsController', {
				$scope: theScope,
				$http: $http,
				dataService: dataService
			});


			//run all calls
			$httpBackend.flush();

			expect(controller).not.toBeNull();
		}));
	});

});