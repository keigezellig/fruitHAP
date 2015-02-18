
var config = get_config("./config.json");

var apikey = "v1cULYbIkSKLywzH0b3k5xf3mgsuWyWJE2ujxvzW0Rqay";
//var apikey = "v1cULYbIkSKLywzH0b3k5xf3mgsuWyWJE2ujxvzW0Rqaz";
var channel = "mydoorbell";
var noteTitle = "Ding dong";
var deviceParams = {channel_tag: 'mydoorbell'};

var imgPath = "image.jpg";

var PushBullet = require('pushbullet');
var fs = require('fs');

var pusher = new PushBullet(apikey);


var context = new require('rabbit.js').createContext('amqp://admin:admin@192.168.1.80');
var sub = context.socket('SUB', {routing: 'topic', persistent: true});
sub.connect('FruitHAPExchange','alerts')
sub.setEncoding('utf8');
sub.on('data', function(alert)
{ 
    console.log("Received alert!");
  var alertObject = JSON.parse(alert);
  var timestamp = new Date(alertObject['Timestamp']);

  if (alertObject.EncodedImage != "") {
      base64_decode(alertObject['EncodedImage'], imgPath);
  }

    var noteBody = "The doorbell rang at "+timestamp;

    console.log("Sending note");
  pusher.note(deviceParams, noteTitle, noteBody, function(error, response)
  {
    console.log("Response: "+JSON.stringify(response));
    if (error)
    {
        console.log("Error while sending to push bullet: "+error.message);
    }
  });

  console.log("Sending image");
  pusher.file(deviceParams, imgPath, 'Camera image', function(error, response) {});
 
});


function get_config(filename)
{

    var nconf = require('nconf');

    nconf.use('file', { file: filename });
    nconf.load();

    console.log("Loaded configuration: "+ nconf.get())

    return nconf.get();

}

// function to create file from base64 encoded string
function base64_decode(base64str, file) {
    // create buffer object from base64 encoded string, it is important to tell the constructor that the string is base64 encoded
    var bitmap = new Buffer(base64str, 'base64');
    // write buffer to file
    fs.writeFileSync(file, bitmap);
    console.log('******** File created from base64 encoded string ********');
}
