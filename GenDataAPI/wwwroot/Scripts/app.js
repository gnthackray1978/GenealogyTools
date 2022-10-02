"use strict";

function PlaceObj(webConsole) {
    this.console = webConsole;
    this.count = 0;
    this.failures = [];
    this.data;
}

PlaceObj.prototype = {

    displayStats: function () {
        var that = this;

        $.ajax({
            url: "./info/",
            type: "get", //send it through get method
            data: {
                infoType: 'unknown_places_count'
            },
            success: function (result) {
                that.console.displayStats(result);
            },
            error: function (xhr) {
                //Do Something to handle error
            }
        });

        return true;
    },

    addResetMissingPlaces: function () {

        var that = this;

        var Upload = {
            Value: 'addResetMissingPlaces'
        };

        $.ajax({
            type: "post",
            url: "/data/places",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }
        });
        return true;
    },

    updatePlaceMetadata: function () {

        var that = this;

        var Upload = {
            Value: 'updatePlaceMetadata'
        };

        $.ajax({
            type: "put",
            url: "/data/places", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
         
        return true;
    },

    setOriginPerson: function () {

        var that = this;

        var Upload = {
            Value: 'setOriginPerson'
        };

        $.ajax({
            type: "post",
            url: "/data/origins", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    importPersons: function () {

        var that = this;

        var Upload = {  };

        $.ajax({
            type: "post",
            url: "/data/persons", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    createDupeView: function () {

        var that = this;

        var Upload = {   };

        $.ajax({
            type: "post",
            url: "/data/dupes",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    createTreeRecord: function () {

        var that = this;

        var Upload = {
            Value: 'createTreeRecord'
        };

        $.ajax({
            type: "post",
            url: "/data/trees", 
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    createTreeGroups: function () {

        var that = this;

        var Upload = {
            Value: 'createTreeGroup'
        };

        $.ajax({
            type: "post",
            url: "/data/treegroups", 
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    createTreeGroupMappings: function () {

        var that = this;

        var Upload = {
            Value: 'createTreeGroupMappings'
        };

        $.ajax({
            type: "post",
            url: "/data/treegroupmappings",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },

    azureimport: function () {

        var that = this;

        var Upload = {
            Value: 'azureimport'
        };

        $.ajax({
            type: "post",
            url: "/data/azure", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayStats();
            }

        });
        return true;
    },


    saveGeoCodedLocationToServer: function (placeLookup) {
        var that = this;

        $.ajax({
            type: "post",
            url: "./geocode", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(placeLookup)  //the parameter in method

        });

         

    },


    getUnEncodedLocationsFromServer: function () {
       
        var sh = this;
        var printBasic = this.console.printOutputLine;
        let displayStats = this.displayStats;

        printBasic('GET geocode endpoint for unencoded places');
         
        $.ajax({
            type: "get", //send it through get method
            url: "./geocode/",
            data: {
                infoType: ''
            },
            success: function (result) {
                if (result && result.results) {
                    printBasic('GET geocode returned ' + result.results.length + ' places');

                    sh.data = result.results;

                    sh.start();
                }
                else {
                    printBasic('GET geocode did not return data');
                }
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
        var printOutputLine = this.console.printOutputLine;
        var printGeoCodeProgressCount = this.console.printGeoCodeProgressCount;
        var printTrace = this.console.printTrace;

        var geocoder = new google.maps.Geocoder();

        var idx = 0
     
        var searchAddress = () => {

            if (sh.data && (sh.data.length-1) == idx) {
                sh.displayStats();
                return;
            }

            let d = sh.data[idx];

            if (!d)
                return;

            printTrace(idx);

            console.log('searching: ' + d.placeformatted);

            geocoder.geocode({
                address: d.placeformatted
            }, (results, status) => {

                sh.count++;
                
                printGeoCodeProgressCount(sh.count);

                var result = {
                    placeid: d.placeid,
                    place: '',
                    placeformatted: d.placeformatted,
                    //results: JSON.stringify(results)
                };

                switch (status) {
                    case "OVER_QUERY_LIMIT":
                        console.log('OVER_QUERY_LIMIT');
                        printTrace('GEOCODE FAILED ' + status);

                        setTimeout(function () {
                            console.log('re-search');
                            
                                searchAddress();

                        }, 15000);

                        break;
                    case "INVALID_REQUEST":
                        // code block
                        console.log('INVALID_REQUEST');
                        printTrace('GEOCODE FAILED searched:' + d.placeformatted + ' status ' + status);

                        setTimeout(function () {
                            console.log('re-search');

                            idx++;

                            if (sh.data.length > idx)
                                searchAddress();
                        }, 3000);
                        break;

                    default:
                        console.log('saving');
                        result.results = JSON.stringify(results);

                        sh.console.printTrace(d.placeformatted, results);
                        sh.saveGeoCodedLocationToServer(result);

                        setTimeout(function () {
                            idx++;
                            searchAddress();
                        }, 500);
                }
                 
            });

        };
     
        searchAddress();
  
    
        return true;
    }

}




function initMap() {
    console.log("initMap");    
}