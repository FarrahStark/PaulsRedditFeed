function monitorStats() {
    let subreddits = {};
    console.log("starting stats monitoring");
    const statsHub = new signalR.HubConnectionBuilder().withUrl("/stats").build();
    statsHub.on("ReceiveMessage", async (statsJson) => {
        var subreddit = JSON.parse(statsJson);
        subreddits[subreddit.Title] = subreddit;
        var values = Object.values(subreddits).sort((a, b) => a.Title.localeCompare(b.Title));
        // Get the view with the rendered data since we are using server-side MVC
        const response = await fetch("Subreddits", {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(values),
        });

        // Update the view with the correct information
        const listElement = document.getElementById("subreddit-list");
        listElement.innerHTML = await response.text();
    });
    statsHub.start();
}

monitorStats();
