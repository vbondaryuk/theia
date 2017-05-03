# Theia

Theia это система динамического расчета обьектов на основании бизнес правил.

  - Данные формата json
  - Правила расчета на языке бизнес правил Drools
  - RestApi позволяющее взаимодействовать с приложениями на любых языках

## 1. Использование

Входной json должен иметь следующий формат:
```json
{  
   "Rules":[  ],
   "CalculationObjects":[  ]
}
```
| Поле | Обязательно |Описание |
| ------ | ------ |------ |
| Rules | да | Массив правил (п. 1.1) |
| CalculationObjects | да | Массив обьектов для расчета (п. 1.2) |

Ответный json имеет следующий формат:
```json
{  
   "FiredRules": 0,
   "CalculationObjects":[  ]
}
```
| Поле | Описание |
| ------ | ------ |
| FiredRules | количество отработанных правил |
| CalculationObjects | Массив обьектов для расчета (п. 1.2) |

### 1.1 Правила (Rules)
Поле Rules содержит массив правил следующего формата:
```json
{  
    "Priority":0,
    "Source":""
}
```
| Поле | Обязательно |Описание |
| ------ | ------ |------ |
| Priority | нет (default: 0) |Приоритет выполнения правила. Правило с большим приоритетом будет выполнятся в первую очередь. Правила с одинаковым приоритетом будут выполняться одновременно |
| Source | да | Исходный код правила в формате string |

Пример заполненного правила:
```json
"Rules":[  
      {  
         "Priority":0,
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

В исходном коде правила не нужно указывать **package** и **import** для объектов. потому что будут добавлены только те объекты которые указаны в исходном формате.

### 1.2 Объекты (CalculationObjects)

Поле CalculationObjects содержит массив объектов следующего формата:
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

| Поле | Обязательно |Описание |
| ------ | -- | ------ |
| RootClassName | да | наименование **root** класса. Указывается так как из json понять наименование **root** класса невозможно. |
| Schema | нет | Json schema объекта. Рекомендуется указывать при больших объемах данных.|
| Data | да | массив объектов. Объекты должны быть **только**  одного типа. Вложенные объекты допускаются |

Подробнее о json schema можно посмотреть:
- [Документация](https://spacetelescope.github.io/understanding-json-schema/index.html)
- [Online Editor](https://jsonschema.net/#/editor)
- **Рекомендуется** Создание json schema при помощи локального сервиса(п. 2)

Пример с вложенными объектами:

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

в правилах к таким объектам можно будет обращаться по полям: **tags**, **dimensions**

### 2. Создание схем
Для создания схем на основании json создан сервис который работает по [host]:[port]/jsonschema
Данный сервис принимает **post** запрос с json в body:
Пример post запроса:
```sh
POST /jsonschema HTTP/1.1
Host: [host]:[post]
Content-Type: application/json
Cache-Control: no-cache

{  
   "Rules":{}
}
```

### 3. Примеры запросов
 - Запрос с созданием нового объекта и приоритизацией
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
	personCount : Number( ) from accumulate ( $pesron : Person(Age > 30), count($pesron))      
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
 - первое отрабатывает правило которое добавляет новый объект Person.
 - выполняется поиск количества объектов Person у которых Age < 25.
 - обновляются поле Count всех объектов Person
 
ответ:
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

- запрос с использованием вложенных объектов, аккумулирующих функций и дополнительной функции **round**
 запрос:
```json
{  
   "Rules":[        
	  {
		"Priority":0,
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
ответ
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
в данном примере можно отметить использование функции из языка C# **cli.System.Math.Round**. Для использования необходимо указывать [cli].[наименование функции с полным алиасом]