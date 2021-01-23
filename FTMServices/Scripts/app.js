"use strict";

function PlaceObj() {
    this.count = 0;
    this.failures = [];
    this.data;
}

PlaceObj.prototype = {

    countOfUnknownLocations: function () {
       
        $.ajax({
            url: "./api/info/",
            type: "get", //send it through get method
            data: {
                infoType: 'unknown_places_count'
            },
            success: function (result) {
                document.getElementById("count").innerHTML = result.RecordCount;
                
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

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    addResetMissingPlaces: function () {

        var Upload = {
            Value: 'addResetMissingPlaces'
        };

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    updatePlaceMetadata: function () {

        var Upload = {
            Value: 'updatePlaceMetadata'
        };

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    setOriginPerson: function () {

        var Upload = {
            Value: 'setOriginPerson'
        };

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    setDateLocPop: function () {

        var Upload = {
            Value: 'setDateLocPop'
        };

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    createDupeView: function () {

        var Upload = {
            Value: 'setDateLocPop'
        };

        $.post("./api/data", Upload,
            function (data, status) {

            });
        return true;
    },

    saveGeoCodedLocationToServer: function (placeLookup) {
        $.post("./api/geocode", placeLookup,
            function (data, status) {

            });

    },


    getUnEncodedLocationsFromServer: function () {
        $('#discussion').append('GET geocode endpoint for unencoded places<br />');
        var sh = this;

        $.ajax({
            url: "./api/geocode", success: function (result) {
               
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