/**
 * Created by developer on 18-2-15.
 */
var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var rabbitcontext = new require('rabbit.js').createContext('amqp://admin:admin@192.168.1.80');

app.get('/', function(req, res){
    res.sendFile(__dirname+'/index.html');
});

http.listen(3000, function(){
    console.log('listening on *:3000');
});


io.on('connection', function(connection){

    connection.on('disconnect', function(){
        console.log('user disconnected');
        sub.close();
    });

    console.log('a user connected');
    var sub = rabbitcontext.socket('SUB', {routing: 'topic', persistent: true});
    sub.setEncoding('utf8');

    sub.on('data', function(alert) {

        console.log("received alert..");
        var alertObject = JSON.parse(alert);
        console.log("Sending alert to browser");
        connection.send(alertObject);

    });

    sub.connect('FruitHAPExchange','alerts');

});

// function to create file from base64 encoded string
function base64_decode(base64str, file) {
    // create buffer object from base64 encoded string, it is important to tell the constructor that the string is base64 encoded
    var bitmap = new Buffer(base64str, 'base64');
    // write buffer to file
    fs.writeFileSync(file, bitmap);
    console.log('******** File created from base64 encoded string ********');
}