var app = angular.module('insertItem', []);

app.controller('itemCtrl', function ($scope) {
    $scope.items = [1];
    $scope.addNewItem = function () {
        var newItemNo = $scope.items.length + 1;
        $scope.items.push(newItemNo);
    };
    $scope.removeItem = function () {
        var lastItem = $scope.items.length - 1;
        $scope.items.splice(lastItem);
    };

});

app.controller('subitemCtrl', function ($scope) {
    $scope.subitems = [1];
    $scope.addNewItem1 = function () {
        var newItemNo = $scope.subitems.length + 1;
        $scope.subitems.push(newItemNo);
    };
    $scope.removeItem1 = function () {
        var lastItem = $scope.subitems.length - 1;
        $scope.subitems.splice(lastItem);
    };
});


/************ HOTEL ************/

app.controller('itemCtrlHotel', function ($scope) {
    $scope.items = [1];
    $scope.addNewItemHotel = function () {
        var newItemNo = $scope.items.length + 1;
        $scope.items.push(newItemNo);
    };
    $scope.removeItemHotel = function () {
        var lastItem = $scope.items.length - 1;
        $scope.items.splice(lastItem);
    };

});

app.controller('subitemCtrlHotel', function ($scope) {
    $scope.subitems = [1];
    $scope.addNewItemHotel1 = function () {
        var newItemNo = $scope.subitems.length + 1;
        $scope.subitems.push(newItemNo);
    };
    $scope.removeItemHotel1 = function () {
        var lastItem = $scope.subitems.length - 1;
        $scope.subitems.splice(lastItem);
    };
});
