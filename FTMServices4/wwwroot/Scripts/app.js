"use strict";

function PlaceObj() {
    this.count = 0;
    this.failures = [];
    this.data;
}

PlaceObj.prototype = {

    countOfUnknownLocations: function () {
       
        $.ajax({
            url: "./info/",
            type: "get", //send it through get method
            data: {
                infoType: 'unknown_places_count'
            },
            success: function (result) {
                document.getElementById("count").innerHTML = result.recordCount;
                
                //
            },
            error: function (xhr) {
                //Do Something to handle error
            }
        });

        return true;
    },
    
    backupAndDecryptFTMDB: function () { 

        var Upload = {
            Value: 'backupAndDecryptFTMDB'
        };

        $.post("./data", Upload,
            function (data, status) {

            });
        return true;
    },

    addResetMissingPlaces: function () {

        var Upload = {
            Value: 'addResetMissingPlaces'
        };

        $.post("./data", Upload,
            function (data, status) {

            });
        return true;
    },

    updatePlaceMetadata: function () {

        var Upload = {
            Value: 'updatePlaceMetadata'
        };

        $.ajax({
            type: "post",
            url: "/data", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload)  //the parameter in method

        });
         
        return true;
    },

    setOriginPerson: function () {

        var Upload = {
            Value: 'setOriginPerson'
        };

        $.post("./data", Upload,
            function (data, status) {

            });
        return true;
    },

    setDateLocPop: function () {

        var Upload = {
            Value: 'setDateLocPop'
        };

        $.post("./data", Upload,
            function (data, status) {

            });
        return true;
    },

    createDupeView: function () {

        var Upload = {
            Value: 'setDateLocPop'
        };

        $.post("./data", Upload,
            function (data, status) {

            });
        return true;
    },

    saveGeoCodedLocationToServer: function (placeLookup) {
        $.post("./geocode", placeLookup,
            function (data, status) {

            });

    },


    getUnEncodedLocationsFromServer: function () {
        $('#discussion').append('GET geocode endpoint for unencoded places<br />');
        var sh = this;

        $.ajax({
            url: "./geocode", success: function (result) {
               
                $('#discussion').append('GET geocode returned data<br />');
                sh.data = result;
                sh.start();
            }
        });

        return true;
    },

    start: function() {
        var sh = this;
        sh.count = 0;
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

                sh.count++;
                document.getElementById("geocodecount").innerHTML = sh.count;
                

                if (status == "OVER_QUERY_LIMIT") {
                 
                    $('#discussion').append('GEOCODE FAILED ' + status + '<br />');
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
                   // $('#discussion').append('GEOCODE SUCCESS Posting to server<br />');
                   
                    sh.saveGeoCodedLocationToServer(result);

                    setTimeout(function () {
                        idx++;
                        searchAddress(sh.data[idx]);
                    }, 500);
                }
            });

        };
     
        searchAddress(data[idx]);
  
    
        return true;
    }

}




function initMap() {
    console.log("initMap");    
}