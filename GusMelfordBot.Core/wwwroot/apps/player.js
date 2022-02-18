let playerPath = "apps/player.html";
let baseUrl = document.location.href.replace(playerPath, "");
let isKeyInputActive = false;
let degreeOfCoup = 360;

async function initPlayer() {
    await fetch(baseUrl + "player/video/reset");
    document.addEventListener("keydown", keyDownHandler);
    createElementVideoInfo();
    
    let information = await executeRequest("info");
    document.title = information["playerInformation"]["name"] + " v" + information["playerInformation"]["version"];
    
    let playerTitle = document.getElementById("player-title");
    playerTitle.innerText = information["playerInformation"]["name"] + " v" + information["playerInformation"]["version"];
}

async function keyDownHandler(event) {
    if(!isKeyInputActive){
        isKeyInputActive = true;
        
        if (event.key === "x" || event.key === "ArrowRight" || event.key === "X") {
            await changeVideo("player/video/next");
        }
        else if (event.key === "z" || event.key === "ArrowLeft" || event.key === "Z") {
            await changeVideo("player/video/prev");
        }
        else if(event.key === "r" || event.key === "R" || event.key === "К" || event.key === "к"){
            rotateVideo();
        }
        
        isKeyInputActive = false;
    }
}

function rotateVideo(){
    degreeOfCoup -= 90;
    if(degreeOfCoup === 0){
        degreeOfCoup = 360;
    }

    document.getElementById("video-block")
        .style.transform = "rotate(" + degreeOfCoup + "deg)";
}

async function changeVideo(methodName) {
    
    let videoBlocks = document.getElementsByClassName("video-block");
    for (let i = 0; i < videoBlocks.length; i++) {
        videoBlocks[i].remove();
    }
    
    degreeOfCoup = 360;
    createElementVideoInfo(await executeRequest(methodName));
    let video = createElementVideo();
    let source = createElementVideoSource("player/video/current" + "?updated=" + new Date());
    
    let videoContainer = document.getElementById("video-container");
    video.appendChild(source);
    videoContainer.appendChild(video);
    document.body.requestFullscreen().then(r => r);
}

function createElementVideo(){
    let video = document.createElement("video");
    video.setAttribute("id", "video-block");
    video.setAttribute("class", "video-block");
    video.setAttribute("name", "media");
    video.setAttribute("controls", "controls");
    video.setAttribute("autoplay", "autoplay");
    video.setAttribute("loop", "loop");
    return video;
}

function createElementVideoSource(methodName){
    let source = document.createElement("source");
    source.setAttribute("src", baseUrl + methodName);
    source.setAttribute("type", "video/mp4");
    return source;
}

function createElementVideoInfo(info){
    let div = document.getElementById("video-info");
    let divAccompanyingCommentary = document.getElementById("accompanyingCommentary");
    if(div !== null){
        div.remove();
    }

    if(divAccompanyingCommentary !== null){
        divAccompanyingCommentary.remove();
    }
    
    div = document.createElement("div");
    div.setAttribute("id", "video-info");
    
    for (let prop in info) {
        let p = document.createElement("p");
        p.innerText = info[prop];
        
        if(prop === "accompanyingCommentary"){
            divAccompanyingCommentary = document.createElement("div");
            divAccompanyingCommentary.setAttribute("id", "accompanyingCommentary")
            p.style.fontSize = "xx-large";
            p.style.color = "yellow";
            divAccompanyingCommentary.appendChild(p);
            document.getElementById("info-container").appendChild(divAccompanyingCommentary);
            continue;
        }
        div.appendChild(p);
    }
    
    let infoContainer = document.getElementById("info");
    infoContainer.appendChild(div);
}

async function executeRequest(requestUrl) {
    let response = await fetch(baseUrl + requestUrl);
    return await response.json();
}