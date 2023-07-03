function WebConsole() {

}

WebConsole.prototype = {
    printOutputLine: function (message) {

        var last = $("#output").last();

        //we have a last element
        if (last && last.length > 0) {
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
        document.getElementById("personCount").innerHTML = 'Persons: ' + statsObject.personViewCount;
        document.getElementById("marriagecount").innerHTML = 'Marriages: ' + statsObject.marriagesCount;
        document.getElementById("treerecordcount").innerHTML = 'Match Trees: ' + statsObject.treeRecordCount;
    },
    displayPlaceStats: function (statsObject) {
        document.getElementById("placesCount").innerHTML = 'Places: ' + statsObject.placesCount;
        document.getElementById("incompleteCount").innerHTML = 'Bad Names: ' + statsObject.badLocationsCount;
        document.getElementById("unsearchedCount").innerHTML = 'Unsearched: ' + statsObject.unsearched;
        document.getElementById("notfoundCount").innerHTML = 'Not Found: ' + statsObject.notFound;
    },

    displayGedStats: function (statsObject,appObj) { 
        console.log('displayGedStats called');
        $('.del-ged-row').unbind('click');
        $('.sel-ged-row').unbind('click');
        $('#ged-file-list-body').empty();

        statsObject.forEach(function(current) {
            var dateImported = current.dateImported;
            var fileName = current.fileName;
            var fileSize = current.fileSize;
            var rowSelected = '<tr class = "selectedRow">';
            var rowUnSelected = '<tr class = "unselectedRow">';

            $('#ged-file-list').append((current.selected ? rowSelected : rowUnSelected)
                + '<td><a data-id= "' + current.id + '" class = "sel-ged-row" href= "#">' + fileName + '</a></td>'
                + '<td class = "tdFileSize">' + fileSize + '</td>'
                + '<td class = "tdDate">'+ dateImported + '</td>'
                + '<td><a data-id= "'+ current.id +'" class = "del-ged-row" href= "#">Delete</a></td>'
                + '</tr>');
        });

        $('.del-ged-row').bind('click', (x) => {
            appObj.deleteGed($(x.currentTarget).data('id'));
        });
        $('.sel-ged-row').bind('click', (x) => {
            appObj.selectGed($(x.currentTarget).data('id'));
        }
        );
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