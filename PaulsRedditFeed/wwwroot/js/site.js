function monitorStats() {
    console.log("starting stats monitoring");
    const statsHub = new signalR.HubConnectionBuilder().withUrl("/stats").build();
    statsHub.on("ReceiveMessage", async (statsJson) => {
        const subreddits = JSON.parse(statsJson);

        // Get the view with the rendered data since we are using server-side MVC
        const response = await fetch("Subreddits", {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(subreddits),
        });

        // Update the view with the correct information
        const listElement = document.getElementById("subreddit-list");
        listElement.innerHTML = await response.text();
    });
    statsHub.start();
}

monitorStats();
