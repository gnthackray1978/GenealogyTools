"use strict";

var failures = [];

function start(startnumber) {
  
    var geocoder = new google.maps.Geocoder();

    var idx = startnumber;

    var dataSection = [];

    while (idx < (startnumber +250)) {
        dataSection.push(data[idx]);
        idx++;
    }

    idx = 0;

    var searchAddress = (d) => {

        if (d.PlaceFormatted)
            console.log(d.PlaceFormatted);

        document.getElementById("progress").innerHTML = idx;

        geocoder.geocode({
            address: d.PlaceFormatted
        }, (results, status) => {

            if (status == "OVER_QUERY_LIMIT") {
                console.log('failed trying again');
                setTimeout(function () {                   
                    searchAddress(dataSection[idx]);
                }, 3000);
            }
            else {

                var element = document.createElement("p");
                var result = {
                    id: d.PlaceId,
                    query: d.address,
                    results: results
                };

                element.appendChild(document.createTextNode(JSON.stringify(result)));
                document.getElementById('output').appendChild(element);

                setTimeout(function () {
                    idx++;
                    searchAddress(dataSection[idx]);
                }, 500);
            }
        });

    };
     
    searchAddress(dataSection[idx]);
  
    

}

function initMap() {
    console.log("initMap");    
}