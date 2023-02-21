async function renderStats() {
    console.log("rendering");
    const subreddits = [
        { Title: "Subreddit1", TopPostTitle: "Whoa dude!", ActiveUserCount: 2402 },
        { Title: "Subreddit2", TopPostTitle: "Candy!", ActiveUserCount: 15 },
        { Title: "Subreddit3", TopPostTitle: "Cat Tax", ActiveUserCount: 90241 },
    ]
    console.log(subreddits);

    const response = await fetch("Subreddits", {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(subreddits),
    });

    const listElement = document.getElementById("subreddit-list");
    listElement.innerHTML = await response.text();
}