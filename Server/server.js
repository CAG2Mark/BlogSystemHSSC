// server.js
// where your node app starts

// init project
const express = require("express");
const request = require("request");

const app = express();

// we've started you off with Express,
// but feel free to use whatever libs or frameworks you'd like through `package.json`.

// http://expressjs.com/en/starter/static-files.html
app.use(express.static("public"));

// http://expressjs.com/en/starter/basic-routing.html
app.get("/", function (req, res, next) {

    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");


    var query = req.query.q.toLowerCase();
    var postsLink = req.query.postsLink;



    request(postsLink, (error, response, body) => {
        var json = JSON.parse(body);

        json = json.filter(obj => {
            if (obj.Title.toLowerCase().includes(query)) {
                obj.SearchRelevance = 3;
                return true;
            }
            if (obj.Author.toLowerCase().includes(query)) {
                obj.SearchRelevance = 2;
                return true;
            }
            if (obj.Preview.toLowerCase().includes(query)) {
                obj.SearchRelevance = 1;
                return true;
            }

            return false;
        });

        json = json.sort((a, b) => b - a);

        res.end(JSON.stringify(json));
    });
});

// listen for requests :)
const listener = app.listen(process.env.PORT, function () {
    console.log("Your app is listening on port " + listener.address().port);
});