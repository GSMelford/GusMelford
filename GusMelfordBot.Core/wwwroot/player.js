function start(){
    document.addEventListener("keyup", keyDownHandler);
}

function keyDownHandler(event){
    if(event.key === "z"){
        nextVideo();
    }
}

function nextVideo(){
    document.getElementById("video-block").remove();
    
    let video = document.createElement("video");
    video.setAttribute("id", "video-block");
    video.setAttribute("name", "media");
    video.setAttribute("controls", "controls");
    video.setAttribute("autoplay", "autoplay");
    video.setAttribute("loop", "loop");

    let source = document.createElement("source");
    source.setAttribute("src", "https://dev.tebot.site/gusBot/Player/video/new/next");
    source.setAttribute("type", "video/mp4");

    video.appendChild(source);
    document.getElementById("video-container").appendChild(video);
    document.body.requestFullscreen().then(r => r);
}



