var express = require('express');
var bodyParser = require("body-parser");
//var GenerateSchema = require('generate-schema');
var jsonSchemaGenerator = require('json-schema-generator');
var app = express();


app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

app.post('/jsonSchema', function(req, res) {
	var body = req.body;
    //var schema = GenerateSchema.json('Person', body);
	var schema = jsonSchemaGenerator(body);
    res.send(schema);
});

var server = app.listen(8081, function () {

  var host = server.address().address
  var port = server.address().port

  console.log("Example app listening at http://%s:%s", host, port)

})