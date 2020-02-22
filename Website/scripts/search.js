var urlStr = window.location.href;
var url = new URL(urlStr);

var searchQuery = url.searchParams.get("q");

if (!searchQuery) {
    document.getElementById("loading-icon").classList.add("hidden");
} else {

    document.getElementById("body-search-textbox").value = searchQuery;

    //var postsLink = new URL("./blog/posts.json", document.baseURI).href;
    var postsLink = "https://ngp16.github.io/BlogSystemOnlineTest/blog/posts.json";

    postsLink = encodeURIComponent(postsLink);

    var requestLink = `https://hssc-search-system.glitch.me/?q=${searchQuery}&postsLink=${postsLink}`;

    var template = document.getElementById("post-template");

    getData();
}

function getData() {

    fetch(requestLink, {
            mode: 'cors'
        })
        .then(response => {
            if (!response.ok) {
                throw Error(response.statusText);
            }
            return response.text();
        })
        .then(
            (response) => {

                // clear loading icon
                document.getElementById("loading-icon").classList.add("hidden");

                var json = JSON.parse(response);

                for (var i = 0; i < json.length; i++) {

                    var post = json[i];

                    var newItem = template.cloneNode(true);
                    newItem.removeAttribute("id");

                    setVal(newItem, "post-template-title", post.Title);
                    setVal(newItem, "post-template-author", post.Author);
                    setVal(newItem, "post-template-date", post.PublishTimeStr);
                    setVal(newItem, "post-template-preview", post.Preview);
                    setImg(newItem, "post-header-image-mobile", post.HeaderImageName, post.IsHeaderImageSet);
                    setImg(newItem, "post-header-image-desktop", post.HeaderImageName, post.IsHeaderImageSet);

                    document.getElementById("search-results-container").appendChild(newItem);

                    newItem.setAttribute("href", `./blog/${post.HtmlFriendlyTitle}.html`);
                }
            }
        )
        .catch(
            (error) => {
                alert(error.toString());
                return;
            }
        );
}

function setVal(item, id, val) {
    var obj = item.querySelector("#" + id);
    obj.innerHTML = val;
    obj.removeAttribute("id");
}

function setImg(item, id, val, hasImage) {

    var obj = item.querySelector("#" + id);

    if (hasImage) {
        obj.setAttribute("src", `./content/images/${val}`);
    } else {
        obj.classList.add("hidden");
    }

    obj.removeAttribute("id");
}