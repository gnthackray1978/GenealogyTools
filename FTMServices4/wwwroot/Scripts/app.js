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
    
    

    addResetMissingPlaces: function () {

        var Upload = {
            Value: 'addResetMissingPlaces'
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

        $.ajax({
            type: "post",
            url: "/data", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload)  //the parameter in method

        });
        return true;
    },

    clearData: function () {

        var Upload = {
            Value: 'cleardata'
        };

        $.ajax({
            type: "post",
            url: "/data",  
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload)  

        });
        return true;
    },

    setDateLocPop: function () {

        var Upload = {
            Value: 'setDateLocPop'
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

    createDupeView: function () {

        var Upload = {
            Value: 'createDupeView'
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

    createTreeRecord: function () {

        var Upload = {
            Value: 'createTreeRecord'
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


    saveGeoCodedLocationToServer: function (placeLookup) {

        $.ajax({
            type: "post",
            url: "./geocode", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(placeLookup)  //the parameter in method

        });

         

    },


    getUnEncodedLocationsFromServer: function () {
        $('#discussion').append('GET geocode endpoint for unencoded places<br />');
        var sh = this;

        //$.ajax({
        //    type: "get", //send it through get method
        //    url: "./geocode/",
        //    success: function (result) {
               
        //        $('#discussion').append('GET geocode returned data<br />');
        //        sh.data = result;
        //        sh.start();
        //    }
        //});
        

        $.ajax({
            type: "get", //send it through get method
            url: "./geocode/",
            data: {
                infoType: ''
            },
            success: function (result) {
                $('#discussion').append('GET geocode returned data<br />');
                sh.data = result.results;
                sh.start();
            },
            error: function (xhr) {
                //Do Something to handle error
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

            if (d.placeformatted)
                console.log(d.placeformatted);

            document.getElementById("progress").innerHTML = idx;

            geocoder.geocode({
                address: d.placeformatted
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
                    //    placeid: 0,
                    //    Place: '',
                    //    placeformatted: '',
                    //    Output: ''
                    //};

                    var element = document.createElement("p");

                    var result = {
                        placeid: d.placeid,
                        place : '',
                        placeformatted: d.placeformatted,
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