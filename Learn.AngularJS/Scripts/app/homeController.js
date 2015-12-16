angular.module("myApp", [])
.controller('MyController', function ($scope, $timeout) {
    var updateClock = function () {
        var dateObj = new Date();
        $scope.clock = "" + dateObj.getHours() + ":" + dateObj.getMinutes() + ":" + dateObj.getSeconds();

        //        setInterval(function () {
        //            updateClock();
        //            //$scope.$apply(updateClock);
        //        }, 2000);
        $timeout(function () {
            updateClock();
        }, 1000);
    };

    updateClock();
})
.controller("MyController2", function ($scope) {
    $scope.Name = "Mei";
})
.controller("CountController", function ($scope) {
    $scope.count = 0;
    $scope.up = function (inputCount) {
        $scope.count += inputCount;
    };
    $scope.down = function (inputCount) {
        $scope.count -= inputCount;
    };
});