let playerPath = "apps/player.html";
let baseUrl = document.location.href.replace(playerPath, "");
let isKeyInputActive = false;
let degreeOfCoup = 360;
let contents = [];
let cursor = -1;
let message;

async function Init(){
    fetch(baseUrl + "app/content/cache");
    document.addEventListener("keydown", keyDownHandler);

    let information = await executeRequest("system/info");
    document.title = information["systemName"] + " v" + information["systemVersion"];

    let playerTitle = document.getElementById("player-title");
    playerTitle.innerText = information["systemName"] + " Player" + " v1.6";

    contents = await executeRequest("app/content/info" + document.location.search);
    await changeVideo("next");
}

async function changeVideo(direction) {
    let videoBlocks = document.getElementsByClassName("video-block");
    for (let i = 0; i < videoBlocks.length; i++) {
        videoBlocks[i].remove();
    }
    degreeOfCoup = 360;
    
    updateCursor(direction);
    updateElementVideoInfo();

    let video = createElementVideo();
    let source = document.createElement("source");
    source.setAttribute("src", baseUrl + "app/content?contentId=" + contents[cursor]["id"]);
    source.setAttribute("type", "video/mp4");

    let videoContainer = document.getElementById("video-container");
    video.appendChild(source);
    videoContainer.appendChild(video);
    document.body.requestFullscreen().then(r => r);
}

function updateCursor(direction){
    if(direction === "next"){
        if(cursor + 1 >= contents.length){
            cursor = 0;
        }
        else {
            cursor++;
        }
    }
    else if(direction === "prev"){
        if(cursor - 1 <= -1){
            cursor = contents.length - 1;
        }
        else {
            cursor--;
        }
    }
}

async function keyDownHandler(event) {
    if(!isKeyInputActive){
        isKeyInputActive = true;
        
        if (event.key === "x" || event.key === "ArrowRight" || event.key === "X") {
            await changeVideo("next");
        }
        else if (event.key === "z" || event.key === "ArrowLeft" || event.key === "Z") {
            await changeVideo("prev");
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

function updateElementVideoInfo(){
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
    
    let content = contents[cursor];
    div.appendChild(createP(content["id"]));
    div.appendChild(createP(content["senderName"]));
    div.appendChild(createP(content["description"]));
    div.appendChild(createA(content["refererLink"]));
    updatePAccompanyingCommentary(createP(content["accompanyingCommentary"]), divAccompanyingCommentary);
    
    let infoContainer = document.getElementById("info");
    infoContainer.appendChild(div);
}

function createP(innerText){
    let p = document.createElement("p");
    p.innerText = innerText;
    return p;
}

function createA(innerText){
    let a = document.createElement("a");
    a.innerText = innerText;
    a.href = innerText;
    return a;
}

function updatePAccompanyingCommentary(p, divAccompanyingCommentary){
    divAccompanyingCommentary = document.createElement("div");
    divAccompanyingCommentary.setAttribute("id", "accompanyingCommentary")
    p.style.fontSize = "xx-large";
    p.style.color = "yellow";
    divAccompanyingCommentary.appendChild(p);
    document.getElementById("info-container").appendChild(divAccompanyingCommentary);
}

async function executeRequest(requestUrl) {
    let response = await fetch(baseUrl + requestUrl);
    return await response.json();
}