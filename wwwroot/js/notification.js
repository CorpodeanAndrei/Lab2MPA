const messages = document.getElementById('messages');
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Notification")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on('newMessage', (sender, messageText) => {
    console.log(`${sender}:${messageText}`);
    const newMessage = document.createElement('li');
    newMessage.appendChild(document.createTextNode(`${sender}:${messageText}`));
    messages.appendChild(newMessage);
});

connection.start()
    .then(() => console.log('connected!'))
    .catch(console.error);
 );