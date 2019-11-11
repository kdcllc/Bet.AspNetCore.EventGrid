# Azure CLI and Azure Event Grid Topic


1. Register Even Grid Resource Provider with the Subscription

Check if the subscription has it register already

```bash
    az provider show --namespace Microsoft.EventGrid --query "registrationState"
```

If not run the registration and re-run the check.

```bash
    az provider register --namespace Microsoft.EventGrid
```

When registrationState is Registered, you're ready to continue.

2. Create a custom topic

List all locations available

```bash
    az account list-locations --output table
    az configure --defaults location=<location>
```

```bash
    # setup
    $subId= <subscription id>
    $rsGroup= <resource group name>
    $topicName= <topic name>

    # create event grid topic
    az eventgrid topic create --name $topicname -l centralus -g $rsGroup

    # retrieve event grid topic endpoint
    $endpoint=$(az eventgrid topic show --name $topicName -g $rsGroup --query "endpoint" --output tsv)

    # retrieve event grid topic key
    $key=$(az eventgrid topic key list --name $topicName -g $rsGroup --query "key1" --output tsv)

    # url of the web or func
    $appEndpoint=https://$sitename.azurewebsites.net/api/updates

    #
    $gridSubName
    az eventgrid event-subscription create \
      --source-resource-id "/subscriptions/$subId/resourceGroups/$rsGroup/providers/Microsoft.EventGrid/topics/$topicname"
      --name $gridSubName
      --endpoint $appEndpoint

```

## Resources

[Quickstart: Route custom events to web endpoint with Azure CLI and Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/custom-event-quickstart)
[Quickstart: Route custom events to web endpoint with the Azure portal and Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/custom-event-quickstart-portal)