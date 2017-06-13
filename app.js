var app = angular.module('kino', []);
app.controller('kinoCtrl',
    function ($scope, $http) {
        $scope.loading = true;

        $scope.addDaysToDate = function (date, daysToAdd) {
            var addedDate = new Date(date);
            addedDate.setDate(addedDate.getDate() + daysToAdd);
            return addedDate;
        }

        $scope.getDateString = function (date) {
            return date.toISOString().substr(0, 10);
        }

        $scope.getDateFromUrl = function (urlDate) {
            if (typeof urlDate === "undefined") {
                return new Date();
            }
            if (urlDate.toLowerCase() === "zitra") {
                return $scope.addDaysToDate(new Date(), 1);
            }
            return new Date(urlDate);
        }

        $scope.displayCinemas = function (cityName, urlDate) {
            $scope.city = cityName;
            $url = "https://kinodnesapi.azurewebsites.net/api/kino/" + cityName;
            var date = $scope.getDateFromUrl(urlDate);
            // Today
            if ($scope.getDateString(date) == $scope.getDateString(new Date())) {
                $scope.previous = "";
                $scope.current = $scope.getDateString(date);
                $scope.next = $scope.getDateString($scope.addDaysToDate(date, 1));
                var todayLocation = location.origin + "/" + cityName;
                history.pushState(todayLocation, "", todayLocation);
            } else {
                $scope.current = $scope.getDateString(date);
                $scope.previous = $scope.getDateString($scope.addDaysToDate(date, -1));
                $scope.next = $scope.getDateString($scope.addDaysToDate(date, 1));
                $url += "/" + $scope.current;
                var newLocation = location.origin + "/" + cityName + "/" + $scope.current;
                history.pushState(newLocation, "", newLocation);
            }

            $http.get($url)
                .then(function (response) {
                    $scope.loading = false;
                    document.getElementById("listings").style.display = "";
                    if (response.data.length === 0) {
                        document.getElementById("noMoviesMessage").style.display = "";
                    } else {
                        $scope.cinemaListings = response.data;                        
                        document.getElementById("noMoviesMessage").style.display = "none";
                    }
                });
        };

        $scope.selectCity = function (cityName) {            
            $scope.loading = true;
            $scope.city = cityName;
            document.getElementById("districtButtons").style.display = "none";
            $scope.displayCinemas(cityName);
        };

        $scope.hideCityName = function (cityAndCinema) {
            return cityAndCinema.replace(/.*?- /, "");
        };

        $scope.displayCityList = function () {            
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
            var city = arguments[1];
            $scope.date = arguments[2];
            $scope.displayCinemas(city, $scope.date);
        }
    });

window.onpopstate = function (e) {
    if (e.state) {
        location.href = e.state;
    }
};