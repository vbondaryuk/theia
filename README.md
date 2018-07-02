# Theia

Theia is system for dynamic calculation objects based on Drools Rule Engine

  - Objects in JSON format
  - Business Rules are rules on DRL (Drools Rule Language) format
  - RestApi for communication between Theia and your application
  - Easy deploy ensure docker, see file docker-compose.prod.yml

## 1. Usages

Request JSON has to format like:
```json
{  
   "Rules":[  ],
   "CalculationObjects":[  ]
}
```
| Field | Require | Description |
| ------ | ------ |------ |
| Rules | yes | Array rule (p. 1.1) |
| CalculationObjects | yes | Array object for calculation (p. 1.2) |

Response JSON has format:
```json
{  
   "FiredRules": 0,
   "CalculationObjects":[  ]
}
```
| Field | Description |
| ------ | ------ |
| FiredRules | Count fired rules |
| CalculationObjects | Array calculated objects (p. 1.2) |

### 1.1 Rules
Field **Rules** contains array rules:
```json
{  
    "Priority":0,
    "Source":""
}
```
| Field | Require | Description |
| ------ | ------ |------ |
| Priority | no (default: 0) | Rule calculation priority. When system calculates rules, Firstly will be calculated rules with highest priority, then with less piority. Rules with equals priority will calculated together. |
| Source | yes | Source code rule. Field is string. Rule in DRL format |

This example illustrates a simple rule with filled priority:
```json
"Rules":[  
      {  
         "Priority":1,
         "Source":"
dialect \"mvel\"
rule \"test\" 
when 
    $pe:Person() 
then      
    $pe.Age = 1111;
end"
      }
]      
```

In source code you haven't to put **package** and **import** for object, because you can use only objects from JSON.

### 1.2 Objects (CalculationObjects)

Contains array objects. It has to have format like:
```json
{  
    "RootClassName":"Person",
    "Schema":"",
    "Data":[  
        {  
           "Age":38,
           "FirstName":"Kelly"
        }
    ]
}
```

| Field | Require | Description |
| ------ | -- | ------ |
| RootClassName | yes | **root** class name. You have to put it because it's impossible retrieve base class name from JSON |
| Schema | no | Json schema's object. Recommended to put it if you have large object's array(it reduces calculation time) |
| Data | yes | object's array. Only one type objects possible. Embedded objects are possible |

About json schema you can see:


  - [Documentation](https://spacetelescope.github.io/understanding-json-schema/index.html)
  - [Online Editor](https://jsonschema.net/#/editor)
  - **Recommended** creates json schema with theia.jsonschemaservice(p. 2)

This example illustrates main object with two embedded objects(array: tags and object: dimensions)
```json
{  
    "RootClassName":"Coordinate",
    "Data":[  
        {  
           "id":2,
           "name":"An ice sculpture",
           "price":12.50,
           "tags":[  
                  "cold",
                  "ice"
               ],
           "dimensions":{  
              "length":7.0,
              "width":12.0,
              "height":9.5,
           }
        }
    ]
}
```

In rules you can use this objects by fields: **tags**, **dimensions**

### 2. JSON Schema creation
For creation JSON Schema you can use the theia.jsonschemaservice, it's available in [host]:[port]/jsonschema
where host/port you can set in config files. If you will use docker containers it will set up automatically.

This services accessed by using **post** method and read json from body:
Example:
```sh
POST /jsonschema HTTP/1.1
Host: [host]:[port]
Content-Type: application/json
Cache-Control: no-cache

{  
   "Rules":{}
}
```
response will be json schema for this object.

### 3. Calculation query examples
 - Query with example creation new object from rule, priority and DRL aggregation function
request:
```json
{  
   "Rules":[  
      {  
         "Priority":0,
         "Source":"
dialect \"mvel\"   
rule \"test\" 
when 
	$pe:Person() 
	personCount : Number( ) from accumulate ( $person : Person(Age > 30), count($person))      
then      
	$pe.Count = personCount;
end"
      },
 {  
         "Priority":1,
         "Source":"
dialect \"mvel\"   
rule \"test\"
	no-loop true   
when 
	$pe:Person(Age < 25) 
then      
	Person person = new Person();
	person.Age = 999;
	insert(person);
end"
      }
   ],
   "CalculationObjects":[  
      {  
         "RootClassName":"Person",
         "Schema":"",
         "Data":[  
            {  
               "Age":24,
               "FirstName":"Kelly",
               "LastName":"Tanner",
               "gender":"male",
               "company":"SKYBOLD",
               "email":"kellytanner@skybold.com",
               "phone":"+1 (853) 470-2232",
               "address":"616 Verona Place, Whipholt, Pennsylvania, 7892",
               "registered":"2015-09-26T03:13:40 -06:00",
               "Count":0
            }
         ]
      }
   ]
}
```
 - first rule will create new object Person and  will add it to calculation session.
 - aggregation function will find and calculate count of person, whose age less then 25. 
 - update all Person's field(Count)
 
response:
```json
{
  "FiredRules": 3,
  "CalculationObjects": [
    {
      "RootClassName": "Person",
      "Schema": null,
      "Data": [
        {
          "Age": 24,
          "FirstName": "Kelly",
          "LastName": "Tanner",
          "gender": "male",
          "company": "SKYBOLD",
          "email": "kellytanner@skybold.com",
          "phone": "+1 (853) 470-2232",
          "address": "616 Verona Place, Whipholt, Pennsylvania, 7892",
          "registered": "2015-09-26T03:13:40 -06:00",
          "Count": 1
        },
        {
          "Age": 999,
          "FirstName": null,
          "LastName": null,
          "gender": null,
          "company": null,
          "email": null,
          "phone": null,
          "address": null,
          "registered": null,
          "Count": 1
        }
      ]
    }
  ]
}
```

- Query with example embeded objects, DRL aggregation function, and additional function **roundFunc**

request:
```json
{  
   "Rules":[        
	  {
        "Source":"  
function double roundFunc( double a, int scale ){
  return cli.System.Math.Round( a, scale );
}

rule \"Longitude_TEST\"
dialect \"mvel\"
when
	$coordinate : Coordinate()
	$ds : dimensions(width == 1.0) from $coordinate.dimensions
	$sumLatitude : Number( ) from accumulate ( $coord : Coordinate(), sum($coord.warehouseLocation.latitude))
	$sumLongitude : Number( ) from accumulate ( $coord : Coordinate(), sum($coord.warehouseLocation.longitude))
then
	$ds.zindex = $sumLatitude;
	$ds.zindex2 = roundFunc($sumLongitude, 2);
end" 
 }
   ],
   "CalculationObjects":[  
      {  
         "RootClassName":"Coordinate",
         "Data":[  
            {  
               "id":2,
               "name":"An ice sculpture",
               "price":12.50,
               "tags":[  
                  "cold",
                  "ice"
               ],
               "dimensions":{  
                  "length":7.0,
                  "width":12.0,
                  "height":9.5,
		  "zindex":0,
	          "zindex2":0
               },
               "warehouseLocation":{  
                  "latitude":-78.75,
                  "longitude":20.4
               }
            },
            {  
               "id":3,
               "name":"A blue mouse",
               "price":25.50,
               "dimensions":{  
                  "length":3.1,
                  "width":1.0,
                  "height":1.0,
		  "zindex":0,
		  "zindex2":0
               },
               "warehouseLocation":{  
                  "latitude":54.4,
                  "longitude":-32.7
               }
            }
         ]
      }
   ]
}
```

response:
```json
{
  "FiredRules": 1,
  "CalculationObjects": [
    {
      "RootClassName": "Coordinate",
      "Schema": null,
      "Data": [
        {
          "id": 2,
          "name": "An ice sculpture",
          "price": 12.5,
          "tags": [
            "cold",
            "ice"
          ],
          "dimensions": {
            "length": 7,
            "width": 12,
            "height": 9.5,
            "zindex": 0,
            "zindex2": 0
          },
          "warehouseLocation": {
            "latitude": -78.75,
            "longitude": 20.4
          }
        },
        {
          "id": 3,
          "name": "A blue mouse",
          "price": 25.5,
          "tags": null,
          "dimensions": {
            "length": 3.1,
            "width": 1,
            "height": 1,
            "zindex": -24.35,
            "zindex2": -12.3
          },
          "warehouseLocation": {
            "latitude": 54.4,
            "longitude": -32.7
          }
        }
      ]
    }
  ]
}
```
In this example you can see method from C# language **cli.System.Math.Round**. For use this opportunity you have to use construction like: [cli].[namespace].[class].[method]
