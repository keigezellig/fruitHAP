/*
 * Configuration and helpers
 */
var namespace = '/sensoralerts';
var connectionscounter = 0;
var serverport = 8989;
var rabbitExchange = 'FruitHAP_PubSubExchange';
var topic = 'alerts';

/*
 * Project dependencies
 */
var io = require('socket.io')(serverport);
var rabbitcontext = new require('rabbit.js').createContext('amqp://admin:admin@192.168.1.80');

/*
 * Collects data
 */

// Implement the methods to handle the new events
function newEventCallback(eventName, msg) {

	// Invokes propagation
	propagatesEvent(eventName, JSON.stringify(msg));
}

// Connection to external data sources
function connectToExternalSources() {
	
    var sub = rabbitcontext.socket('SUB', {routing: 'topic', persistent: true});
    sub.setEncoding('utf8');

    sub.on('data', function(sensorAlert) {

        console.log("received alert..");
        var alertObject = JSON.parse(sensorAlert);
		newEventCallback(alertObject.SensorName, alertObject);
    });

    sub.connect(rabbitExchange,topic);

}

/*
 * Data propagation
 */

// Propagates event through all the connected clients
function propagatesEvent(eventName, event) {
	if (connectionscounter>0) {
		io.of(namespace).emit(eventName, event);
		console.log("New event propagated in: Namespace='%s' EventName='%s' Event='%s'", namespace, eventName, event);
	}
}

/*
 * Handle Sockets.io connections
 */

// Event handlers
io.of(namespace).on('connection', function(socket) {
	
	console.log("New client connected.");
	
	// Do some logic for every new connection
	connectionscounter++;
	
	// On subscribe events join client to room
    socket.on('subscribe', function(room) {
    	socket.join(room);
    	console.log("Client joined room: " + room);
    });
        
    // On disconnect events
    socket.on('disconnect', function(socket) {
    	console.log("Client disconnect.");
    	connectionscounter--;
    });
    
});

/*
 *  Run
 */
console.log("Starting Node.js server with namespace='%s'", namespace);
connectToExternalSources();
