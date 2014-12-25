/// <reference path="~/Scripts/jasmine.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-mocks.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-route.js" />
/// <reference path="~/../JT76.Ui/Scripts/angular-sanitize.js" />
/// <reference path="~/../JT76.Ui/JT76Scripts/homeAdmin.js" />

describe("homeAdmin tests ", function () {

    beforeEach(function () {
        module("homeAdmin");
    });

    var $httpBackend;

    beforeEach(inject(function ($injector) {
        $httpBackend = $injector.get("$httpBackend");

        $httpBackend.when("GET", "/api/v1/logMessages")
            .respond([{
                    strLogMessage: "This is a log message 1",
                    dtCreated: "12/25/2014"
            },
            {
                strLogMessage: "This is a log message 2",
                dtCreated: "12/25/2014"
            },
            {
                strLogMessage: "This is a log message 3",
                dtCreated: "12/25/2014"
            }]);

        $httpBackend.when("GET", "/api/v1/errors")
        .respond([{
                strMessage: "This is an error 1",
                strErrorLevel: "Seed",
                strSource: "This is the source",
                strStackTrace: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec imperdiet sem. Nunc fermentum blandit bibendum. Sed viverra metus purus, id aliquet lorem consectetur eget. Maecenas sit amet diam commodo, rutrum turpis vitae, tristique mi. Donec convallis iaculis dictum. Cras posuere venenatis est nec eleifend. Pellentesque pretium dolor in quam accumsan accumsan. Vivamus facilisis eu massa ut bibendum. Nam porta tortor vel facilisis volutpat. Quisque pretium dolor lorem, nec suscipit ligula molestie sed. Fusce varius, nunc eget pulvinar rhoncus, justo erat tincidunt purus, sed porta mi purus a ante. Sed in dolor id tellus fringilla pharetra. Integer placerat quam ut ante vehicula faucibus. Fusce et justo vitae nibh blandit elementum non et libero. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae Praesent sed turpis at lectus lacinia tempor. Quisque a ullamcorper nibh. Donec et tristique elit. Proin vulputate, sem sed feugiat dictum, magna lorem facilisis risus, vitae tincidunt lectus massa vitae nisl. Duis nisi augue, cursus sit amet sollicitudin eu, dignissim in augue. Proin nisl sapien, consequat ut enim a, dictum fermentum eros. Nunc id sagittis lorem. Suspendisse potenti. Nulla eu rutrum eros, nec maximus quam. Vestibulum tincidunt, magna ac mollis dictum, justo lorem rhoncus augue, ac pretium enim justo vitae magna. Quisque consectetur blandit purus eget feugiat. Vestibulum elementum est a eros facilisis convallis nec eget orci. Nam id dictum turpis. Donec venenatis tellus et convallis porta. Donec cursus nibh id congue sagittis. Etiam porta accumsan est sit amet malesuada. Aenean ullamcorper volutpat mi.",
                strAdditionalInformation: "This should be some UI specific remarks",
                dtCreated: "12/25/2014"
            },
            {
                strMessage: "This is an error 2",
                strErrorLevel: "Seed",
                strSource: "This is the source",
                strStackTrace: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec imperdiet sem. Nunc fermentum blandit bibendum. Sed viverra metus purus, id aliquet lorem consectetur eget. Maecenas sit amet diam commodo, rutrum turpis vitae, tristique mi. Donec convallis iaculis dictum. Cras posuere venenatis est nec eleifend. Pellentesque pretium dolor in quam accumsan accumsan. Vivamus facilisis eu massa ut bibendum. Nam porta tortor vel facilisis volutpat. Quisque pretium dolor lorem, nec suscipit ligula molestie sed. Fusce varius, nunc eget pulvinar rhoncus, justo erat tincidunt purus, sed porta mi purus a ante. Sed in dolor id tellus fringilla pharetra. Integer placerat quam ut ante vehicula faucibus. Fusce et justo vitae nibh blandit elementum non et libero. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae Praesent sed turpis at lectus lacinia tempor. Quisque a ullamcorper nibh. Donec et tristique elit. Proin vulputate, sem sed feugiat dictum, magna lorem facilisis risus, vitae tincidunt lectus massa vitae nisl. Duis nisi augue, cursus sit amet sollicitudin eu, dignissim in augue. Proin nisl sapien, consequat ut enim a, dictum fermentum eros. Nunc id sagittis lorem. Suspendisse potenti. Nulla eu rutrum eros, nec maximus quam. Vestibulum tincidunt, magna ac mollis dictum, justo lorem rhoncus augue, ac pretium enim justo vitae magna. Quisque consectetur blandit purus eget feugiat. Vestibulum elementum est a eros facilisis convallis nec eget orci. Nam id dictum turpis. Donec venenatis tellus et convallis porta. Donec cursus nibh id congue sagittis. Etiam porta accumsan est sit amet malesuada. Aenean ullamcorper volutpat mi.",
                strAdditionalInformation: "This should be some UI specific remarks",
                dtCreated: "12/25/2014"
            },
            {
                strMessage: "This is an error 3",
                strErrorLevel: "Seed",
                strSource: "This is the source",
                strStackTrace: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec imperdiet sem. Nunc fermentum blandit bibendum. Sed viverra metus purus, id aliquet lorem consectetur eget. Maecenas sit amet diam commodo, rutrum turpis vitae, tristique mi. Donec convallis iaculis dictum. Cras posuere venenatis est nec eleifend. Pellentesque pretium dolor in quam accumsan accumsan. Vivamus facilisis eu massa ut bibendum. Nam porta tortor vel facilisis volutpat. Quisque pretium dolor lorem, nec suscipit ligula molestie sed. Fusce varius, nunc eget pulvinar rhoncus, justo erat tincidunt purus, sed porta mi purus a ante. Sed in dolor id tellus fringilla pharetra. Integer placerat quam ut ante vehicula faucibus. Fusce et justo vitae nibh blandit elementum non et libero. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae Praesent sed turpis at lectus lacinia tempor. Quisque a ullamcorper nibh. Donec et tristique elit. Proin vulputate, sem sed feugiat dictum, magna lorem facilisis risus, vitae tincidunt lectus massa vitae nisl. Duis nisi augue, cursus sit amet sollicitudin eu, dignissim in augue. Proin nisl sapien, consequat ut enim a, dictum fermentum eros. Nunc id sagittis lorem. Suspendisse potenti. Nulla eu rutrum eros, nec maximus quam. Vestibulum tincidunt, magna ac mollis dictum, justo lorem rhoncus augue, ac pretium enim justo vitae magna. Quisque consectetur blandit purus eget feugiat. Vestibulum elementum est a eros facilisis convallis nec eget orci. Nam id dictum turpis. Donec venenatis tellus et convallis porta. Donec cursus nibh id congue sagittis. Etiam porta accumsan est sit amet malesuada. Aenean ullamcorper volutpat mi.",
                strAdditionalInformation: "This should be some UI specific remarks",
                dtCreated: "12/25/2014"
            }]);


    }));

    afterEach(function () {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });

    describe("dataService tests ", function () {

        it("can load log messages", inject(function (dataService) {
            expect(dataService.logMessages).toEqual([]);

            //expecting a call to be made
            $httpBackend.expect("GET", "/api/v1/logMessages");
            dataService.getLogMessages();
            //run all calls
            $httpBackend.flush();
            expect(dataService.logMessages.length).toBeGreaterThan(0);
            expect(dataService.logMessages.length).toEqual(3);
        }));

        it("can load errors", inject(function (dataService) {
            expect(dataService.errors).toEqual([]);

            //expecting a call to be made
            $httpBackend.expect("GET", "/api/v1/errors");
            dataService.getErrors();
            //run all calls
            $httpBackend.flush();
            expect(dataService.errors.length).toBeGreaterThan(0);
            expect(dataService.errors.length).toEqual(3);
        }));

    });

    describe("adminController ", function () {

        it("load data", inject(function ($controller, $http, dataService) {
            //let injection create this
            var theScope = {};

            //expecting a call to be made
            $httpBackend.expect("GET", "/api/v1/errors");
            //expecting a call to be made
            $httpBackend.expect("GET", "/api/v1/logMessages");

            var controller = $controller('adminController', {
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