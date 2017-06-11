var app = angular.module('kino', []);
app.controller('kinoCtrl',
    function ($scope, $http) {
        $scope.loading = true;

        $scope.displayCinemas = function (cityName) {
            $http.get("https://kinodnesapi.azurewebsites.net/api/kino/" + cityName)
                .then(function (response) {
                    if (response.data.length === 0) {
                        document.getElementById("noMoviesMessage").style.display = "";
                    }
                    $scope.cinemaListings = response.data;
                    $scope.loading = false;
                    document.getElementById("listings").style.display = "";
                });
        };

        $scope.selectCity = function (cityName) {
            $scope.loading = true;
            document.getElementById("districtButtons").style.display = "none";
            var newLocation = location.href + cityName;
            history.pushState(newLocation, "", newLocation);
            $scope.displayCinemas(cityName);
        };

        $scope.hideCityName = function (cityAndCinema) {
            return cityAndCinema.replace(/.*?- /, "");
        };

        $scope.displayCityList = function () {
            history.pushState(location.href, "", location.href);
            $http.get("https://kinodnesapi.azurewebsites.net/api/kino/Cities")
                .then(function (response) {
                    $scope.cityList = response.data;
                    document.getElementById("districtButtons").style.display = "";
                    $scope.loading = false;
                });
        }

        var district = location.href.substr(location.href.lastIndexOf('/') + 1);
        if (district.length == 0) {
            $scope.displayCityList();
        } else {
            $scope.displayCinemas(district);
        }
    });

window.onpopstate = function (e) {
    if (e.state) {
        location.href = e.state;
    }
};