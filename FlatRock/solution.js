window.addEventListener('load', solution);

function solution() {
    const items = Array.from(document.querySelectorAll('.item'));

    //decoding HTML entities in a string, ensuring that special characters are correctly represented in the resulting text content.
    const decodeHTML = (htmlString) => {
        const div = document.createElement('div');
        div.innerHTML = htmlString;
        return div.textContent;
    };
    

    let productArray = [];
    
    //iterating through all the 3 divs with class="item" to extract the needed productName, price and rating
    for (const item of items) {
        const currProductName = decodeHTML(item.querySelector('img').getAttribute('alt').trim());
        const currPrice = item.querySelector('.price-display > span[style="display: none"]').textContent.trim().replace('$', '').replace(',', '')
        let currRating = parseFloat(item.getAttribute('rating'));

     // scale down the ratings to normalize them to be out of 5
        if (currRating > 5) {
            let divider = 2;
            while ((currRating / divider) > 5) {
                divider++;
            }
            currRating /= divider;
        }

        productArray.push({
            productName: currProductName,
            price: currPrice,
            rating: currRating.toString(),
        });
    }

    console.log(JSON.stringify(productArray, null, 3));
}

   

