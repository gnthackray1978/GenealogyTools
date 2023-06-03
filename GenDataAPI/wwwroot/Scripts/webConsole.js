function WebConsole() {

}

//var printOutput = (message) => {
//    wc.printOutputLine(message);
//};

//var printTrace = (message) => {
//    wc.printTrace(message);
//};

//var printTraceLine = (message) => {
//    wc.printTraceLine(message);
//};

//var printErrorLine = (message) => {
//    wc.printErrorLine(message);
//};

WebConsole.prototype = {
    printOutputLine: function (message) {

        var last = $("#output").last();

        //we have a last element
        if (last && last.length >0) {
            var contents = last[0].innerText;

            //it includes the message
            if (contents.includes(message)) {

                var parts = contents.split('[');

                if (parts.length > 1) {
                    var r = /\d+/;
                    var count = parts[1].match(r);

                    if (count.length > 0) {
                        var num = Number(count[0]);

                        num++;

                        message = message + '[' + num + ']';

                        last[0].innerText = message;
                    }
                } else //no
                {
                    message = message + '[2]';
                    last[0].innerText = message;
                }
            } else {

                $('#output').append('<div>' + message + '</div>');
            }


        } else {
            $('#output').append('<div>' + message + '</div>');
        }

        return true;
    },
  
    displayPeopleStats: function (statsObject) {
        document.getElementById("dupeCount").innerHTML = 'Dupes:' + statsObject.dupeEntryCount;
        document.getElementById("originCount").innerHTML = 'Origin Mappings:' + statsObject.originMappingCount;
        document.getElementById("personCount").innerHTML = 'Persons: '+statsObject.personViewCount;
        document.getElementById("marriagecount").innerHTML = 'Marriages: ' + statsObject.marriagesCount;
        document.getElementById("treerecordcount").innerHTML = 'Match Trees: ' + statsObject.treeRecordCount;
    },  
    displayPlaceStats: function (statsObject) {
        document.getElementById("placesCount").innerHTML = 'Places: ' + statsObject.placesCount;
        document.getElementById("incompleteCount").innerHTML = 'Bad Names: ' + statsObject.badLocationsCount;
        document.getElementById("unsearchedCount").innerHTML = 'Unsearched: ' + statsObject.unsearched;
        document.getElementById("notfoundCount").innerHTML = 'Not Found: ' + statsObject.notFound;
    },  

    printGeoCodeProgressCount: function (number) {  
        document.getElementById("geocodecount").innerHTML = number;
    },

    printTrace: function (number) {
        document.getElementById("debug-progress").innerHTML = number; 
    },
    printTraceLine: function (message) {
        $('#debug-trace').append('<div>' + message + '</div>');
    },

    printAddressToOutput: function (search, input) {
        var output = 'failed';

        if (input && input.length > 0) {
            output = input[0].formatted_address;
            this.printTrace(search + ' ' + output);           
        }
        else {
            this.printOutputLine(search + ' ' +output);
        }
    }
}