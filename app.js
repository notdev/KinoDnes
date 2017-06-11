var app = angular.module('kino', []);
app.controller('kinoCtrl',
    function ($scope, $http) {
        $scope.loading = true;

        $scope.getDateString = function (date) {
            if (date === "zitra") {
                date = new Date();
                date.setDate(date.getDate() + 1);
            } else {
                date = new Date(date);
            }

            var datestring = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
            return datestring;
        }

        $scope.displayCinemas = function (cityName, date) {
            $url = "https://kinodnesapi.azurewebsites.net/api/kino/" + cityName;
            if (typeof date !== "undefined") {
                $url += "/" + $scope.getDateString(date);
            };

            $http.get($url)
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

        if (location.pathname === "/") {
            $scope.displayCityList();
        } else {
            var arguments = location.pathname.split("/");
            city = arguments[1];
            date = arguments[2];
            $scope.displayCinemas(city, date);
        }
    });

window.onpopstate = function (e) {
    if (e.state) {
        location.href = e.state;
    }
};