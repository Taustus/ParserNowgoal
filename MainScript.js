var elems = document.getElementById("table_live").children[0].children;
var matches = [];
for (var i = 0; i < elems.length; i++) {
    if (elems[i].children.length > 9) {
        matches.push(elems[i]);
    }
}
var goodMatch = {
    'HT': "",
    'X': "",
    'J': "",
    'Y': "",
    'Z': "",
    'W': "",
    "ID": "",
    "URL": "",
    "Team": ""
};
var goodMatches = []
for (var i = 0; i < matches.length; i++) {
    goodMatch.HT = matches[i].getElementsByClassName("status")[0].innerText;
    var odds = matches[i].getElementsByClassName("oddstd");
    try {
        goodMatch.X = odds[0].childNodes[0].innerText;
        goodMatch.Z = odds[0].childNodes[1].innerText;
        goodMatch.J = odds[1].childNodes[0].innerText;
        goodMatch.Y = odds[2].childNodes[0].innerText;
        goodMatch.W = odds[2].childNodes[1].innerText;
        goodMatch.ID = matches[i].getAttribute("id").split("_")[1];
        goodMatch.URL = "http://www.nowgoal.group/detail/" + goodMatch.ID + ".html";
        goodMatch.Team = matches[i].childNodes[4].children[3].innerText + " - " + matches[i].childNodes[6].children[0].innerText;
        if (true) {//goodMatch.HT != "OT" && (goodMatch.HT == "HT" || parseInt(goodMatch.HT) < 100)) {
            var j = parseFloat(goodMatch.J)
            if (parseFloat(goodMatch.J) != 1000) {
                if (parseFloat(goodMatch.X) < 100) {
                    if (parseFloat(goodMatch.Z) < parseFloat(goodMatch.W)) {
                        goodMatches.push(Object.assign({}, goodMatch));
                    }
                }
                else if (parseFloat(goodMatch.Y) < 100) {
                    if (parseFloat(goodMatch.W) < parseFloat(goodMatch.Z)) {
                        goodMatches.push(Object.assign({}, goodMatch));
                    }
                }
            }
        }
    }
    catch (Exception) { }
}
JSON.stringify(goodMatches)
