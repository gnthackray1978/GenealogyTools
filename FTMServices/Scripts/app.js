"use strict";

function PlaceObj() {
    this.failures = [];
    this.data;
}

PlaceObj.prototype = {

    countOfUnknownLocations: function () {

        $.ajax({
            url: "./api/info", success: function (result) {
                document.getElementById("count").innerHTML = result.RecordCount;
            }
        });

    },

    postService: function (placeLookup) {    
        $.post("./api/data", placeLookup,
            function (data, status) {

            });

    },

    callService : function() {
        var sh = this;

        $.ajax({
            url: "./api/data", success: function (result) {
                console.log("call service return");
                sh.data = result;
                sh.start();
            }
        });

    },

    start: function() {
        var sh = this;
        var geocoder = new google.maps.Geocoder();

        var idx = 0
     
        var searchAddress = (d) => {

            if (!d)
                return;

            if (d.PlaceFormatted)
                console.log(d.PlaceFormatted);

            document.getElementById("progress").innerHTML = idx;

            geocoder.geocode({
                address: d.PlaceFormatted
            }, (results, status) => {

                if (status == "OVER_QUERY_LIMIT") {
                    console.log('failed trying again');
                    setTimeout(function () {                   
                        searchAddress(sh.data[idx]);
                    }, 3000);
                }
                else {

                    //var placeLookup = {
                    //    PlaceId: 0,
                    //    Place: '',
                    //    PlaceFormatted: '',
                    //    Output: ''
                    //};

                    var element = document.createElement("p");

                    var result = {
                        PlaceId: d.PlaceId,
                        Place : '',
                        PlaceFormatted: d.PlaceFormatted,
                        results: JSON.stringify(results)
                    };

                    element.appendChild(document.createTextNode(JSON.stringify(result)));
                    document.getElementById('output').appendChild(element);

                    console.log('got result - posting');
                    sh.postService(result);

                    setTimeout(function () {
                        idx++;
                        searchAddress(sh.data[idx]);
                    }, 500);
                }
            });

        };
     
        searchAddress(data[idx]);
  
    

    }

}




function initMap() {
    console.log("initMap");    
}