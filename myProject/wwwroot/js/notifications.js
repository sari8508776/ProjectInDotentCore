// SignalR client (KS-style): listen for ReceiveActivity(username, action, itemName)
(function () {
    // Make sure the SignalR client script is loaded
    if (typeof signalR === 'undefined') {
        console.warn('SignalR client library not found. Include the signalr script before notifications.js');
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/activityHub', { accessTokenFactory: () => localStorage.getItem('token') })
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveActivity', (username, action, itemName) => {
        try {
            const text = `${username} ${action} ${itemName}`;
            console.log('Activity:', text);
            const toast = document.createElement('div');
            toast.className = 'signalr-toast';
            toast.textContent = text;
            toast.style.position = 'fixed';
            toast.style.right = '12px';
            toast.style.bottom = '12px';
            toast.style.background = 'rgba(0,0,0,0.8)';
            toast.style.color = '#fff';
            toast.style.padding = '8px 12px';
            toast.style.borderRadius = '6px';
            toast.style.zIndex = 9999;
            document.body.appendChild(toast);
            setTimeout(() => { try { toast.remove(); } catch (e) { } }, 5000);
        } catch (e) {
            console.error('Error handling ReceiveActivity', e);
        }
    });

    connection.start().then(() => console.log('SignalR connected to /activityHub')).catch(err => console.error('SignalR connection error:', err));
})();
