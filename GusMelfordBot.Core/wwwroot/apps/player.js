let playerPath = "apps/player.html";
let baseUrl = document.location.href.replace(playerPath, "");
let isVideoChanging = false;
let degreeOfCoup = 450;

async function initPlayer() {
    document.addEventListener("keyup", keyDownHandler);
    await executeRequest("player/start");
    createElementVideoInfo();
    
    let information = await executeRequest("systemData");
    document.title = information["playerInformation"]["name"] + " v" + information["playerInformation"]["version"];
    
    let playerTitle = document.getElementById("player-title");
    playerTitle.innerText = information["playerInformation"]["name"] + " v" + information["playerInformation"]["version"];
}

async function keyDownHandler(event) {
    if(!isVideoChanging){
        isVideoChanging = true;
        
        if (event.key === "x" || event.key === "ArrowRight" || event.key === "X") {
            await changeVideo("player/video/new/next");
        }
        else if (event.key === "z" || event.key === "ArrowLeft" || event.key === "Z") {
            await changeVideo("player/video/new/prev");
        }
        else if(event.key === "r"){
            degreeOfCoup -= 90;
            if(degreeOfCoup === 0){
                degreeOfCoup = 450;
            }
            
            document.getElementById("video-block").style.transform = "rotate(" + degreeOfCoup + "deg)";
        }

        isVideoChanging = false;
    }
}

async function changeVideo(methodName) {
    let videoBlocks = document.getElementsByClassName("video-block");
    for (let i = 0; i < videoBlocks.length; i++) {
        videoBlocks[i].remove();
    }

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
    if(div !== null){
        div.remove();
    }
    
    div = document.createElement("div");
    div.setAttribute("id", "video-info");

    for (let prop in info) {
        console.log("info." + prop + " = " + info[prop]);
        let p = document.createElement("p");
        p.innerText = info[prop];
        div.appendChild(p);
    }
    
    let infoContainer = document.getElementById("info");
    infoContainer.appendChild(div);
}

async function executeRequest(requestUrl) {
    let response = await fetch(baseUrl + requestUrl);
    return await response.json();
}