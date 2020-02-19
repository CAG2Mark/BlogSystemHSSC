class Gallery {
    constructor(galleryNode) {
        this.galleryNode = galleryNode;

        this.currentPageIndex = 0;
        this.pages = galleryNode.getElementsByClassName("gallery-container");
        this.pageCount = this.pages.length;

        var indicatorTemplate = galleryNode.querySelector(".template-indicator");
        var indicatorRegion = galleryNode.querySelector(".region-indicators");

        for (var i = 0; i < this.pageCount; i++) {
            

            // initialize the indicators
            var newNode = indicatorTemplate.cloneNode(true);

            newNode.classList.remove("template");
            newNode.classList.remove("template-indicator");

            newNode.setAttribute("data-indicator-index", i);

            indicatorRegion.appendChild(newNode);

            newNode.addEventListener("click", (e) => {
                var sender = e.srcElement;

                var index = sender.getAttribute("data-indicator-index");

                if (this.currentPageIndex == index) return;
                // update selected status

                this.transitionPage(this.currentPageIndex, index, index < this.currentPageIndex);

            });

            // initialize the pages
            if (i == this.currentPageIndex) {
                this.pages[i].style.left = "0";

                newNode.classList.add("indicator-selected");
                this.selectedIndicator = newNode;
            } else {
                this.pages[i].style.left = "100%";
            }
        }

        this.indicators = indicatorRegion.querySelectorAll(".indicator:not(.template)");


        this.nextPageInterval = setInterval(() => {
            //this.nextPage();
        }, 7000);
    }

    // next page
    nextPage() {
        var prevIndex = this.currentPageIndex;
        // get next page
        this.currentPageIndex++;
        if (this.currentPageIndex > this.pageCount - 1) this.currentPageIndex = 0;

        this.transitionPage(prevIndex, this.currentPageIndex, false);
    }

    // previous page
    prevPage() {
        var prevIndex = this.currentPageIndex;
        // get prev page
        this.currentPageIndex--;
        if (this.currentPageIndex < 0) this.currentPageIndex = this.pageCount - 1;

        this.transitionPage(prevIndex, this.currentPageIndex, true);
    }

    transitionPage(current, to, leftToRight) {
        var nextIndicator = this.indicators[to];

        nextIndicator.classList.add("indicator-selected");
        this.selectedIndicator.classList.remove("indicator-selected");
        this.selectedIndicator = nextIndicator;

        var currentPage = this.pages[current];
        var nextPage = this.pages[to];

        if (current == to) return;

        // stop the interval.

        clearInterval(this.nextPageInterval);

        this.currentPageIndex = to;

        currentPage.style.left = "0";

        // transition right to left
        if (!leftToRight) {
            // prep
            nextPage.style.left = "100%";
        } else {
            nextPage.style.left = "-100%";
        }

        setTimeout(() => {
            // add transiton
            currentPage.classList.add("transitioning");
            nextPage.classList.add("transitioning");

            // move
            if (!leftToRight) {
                currentPage.style.left = "-100%";
                nextPage.style.left = "0";
            } else {
                currentPage.style.left = "100%";
                nextPage.style.left = "0";
            }
        }, 1);


        // remove transition
        setTimeout(() => {
            currentPage.classList.remove("transitioning");
            nextPage.classList.remove("transitioning");
        }, 700);

        this.nextPageInterval = setInterval(() => {
            //this.nextPage();
        }, 7000);

    }
}

var galleries = document.getElementsByClassName("gallery");
var galleriesArr = new Array(galleries.length)

for (var i = 0; i < galleries.length; i++) {
    galleriesArr[i] = new Gallery(galleries[i]);
}
