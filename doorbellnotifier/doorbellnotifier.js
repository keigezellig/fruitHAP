var fs = require('fs');
var config = get_config(__dirname + "/config.json");
var PushBullet = require('pushbullet');
var pusher = new PushBullet(config.pushbullet.apikey);
var context = new require('rabbit.js').createContext(config.mq.connection_string);

var deviceParams = {channel_tag: config.pushbullet.channel};
var imgPath = "image.jpg";
var sub = context.socket('SUB', {routing: 'topic', persistent: true});
sub.connect(config.mq.exchange_name,config.mq.routing_key);
sub.setEncoding('utf8');
sub.on('data', function(alert)
{ 
    console.log("Received alert!");
  var alertObject = JSON.parse(alert);
  var timestamp = new Date(alertObject.Timestamp);

  if (alertObject.EncodedImage != null && alertObject.EncodedImage != "" )
  {
      base64_decode(alertObject['EncodedImage'], imgPath);
  }

    var noteBody = config.pushbullet.note_text+" "+timestamp;

    console.log("Sending note");
    pusher.note(deviceParams, config.pushbullet.note_title, noteBody, function(error, response)
  {
    console.log("Response: "+JSON.stringify(response));
    if (error)
    {
        console.log("Error while sending to push bullet: "+error.message);
    }
  });

  console.log("Sending image");
  pusher.file(deviceParams, imgPath, 'Camera image', function(error, response)
  {
      if (error)
      {
          console.log("Error while sending image to push bullet: "+error.message);
      }
      console.log("Deleting temp image file");
      fs.unlinkSync(imgPath);
  });
 
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
