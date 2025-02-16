document.addEventListener("DOMContentLoaded", function () {
    const urlInput = document.getElementById("urlInput");
    const fetchDataButton = document.getElementById("fetchDataButton");
    const gallery = document.getElementById("imageGallery");
    const totalWordCount = document.getElementById("totalWordCount");
    const wordTableBody = document.querySelector("#wordTable tbody");
    const wordCountContainer = document.getElementById("wordCountContainer");
    const validationMessage = document.getElementById("validationMessage");

    fetchDataButton?.addEventListener("click", function (e) {
        e.preventDefault(); 
        fetchData();
    });

    async function fetchData() {
        let url = urlInput.value.trim();
        if (!validateURL(url)) return;

        gallery.innerHTML = "";
        wordTableBody.innerHTML = "";
        totalWordCount.textContent = "";
        wordCountContainer.style.display = "none";

        try {
            // Fetch images and word count simultaneously
            await Promise.all([fetchImages(url), fetchWordCount(url)]);

            // Smoothly scroll to word count section once data is fetched
            setTimeout(() => {
                wordCountContainer.scrollIntoView({ behavior: "smooth" });
            }, 500);
        } catch (error) {
            console.error("Error fetching data:", error);
        }
    }

    async function fetchImages(url) {
        try {
            const response = await fetch(`/WebScraper/GetImages?url=${encodeURIComponent(url)}`);
            if (!response.ok) throw new Error("Failed to fetch images");

            const images = await response.json();
            gallery.innerHTML = "";

            if (images.length === 0) {
                gallery.innerHTML = "<p>No images found.</p>";
                return;
            }

            images.forEach(imgUrl => {
                const fullUrl = new URL(imgUrl, url).href;
                const img = document.createElement("img");
                img.src = fullUrl;
                img.classList.add("gallery-img");
                gallery.appendChild(img);
            });

        } catch (error) {
            console.error("Error fetching images:", error);
        }
    }

    async function fetchWordCount(url) {
        try {
            const response = await fetch(`/WebScraper/GetWordCount?url=${encodeURIComponent(url)}`);
            if (!response.ok) throw new Error("Failed to fetch word count");

            const data = await response.json();
            wordTableBody.innerHTML = ""; 

            if (!data.topWords || data.topWords.length === 0) {
                wordTableBody.innerHTML = "<tr><td colspan='2' style='text-align:center;'>No words found on the page.</td></tr>";
                return;
            }

            wordCountContainer.style.display = "block";
            totalWordCount.textContent = `${data.totalWords}`;

            data.topWords.forEach(wordObj => {
                let row = wordTableBody.insertRow();
                row.insertCell(0).textContent = wordObj.word;
                row.insertCell(1).textContent = wordObj.count;
            });

        } catch (error) {
            console.error("Error fetching word count:", error);
        }
    }

    function validateURL(url) {
        if (!url) {
            showValidationMessage("Please enter a URL.");
            return false;
        }
        if (!/^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(url)) {
            showValidationMessage("Invalid URL format.");
            return false;
        }
        clearValidationMessage();
        return true;
    }

    function showValidationMessage(message) {
        validationMessage.textContent = message;
        validationMessage.style.color = "red";
        urlInput.style.border = "2px solid red";
    }

    function clearValidationMessage() {
        validationMessage.textContent = "";
        urlInput.style.border = "";
    }
});
