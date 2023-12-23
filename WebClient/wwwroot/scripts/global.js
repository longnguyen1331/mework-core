function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}



function initWebViewer(pdfUrl) {
    const viewerElement = document.getElementById('viewer');
    console.log('pdfurl:', pdfUrl);
    WebViewer({
        path: 'lib',
        initialDoc: pdfUrl, // replace with your own PDF file
    }, viewerElement).then((instance) => {
    })
}

function printAsFile(elementId) {

    var iframe = document.getElementById(elementId);
    var iframeWindow = iframe.contentWindow;
    iframeWindow.print();
}


function downloadURI(uri, name) {
    var link = document.createElement("a");
    // If you don't know the name or want to use
    // the webserver default set name = ''
    link.setAttribute('download', name);
    link.href = uri;
    document.body.appendChild(link);
    link.click();
    link.remove();
}

function downloadDifferentDomain(uri, name) {
    fetch(uri, {
        method: 'GET'
    }).then(resp => resp.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = name; // the filename you want
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
}


function setTitle(title) {
    document.title = title;
}


function setFavicon(path) {

    const link = document.querySelector("link[rel='icon']");
    link.href = path;
}




///////////////////

// var myState = {
//     pdf: null,
//     currentPage: 1,
//     zoom: 1,
//     route:0
// }
// function RunPdf(url){
//
//     url = "https://localhost:7083/temp.pdf";
//     pdfjsLib.getDocument(url).then((pdf) => {
//
//         myState.pdf = pdf;
//         render();
//     });
// }
//
// var iframe = document.getElementById('bv-1999');
//
// function render() {
//     myState.pdf.getPage(myState.currentPage).then((page) => {
//
//         var canvas = document.getElementById("pdf_renderer");
//         var ctx = canvas.getContext('2d');
//
//
//
//
//         var viewport = page.getViewport(myState.zoom,myState.route);
//         canvas.width = viewport.width;
//         canvas.height = viewport.height;
//
//
//         page.render({
//             canvasContext: ctx,
//             viewport: viewport,
//             route : myState.route
//         }).promise.then(function(x) {
//             iframe.contentDocument.body.appendChild(x);
//         });
//     });
// }
//
//
// function go_next(){
//     if(myState.pdf == null || myState.currentPage > myState.pdf._pdfInfo.numPages)
//         return;
//     myState.currentPage += 1;
//     document.getElementById("current_page").value = myState.currentPage;
//     render();
// }
//
// function go_previous(){
//     if(myState.pdf == null || myState.currentPage == 1)
//         return;
//     myState.currentPage -= 1;
//     document.getElementById("current_page").value = myState.currentPage;
//     render();
// }
//
// function zoom_in(){
//     if(myState.pdf == null) return;
//     myState.zoom += 0.5;
//     render();
// }
// function zoom_out(){
//     if(myState.pdf == null) return;
//     myState.zoom -= 0.5;
//     render();
// }
//
// function routeNext(){
//     if(myState.pdf == null) return;
//     myState.route +=90;
//     render();
// }

// create listener
(function () {
    "use strict";
    document.addEventListener("DOMContentLoaded", function (event) {
        window.parent.postMessage(
            {
                height: getActualWidth()
            },
            '*'
        );
    });
})();

function getActualWidth() {
    var actualWidth = window.innerWidth ||
        document.documentElement.clientWidth ||
        document.body.clientWidth ||
        document.body.offsetWidth;

    return actualWidth;
}