/*
 * Configuration and helpers
 */

var http = require('http').Server();
var io = require('socket.io')(http);
var connectionscounter = 0;


/*
 * Handle Sockets.io connections
 */

// Event handlers
io.of(config.namespace).on('connection', function (socket) {
    // Do some logic for every new connection
    connectionscounter++;
    console.log("New client connected. Number of connected clients %s", connectionscounter);
    
    // On subscribe events join client to room
    socket.on('subscribe', function (room) {
        socket.join(room);
        console.log("Client joined room: " + room);
    });
    
    // On disconnect events
    socket.on('disconnect', function (socket) {
        console.log("Client disconnect.");
        connectionscounter--;
    });
    
});



console.log("Load and check config");
var config = loadConfig(__dirname + "/config.json");


http.listen(config.serverPort, function () {
    console.log("Starting Node.js server with namespace='%s' on port %s", config.namespace, config.serverPort);
});

connectToExternalSources();



// Connection to external data sources
function connectToExternalSources() {
	
    var rabbitcontext = new require('rabbit.js').createContext(config.fruitHAPSettings.connectionString);
    rabbitcontext.on('ready', function () {
        
        console.log("Connected to rabbitmq server...");
        var sub = rabbitcontext.socket('SUB', { routing: 'topic', persistent: true });
        sub.setEncoding('utf8');
        
        sub.on('data', function (sensorAlert) {
            
            console.log("received alert..");
            var alertObject = JSON.parse(sensorAlert);
            newEventCallback(alertObject, config.namespace, config.rooms);
        });
        
        sub.connect(config.fruitHAPSettings.exchange, config.fruitHAPSettings.topic);
    });
}

/*
 * Collects data
 */
// Implement the methods to handle the new events
function newEventCallback(event, namespace, rooms) {
    
    sensorName = event.SensorName;
    room = getRoomForEvent(sensorName, rooms);
    
    if (room != null) {
        // Invokes propagation
        propagatesEvent(namespace, room, sensorName, JSON.stringify(event));
    }
}

/*
 * Data propagation
 */

// Propagates event through all the connected clients
function propagatesEvent(namespace, room, eventName, event) {
    if (connectionscounter > 0) {
        io.of(namespace).to(room).emit(eventName, event);
        console.log("New event propagated in: Namespace='%s', Room='%s', EventName='%s' Event='%s'", namespace, room, eventName, event);
    }
    else
        console.log("No connections");
}

function getRoomForEvent(sensorName, rooms)
{
    for (var room in rooms) {
        for (var sensor in rooms[room].sensors) {
            if (rooms[room].sensors[sensor] == sensorName) {
                return rooms[room].name;
            }
        }

    }

    return null;
}




function loadConfig(filename) {
    
    var nconf = require('nconf');
    
    nconf.use('file', { file: filename });
    nconf.load();
    
    console.log("Loaded configuration: " + JSON.stringify(nconf.get()));
    
    return nconf.get();
}

/*
 *  Run
 */

