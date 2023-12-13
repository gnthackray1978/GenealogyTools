"use strict";

function PlaceObj(webConsole) {
    this.console = webConsole;
    this.count = 0;
    this.failures = [];
    this.data;
}

PlaceObj.prototype = {

    displayCorrectImportUpdateSections: function () {

        var that = this;

        $.ajax({
            url: "./ged/status/",
            type: "get", //send it through get method            
            success: function (result) {
                that.console.displayCorrectImportUpdateSections(result, that);
            },
            error: function (err) {
                that.console.printError(err);
            }
        });

        return true;
    },

    displayPeopleStats: function () {
        var that = this;

        $.ajax({
            url: "./info/people/",
            type: "get", //send it through get method            
            success: function (result) {
                that.console.displayPeopleStats(result);
            },
            error: (e) => that.console.printError(e)
        });

        return true;
    },

    deleteGed: function (id) {
        
        var that = this;
         
        $.ajax({
            type: "delete",
            url: "/ged/delete",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(id),
            success: function (response) {
                that.displayGedStats(() => {
                    that.displayCorrectImportUpdateSections();
                });
            },
            error: (e) => that.console.printError(e)
        });
    },

    selectGed: function (id) {
        
        var that = this;
 
        $.ajax({
            type: "put",
            url: "/ged/select", 
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(id),
            success: function (response) {
                that.displayGedStats(() => {
                    that.displayCorrectImportUpdateSections();
                });
                
            },
            error: (e) => that.console.printError(e)
        });
    },

    displayGedStats: function (success) {
        var that = this;

        $.ajax({
            url: "./info/gedfiles/",
            type: "get", //send it through get method            
            success: function (result) {
                that.console.displayGedStats(result, that);

                if (success)
                    success();
            },
            error: (e) => that.console.printError(e)
        });

        return true;
    },

    displayPlaceStats: function () {
        var that = this;

        $.ajax({
            url: "./info/places/",
            type: "get", //send it through get method            
            success: function (result) {
                that.console.displayPlaceStats(result);
            },
            error: (e) => that.console.printError(e)
        });

        return true;
    },

    // add persons locations into places cache
    importGed: function () {

        var that = this;


        // Checking whether FormData is available in browser  
        if (window.FormData !== undefined) {

            var fileUpload = $("#gedUpload").get(0);
            var files = fileUpload.files;

            var formData = new FormData();
            var tags = '';

            for (var i = 0; i != files.length; i++) {
                formData.append("files", files[i]);
                tags += files[i].name + '|';
            }
 
            formData.append("tags", tags);

            //data.append(files[i].name, files[i]);
            $.ajax({
                url: '/ged/add',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: formData,
                success: function (result) {
                    that.displayGedStats(() => {
                        that.displayCorrectImportUpdateSections();
                    });
                },
                error: (e) => that.console.printError(e)
            });
        } else {
          //  alert("FormData is not supported.");

            that.console.printOutputLine("FormData is not supported.");
        }  
        return true;
    },

    // add persons locations into places cache
    addMissingPlaces: function () {

        var that = this;
         
        $.ajax({
            type: "post",
            url: "/data/persons/locations",
            contentType: "application/json",
            dataType: "json", 
            success: function (response) {
                that.displayPlaceStats();
            },
            error: (e) => that.console.printError(e)
        });
        return true;
    },

    updateLocations: function () {

        var that = this;

        var Upload = {
            Value: 'setOriginPerson'
        };

        $.ajax({
            type: "put",
            url: "/data/persons/locations",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayPeopleStats();
            },
            error: (e) => that.console.printError(e)

        });
        return true;
    },

    importPersons: function () {

        var that = this;

        var Upload = {  };

        $.ajax({
            type: "post",
            url: "/data/persons/add",  
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayPeopleStats();

                that.displayGedStats(() => {
                    that.displayCorrectImportUpdateSections();
                });

            },
            error: (e) => that.console.printError(e)

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
                that.displayPeopleStats();
            },
            error: (e) => that.console.printError(e)

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
                that.displayPeopleStats();
            },
            error: (e) => that.console.printError(e)


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
                that.displayPeopleStats();
            },
            error: (e) => that.console.printError(e)
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
                that.displayPeopleStats();
            },
            error: (e) => that.console.printError(e)
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
                that.displayPeopleStats();
            }

        });
        return true;
    },

    //geocode

    saveGeoCodedLocationToServer: function (placeLookup) {
        var that = this;

        $.ajax({
            type: "post",
            url: "./geocode", // "/api/controllerName/methodName"
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(placeLookup),  //the parameter in method
            error: (e) => that.console.printError(e)
        });

         

    },

    updatePlaceMetadata: function () {

        var that = this;

        var Upload = {
            Value: 'updatePlaceMetadata'
        };

        $.ajax({
            type: "put",
            url: "./geocode",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(Upload),
            success: function (response) {
                that.displayPlaceStats();
            },
            error: (e) => that.console.printError(e)
        });

        return true;
    },

    getUnEncodedLocationsFromServer: function () {
       
        var sh = this;
        var printBasic = this.console.printOutputLine;
        
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
            error: (e) => that.console.printError(e)
        });



        return true;
    },

    start: function () {
        //https://www.geoapify.com/exceeding-google-geocoding-api-rate-limit-with-over_query_limit-status
        var sh = this;
        sh.count = 0; 
        var printOutputLine = this.console.printOutputLine;
        var printGeoCodeProgressCount = this.console.printGeoCodeProgressCount;
        var printTrace = this.console.printTrace;

        var geocoder = new google.maps.Geocoder();

        var idx = 0
     
        var searchAddress = () => {

            if (sh.data && sh.data.length == idx) {
                sh.displayPlaceStats();
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
                        }, 1000);
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