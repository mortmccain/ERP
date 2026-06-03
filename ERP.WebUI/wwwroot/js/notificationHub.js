// Connects to the NotificationHub and calls a .NET method on the Blazor component
// when a "RefreshData" message arrives.
 function connectToHub(dotNetHelper, hubUrl, methodName) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    connection.on("RefreshData", (dataType) => {
        dotNetHelper.invokeMethodAsync(methodName, dataType);
    });

    connection.start().then(() => {
        console.log("NotificationHub connected");
    }).catch(err => console.error("Hub connection error:", err));

    // Return the connection so it can be stopped later if needed
    return connection;
}