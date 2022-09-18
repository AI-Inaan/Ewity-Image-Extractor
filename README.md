# Ewity Image Extractor 
this is a tool i wrote to download all the images uploaded to ewity. ewity does not provide a proper way to export products and url to items so this is a dirty work around for getting those images. 


## Usage

this is an explanation on how make ewitydata.json

for starters you need to be logged into ewity 
open inspect element and check the network tab

look for a request made to ewity servers and try to locate the following field in the headers

"authorization: Bearer xxxxxxxxxxxxxxxxxxxxxxxx"

note down the following token. it will be needed later

next you will need to find the location ID
here is the code on how to do that

```
fetch("https://app.ewitypos.com/api/v1/locations", {
  "headers": {
    "accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,/;q=0.8",
    "accept-language": "en-US,en;q=0.9",
    "authorization": "Bearer xxxxxxxxxxxxxxxxxxx",
    "sec-ch-ua": "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"",
    "sec-ch-ua-mobile": "?0",
    "sec-ch-ua-platform": "\"Windows\"",
    "sec-fetch-dest": "empty",
    "sec-fetch-mode": "cors",
    "sec-fetch-site": "same-origin",
    "x-client": "Web",
    "x-pos-client-id": "a0c5bf92-e8bc-4db1-894e-b749f9d0b578"
  },
  "referrer": "https://app.ewitypos.com/products/all",
  "referrerPolicy": "strict-origin-when-cross-origin",
  "body": null,
  "method": "GET",
  "mode": "cors",
  "credentials": "include"
}).then(response => response.text()).then(result => console.log(result)).catch(error => console.log('error', error));
```

remember to replace authorization with the auth key we noted down before
to run this code go to inspect element and paste it into the console and hit enter
this should return json data regarding locations

```
{"data":
[{"id":1234,
"name":"",
"display_name":"",
"type":"retail","tax_number":"",
"tax_activity_number":"",
"street_address":"",
"city":"'",
"":"",
"phone":"",
"bill_note":null,
"price_level":null}],
"search":{"searches":[]}
}
```

what we are intrested in is the id so take note of that

```
fetch("https://app.ewitypos.com/api/v1/products/locations/{ID}/full-list", {
  "headers": {
    "accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,/;q=0.8",
    "accept-language": "en-US,en;q=0.9",
    "authorization": "Bearer xxxxxxxxxxxxxxxxxxxxxx",
    "sec-ch-ua": "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"",
    "sec-ch-ua-mobile": "?0",
    "sec-ch-ua-platform": "\"Windows\"",
    "sec-fetch-dest": "empty",
    "sec-fetch-mode": "cors",
    "sec-fetch-site": "same-origin",
    "x-client": "Web",
    "x-pos-client-id": "a0c5bf92-e8bc-4db1-894e-b749f9d0b578"
  },
  "referrer": "https://app.ewitypos.com/products/all",
  "referrerPolicy": "strict-origin-when-cross-origin",
  "body": null,
  "method": "GET",
  "mode": "cors",
  "credentials": "include"
}).then(response => response.text()).then(result => console.log(result)).catch(error => console.log('error', error));
```

take the following code and replace {ID} with the id we got and also make sure to enter in your auth code in the header
once you do this and run the command in the console you should get a large json response. just copy it and paste it into
a file with the name ewitydata.json and youre good to go

## why is getting the json data not automated
its best if any and all communication with a POS system is handled by an IT staff who knows what they are doing and knows exactly what commands is being ran. also api access is a paid feature so i didnt want this to be full on wrapper to bypass their paid features. just a simple tool to get me the images whenever i need them. 

Made with ♥ in Maldives
