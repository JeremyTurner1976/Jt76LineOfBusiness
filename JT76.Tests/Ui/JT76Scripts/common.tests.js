/// <reference path="~/Scripts/jasmine.js" />
/// <reference path="~/../Jt76.Ui/JT76Scripts/jtcommon.js" />


describe("jtcommon tests ", function() {
    it("isDebug", function() {
        expect(jtcommon.isDebug).toEqual(true);
    });

    it("log", function () {
        expect(jtcommon.log).toBeDefined();
        jtcommon.log("testing");
    });
});