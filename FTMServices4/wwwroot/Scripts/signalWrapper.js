function SignalWrapper() {
    
}

SignalWrapper.prototype = {
    run: function (signalR, fOutput) {



        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationhub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        async function start() {
            try {
                await connection.start();
                console.log("SignalR Connected.");
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        connection.on("Notify", function (user, message) {

            if (fOutput) {
                fOutput(user);
            }
            
        });

        connection.on("Update", function (user, message) {

            var listItem = $("ul").first();

            if (listItem) {
                listItem.html(user + '<br />');
            } else {
                $('#header').append(user + '<br />');
            }

        });

        connection.onclose(start);

        // Start the connection.
        start();


        return true;
    }
}