angular.module("emailParser", [])
.config(["$interpolateProvider", function ($interpolate) {
    $interpolate.startSymbol("__");
    $interpolate.endSymbol("__");
} ])
.factory("EmailParser", ["$interpolate", function ($interpolate) {
    return { parse: function (text, context) {
        var template = $interpolate(text);
        return template(context);
    }
    };
} ]);

angular.module("myApp", [])
.controller('MyController', function ($scope, $timeout) {
    var updateClock = function () {
        var dateObj = new Date();
        $scope.today = dateObj;
        $scope.clock = "." + dateObj.getMilliseconds();

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

angular.module("myApp2", ['emailParser'])
.controller("WriteEmailController", ["$scope", "EmailParser", function ($scope, EmailParser) {
    $scope.$watch("emailBody", function (body) {
        if (body) {
            $scope.previewText = EmailParser.parse(body, { to: $scope.to });
        }
    });
} ])
.controller("register", function ($scope) {
    $scope.Name = "Hao";

    $scope.submit = function () {
        console.log("submit");
        $scope.postData = "abcd2";
    }
});