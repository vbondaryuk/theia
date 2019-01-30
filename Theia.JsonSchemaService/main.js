const express = require('express');
const bodyParser = require("body-parser");
//var GenerateSchema = require('generate-schema');
const jsonSchemaGenerator = require('json-schema-generator');
const app = express();


app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

app.post('/jsonSchema', (req, res) => {
    const body = req.body;
    //var schema = GenerateSchema.json('Person', body);
    const schema = jsonSchemaGenerator(body);
    res.send(schema);
});

var server = app.listen(8081, () => {

    const host = server.address().address;
    const port = server.address().port;

    console.log("Schema generator service is listening at http://%s:%s", host, port)

})