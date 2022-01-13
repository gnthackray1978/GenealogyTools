function WebConsole() {

}

WebConsole.prototype = {
    printBasic: function (message) {

        var last = $("#discussion").last();

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

                $('#discussion').append('<div>' + message + '</div>');
            }


        } else {
            $('#discussion').append('<div>' + message + '</div>');
        }

        return true;
    },


    printLocationCount: function(number) {
        document.getElementById("count").innerHTML = number;
    },

    printGeoCodeProgressCount: function (number) {  
        document.getElementById("geocodecount").innerHTML = number;
    },

    printProgressCount: function (number) {
        document.getElementById("progress").innerHTML = number; 
    },

    printToOutput: function (search, input) {

        var output = 'failed';

        if (input && input.length > 0) {
            output = input[0].formatted_address;

            document.getElementById("progress").innerHTML = search + ' ' +output; 
            //var element = document.createElement("p");
            //element.appendChild(document.createTextNode(output));
            //document.getElementById('output').appendChild(element);
        }
        else {

            this.printBasic(search + ' ' +output);
        }

    }
}