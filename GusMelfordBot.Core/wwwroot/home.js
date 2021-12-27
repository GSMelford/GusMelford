let homePath = "home.html";
let baseUrl = document.location.href.replace(homePath, "");

async function initHomePage(){
    let information = await executeRequest("systemData");
    let botFullName = information.name + " v" + information.version;
    document.title = botFullName;
    
    let botTitle = document.getElementById("bot-title");
    botTitle.innerText = botFullName;

    let playerButton = document.getElementById("player-button");
    playerButton.value = "View " + information["playerInformation"]["count"] + " memes";
}

function redirectToPlayer(){
    document.location.href = baseUrl + "apps/player.html";
}

async function executeRequest(requestUrl) {
    let response = await fetch(baseUrl + requestUrl);
    return await response.json();
}