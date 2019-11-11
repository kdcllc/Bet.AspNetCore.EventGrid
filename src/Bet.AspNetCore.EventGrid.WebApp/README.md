# Bet.AspNetCore.EventGrid.WebApp

The Sample application that demonstrates usage of `Bet.AspNetCore.EventGrid` `Webhooks` middleware.

Make sure that `Websockets` are enabled for IIS hosting.

[Live Event Viewer](https://bet-eventgrid.azurewebsites.net/events/viewer)

https://bet-eventgrid.azurewebsites.net/webhooks

Event Grid topic created with Event Schema set to `Cloud Event Schema`

## Testing

```json
[
  {
    "topic": "/subscriptions/1234/resourceGroups/test-rs-gp/providers/Microsoft.EventGrid/topics/test-eg-topic",
    "subject": "Group.Employee",
    "id": "ab8d8ef9-b929-46d8-b205-d72113750da1",
    "eventType": "Group.Employee",
    "eventTime": "2019-04-28T18:51:17.7994409Z",
    "data":{
    	"Id": "12345",
    	"FullName": "hello"
    },
    "dataVersion": "2",
    "metadataVersion": "1"
  }
]
```